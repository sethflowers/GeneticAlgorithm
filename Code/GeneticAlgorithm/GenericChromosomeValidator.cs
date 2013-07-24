//-----------------------------------------------------------------------
// <copyright file="GenericChromosomeValidator.cs" company="Seth Flowers">
//     All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace GeneticAlgorithm
{
    using System;

    /// <summary>
    /// Provides a generic mechanism to determine if the solution represented by a given chromosome is valid.
    /// </summary>
    /// <typeparam name="T">The type of each gene in this chromosome.</typeparam>
    public class GenericChromosomeValidator<T> : ChromosomeValidator<T>
    {
        /// <summary>
        /// Provides a generic mechanism to determine if the solution represented by a given chromosome is valid.
        /// </summary>
        private readonly Func<Chromosome<T>, bool> validatorFunction;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericChromosomeValidator{T}"/> class.
        /// </summary>
        /// <param name="validatorFunction">Provides a generic mechanism to determine if the solution represented by a given chromosome is valid.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Code Analysis doesn't make exceptions for Lambda expressions, but should.")]
        public GenericChromosomeValidator(Func<Chromosome<T>, bool> validatorFunction)
        {
            this.validatorFunction = validatorFunction;
        }

        /// <summary>
        /// Determines whether the specified chromosome is valid.
        /// </summary>
        /// <param name="chromosome">The chromosome.</param>
        /// <returns>
        ///   <c>true</c> if the specified chromosome is valid; otherwise, <c>false</c>.
        /// </returns>
        public override bool IsValid(Chromosome<T> chromosome)
        {
            return this.validatorFunction(chromosome);
        }
    }
}