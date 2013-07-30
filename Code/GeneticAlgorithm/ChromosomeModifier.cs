﻿//-----------------------------------------------------------------------
// <copyright file="ChromosomeModifier.cs" company="Seth Flowers">
//     All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace GeneticAlgorithm
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Provides a mechanism to alter chromosomes in a biologically inspired manner.
    /// </summary>
    /// <typeparam name="T">The type of genes in the chromosome.</typeparam>
    public class ChromosomeModifier<T>
    {
        /// <summary>
        /// The rate at which a mutation should occur to a single chromosome.
        /// This should be a number between 0 and 1, and represents a percent.
        /// </summary>
        private readonly double mutationRate;

        /// <summary>
        /// The rate at which a crossover should occur between genes in two chromosomes.
        /// This should be a number between 0 and 1, and represents a percent.
        /// </summary>
        private readonly double crossoverRate;

        /// <summary>
        /// Represents a pseudo-random number generator, a device that produces a sequence
        /// of numbers that meet certain statistical requirements for randomness.
        /// </summary>
        private readonly Random randomGenerator;

        /// <summary>
        /// The number of crossover points to use during the crossover mechanism.
        /// </summary>
        private readonly int numberOfCrossoverPoints;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChromosomeModifier{T}"/> class.
        /// </summary>
        public ChromosomeModifier()
            : this(new Random())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChromosomeModifier{T}" /> class.
        /// </summary>
        /// <param name="randomGenerator">Represents a pseudo-random number generator, a device that produces a sequence of numbers that meet certain statistical requirements for randomness.</param>
        public ChromosomeModifier(Random randomGenerator)
            : this(randomGenerator, 0.07d, 0.02d, 2)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChromosomeModifier{T}" /> class.
        /// </summary>
        /// <param name="randomGenerator">Represents a pseudo-random number generator, a device that produces a sequence of numbers that meet certain statistical requirements for randomness.</param>
        /// <param name="mutationRate">The rate at which a mutation should occur to a single chromosome. This should be a number between 0 and 1, and represents a percent.</param>
        /// <param name="crossoverRate">The rate at which a crossover should occur between genes in two chromosomes. This should be a number between 0 and 1, and represents a percent.</param>
        /// <param name="numberOfCrossoverPoints">The number of crossover points to use during the crossover mechanism.</param>
        public ChromosomeModifier(
            Random randomGenerator, 
            double mutationRate, 
            double crossoverRate, 
            int numberOfCrossoverPoints)
        {
            this.randomGenerator = randomGenerator;
            this.mutationRate = mutationRate;
            this.crossoverRate = crossoverRate;
            this.numberOfCrossoverPoints = numberOfCrossoverPoints;
        }

        /// <summary>
        /// Mutates the genes in the given chromosome based on the mutation rate.
        /// Each gene has a chance to be mutated with it's neighbor.
        /// </summary>
        /// <param name="chromosome">The chromosome to potentially mutate.</param>
        public virtual void Mutate(Chromosome<T> chromosome)
        {
            if (chromosome == null)
            {
                throw new ArgumentNullException("chromosome", "Unable to mutate a null chromosome.");
            }
            else if (chromosome.Genes == null || chromosome.Genes.Count == 0)
            {
                throw new ArgumentException("Unable to mutate a chromosome with no genes.", "chromosome");
            }

            for (int i = 0; i < chromosome.Genes.Count; i++)
            {
                int nextIndex = i < chromosome.Genes.Count - 1 ? i + 1 : 0;

                if (this.randomGenerator.NextDouble() < this.mutationRate)
                {
                    SwapGenes(chromosome, i, chromosome, nextIndex);
                }
            }
        }

        /// <summary>
        /// <para>Swaps genes from the two given chromosomes based on the crossover rate.</para>
        /// <para>This performs a one-point crossover.</para>
        /// </summary>
        /// <param name="dad">The first chromosome to use to swap genes.</param>
        /// <param name="mom">The second chromosome to use to swap genes.</param>
        public virtual void Crossover(Chromosome<T> dad, Chromosome<T> mom)
        {
            if (dad == null)
            {
                throw new ArgumentNullException("dad", "Unable to crossover a null chromosome.");
            }
            else if (mom == null)
            {
                throw new ArgumentNullException("mom", "Unable to crossover a null chromosome.");
            }
            else if (dad.Genes == null || dad.Genes.Count == 0)
            {
                throw new ArgumentException("Unable to crossover a chromosome with no genes.", "dad");
            }
            else if (mom.Genes == null || mom.Genes.Count == 0)
            {
                throw new ArgumentException("Unable to crossover a chromosome with no genes.", "mom");
            }

            if (this.randomGenerator.NextDouble() < this.crossoverRate)
            {
                IList<int> crossoverPoints = new List<int>();

                for (int i = 0; i < this.numberOfCrossoverPoints; i++)
                {
                    crossoverPoints.Add(this.randomGenerator.Next(dad.Genes.Count));
                }

                if (crossoverPoints.Any())
                {
                    for (int i = 0; i < crossoverPoints.Count; i += 2)
                    {
                        int currentCrossoverPoint = crossoverPoints[i];
                        int stop = i < crossoverPoints.Count - 1 ? crossoverPoints[i + 1] : dad.Genes.Count;

                        for (int j = currentCrossoverPoint; j < stop; j++)
                        {
                            SwapGenes(dad, j, mom, j);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Swaps a gene at the given index in the first chromosome, with the gene at the given index in the second chromosome.
        /// </summary>
        /// <param name="a">The first chromosome to swap a gene from.</param>
        /// <param name="indexInA">The index of the gene in the first chromosome to swap.</param>
        /// <param name="b">The second chromosome to swap a gene from.</param>
        /// <param name="indexInB">The index of the gene in the second chromosome to swap.</param>
        private static void SwapGenes(Chromosome<T> a, int indexInA, Chromosome<T> b, int indexInB)
        {
            T temp = a.Genes[indexInA];
            a.Genes[indexInA] = b.Genes[indexInB];
            b.Genes[indexInB] = temp;
        }
    }
}