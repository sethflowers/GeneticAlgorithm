//-----------------------------------------------------------------------
// <copyright file="RouletteSelector.cs" company="Seth Flowers">
//     All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace GeneticAlgorithm
{
    using System;
    using System.Linq;

    /// <summary>
    /// Provides a mechanism to select a chromosome from a population of chromosomes based on some fitness criteria.
    /// </summary>
    /// <typeparam name="T">The type of data represented by a gene.</typeparam>
    public class RouletteSelector<T> : ChromosomeSelector<T>
    {
        /// <summary>
        /// Represents a pseudo-random number generator, a device that produces a sequence
        /// of numbers that meet certain statistical requirements for randomness.
        /// </summary>
        private readonly Random randomGenerator;

        /// <summary>
        /// Initializes a new instance of the <see cref="RouletteSelector{T}" /> class.
        /// </summary>
        public RouletteSelector()
            : this(new Random())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RouletteSelector{T}" /> class.
        /// </summary>
        /// <param name="randomGenerator">Represents a pseudo-random number generator, a device that produces a sequence of numbers that meet certain statistical requirements for randomness.</param>
        public RouletteSelector(Random randomGenerator)
        {
            this.randomGenerator = randomGenerator;
        }

        /// <summary>
        /// Returns a chromosome based on a roulette selection where the chance of being selected is
        /// proportional to the chromosomes fitness compared to the total fitness of all chromosomes.
        /// Chromosomes with a higher fitness have a higher likelihood of being chosen.
        /// However, all chromosomes have a non-zero chance of being selected.
        /// </summary>
        /// <param name="chromosomes">The chromosomes.</param>
        /// <param name="totalFitness">The total fitness.</param>
        /// <returns>
        /// Returns a chromosome based on a roulette selection where the chance of being selected is proportional to the chromosomes fitness compared to the total fitness of all chromosomes.
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

            double target = this.randomGenerator.NextDouble() * totalFitness;
            double sum = 0;

            foreach (Chromosome<T> chromosome in chromosomes)
            {
                sum += chromosome.Fitness;

                if (sum >= target)
                {
                    return chromosome;
                }
            }

            throw new ArgumentOutOfRangeException("totalFitness", "The totalFitness cannot exceed the sum total of all the chromosomes.");
        }
    }
}