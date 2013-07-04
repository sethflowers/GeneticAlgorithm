//-----------------------------------------------------------------------
// <copyright file="ChromosomeValidator.cs" company="Seth Flowers">
//     All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace GeneticAlgorithm
{
    /// <summary>
    /// Provides a mechanism to determine if the solution represented by a given chromosome is valid.
    /// </summary>
    /// <typeparam name="T">The type of data represented by a gene.</typeparam>
    public abstract class ChromosomeValidator<T>
    {
        /// <summary>
        /// Determines whether the specified chromosome represents a valid solution for the problem being solved by the genetic algorithm.
        /// </summary>
        /// <param name="chromosome">The chromosome.</param>
        /// <returns>
        ///   <c>true</c> if the specified chromosome is valid; otherwise, <c>false</c>.
        /// </returns>
        public abstract bool IsValid(Chromosome<T> chromosome);
    }
}