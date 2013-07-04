//-----------------------------------------------------------------------
// <copyright file="DistinctGeneValidator.cs" company="Seth Flowers">
//     All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace GeneticAlgorithm
{
    using System.Linq;

    /// <summary>
    /// Provides a mechanism to determine if the solution represented by a given chromosome is valid.
    /// </summary>
    /// <typeparam name="T">The type of data represented by a gene.</typeparam>
    public class DistinctGeneValidator<T> : ChromosomeValidator<T>
    {
        /// <summary>
        /// Determines whether the specified chromosome is valid.
        /// </summary>
        /// <param name="chromosome">The chromosome.</param>
        /// <returns>
        ///   <c>true</c> if the specified chromosome is valid; otherwise, <c>false</c>.
        /// </returns>
        public override bool IsValid(Chromosome<T> chromosome)
        {
            if (chromosome == null)
            {
                return false;
            }

            return chromosome.Genes.Distinct().Count() == chromosome.Genes.Count;
        }
    }
}