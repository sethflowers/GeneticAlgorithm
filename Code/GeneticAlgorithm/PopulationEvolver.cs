﻿//-----------------------------------------------------------------------
// <copyright file="PopulationEvolver.cs" company="Seth Flowers">
//     All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace GeneticAlgorithm
{
    using System;
    using System.Linq;

    /// <summary>
    /// Provides a mechanism to evolve a population of chromosomes into a new population.
    /// </summary>
    /// <typeparam name="T">The type of data represented by a gene.</typeparam>
    public class PopulationEvolver<T>
    {
        /// <summary>
        /// Provides a mechanism to determine a fitness for a chromosome.
        /// </summary>
        private readonly ChromosomeFitnessCalculator<T> fitnessCalculator;

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
        /// <param name="fitnessCalculator">Provides a mechanism to determine a fitness for a chromosome.</param>
        public PopulationEvolver(ChromosomeFitnessCalculator<T> fitnessCalculator)
            : this(fitnessCalculator, new Random())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PopulationEvolver{T}" /> class.
        /// </summary>
        /// <param name="fitnessCalculator">Provides a mechanism to determine a fitness for a chromosome.</param>
        /// <param name="randomGenerator">Represents a pseudo-random number generator, a device that produces a sequence of numbers that meet certain statistical requirements for randomness.</param>
        public PopulationEvolver(ChromosomeFitnessCalculator<T> fitnessCalculator, Random randomGenerator)
            : this(
                fitnessCalculator,
                new RouletteSelector<T>(randomGenerator),
                new ChromosomeModifier<T>(randomGenerator),
                new DistinctGeneValidator<T>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PopulationEvolver{T}" /> class.
        /// </summary>
        /// <param name="fitnessCalculator">Provides a mechanism to determine a fitness for a chromosome.</param>
        /// <param name="selector">Provides a mechanism to select a chromosome from a population of chromosomes based on some fitness criteria.</param>
        /// <param name="modifier">Provides a mechanism to alter chromosomes in a biologically inspired manner.</param>
        /// <param name="validator">Provides a mechanism to determine if the solution represented by a given chromosome is valid.</param>
        public PopulationEvolver(
            ChromosomeFitnessCalculator<T> fitnessCalculator,
            ChromosomeSelector<T> selector,
            ChromosomeModifier<T> modifier,
            ChromosomeValidator<T> validator)
        {
            this.fitnessCalculator = fitnessCalculator;
            this.selector = selector;
            this.modifier = modifier;
            this.validator = validator;
        }

        /// <summary>
        /// Creates a new population from the chromosomes.
        /// </summary>
        /// <param name="currentPopulation">
        /// The current population of chromosomes that make up the possible solutions to the problem
        /// being solved by this genetic algorithm.</param>
        /// <returns>
        /// Returns a new population of chromosomes.
        /// </returns>
        public virtual ChromosomeCollection<T> Evolve(
            ChromosomeCollection<T> currentPopulation)
        {
            if (currentPopulation == null)
            {
                throw new ArgumentNullException(
                    "currentPopulation",
                    "Unable to create a new population from a null population.");
            }

            // Update the fitness on all the current chromosomes.
            foreach (Chromosome<T> chromosome in currentPopulation)
            {
                chromosome.Fitness = this.fitnessCalculator.Calculate(chromosome);
            }

            // Create a new population of chromosomes.
            ChromosomeCollection<T> newPopulation = new ChromosomeCollection<T>();

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