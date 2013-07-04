//-----------------------------------------------------------------------
// <copyright file="RouletteSelectorTests.cs" company="Seth Flowers">
//     All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace GeneticAlgorithm.Tests
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    /// <summary>
    /// Tests for the ChromosomeSelector class.
    /// </summary>
    [TestClass]
    public class RouletteSelectorTests
    {
        /// <summary>
        /// Validates that the Choose method throws a meaningful exception if the chromosomes argument is null.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Choose_NullChromosomesArgument_ThrowsMeaningfulException()
        {
            try
            {
                new RouletteSelector<int>()
                    .Choose(chromosomes: null, totalFitness: 1);
            }
            catch (ArgumentNullException argumentNullException)
            {
                Assert.AreEqual(
                    string.Format("Unable to select a chromosome from a null collection.{0}Parameter name: chromosomes", Environment.NewLine),
                    argumentNullException.Message);

                throw;
            }
        }

        /// <summary>
        /// Validates that the Choose method throws a meaningful exception if the chromosomes argument is empty.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Choose_EmptyChromosomesArgument_ThrowsMeaningfulException()
        {
            try
            {
                new RouletteSelector<int>()
                    .Choose(chromosomes: new ChromosomeCollection<int>(), totalFitness: 1);
            }
            catch (ArgumentException argumentException)
            {
                Assert.AreEqual(
                    string.Format("Unable to select a chromosome from an empty collection.{0}Parameter name: chromosomes", Environment.NewLine),
                    argumentException.Message);

                throw;
            }
        }

        /// <summary>
        /// Validates that the Choose method returns the correct chromosome based on a calculation
        /// involving all the chromosomes fitness.
        /// The Choose method uses a roulette selection meaning the higher a chromosomes fitness,
        /// the more likely it is to get chosen.
        /// To test this, we can setup the selector with a mocked random generator and some simple chromosomes.
        /// </summary>
        [TestMethod]
        public void Choose_ValidChromosomes_ReturnsCorrectChromosomeBasedOnFitness()
        {
            // Setup a population of simple chromosomes.
            ChromosomeCollection<int> chromosomes = new ChromosomeCollection<int>();

            // Add 10 chromosomes, all with fitness of 1.
            for (int i = 0; i < 10; i++)
            {
                chromosomes.Add(new Chromosome<int>(new[] { 1, 2, 3 }) { Fitness = 1 });
            }

            Mock<Random> random = new Mock<Random>();
            ChromosomeSelector<int> selector = new RouletteSelector<int>(random.Object);

            // Setup the random generator to return 0.7.
            // This means that the selector will multiply this value by the total fitness to get 7.
            // It then iterates through the chromosomes tallying up their individual fitness 
            // until the sum is greater than or equal to the calculated 7.
            // Since we setup the chromosomes to all have fitness 1, 
            // this means that we should get back the seventh chromosome.
            random.Setup(r => r.NextDouble()).Returns(0.7d);

            // Execute the actual method we are testing.
            Chromosome<int> actual = selector.Choose(chromosomes, 10);

            Assert.AreEqual(chromosomes[6], actual);
        }

        /// <summary>
        /// Validates that if the total fitness passed into the Choose method is higher than 
        /// the combined total of all the chromosomes passed into the method, 
        /// that a meaningful exception is thrown.
        /// To test this, we will have to mock the random generator so that it returns something high, like 1.
        /// This is important because the roulette selection method multiples the total fitness passed in
        /// by a randomly generated value to come up with a target. 
        /// It then sums the fitness of all the chromosomes until the sum meets or exceeds the target,
        /// returning the last chromosome added to the sum.
        /// If we mock the random generator to return 1, then we take it out of the equation and are just interested
        /// in the fitness of each chromosome.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Choose_TotalFitnessHigherThanExpected_ThrowsMeaningfulException()
        {
            // Setup a population of simple chromosomes.
            ChromosomeCollection<int> chromosomes = new ChromosomeCollection<int>();

            // Add 10 chromosomes, all with fitness of 1.
            for (int i = 0; i < 10; i++)
            {
                chromosomes.Add(new Chromosome<int>(new[] { 1, 2, 3 }) { Fitness = 1 });
            }

            // Setup the random generator to return 1, taking it out of the equation.
            Mock<Random> random = new Mock<Random>();
            random.Setup(r => r.NextDouble()).Returns(1);
            ChromosomeSelector<int> selector = new RouletteSelector<int>(random.Object);

            try
            {
                // Execute the actual method we are testing.
                // Pass in a total greater than the actual total.
                selector.Choose(chromosomes, 11);
            }
            catch (ArgumentOutOfRangeException argumentOutOfRangeException)
            {
                Assert.AreEqual(
                    string.Format("The totalFitness cannot exceed the sum total of all the chromosomes.{0}Parameter name: totalFitness", Environment.NewLine),
                    argumentOutOfRangeException.Message);

                throw;
            }
        }
    }
}