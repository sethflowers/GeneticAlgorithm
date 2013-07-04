//-----------------------------------------------------------------------
// <copyright file="DistinctGeneValidatorTests.cs" company="Seth Flowers">
//     All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace GeneticAlgorithm.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests for the DistinctGeneValidator class.
    /// </summary>
    [TestClass]
    public class DistinctGeneValidatorTests
    {
        /// <summary>
        /// Validates that IsValid returns false if it is given a null chromosome.
        /// </summary>
        [TestMethod]
        public void IsValid_GivenNullChromosome_ReturnsFalse()
        {
            Assert.IsFalse(new DistinctGeneValidator<int>().IsValid(chromosome: null));
        }

        /// <summary>
        /// Validates that IsValid returns false if it is given a chromosome with two identical genes.
        /// </summary>
        [TestMethod]
        public void IsValid_GivenChromosomeWithNonDistinctGenes_ReturnsFalse()
        {
            Assert.IsFalse(new DistinctGeneValidator<int>().IsValid(
                new Chromosome<int>(new[] { 1, 2, 1 })));
        }

        /// <summary>
        /// Validates that IsValid returns true if it is given a chromosome with all unique genes.
        /// </summary>
        [TestMethod]
        public void IsValid_GivenChromosomeWithDistinctGenes_ReturnsTrue()
        {
            Assert.IsTrue(new DistinctGeneValidator<int>().IsValid(
                new Chromosome<int>(new[] { 1, 2, 3 })));
        }
    }
}