//-----------------------------------------------------------------------
// <copyright file="GenericFitnessCalculator.cs" company="Seth Flowers">
//     All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace GeneticAlgorithm
{
    using System;

    /// <summary>
    /// Provides a generic mechanism to calculate a fitness for a chromosome.
    /// </summary>
    /// <typeparam name="T">The type of each gene in this chromosome.</typeparam>
    public class GenericFitnessCalculator<T> : ChromosomeFitnessCalculator<T>
    {
        /// <summary>
        /// Provides a mechanism to determine the fitness for this potential solution.
        /// </summary>
        private readonly Func<Chromosome<T>, double> fitnessFunction;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="GenericFitnessCalculator{T}"/> class.
        /// </summary>
        /// <param name="fitnessFunction">Provides a mechanism to determine the fitness for this potential solution.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Code Analysis doesn't make exceptions for Lambda expressions, but should.")]
        public GenericFitnessCalculator(Func<Chromosome<T>, double> fitnessFunction)
        {
            this.fitnessFunction = fitnessFunction;
        }

        /// <summary>
        /// Calculates the fitness for the specified chromosome.
        /// </summary>
        /// <param name="chromosome">The chromosome.</param>
        /// <returns>Returns the fitness for the specified chromosome.</returns>
        public override double Calculate(Chromosome<T> chromosome)
        {
            return this.fitnessFunction(chromosome);
        }
    }
}