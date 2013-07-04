//-----------------------------------------------------------------------
// <copyright file="ChromosomeSelector.cs" company="Seth Flowers">
//     All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace GeneticAlgorithm
{
    /// <summary>
    /// Provides a mechanism to select a chromosome from a population of chromosomes based on some fitness criteria.
    /// </summary>
    /// <typeparam name="T">The type of data represented by a gene.</typeparam>
    public abstract class ChromosomeSelector<T>
    {
        /// <summary>
        /// Returns a chromosome where the chance of being selected is
        /// proportional to the chromosomes fitness compared to the total fitness of all chromosomes.
        /// Chromosomes with a higher fitness have a higher likelihood of being chosen.
        /// However, all chromosomes have a non-zero chance of being selected.
        /// </summary>
        /// <param name="chromosomes">The chromosomes.</param>
        /// <param name="totalFitness">The total fitness.</param>
        /// <returns>
        /// Returns a chromosome where the chance of being selected is proportional to the chromosomes fitness compared to the total fitness of all chromosomes.
        /// </returns>
        public abstract Chromosome<T> Choose(
            ChromosomeCollection<T> chromosomes,
            double totalFitness);
    }
}