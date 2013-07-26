//-----------------------------------------------------------------------
// <copyright file="GATests.cs" company="Seth Flowers">
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
    /// Tests for the GA class.
    /// </summary>
    [TestClass]
    public class GATests
    {
        /// <summary>
        /// Validates that the Run method throws an exception with a meaningful message if the beginningPopulation argument is null.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Run_NullBeginningPopulation_ThrowsMeaningfulException()
        {
            try
            {
                new GA<int>(new GenericFitnessCalculator<int>(c => 0d)).Run(
                    beginningPopulation: null,
                    numberOfGenerations: 10,
                    numberOfBestChromosomesToPromote: 0);
            }
            catch (ArgumentNullException argumentNullException)
            {
                Assert.AreEqual(
                    string.Format("Unable to run a genetic algorithm with a null beginning population of chromosomes.{0}Parameter name: beginningPopulation", Environment.NewLine),
                    argumentNullException.Message);

                throw;
            }
        }

        /// <summary>
        /// Validates that the genetic algorithm runs the correct number of generations.
        /// This can be tested by mocking the population evolver and verifying the number of times
        /// that Evolve is called is the same as the number of generations argument passed into the run method.
        /// We can also validate that the evolver's evolve method is called with the correct arguments.
        /// </summary>
        [TestMethod]
        public void Run_RunsCorrectNumberOfGenerations()
        {
            // Setup the mock dependencies of the GA.
            Mock<PopulationEvolver<int>> populationEvolver = new Mock<PopulationEvolver<int>>(null);
            Mock<ChromosomeFitnessCalculator<int>> fitnessCalculator = new Mock<ChromosomeFitnessCalculator<int>>();

            // Setup the GA with the mock dependencies.
            GA<int> ga = new GA<int>(fitnessCalculator.Object, populationEvolver.Object);

            // Setup the population that will be passed into the GA.
            ChromosomeCollection<int> beginningPopulation = new ChromosomeCollection<int>();

            // Setup several other generations.
            ChromosomeCollection<int> generationOne = new ChromosomeCollection<int>();
            ChromosomeCollection<int> generationTwo = new ChromosomeCollection<int>();
            ChromosomeCollection<int> generationThree = new ChromosomeCollection<int>();
            ChromosomeCollection<int> generationFour = new ChromosomeCollection<int>();
            ChromosomeCollection<int> generationFive = new ChromosomeCollection<int>();

            // Setup the population evolver to return the desired generation when given the previous generation.
            populationEvolver.Setup(p => p.Evolve(beginningPopulation, 0)).Returns(generationOne);
            populationEvolver.Setup(p => p.Evolve(generationOne, 0)).Returns(generationTwo);
            populationEvolver.Setup(p => p.Evolve(generationTwo, 0)).Returns(generationThree);
            populationEvolver.Setup(p => p.Evolve(generationThree, 0)).Returns(generationFour);
            populationEvolver.Setup(p => p.Evolve(generationFour, 0)).Returns(generationFive);

            // Add chromosomes to each generation.
            beginningPopulation.Add(new Chromosome<int>(new[] { 0 }));
            generationOne.Add(new Chromosome<int>(new[] { 1 }));
            generationTwo.Add(new Chromosome<int>(new[] { 2 }));
            generationThree.Add(new Chromosome<int>(new[] { 3 }));
            generationFour.Add(new Chromosome<int>(new[] { 4 }));
            generationFive.Add(new Chromosome<int>(new[] { 5 }));

            // Execute the code to test.
            ChromosomeCollection<int> endingPopulation = ga.Run(beginningPopulation, numberOfGenerations: 5, numberOfBestChromosomesToPromote: 0);

            // Validate that we got back the expected population.
            Assert.AreEqual(generationFive, endingPopulation);
        }

        /// <summary>
        /// Validates that the Run method updates the fitness on each chromosome in each population.
        /// </summary>
        [TestMethod]
        public void Run_UpdatesTheFitnessOnEachPopulation()
        {
            // Setup the mock dependencies of the GA.
            Mock<PopulationEvolver<int>> populationEvolver = new Mock<PopulationEvolver<int>>(null);
            Mock<ChromosomeFitnessCalculator<int>> fitnessCalculator = new Mock<ChromosomeFitnessCalculator<int>>();

            // Setup the GA with the mock dependencies.
            GA<int> ga = new GA<int>(fitnessCalculator.Object, populationEvolver.Object);

            // Setup the population that will be passed into the GA.
            ChromosomeCollection<int> beginningPopulation = new ChromosomeCollection<int>();

            // Setup other generations.
            ChromosomeCollection<int> generationOne = new ChromosomeCollection<int>();
            ChromosomeCollection<int> generationTwo = new ChromosomeCollection<int>();

            // Add chromosomes to each generation.
            beginningPopulation.Add(new Chromosome<int>(new[] { 0 }));
            generationOne.Add(new Chromosome<int>(new[] { 1 }));
            generationTwo.Add(new Chromosome<int>(new[] { 2 }));

            // Setup the population evolver to return the desired generation when given the previous generation.
            populationEvolver.Setup(p => p.Evolve(beginningPopulation, 0)).Returns(generationOne);
            populationEvolver.Setup(p => p.Evolve(generationOne, 0)).Returns(generationTwo);

            // Setup the fitness calculator to set the fitness on the chromosomes.
            fitnessCalculator.Setup(f => f.Calculate(It.IsAny<Chromosome<int>>())).Returns(1);

            // Execute the code to test.
            ChromosomeCollection<int> endingPopulation = ga.Run(beginningPopulation, numberOfGenerations: 2, numberOfBestChromosomesToPromote: 0);

            // Validate that the fitness was set on each chromosome in each population.
            Assert.IsTrue(beginningPopulation.All(c => c.Fitness == 1), "Beginning Population");
            Assert.IsTrue(generationOne.All(c => c.Fitness == 1), "Generation one Population");
            Assert.IsTrue(generationTwo.All(c => c.Fitness == 1), "Generation two (ending) Population");
        }

        /// <summary>
        /// Validates that the epoch event is fired for each generation.
        /// </summary>
        [TestMethod]
        public void Run_FiresEpochEventForEachGeneration()
        {
            // Setup the mock dependencies of the GA.
            Mock<PopulationEvolver<int>> populationEvolver = new Mock<PopulationEvolver<int>>(null);
            Mock<ChromosomeFitnessCalculator<int>> fitnessCalculator = new Mock<ChromosomeFitnessCalculator<int>>();

            // Setup the GA with the mock dependencies.
            GA<int> ga = new GA<int>(fitnessCalculator.Object, populationEvolver.Object);

            // Setup the population that will be passed into the GA.
            ChromosomeCollection<int> beginningPopulation = new ChromosomeCollection<int>();

            // Setup several other generations.
            ChromosomeCollection<int> generationOne = new ChromosomeCollection<int>();
            ChromosomeCollection<int> generationTwo = new ChromosomeCollection<int>();
            ChromosomeCollection<int> generationThree = new ChromosomeCollection<int>();

            // Setup the population evolver to return the desired generation when given the previous generation.
            populationEvolver.Setup(p => p.Evolve(beginningPopulation, 0)).Returns(generationOne);
            populationEvolver.Setup(p => p.Evolve(generationOne, 0)).Returns(generationTwo);
            populationEvolver.Setup(p => p.Evolve(generationTwo, 0)).Returns(generationThree);

            // Add chromosomes to each generation.
            beginningPopulation.Add(new Chromosome<int>(new[] { 0 }));
            generationOne.Add(new Chromosome<int>(new[] { 1 }));
            generationTwo.Add(new Chromosome<int>(new[] { 2 }));
            generationThree.Add(new Chromosome<int>(new[] { 3 }));

            List<ChromosomeCollection<int>> generationsEpoched = new List<ChromosomeCollection<int>>();

            // Attach to the epoch event, capturing which generations were epoched.
            ga.Epoch += (o, e) => generationsEpoched.Add(e.Data);

            // Execute the code to test.
            ga.Run(beginningPopulation, numberOfGenerations: 3, numberOfBestChromosomesToPromote: 0);

            // Validate that the event was raised for each generation epoched.
            Assert.AreEqual(3, generationsEpoched.Count, "Count");
            Assert.AreEqual(generationOne, generationsEpoched[0], "Generation One");
            Assert.AreEqual(generationTwo, generationsEpoched[1], "Generation Two");
            Assert.AreEqual(generationThree, generationsEpoched[2], "Generation Three");
        }

        /// <summary>
        /// Validates that the GA just passes the numberOfBestChromosomesToPromote argument to the PopulationEvolver for each epoch.
        /// </summary>
        [TestMethod]
        public void Run_ProvidesTheCorrectNumberOfChromosomesToTakeForwardToEvolver()
        {
            int numberOfChromosomesToTakeForward = 3;

            // Setup the mock dependencies of the GA.
            Mock<PopulationEvolver<int>> populationEvolver = new Mock<PopulationEvolver<int>>(null);
            Mock<ChromosomeFitnessCalculator<int>> fitnessCalculator = new Mock<ChromosomeFitnessCalculator<int>>();

            // Setup the GA with the mock dependencies.
            GA<int> ga = new GA<int>(fitnessCalculator.Object, populationEvolver.Object);

            // Setup the population that will be passed into the GA.
            ChromosomeCollection<int> beginningPopulation = new ChromosomeCollection<int>();

            // Setup another generation to return.
            ChromosomeCollection<int> generationOne = new ChromosomeCollection<int>();

            // Setup the population evolver to return the desired generation when given the previous generation.
            populationEvolver.Setup(p => p.Evolve(beginningPopulation, numberOfChromosomesToTakeForward)).Returns(generationOne);

            // Add chromosomes to each generation.
            beginningPopulation.Add(new Chromosome<int>(new[] { 0, 1, 4, 2, 3 }));
            generationOne.Add(new Chromosome<int>(new[] { 1, 1, 4, 3, 7 }));

            // Execute the code to test.
            ChromosomeCollection<int> endingPopulation = ga.Run(
                beginningPopulation, numberOfGenerations: 1, numberOfBestChromosomesToPromote: numberOfChromosomesToTakeForward);

            // Validate that we got back the expected population.
            Assert.AreEqual(generationOne, endingPopulation);
        }

        /// <summary>
        /// Validates that the Run method does not recalculate the fitness for the
        /// top number of chromosomes to bring forward to the next generation.
        /// This is because these chromosomes have already had their fitness calculated,
        /// and this may be a very expensive operation.
        /// To test this, we will setup a generation with four chromosomes.
        /// We will call the run method, with the intention of taking the best two chromosomes forward.
        /// We will also setup a mock fitness calculator that sets the fitness to a known value.
        /// We will make the ending population have two chromosomes that have a fitness and two that do not.
        /// The GA should only calculate the fitness on the chromosomes that do not have a fitness.
        /// We should be able to validate that two of the chromosomes in the ending population 
        /// have their old fitness, not the fitness set by the fitness calculator.
        /// </summary>
        [TestMethod]
        public void Run_DoesntRecalculateFitnessOnChromosomesBroughtForward()
        {
            int numberOfChromosomesToTakeForward = 2;

            // Setup the mock dependencies of the GA.
            Mock<PopulationEvolver<int>> populationEvolver = new Mock<PopulationEvolver<int>>(null);
            Mock<ChromosomeFitnessCalculator<int>> fitnessCalculator = new Mock<ChromosomeFitnessCalculator<int>>();

            // Setup the GA with the mock dependencies.
            GA<int> ga = new GA<int>(fitnessCalculator.Object, populationEvolver.Object);

            // Setup the population that will be passed into the GA.
            ChromosomeCollection<int> beginningPopulation = new ChromosomeCollection<int>();

            // Setup another generation to return.
            ChromosomeCollection<int> generationOne = new ChromosomeCollection<int>();

            // Setup the population evolver to return the desired generation when given the previous generation.
            populationEvolver.Setup(p => p.Evolve(beginningPopulation, numberOfChromosomesToTakeForward)).Returns(generationOne);

            // Add chromosomes to each generation.
            beginningPopulation.Add(new Chromosome<int>(new[] { 0, 1 }) { Fitness = 3.2 });

            // Add four chromosomes to the generation being returned, two of them having non-default fitnesses.
            generationOne.Add(new Chromosome<int>(new[] { 4, 3 }) { Fitness = 3.2 });
            generationOne.Add(new Chromosome<int>(new[] { 1, 3 }) { Fitness = 0.0 });
            generationOne.Add(new Chromosome<int>(new[] { 2, 2 }) { Fitness = 4.1 });
            generationOne.Add(new Chromosome<int>(new[] { 5, 1 }) { Fitness = 0.0 });

            // Setup the fitness calculator to return a known fitness.
            fitnessCalculator.Setup(f => f.Calculate(It.IsAny<Chromosome<int>>())).Returns(1.1);

            // Execute the code to test.
            ChromosomeCollection<int> endingPopulation = ga.Run(
                beginningPopulation, numberOfGenerations: 1, numberOfBestChromosomesToPromote: numberOfChromosomesToTakeForward);

            // Validate that two of the chromosomes in the returned generation have the fitness from the fitness calculator.
            Assert.AreEqual(2, endingPopulation.Count(c => c.Fitness == 1.1));

            // Validate that two of the chromosomes have their original fitness.
            Assert.AreEqual(1, endingPopulation.Count(c => c.Genes[0] == 4 && c.Genes[1] == 3 && c.Fitness == 3.2));
            Assert.AreEqual(1, endingPopulation.Count(c => c.Genes[0] == 2 && c.Genes[1] == 2 && c.Fitness == 4.1));
        }
    }
}