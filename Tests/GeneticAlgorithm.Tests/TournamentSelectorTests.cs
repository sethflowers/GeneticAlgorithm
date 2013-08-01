//-----------------------------------------------------------------------
// <copyright file="TournamentSelectorTests.cs" company="Seth Flowers">
//     All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace GeneticAlgorithm.Tests
{
    using System;
    using System.Collections.Generic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    /// <summary>
    /// Tests for the TournamentSelector class.
    /// </summary>
    [TestClass]
    public class TournamentSelectorTests
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
                new TournamentSelector<int>()
                    .Choose(chromosomes: null, totalFitness: 0);
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
                new TournamentSelector<int>()
                    .Choose(chromosomes: new ChromosomeCollection<int>(), totalFitness: 0);
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
        /// Validates that the Choose method throws a meaningful exception if the number of players is out of bounds.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Choose_NumberOfPlayersOutOfBounds_ThrowsMeaningfulException()
        {
            try
            {
                ChromosomeCollection<int> chromosomes = new ChromosomeCollection<int>();
                chromosomes.Add(new Chromosome<int>(new[] { 1 }));

                new TournamentSelector<int>(new Random(), numberOfPlayers: -1)
                    .Choose(chromosomes, totalFitness: 0);
            }
            catch (ArgumentOutOfRangeException exception)
            {
                Assert.AreEqual(
                    string.Format("The number of players in a tournament selection needs to be between 2 and the size of the population.{0}Parameter name: chromosomes", Environment.NewLine),
                    exception.Message);

                throw;
            }
        }

        /// <summary>
        /// Validates that Choose selects the most fit individual from a group of individuals.
        /// </summary>
        [TestMethod]
        public void Choose_ChoosestCorrectIndividualFromTournament()
        {
            ChromosomeCollection<int> chromosomes = new ChromosomeCollection<int>();

            // Add a population of chromosomes with different fitnesses.
            chromosomes.Add(new Chromosome<int>(new[] { 1 }) { Fitness = 3 });
            chromosomes.Add(new Chromosome<int>(new[] { 2 }) { Fitness = 1 });
            chromosomes.Add(new Chromosome<int>(new[] { 3 }) { Fitness = 4 });
            chromosomes.Add(new Chromosome<int>(new[] { 4 }) { Fitness = 2 });
            chromosomes.Add(new Chromosome<int>(new[] { 5 }) { Fitness = 5 });

            // Setup a mock random generator, so we know what indexes will be returned, 
            // which means we know what individual chromosomes will be in the tournament.
            Queue<int> indexes = new Queue<int>(new[] { 0, 2, 3 });
            Mock<Random> random = new Mock<Random>();
            random.Setup(r => r.Next(chromosomes.Count)).Returns(() => indexes.Dequeue());

            // Create the selector.
            TournamentSelector<int> selector = new TournamentSelector<int>(
                random.Object, numberOfPlayers: indexes.Count);

            // Execute the code to test.
            Chromosome<int> actual = selector.Choose(chromosomes, totalFitness: 0);

            // Validate that we received the correct chromosome.
            Assert.AreEqual(chromosomes[2], actual);
        }
    }
}