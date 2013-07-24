//-----------------------------------------------------------------------
// <copyright file="GenericChromosomeValidatorTests.cs" company="Seth Flowers">
//     All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace GeneticAlgorithm.Tests
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests for the GenericChromosomeValidator class.
    /// </summary>
    [TestClass]
    public class GenericChromosomeValidatorTests
    {
        /// <summary>
        /// Validates that the IsValid function uses the lambda expression passed into the constructor.
        /// To test this, we can just pass in a function that returns either true or false,
        /// and assert that the correct value is returned.
        /// </summary>
        [TestMethod]
        public void IsValid_ReturnsValueFromFunctionPassedIntoConstructor()
        {
            Chromosome<int> chromosome = new Chromosome<int>(new[] { 0 });

            Assert.IsTrue(new GenericChromosomeValidator<int>(c => true).IsValid(chromosome));
            Assert.IsFalse(new GenericChromosomeValidator<int>(c => false).IsValid(chromosome));
        }

        /// <summary>
        /// Validates that the Calculate function uses the lambda expression passed into the constructor correctly.
        /// To test this, we want to make sure that the chromosome passed into the lambda expression is the same
        /// as the chromosome passed into the Calculate method.
        /// </summary>
        [TestMethod]
        public void IsValid_PassesChromosomeIntoFunctionPassedIntoConstructor()
        {
            Chromosome<int> expectedChromosome = new Chromosome<int>(new[] { 0 });
            
            Chromosome<int> actualChromosome = null;
            Func<Chromosome<int>, bool> validatorFunction = c =>
            {
                actualChromosome = c;
                return true;
            };

            GenericChromosomeValidator<int> validator =
                new GenericChromosomeValidator<int>(validatorFunction);

            // Execute the code to test.
            validator.IsValid(expectedChromosome);

            // Validate that the actual chromosome is the same as the expected chromosome.
            Assert.AreEqual(expectedChromosome, actualChromosome);
        }
    }
}
