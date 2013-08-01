//-----------------------------------------------------------------------
// <copyright file="TournamentSelector.cs" company="Seth Flowers">
//     All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace GeneticAlgorithm
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Provides a mechanism to select a chromosome from a population of chromosomes based on some fitness criteria.
    /// </summary>
    /// <typeparam name="T">The type of data represented by a gene.</typeparam>
    public class TournamentSelector<T> : ChromosomeSelector<T>
    {
        /// <summary>
        /// Represents a pseudo-random number generator, a device that produces a sequence
        /// of numbers that meet certain statistical requirements for randomness.
        /// </summary>
        private readonly Random randomGenerator;

        /// <summary>
        /// The number of players to take part in each tournament.
        /// </summary>
        private readonly int numberOfPlayers;

        /// <summary>
        /// Initializes a new instance of the <see cref="TournamentSelector{T}" /> class.
        /// </summary>
        public TournamentSelector()
            : this(new Random(), numberOfPlayers: 1)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TournamentSelector{T}" /> class.
        /// </summary>
        /// <param name="randomGenerator">Represents a pseudo-random number generator, a device that produces a sequence of numbers that meet certain statistical requirements for randomness.</param>
        /// <param name="numberOfPlayers">The number of players to take part in each tournament.</param>
        public TournamentSelector(Random randomGenerator, int numberOfPlayers)
        {
            this.randomGenerator = randomGenerator;
            this.numberOfPlayers = numberOfPlayers;
        }

        /// <summary>
        /// Gets a chromosome based on a tournament selection where N individuals are
        /// randomly selected from the population and then the most fit individual from that selection is returned.
        /// </summary>
        /// <param name="chromosomes">The chromosomes.</param>
        /// <param name="totalFitness">The total fitness. This argument is ignored.</param>
        /// <returns>
        /// Returns a chromosome based on a tournament selection where N individuals are
        /// randomly selected from the population and then the most fit individual from that selection is returned.
        /// </returns>
        public override Chromosome<T> Choose(
            ChromosomeCollection<T> chromosomes,
            double totalFitness)
        {
            if (chromosomes == null)
            {
                throw new ArgumentNullException("chromosomes", "Unable to select a chromosome from a null collection.");
            }
            else if (chromosomes.Count == 0)
            {
                throw new ArgumentException("Unable to select a chromosome from an empty collection.", "chromosomes");
            }
            else if (this.numberOfPlayers < 2 || this.numberOfPlayers > chromosomes.Count)
            {
                throw new ArgumentOutOfRangeException("numberOfPlayers", "The number of players in a tournament selection needs to be between 2 and the size of the population.");
            }

            HashSet<Chromosome<T>> players = new HashSet<Chromosome<T>>();

            while (players.Count < this.numberOfPlayers)
            {
                Chromosome<T> currentChromosome = chromosomes[this.randomGenerator.Next(chromosomes.Count)];
                players.Add(currentChromosome);
            }

            return players.OrderByDescending(c => c.Fitness).First();
        }
    }
}