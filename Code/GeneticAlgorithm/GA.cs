//-----------------------------------------------------------------------
// <copyright file="GA.cs" company="Seth Flowers">
//     All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace GeneticAlgorithm
{
    using System;

    /// <summary>
    /// A generic Genetic Algorithm.
    /// </summary>
    /// <typeparam name="T">The type of data represented by a gene.</typeparam>
    public class GA<T>
    {
        /// <summary>
        /// Provides a mechanism to determine a fitness for a chromosome.
        /// </summary>
        private readonly ChromosomeFitnessCalculator<T> fitnessCalculator;

        /// <summary>
        /// Provides a mechanism to evolve a population of chromosomes into a new population.
        /// </summary>
        private readonly PopulationEvolver<T> populationEvolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="GA{T}" /> class.
        /// </summary>
        /// <param name="fitnessCalculator">Provides a mechanism to determine a fitness for a chromosome.</param>
        public GA(ChromosomeFitnessCalculator<T> fitnessCalculator) : 
            this(fitnessCalculator, new PopulationEvolver<T>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GA{T}" /> class.
        /// </summary>
        /// <param name="fitnessCalculator">Provides a mechanism to determine a fitness for a chromosome.</param>
        /// <param name="populationEvolver">Provides a mechanism to evolve a population of chromosomes into a new population.</param>
        public GA(ChromosomeFitnessCalculator<T> fitnessCalculator, PopulationEvolver<T> populationEvolver)
        {
            this.fitnessCalculator = fitnessCalculator;
            this.populationEvolver = populationEvolver;
        }

        /// <summary>
        /// Occurs at the end of every epoch, and contains the newest generation of chromosomes created.
        /// </summary>
        public event EventHandler<EventArgs<ChromosomeCollection<T>>> Epoch = delegate { };

        /// <summary>
        /// Runs the genetic algorithm the given number of generations, starting with the beginning population of chromosomes as the first generation.
        /// </summary>
        /// <param name="beginningPopulation">The beginning population of chromosomes that make up the possible solutions to the problem
        /// being solved by this genetic algorithm.</param>
        /// <param name="numberOfGenerations">The number of generations to evolve the beginning population.</param>
        /// <returns>
        /// Returns a new population of chromosomes.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">beginningPopulation;Unable to run a genetic algorithm with a null beginning population of chromosomes.</exception>
        public ChromosomeCollection<T> Run(
            ChromosomeCollection<T> beginningPopulation,
            int numberOfGenerations)
        {
            if (beginningPopulation == null)
            {
                throw new ArgumentNullException(
                    "beginningPopulation",
                    "Unable to run a genetic algorithm with a null beginning population of chromosomes.");
            }

            ChromosomeCollection<T> currentPopulation = beginningPopulation;

            // Set the fitness for each chromosome in the population.
            this.UpdateFitnessOnPopulation(currentPopulation);
            
            while (numberOfGenerations-- > 0)
            {
                currentPopulation = this.populationEvolver.Evolve(currentPopulation);

                // Set the fitness for each chromosome in the population.
                this.UpdateFitnessOnPopulation(currentPopulation);

                // Let any clients know that we have just reached a new generation.
                this.OnEpoch(new EventArgs<ChromosomeCollection<T>>(currentPopulation));
            }

            return currentPopulation;
        }

        /// <summary>
        /// Called when the Epoch event is ready to be fired.
        /// </summary>
        /// <param name="eventArgs">The event args to supply when raising the Epoch event.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Creating an extra empty class for this is not worth fixing this rule.")]
        protected virtual void OnEpoch(EventArgs<ChromosomeCollection<T>> eventArgs)
        {
            // Fire the event.
            // Note that we don't have to check for null, 
            // since we have an anonymous NO-OP delegate assigned to it.
            this.Epoch(this, eventArgs);
        }

        /// <summary>
        /// Updates the fitness on each chromosome in the population.
        /// </summary>
        /// <param name="population">The population.</param>
        private void UpdateFitnessOnPopulation(ChromosomeCollection<T> population)
        {
            foreach (Chromosome<T> chromosome in population)
            {
                chromosome.Fitness = this.fitnessCalculator.Calculate(chromosome);
            }
        }
    }
}