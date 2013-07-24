//-----------------------------------------------------------------------
// <copyright file="PopulationEvolver.cs" company="Seth Flowers">
//     All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace GeneticAlgorithm
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Provides a mechanism to evolve a population of chromosomes into a new population.
    /// </summary>
    /// <typeparam name="T">The type of data represented by a gene.</typeparam>
    public class PopulationEvolver<T>
    {
        /// <summary>
        /// Provides a mechanism to select a chromosome from a population of chromosomes based on some fitness criteria.
        /// </summary>
        private readonly ChromosomeSelector<T> selector;

        /// <summary>
        /// Provides a mechanism to alter chromosomes in a biologically inspired manner.
        /// </summary>
        private readonly ChromosomeModifier<T> modifier;

        /// <summary>
        /// Provides a mechanism to determine if the solution represented by a given chromosome is valid.
        /// </summary>
        private readonly ChromosomeValidator<T> validator;

        /// <summary>
        /// Initializes a new instance of the <see cref="PopulationEvolver{T}" /> class.
        /// </summary>
        public PopulationEvolver()
            : this(new Random())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PopulationEvolver{T}" /> class.
        /// </summary>
        /// <param name="randomGenerator">Represents a pseudo-random number generator, a device that produces a sequence of numbers that meet certain statistical requirements for randomness.</param>
        public PopulationEvolver(Random randomGenerator)
            : this(
                new RouletteSelector<T>(randomGenerator),
                new ChromosomeModifier<T>(randomGenerator),
                new GenericChromosomeValidator<T>(c => true))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PopulationEvolver{T}" /> class.
        /// </summary>
        /// <param name="selector">Provides a mechanism to select a chromosome from a population of chromosomes based on some fitness criteria.</param>
        /// <param name="modifier">Provides a mechanism to alter chromosomes in a biologically inspired manner.</param>
        /// <param name="validator">Provides a mechanism to determine if the solution represented by a given chromosome is valid.</param>
        public PopulationEvolver(
            ChromosomeSelector<T> selector,
            ChromosomeModifier<T> modifier,
            ChromosomeValidator<T> validator)
        {
            this.selector = selector;
            this.modifier = modifier;
            this.validator = validator;
        }

        /// <summary>
        /// Creates a new population from the chromosomes.
        /// </summary>
        /// <param name="currentPopulation">The current population of chromosomes that make up the possible solutions to the problem
        /// being solved by this genetic algorithm.</param>
        /// <param name="numberOfBestChromosomesToPromote">The number of best chromosomes each generation to automatically promote to the next generation.</param>
        /// <returns>
        /// Returns a new population of chromosomes.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">currentPopulation;Unable to create a new population from a null population.</exception>
        public virtual ChromosomeCollection<T> Evolve(
            ChromosomeCollection<T> currentPopulation,
            int numberOfBestChromosomesToPromote)
        {
            if (currentPopulation == null)
            {
                throw new ArgumentNullException(
                    "currentPopulation",
                    "Unable to create a new population from a null population.");
            }

            // Create a new population of chromosomes.
            ChromosomeCollection<T> newPopulation = new ChromosomeCollection<T>();

            // Bring forward the designated number of most fit chromosomes into the next generation.
            PromoteBestChromosomes(
                numberOfBestChromosomesToPromote, 
                currentPopulation, 
                newPopulation);
            
            // Get the current populations total fitness.
            double totalFitness = currentPopulation.Sum(c => c.Fitness);

            // Select, mutate, and add chromosomes from the current population to the new population.
            while (newPopulation.Count < currentPopulation.Count)
            {
                Chromosome<T> dad = this.selector.Choose(currentPopulation, totalFitness);
                Chromosome<T> mom = this.selector.Choose(currentPopulation, totalFitness);

                // Create clones of the selected chromosomes, so we can modify them and add them to the new population.
                Chromosome<T> dadClone = new Chromosome<T>(dad.Genes.ToList());
                Chromosome<T> momClone = new Chromosome<T>(mom.Genes.ToList());

                this.modifier.Crossover(dadClone, momClone);
                this.modifier.Mutate(dadClone);
                this.modifier.Mutate(momClone);

                this.AddToPopulationIfValid(dadClone, newPopulation, currentPopulation.Count);
                this.AddToPopulationIfValid(momClone, newPopulation, currentPopulation.Count);
            }

            return newPopulation;
        }

        /// <summary>
        /// <para>Promotes the desired number of most fit chromosomes from the current generation to the next generation.</para>
        /// <para>Note that if the parameter is negative, then no exception is thrown, but no chromosomes are promoted.</para>
        /// <para>Likewise, if the parameter is greater than the count in the current generation, we only take the amount in the current generation.</para>
        /// <para>This would be a pointless value though, since there is no room for evolving.</para>
        /// <para>Every generation would be identical.</para>
        /// </summary>
        /// <param name="numberOfBestChromosomesToPromote">The number of best chromosomes to promote.</param>
        /// <param name="currentPopulation">The current population.</param>
        /// <param name="newPopulation">The new population.</param>
        private static void PromoteBestChromosomes(
            int numberOfBestChromosomesToPromote, 
            ChromosomeCollection<T> currentPopulation, 
            ChromosomeCollection<T> newPopulation)
        {
            int numberToTake = Math.Min(
                Math.Max(0, numberOfBestChromosomesToPromote),
                currentPopulation.Count);

            IEnumerable<Chromosome<T>> chromosomesToTake = 
                currentPopulation.OrderByDescending(c => c.Fitness).Take(numberToTake);

            foreach (Chromosome<T> chromosome in chromosomesToTake)
            {
                // Add a clone of the chromosome to the new populations.
                newPopulation.Add(
                    new Chromosome<T>(chromosome.Genes.ToList()) 
                    { 
                        Fitness = chromosome.Fitness 
                    });
            }
        }

        /// <summary>
        /// Adds the given chromosome to the new population if it represents a valid solution to the problem being solved by the genetic algorithm.
        /// </summary>
        /// <param name="chromosome">The chromosome to add to the population if it represents a valid solution.</param>
        /// <param name="newPopulation">The new population.</param>
        /// <param name="requiredPopulationSize">The required size of the new population.</param>
        private void AddToPopulationIfValid(
            Chromosome<T> chromosome,
            ChromosomeCollection<T> newPopulation,
            int requiredPopulationSize)
        {
            if (newPopulation.Count < requiredPopulationSize && this.validator.IsValid(chromosome))
            {
                newPopulation.Add(chromosome);
            }
        }
    }
}