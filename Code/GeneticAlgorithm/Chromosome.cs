//-----------------------------------------------------------------------
// <copyright file="Chromosome.cs" company="Seth Flowers">
//     All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace GeneticAlgorithm
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents a potential solution to the problem being solved by the genetic algorithm.
    /// </summary>
    /// <typeparam name="T">The type of each gene in this chromosome.</typeparam>
    public class Chromosome<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Chromosome{T}"/> class.
        /// </summary>
        /// <param name="genes">The weights passed to the fitness function, that together represent a possible solution to the problem being solved by the genetic algorithm.</param>
        public Chromosome(IList<T> genes)
        {
            this.Genes = genes;
        }

        /// <summary>
        /// Gets the weights passed to the fitness function, that together represent a possible solution to the problem being solved by the genetic algorithm.
        /// </summary>
        public IList<T> Genes { get; private set; }

        /// <summary>
        /// Gets or sets the fitness for this chromosome.
        /// </summary>
        /// <value>
        /// The fitness.
        /// </value>
        public double Fitness { get; set; }
        
        //// need to add a fitness property, and update it from GA, and get rid of CalculateFitness here.
        // I then need to make the selector return a clone of the chromosome, so our new population is not referencing 
        // the same chromosomes as the old population.
    }
}