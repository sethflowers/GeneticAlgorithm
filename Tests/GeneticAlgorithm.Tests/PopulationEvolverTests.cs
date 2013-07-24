//-----------------------------------------------------------------------
// <copyright file="PopulationEvolverTests.cs" company="Seth Flowers">
//     All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace GeneticAlgorithm.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    /// <summary>
    /// Tests for the PopulationEvolver class.
    /// </summary>
    [TestClass]
    public class PopulationEvolverTests
    {
        /// <summary>
        /// Validates that the Evolve method throws an exception with a meaningful message if the currentPopulation argument is null.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Evolve_NullCurrentPopulation_ThrowsMeaningfulException()
        {
            try
            {
                new PopulationEvolver<int>().Evolve(currentPopulation: null);
            }
            catch (ArgumentNullException argumentNullException)
            {
                Assert.AreEqual(
                    string.Format("Unable to create a new population from a null population.{0}Parameter name: currentPopulation", Environment.NewLine),
                    argumentNullException.Message);

                throw;
            }
        }
        
        /// <summary>
        /// Validates that the Evolve method behaves as expected given some simple input.
        /// This method is difficult to test, since it mainly just calls dependencies.
        /// The point of this test is to validate that the interaction between the dependencies is correct.
        /// </summary>
        [TestMethod]
        public void Evolve_SimpleSetup_BehavesAsExpected()
        {
            // Create three chromosomes, with different genes.
            Chromosome<int> chromosomeA = new Chromosome<int>(new[] { 1, 2, 3 });
            Chromosome<int> chromosomeB = new Chromosome<int>(new[] { 4, 5, 6 });
            Chromosome<int> chromosomeC = new Chromosome<int>(new[] { 7, 8, 9 });

            // Create a current population with the three different chromosomes.
            ChromosomeCollection<int> currentPopulation =
                new ChromosomeCollection<int> { chromosomeA, chromosomeB, chromosomeC };

            // Create the mock dependencies of the population evolver.
            Mock<ChromosomeSelector<int>> selector = new Mock<ChromosomeSelector<int>>();
            Mock<ChromosomeModifier<int>> modifier = new Mock<ChromosomeModifier<int>>();
            Mock<ChromosomeValidator<int>> validator = new Mock<ChromosomeValidator<int>>();

            // Create the population evolver object we will be testing, but with the mock dependencies.
            PopulationEvolver<int> populationEvolver = new PopulationEvolver<int>(
                selector.Object,
                modifier.Object,
                validator.Object);

            // We know we only have to go through one and a half iterations to make 3 new chromosomes, 
            // since we make two each iteration, assuming they are valid.
            // This means we only need to setup the selector queue to return two pairs of two, one for each iteration.
            // Setup the selector queue to return unique pairs of the chromosomes for the mom and dad each iteration.
            Queue<Chromosome<int>> selectorQueue = new Queue<Chromosome<int>>(new[] 
            {
                chromosomeA, chromosomeB, 
                chromosomeB, chromosomeC 
            });

            // Setup the selector to return the chromosomes in the order we defined for the queue.
            selector
                .Setup(s => s.Choose(currentPopulation, It.IsAny<double>()))
                .Returns(selectorQueue.Dequeue);

            // The first time Crossover is called, it will be with clones of chromosomeA and chromosomeB, 
            // since that is what we setup the selector to return.
            // This means we can hardcode what Crossover will do this call, 
            // for instance, just swapping the genes at index 0.
            // This is akin to if we did a two point crossover where the indexes were one apart.
            modifier
                .Setup(m => m.Crossover(
                    It.Is<Chromosome<int>>(c => c.Genes[0] == 1 && c.Genes[1] == 2 && c.Genes[2] == 3),
                    It.Is<Chromosome<int>>(c => c.Genes[0] == 4 && c.Genes[1] == 5 && c.Genes[2] == 6)))
                .Callback<Chromosome<int>, Chromosome<int>>((dad, mom) =>
                    {
                        int temp = dad.Genes[0];
                        dad.Genes[0] = mom.Genes[0];
                        mom.Genes[0] = temp;
                    });

            // The second time Crossover is called, it will be with clones of chromosomeB and chromosomeC, 
            // since that is what we setup the selector to return.
            // This means we can hardcode what Crossover will do this call, 
            // for instance, just swapping the genes at index 1.
            // This is akin to if we did a two point crossover where the indexes were one apart.
            modifier
                .Setup(m => m.Crossover(
                    It.Is<Chromosome<int>>(c => c.Genes[0] == 4 && c.Genes[1] == 5 && c.Genes[2] == 6),
                    It.Is<Chromosome<int>>(c => c.Genes[0] == 7 && c.Genes[1] == 8 && c.Genes[2] == 9)))
                .Callback<Chromosome<int>, Chromosome<int>>((dad, mom) =>
                {
                    int temp = dad.Genes[1];
                    dad.Genes[1] = mom.Genes[1];
                    mom.Genes[1] = temp;
                });

            // We call mutate twice each iteration.
            // This means that with the way we setup the selector,
            // we will be calling it once for chromosomeA (in the first iteration),
            // twice for chromosomeB (once in both iterations),
            // and once for chromosomeC (in the second iteration).
            // We can just specify that mutate will always swap the genes at index 0 and 2.
            modifier
                .Setup(m => m.Mutate(It.IsAny<Chromosome<int>>()))
                .Callback<Chromosome<int>>(chromosome =>
                {
                    int temp = chromosome.Genes[0];
                    chromosome.Genes[0] = chromosome.Genes[2];
                    chromosome.Genes[2] = temp;
                });

            // Lastly, setup the validator to always just return true.
            validator.Setup(v => v.IsValid(It.IsAny<Chromosome<int>>())).Returns(true);

            // Execute the code to test.
            ChromosomeCollection<int> newPopulation = populationEvolver.Evolve(currentPopulation);

            // Validate that we got back the expected new chromosomes.
            Assert.AreEqual(3, newPopulation.Count, "The new population should have the same number of chromosomes as the current population.");

            CollectionAssert.AreEqual(new[] { 3, 2, 4 }, newPopulation[0].Genes.ToList(), "The first chromosome isn't as expected.");
            CollectionAssert.AreEqual(new[] { 6, 5, 1 }, newPopulation[1].Genes.ToList(), "The second chromosome isn't as expected.");
            CollectionAssert.AreEqual(new[] { 6, 8, 4 }, newPopulation[2].Genes.ToList(), "The third chromosome isn't as expected.");

            // The following is an explanation stepping through how we got to our expected results.
            // Beginning population
            // 123
            // 456
            // 789

            // First iteration - after selecting chromosome A and B, we crossover, which just swaps genes at 0
            // 423
            // 156

            // First iteration - we then mutate, which swaps genes at 0 and 2
            // 324
            // 651

            // First iteration - new population is now
            // 324
            // 651

            // Second iteration - after selecting chromosome B and C, we crossover, which just swaps genes at 1
            // 486
            // 759

            // Second iteration - we then mutate, which swaps genes at 0 and 2
            // 684
            // 957

            // Second iteration - we add the first chromosome to the new population, 
            // but not the second chromosome, because we've reached the required number of new chromosomes.
            // The new population should be
            // 324
            // 651
            // 684
        }
    }
}