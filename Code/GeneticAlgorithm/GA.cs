//-----------------------------------------------------------------------
// <copyright file="GA.cs" company="Seth Flowers">
//     All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace GeneticAlgorithm
{
    using System;

    /// <summary>
    /// A generic Genetic Algorithm.
    /// </summary>
    /// <typeparam name="T">The type of data represented by a gene.</typeparam>
    public class GA<T>
    {
        /// <summary>
        /// Provides a mechanism to evolve a population of chromosomes into a new population.
        /// </summary>
        private readonly PopulationEvolver<T> populationEvolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="GA{T}" /> class.
        /// </summary>
        /// <param name="fitnessCalculator">Provides a mechanism to determine a fitness for a chromosome.</param>
        public GA(ChromosomeFitnessCalculator<T> fitnessCalculator) : 
            this(new PopulationEvolver<T>(fitnessCalculator))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GA{T}" /> class.
        /// </summary>
        /// <param name="populationEvolver">Provides a mechanism to evolve a population of chromosomes into a new population.</param>
        public GA(PopulationEvolver<T> populationEvolver)
        {
            this.populationEvolver = populationEvolver;
        }

        /// <summary>
        /// Runs the genetic algorithm the given number of generations, starting with the beginning population of chromosomes as the first generation.
        /// </summary>
        /// <param name="beginningPopulation">The beginning population of chromosomes that make up the possible solutions to the problem
        /// being solved by this genetic algorithm.</param>
        /// <param name="numberOfGenerations">The number of generations to evolve the beginning population.</param>
        /// <returns>
        /// Returns a new population of chromosomes.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">beginningPopulation;Unable to run a genetic algorithm with a null beginning population of chromosomes.</exception>
        public ChromosomeCollection<T> Run(
            ChromosomeCollection<T> beginningPopulation,
            int numberOfGenerations)
        {
            if (beginningPopulation == null)
            {
                throw new ArgumentNullException(
                    "beginningPopulation",
                    "Unable to run a genetic algorithm with a null beginning population of chromosomes.");
            }

            ChromosomeCollection<T> currentPopulation = beginningPopulation;

            while (numberOfGenerations-- > 0)
            {
                currentPopulation = this.populationEvolver.Evolve(currentPopulation);
            }

            return currentPopulation;
        }
    }
}