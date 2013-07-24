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
                    numberOfGenerations: 10);
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
            populationEvolver.Setup(p => p.Evolve(beginningPopulation)).Returns(generationOne);
            populationEvolver.Setup(p => p.Evolve(generationOne)).Returns(generationTwo);
            populationEvolver.Setup(p => p.Evolve(generationTwo)).Returns(generationThree);
            populationEvolver.Setup(p => p.Evolve(generationThree)).Returns(generationFour);
            populationEvolver.Setup(p => p.Evolve(generationFour)).Returns(generationFive);

            // Execute the code to test.
            ChromosomeCollection<int> endingPopulation = ga.Run(beginningPopulation, numberOfGenerations: 5);

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
            beginningPopulation.Add(new Chromosome<int>(new[] { 1 }));
            generationOne.Add(new Chromosome<int>(new[] { 2 }));
            generationTwo.Add(new Chromosome<int>(new[] { 3 }));
            
            // Setup the population evolver to return the desired generation when given the previous generation.
            populationEvolver.Setup(p => p.Evolve(beginningPopulation)).Returns(generationOne);
            populationEvolver.Setup(p => p.Evolve(generationOne)).Returns(generationTwo);
            
            // Setup the fitness calculator to set the fitness on the chromosomes.
            fitnessCalculator.Setup(f => f.Calculate(It.IsAny<Chromosome<int>>())).Returns(1);

            // Execute the code to test.
            ChromosomeCollection<int> endingPopulation = ga.Run(beginningPopulation, numberOfGenerations: 2);

            // Validate that the fitness was set on each chromosome in each population.
            Assert.IsTrue(beginningPopulation.All(c => c.Fitness == 1), "Beginning Population");
            Assert.IsTrue(generationOne.All(c => c.Fitness == 1), "Generation one Population");
            Assert.IsTrue(generationTwo.All(c => c.Fitness == 1), "Generation two (ending) Population");
        }
    }
}