﻿//-----------------------------------------------------------------------
// <copyright file="ChromosomeModifierTests.cs" company="Seth Flowers">
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
    /// Tests for the ChromosomeModifier class.
    /// </summary>
    [TestClass]
    public class ChromosomeModifierTests
    {
        /// <summary>
        /// Validates that the Mutate method throws a meaningful exception if the chromosome argument is null.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Mutate_NullChromosomeArgument_ThrowsMeaningfulException()
        {
            try
            {
                new ChromosomeModifier<int>()
                    .Mutate(chromosome: null);
            }
            catch (ArgumentNullException argumentNullException)
            {
                Assert.AreEqual(
                    string.Format("Unable to mutate a null chromosome.{0}Parameter name: chromosome", Environment.NewLine),
                    argumentNullException.Message);

                throw;
            }
        }

        /// <summary>
        /// Validates that the Mutate method throws a meaningful exception if the chromosome argument is has a null Genes property.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Mutate_ChromosomeArgumentWithNullGenes_ThrowsMeaningfulException()
        {
            try
            {
                Chromosome<int> chromosome = new Chromosome<int>(genes: null);
                new ChromosomeModifier<int>()
                    .Mutate(chromosome: chromosome);
            }
            catch (ArgumentException argumentException)
            {
                Assert.AreEqual(
                    string.Format("Unable to mutate a chromosome with no genes.{0}Parameter name: chromosome", Environment.NewLine),
                    argumentException.Message);

                throw;
            }
        }

        /// <summary>
        /// Validates that the Mutate method throws a meaningful exception if the chromosome argument is has an empty Genes collection property.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Mutate_ChromosomeArgumentWithEmptyGenes_ThrowsMeaningfulException()
        {
            try
            {
                Chromosome<int> chromosome = new Chromosome<int>(genes: new int[0]);
                new ChromosomeModifier<int>()
                    .Mutate(chromosome: chromosome);
            }
            catch (ArgumentException argumentException)
            {
                Assert.AreEqual(
                    string.Format("Unable to mutate a chromosome with no genes.{0}Parameter name: chromosome", Environment.NewLine),
                    argumentException.Message);

                throw;
            }
        }

        /// <summary>
        /// Validates that the mutate function behaves as expected given a simple setup using the adjacent strategy.
        /// The mutate function works by iterating over the genes in a chromosome.
        /// Each iteration it gets a random value from the random generator and if that value is less
        /// than the mutation rate, it mutates the current gene.
        /// To test this, we can just mock the random generator to return an expected value at certain
        /// indexes, meaning we can control when we expect the gene to be mutated.
        /// </summary>
        [TestMethod]
        public void Mutate_AdjacentStrategyInSimpleScenario_MutatesSuccessfully()
        {  
            // Setup some data for the test.
            double mutationRate = 0.2d;            
            List<int> genes = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            Chromosome<int> chromosome = new Chromosome<int>(genes);

            // Setup a queue to control the values returned from the random generator.
            // The queue should have the same number of items as the genes.
            // Each value in the queue represents the number returns from random.NextDouble for the gene at the same index.
            // If the value is less than the mutation rate, like 0.1, then it will be swapped.
            // If the value is more than the mutation rate, like 0.3, then it will not be swapped.
            Queue<double> queue = new Queue<double>(new[] { 0.3, 0.1, 0.3, 0.3, 0.3, 0.1, 0.1, 0.3, 0.3, 0.1 });
            Mock<Random> random = new Mock<Random>();
            random.Setup(r => r.NextDouble()).Returns(() => queue.Dequeue());

            ChromosomeModifier<int> modifier = new ChromosomeModifier<int>(
                random.Object, 
                mutationRate, 
                crossoverRate: 0,
                numberOfCrossoverPoints: 1,
                mutationStrategy: MutationStrategy.Adjacent,
                mutationFunction: null);

            // Execute the mutate method that we are testing.
            modifier.Mutate(chromosome);

            // Validate that the chromosome has been mutated as expected.
            List<int> expectedGenes = new List<int> { 9, 2, 1, 3, 4, 6, 7, 5, 8, 0 };
            CollectionAssert.AreEqual(expectedGenes, chromosome.Genes.ToList());
        }

        /// <summary>
        /// Validates that the mutate function behaves as expected given a simple setup using the random strategy.
        /// The mutate function works by iterating over the genes in a chromosome.
        /// Each iteration it gets a random value from the random generator and if that value is less
        /// than the mutation rate, it mutates the current gene.
        /// To test this, we can just mock the random generator to return an expected value at certain
        /// indexes, meaning we can control when we expect the gene to be mutated.
        /// </summary>
        [TestMethod]
        public void Mutate_RandomStrategyInSimpleScenario_MutatesSuccessfully()
        {
            // Setup some data for the test.
            double mutationRate = 0.2d;
            List<int> genes = new List<int> { 0, 1, 2, 3, 4 };
            Chromosome<int> chromosome = new Chromosome<int>(genes);

            // Setup a queue to control the values returned from the random generator.
            // The queue should have the same number of items as the genes.
            // Each value in the queue represents the number returns from random.NextDouble for the gene at the same index.
            // If the value is less than the mutation rate, like 0.1, then it will be swapped.
            // If the value is more than the mutation rate, like 0.3, then it will not be swapped.
            Queue<double> queue = new Queue<double>(new[] { 0.3, 0.1, 0.3, 0.3, 0.3 });
            Mock<Random> random = new Mock<Random>();
            random.Setup(r => r.NextDouble()).Returns(() => queue.Dequeue());

            // Setup another queue to specify which index to swap.
            // We only are going to attempt to swap one gene, which is the gene at index 1,
            // and we expect it to be swapped with the gene at index 3.
            Queue<int> indexQueue = new Queue<int>(new[] { 3 });
            random.Setup(r => r.Next(genes.Count)).Returns(() => indexQueue.Dequeue());

            ChromosomeModifier<int> modifier = new ChromosomeModifier<int>(
                random.Object,
                mutationRate,
                crossoverRate: 0,
                numberOfCrossoverPoints: 1,
                mutationStrategy: MutationStrategy.Random,
                mutationFunction: null);

            // Execute the mutate method that we are testing.
            modifier.Mutate(chromosome);

            // Validate that the chromosome has been mutated as expected.
            List<int> expectedGenes = new List<int> { 0, 3, 2, 1, 4 };
            CollectionAssert.AreEqual(expectedGenes, chromosome.Genes.ToList());
        }

        /// <summary>
        /// Validates that the mutate function behaves as expected given a simple setup using the function strategy.
        /// The mutate function works by iterating over the genes in a chromosome.
        /// Each iteration it gets a random value from the random generator and if that value is less
        /// than the mutation rate, it mutates the current gene.
        /// To test this, we can just mock the random generator to return an expected value at certain
        /// indexes, meaning we can control when we expect the gene to be mutated.
        /// </summary>
        [TestMethod]
        public void Mutate_FunctionStrategyInSimpleScenario_MutatesSuccessfully()
        {
            // Setup some data for the test.
            double mutationRate = 0.2d;
            List<int> genes = new List<int> { 0, 1, 2, 3, 4 };
            Chromosome<int> chromosome = new Chromosome<int>(genes);

            // Setup a queue to control the values returned from the random generator.
            // The queue should have the same number of items as the genes.
            // Each value in the queue represents the number returns from random.NextDouble for the gene at the same index.
            // If the value is less than the mutation rate, like 0.1, then it will be swapped.
            // If the value is more than the mutation rate, like 0.3, then it will not be swapped.
            Queue<double> queue = new Queue<double>(new[] { 0.3, 0.1, 0.3, 0.3, 0.3 });
            Mock<Random> random = new Mock<Random>();
            random.Setup(r => r.NextDouble()).Returns(() => queue.Dequeue());

            // Create a function to mutate the chromosome at the given gene index.
            Action<Chromosome<int>, int> mutationFunction = (c, i) => c.Genes[i] = 12;

            ChromosomeModifier<int> modifier = new ChromosomeModifier<int>(
                random.Object,
                mutationRate,
                crossoverRate: 0,
                numberOfCrossoverPoints: 1,
                mutationStrategy: MutationStrategy.Function,
                mutationFunction: mutationFunction);

            // Execute the mutate method that we are testing.
            modifier.Mutate(chromosome);

            // Validate that the chromosome has been mutated as expected.
            List<int> expectedGenes = new List<int> { 0, 12, 2, 3, 4 };
            CollectionAssert.AreEqual(expectedGenes, chromosome.Genes.ToList());
        }

        /// <summary>
        /// Validates that the Crossover method throws a meaningful exception if the first chromosome argument is null.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Crossover_NullDadChromosomeArgument_ThrowsMeaningfulException()
        {
            try
            {
                new ChromosomeModifier<int>()
                    .Crossover(dad: null, mom: new Chromosome<int>(null));
            }
            catch (ArgumentNullException argumentNullException)
            {
                Assert.AreEqual(
                    string.Format("Unable to crossover a null chromosome.{0}Parameter name: dad", Environment.NewLine),
                    argumentNullException.Message);

                throw;
            }
        }

        /// <summary>
        /// Validates that the Crossover method throws a meaningful exception if the second chromosome argument is null.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Crossover_NullMomChromosomeArgument_ThrowsMeaningfulException()
        {
            try
            {
                new ChromosomeModifier<int>()
                    .Crossover(dad: new Chromosome<int>(null), mom: null);
            }
            catch (ArgumentNullException argumentNullException)
            {
                Assert.AreEqual(
                    string.Format("Unable to crossover a null chromosome.{0}Parameter name: mom", Environment.NewLine),
                    argumentNullException.Message);

                throw;
            }
        }

        /// <summary>
        /// Validates that the Crossover method throws a meaningful exception if the first chromosome argument has a null Genes property.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Crossover_DadChromosomeArgumentWithNullGenes_ThrowsMeaningfulException()
        {
            try
            {
                new ChromosomeModifier<int>()
                    .Crossover(
                        dad: new Chromosome<int>(genes: null),
                        mom: new Chromosome<int>(genes: new[] { 1 }));
            }
            catch (ArgumentException argumentException)
            {
                Assert.AreEqual(
                    string.Format("Unable to crossover a chromosome with no genes.{0}Parameter name: dad", Environment.NewLine),
                    argumentException.Message);

                throw;
            }
        }

        /// <summary>
        /// Validates that the Crossover method throws a meaningful exception if the first chromosome argument has an empty Genes property.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Crossover_DadChromosomeArgumentWithEmptyGenes_ThrowsMeaningfulException()
        {
            try
            {
                new ChromosomeModifier<int>()
                    .Crossover(
                        dad: new Chromosome<int>(genes: new int[0]),
                        mom: new Chromosome<int>(genes: new[] { 1 }));
            }
            catch (ArgumentException argumentException)
            {
                Assert.AreEqual(
                    string.Format("Unable to crossover a chromosome with no genes.{0}Parameter name: dad", Environment.NewLine),
                    argumentException.Message);

                throw;
            }
        }

        /// <summary>
        /// Validates that the Crossover method throws a meaningful exception if the second chromosome argument has a null Genes property.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Crossover_MomChromosomeArgumentWithNullGenes_ThrowsMeaningfulException()
        {
            try
            {
                new ChromosomeModifier<int>()
                    .Crossover(
                        dad: new Chromosome<int>(genes: new[] { 1 }),
                        mom: new Chromosome<int>(genes: null));
            }
            catch (ArgumentException argumentException)
            {
                Assert.AreEqual(
                    string.Format("Unable to crossover a chromosome with no genes.{0}Parameter name: mom", Environment.NewLine),
                    argumentException.Message);

                throw;
            }
        }

        /// <summary>
        /// Validates that the Crossover method throws a meaningful exception if the second chromosome argument has an empty Genes property.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Crossover_MomChromosomeArgumentWithEmptyGenes_ThrowsMeaningfulException()
        {
            try
            {
                new ChromosomeModifier<int>()
                    .Crossover(
                        dad: new Chromosome<int>(genes: new[] { 1 }),
                        mom: new Chromosome<int>(genes: new int[0]));
            }
            catch (ArgumentException argumentException)
            {
                Assert.AreEqual(
                    string.Format("Unable to crossover a chromosome with no genes.{0}Parameter name: mom", Environment.NewLine),
                    argumentException.Message);

                throw;
            }
        }

        /// <summary>
        /// Validates that if the double returned from the random generator 
        /// is not less than the crossover rate, then no genes are swapped.
        /// </summary>
        [TestMethod]
        public void Crossover_CrossoverRateNotMet_DoesntSwapGenes()
        {
            double crossoverRate = 0.1;

            Mock<Random> random = new Mock<Random> { CallBase = true };
            ChromosomeModifier<int> modifier = new ChromosomeModifier<int>(
                randomGenerator: random.Object,
                mutationRate: 0.5,
                crossoverRate: crossoverRate,
                numberOfCrossoverPoints: 1);

            // Setup some chromosomes so we can validate they didn't get modified.
            IList<int> dadsGenes = new[] { 1, 2, 3, 4, 5 };
            IList<int> momsGenes = new[] { 6, 7, 8, 9, 10 };
            
            // Make sure we clone the lists when we pass them into the chromosome.
            // This is because we need to compare the original lists against the chromosomes genes.
            // In other words, the list is a reference type, so we can't refer to the same list for this test.
            Chromosome<int> dad = new Chromosome<int>(dadsGenes.ToList());
            Chromosome<int> mom = new Chromosome<int>(momsGenes.ToList());

            // Setup the random generator to return something greater 
            // than crossover rate for the NextDouble method.
            // This ensures that we do NOT fall into the swap logic.
            random.Setup(r => r.NextDouble()).Returns(crossoverRate + 0.01);

            // Execute the code to test.
            modifier.Crossover(dad, mom);

            // Validate that the genes didn't get modified.
            CollectionAssert.AreEqual(dadsGenes.ToList(), dad.Genes.ToList());
            CollectionAssert.AreEqual(momsGenes.ToList(), mom.Genes.ToList());
        }

        /// <summary>
        /// Validates that if the double returned from the random generator 
        /// is less than the crossover rate, then the genes are swapped correctly.
        /// This tests a one point crossover.
        /// </summary>
        [TestMethod]
        public void Crossover_CrossoverRateMet_SwapsGenesCorrectlyInOnePointCrossover()
        {
            double crossoverRate = 0.1;

            Mock<Random> random = new Mock<Random> { CallBase = true };
            ChromosomeModifier<int> modifier = new ChromosomeModifier<int>(
                randomGenerator: random.Object,
                mutationRate: 0.5,
                crossoverRate: crossoverRate,
                numberOfCrossoverPoints: 1);

            // Make sure we clone the lists when we pass them into the chromosome.
            // This is because we need to compare the original lists against the chromosomes genes.
            // In other words, the list is a reference type, so we can't refer to the same list for this test.
            Chromosome<int> dad = new Chromosome<int>(new[] { 1, 2, 3, 4, 5 });
            Chromosome<int> mom = new Chromosome<int>(new[] { 6, 7, 8, 9, 10 });

            // Setup the random generator to return something less 
            // than crossover rate for the NextDouble method.
            // This ensures that we fall into the swap logic.
            random.Setup(r => r.NextDouble()).Returns(crossoverRate - 0.01);

            // Setup the random generator to specify the point at which we should swap.
            random.Setup(r => r.Next(dad.Genes.Count)).Returns(2);

            // Setup the expected genes after swapping at a crossover index of 2.
            IList<int> expectedDadsGenes = new[] { 1, 2, 8, 9, 10 };
            IList<int> expectedMomsGenes = new[] { 6, 7, 3, 4, 5 };

            // Execute the code to test.
            modifier.Crossover(dad, mom);

            // Validate that the genes got swapped correctly.
            CollectionAssert.AreEqual(expectedDadsGenes.ToList(), dad.Genes.ToList());
            CollectionAssert.AreEqual(expectedMomsGenes.ToList(), mom.Genes.ToList());
        }

        /// <summary>
        /// Validates that if the double returned from the random generator 
        /// is less than the crossover rate, then the genes are swapped correctly.
        /// This tests a two point crossover.
        /// </summary>
        [TestMethod]
        public void Crossover_CrossoverRateMet_SwapsGenesCorrectlyInTwoPointCrossover()
        {
            double crossoverRate = 0.1;

            Mock<Random> random = new Mock<Random> { CallBase = true };
            ChromosomeModifier<int> modifier = new ChromosomeModifier<int>(
                randomGenerator: random.Object,
                mutationRate: 0.5,
                crossoverRate: crossoverRate,
                numberOfCrossoverPoints: 2);

            // Make sure we clone the lists when we pass them into the chromosome.
            // This is because we need to compare the original lists against the chromosomes genes.
            // In other words, the list is a reference type, so we can't refer to the same list for this test.
            Chromosome<int> dad = new Chromosome<int>(new[] { 1, 2, 3, 4, 5, 6, 7 });
            Chromosome<int> mom = new Chromosome<int>(new[] { 8, 9, 10, 11, 12, 13, 14 });

            // Setup the random generator to return something less 
            // than crossover rate for the NextDouble method.
            // This ensures that we fall into the swap logic.
            random.Setup(r => r.NextDouble()).Returns(crossoverRate - 0.01);

            Queue<int> pointsToReturn = new Queue<int>(new[] { 2, 4 });

            // Setup the random generator to specify the point at which we should swap.
            random.Setup(r => r.Next(dad.Genes.Count)).Returns(pointsToReturn.Dequeue);

            // Setup the expected genes after swapping at crossover indexes of 2, and 4.
            IList<int> expectedDadsGenes = new[] { 1, 2, 10, 11, 5, 6, 7 };
            IList<int> expectedMomsGenes = new[] { 8, 9, 3, 4, 12, 13, 14 };

            // Execute the code to test.
            modifier.Crossover(dad, mom);

            // Validate that the genes got swapped correctly.
            CollectionAssert.AreEqual(expectedDadsGenes.ToList(), dad.Genes.ToList());
            CollectionAssert.AreEqual(expectedMomsGenes.ToList(), mom.Genes.ToList());
        }

        /// <summary>
        /// Validates that if the double returned from the random generator 
        /// is less than the crossover rate, then the genes are swapped correctly.
        /// This tests a three point crossover.
        /// </summary>
        [TestMethod]
        public void Crossover_CrossoverRateMet_SwapsGenesCorrectlyInThreePointCrossover()
        {
            double crossoverRate = 0.1;

            Mock<Random> random = new Mock<Random> { CallBase = true };
            ChromosomeModifier<int> modifier = new ChromosomeModifier<int>(
                randomGenerator: random.Object,
                mutationRate: 0.5,
                crossoverRate: crossoverRate,
                numberOfCrossoverPoints: 3);

            // Make sure we clone the lists when we pass them into the chromosome.
            // This is because we need to compare the original lists against the chromosomes genes.
            // In other words, the list is a reference type, so we can't refer to the same list for this test.
            Chromosome<int> dad = new Chromosome<int>(new[] { 1, 2, 3, 4, 5, 6, 7 });
            Chromosome<int> mom = new Chromosome<int>(new[] { 8, 9, 10, 11, 12, 13, 14 });

            // Setup the random generator to return something less 
            // than crossover rate for the NextDouble method.
            // This ensures that we fall into the swap logic.
            random.Setup(r => r.NextDouble()).Returns(crossoverRate - 0.01);

            Queue<int> pointsToReturn = new Queue<int>(new[] { 2, 4, 6 });

            // Setup the random generator to specify the point at which we should swap.
            random.Setup(r => r.Next(dad.Genes.Count)).Returns(pointsToReturn.Dequeue);

            // Setup the expected genes after swapping at crossover indexes of 2, 4 and 6.
            IList<int> expectedDadsGenes = new[] { 1, 2, 10, 11, 5, 6, 14 };
            IList<int> expectedMomsGenes = new[] { 8, 9, 3, 4, 12, 13, 7 };

            // Execute the code to test.
            modifier.Crossover(dad, mom);

            // Validate that the genes got swapped correctly.
            CollectionAssert.AreEqual(expectedDadsGenes.ToList(), dad.Genes.ToList());
            CollectionAssert.AreEqual(expectedMomsGenes.ToList(), mom.Genes.ToList());
        }

        /// <summary>
        /// Validates that if the crossover points returned from the random generator are out of order,
        /// which is very likely, that they are sorted prior to the algorithm crossing over the genes.
        /// To test this, we just setup the random generator to return points that are descending,
        /// and then validate the genes are crossed over correctly.
        /// </summary>
        [TestMethod]
        public void Crossover_CrossoverRateMet_SortsCrossoverPointsBeforeCrossingOver()
        {
            double crossoverRate = 0.1;

            Mock<Random> random = new Mock<Random> { CallBase = true };
            ChromosomeModifier<int> modifier = new ChromosomeModifier<int>(
                randomGenerator: random.Object,
                mutationRate: 0.5,
                crossoverRate: crossoverRate,
                numberOfCrossoverPoints: 3);

            // Make sure we clone the lists when we pass them into the chromosome.
            // This is because we need to compare the original lists against the chromosomes genes.
            // In other words, the list is a reference type, so we can't refer to the same list for this test.
            Chromosome<int> dad = new Chromosome<int>(new[] { 1, 2, 3, 4, 5, 6, 7 });
            Chromosome<int> mom = new Chromosome<int>(new[] { 8, 9, 10, 11, 12, 13, 14 });

            // Setup the random generator to return something less 
            // than crossover rate for the NextDouble method.
            // This ensures that we fall into the swap logic.
            random.Setup(r => r.NextDouble()).Returns(crossoverRate - 0.01);

            Queue<int> pointsToReturn = new Queue<int>(new[] { 6, 4, 2 });

            // Setup the random generator to specify the point at which we should swap.
            random.Setup(r => r.Next(dad.Genes.Count)).Returns(pointsToReturn.Dequeue);

            // Setup the expected genes after swapping at crossover indexes of 2, 4 and 6.
            IList<int> expectedDadsGenes = new[] { 1, 2, 10, 11, 5, 6, 14 };
            IList<int> expectedMomsGenes = new[] { 8, 9, 3, 4, 12, 13, 7 };

            // Execute the code to test.
            modifier.Crossover(dad, mom);

            // Validate that the genes got swapped correctly.
            CollectionAssert.AreEqual(expectedDadsGenes.ToList(), dad.Genes.ToList());
            CollectionAssert.AreEqual(expectedMomsGenes.ToList(), mom.Genes.ToList());
        }
    }
}