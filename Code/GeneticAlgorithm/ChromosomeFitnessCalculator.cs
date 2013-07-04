//-----------------------------------------------------------------------
// <copyright file="ChromosomeFitnessCalculator.cs" company="Seth Flowers">
//     All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace GeneticAlgorithm
{
    /// <summary>
    /// Provides a mechanism to determine a fitness for a chromosome.
    /// </summary>
    /// <typeparam name="T">The type of genes in the chromosome.</typeparam>
    public abstract class ChromosomeFitnessCalculator<T>
    {
        /// <summary>
        /// Calculates the fitness for the specified chromosome.
        /// </summary>
        /// <param name="chromosome">The chromosome.</param>
        /// <returns>Returns the fitness for the given chromosome.</returns>
        public abstract double Calculate(Chromosome<T> chromosome);
    }
}