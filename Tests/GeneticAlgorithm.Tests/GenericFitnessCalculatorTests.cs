//-----------------------------------------------------------------------
// <copyright file="GenericFitnessCalculatorTests.cs" company="Seth Flowers">
//     All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace GeneticAlgorithm.Tests
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests for the GenericFitnessCalculator class.
    /// </summary>
    [TestClass]
    public class GenericFitnessCalculatorTests
    {
        /// <summary>
        /// Validates that the Calculate function uses the lambda expression passed into the constructor.
        /// To test this, we can just pass in a function that returns some random value,
        /// and assert that the correct value is returned.
        /// </summary>
        [TestMethod]
        public void Calculate_ReturnsValueFromFunctionPassedIntoConstructor()
        {
            Chromosome<int> chromosome = new Chromosome<int>(new[] { 0 });

            GenericFitnessCalculator<int> fitnessCalculator =
                new GenericFitnessCalculator<int>(c => 175d);

            Assert.AreEqual(175d, fitnessCalculator.Calculate(chromosome));
        }

        /// <summary>
        /// Validates that the Calculate function uses the lambda expression passed into the constructor correctly.
        /// To test this, we want to make sure that the chromosome passed into the lambda expression is the same
        /// as the chromosome passed into the Calculate method.
        /// </summary>
        [TestMethod]
        public void Calculate_PassesGenesIntoFunctionPassedIntoConstructor()
        {
            Chromosome<int> expectedChromosome = new Chromosome<int>(new[] { 0 });
            
            Chromosome<int> actualChromosome = null;
            Func<Chromosome<int>, double> fitnessFunction = c =>
            {
                actualChromosome = c;
                return 175d;
            };

            GenericFitnessCalculator<int> fitnessCalculator =
                new GenericFitnessCalculator<int>(fitnessFunction);

            // Execute the code to test.
            fitnessCalculator.Calculate(expectedChromosome);

            // Validate that the actual chromosome is the same as the expected chromosome.
            Assert.AreEqual(expectedChromosome, actualChromosome);
        }
    }
}
