//-----------------------------------------------------------------------
// <copyright file="ChromosomeCollection.cs" company="Seth Flowers">
//     All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace GeneticAlgorithm
{
    using System.Collections.ObjectModel;

    /// <summary>
    /// Represents a population of chromosomes.
    /// </summary>
    /// <typeparam name="T">The type of each gene in the chromosomes.</typeparam>
    public class ChromosomeCollection<T> : Collection<Chromosome<T>>
    {
    }
}