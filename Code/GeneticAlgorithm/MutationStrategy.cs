//-----------------------------------------------------------------------
// <copyright file="MutationStrategy.cs" company="Seth Flowers">
//     All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace GeneticAlgorithm
{
    /// <summary>
    /// Represents the type of mutation the Chromosome modifier will use.
    /// </summary>
    public enum MutationStrategy
    {
        /// <summary>
        /// The gene will be swapped with an adjacent gene.
        /// </summary>
        Adjacent,

        /// <summary>
        /// The gene will be swapped with a random gene in the chromosome.
        /// </summary>
        Random,

        /// <summary>
        /// The gene will be replaced with the output of a client-defined function.
        /// </summary>
        Function
    }
}