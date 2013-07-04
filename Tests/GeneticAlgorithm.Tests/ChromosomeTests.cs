//-----------------------------------------------------------------------
// <copyright file="ChromosomeTests.cs" company="Seth Flowers">
//     All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace GeneticAlgorithm.Tests
{
    using System;
    using System.Collections.Generic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests for the Chromosome class.
    /// </summary>
    [TestClass]
    public class ChromosomeTests
    {
        /// <summary>
        /// Validates that the Genes property is set from the argument passed into the constructor.
        /// </summary>
        [TestMethod]
        public void Constructor_GenesProperty_SetCorrectly()
        {
            IList<double> genes = new[] { 0d };
            Assert.AreEqual(genes, new Chromosome<double>(genes).Genes);
        }

        /// <summary>
        /// Validates that the Fitness property behaves as expected.
        /// </summary>
        [TestMethod]
        public void Fitness_GetReturnsWhatIsSet()
        {
            Chromosome<int> chromosome = new Chromosome<int>(new[] { 0 });
            chromosome.Fitness = 11;

            Assert.AreEqual(11, chromosome.Fitness);
        }
    }
}