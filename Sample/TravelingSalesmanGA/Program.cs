//-----------------------------------------------------------------------
// <copyright file="Program.cs" company="Seth Flowers">
//     All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace TravelingSalesmanGA
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using GeneticAlgorithm;

    /// <summary>
    /// A demonstration of using a genetic algorithm to produce a solution to the Traveling Salesman problem.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point into the demonstration.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Console.WriteLine(System.String,System.Object,System.Object)", Justification = "This is meant as a demonstration. A resource table would be going overboard. I might fix this later.")]
        public static void Main()
        {
            Console.WriteLine("Running...");

            ChromosomeFitnessCalculator<City> chromosomeFitnessCalculator =
                new GenericFitnessCalculator<City>(GetFitness);

            ChromosomeCollection<City> beginningPopulation =
                GetBeginningPopulation(numberOfCities: 20, populationSize: 400);

            Random random = new Random();

            PopulationEvolver<City> populationEvolver = new PopulationEvolver<City>(
                new TournamentSelector<City>(random, numberOfPlayers: 40),
                new ChromosomeModifier<City>(random), 
                new DistinctGeneValidator<City>());

            GA<City> geneticAlgorithm = new GA<City>(
                chromosomeFitnessCalculator,
                populationEvolver);

            ChromosomeCollection<City> endingPopulation =
                geneticAlgorithm.Run(
                    beginningPopulation, 
                    numberOfGenerations: 1000, 
                    numberOfBestChromosomesToPromote: 30);

            Chromosome<City> bestChromosome = endingPopulation
                .OrderByDescending(c => c.Fitness).First();

            Console.WriteLine("Best Fitness: {0}{1}", bestChromosome.Fitness, Environment.NewLine);

            foreach (City city in bestChromosome.Genes)
            {
                Console.WriteLine(city.Name);
            }

            Console.ReadLine();
        }

        /// <summary>
        /// Gets the beginning population of randomized chromosomes
        /// representing possible solutions to the traveling salesman problem.
        /// </summary>
        /// <param name="numberOfCities">The number of cities.</param>
        /// <param name="populationSize">Size of the population.</param>
        /// <returns>
        /// Returns a randomized population of chromosomes
        /// representing possible solutions to the traveling salesman problem.
        /// </returns>
        private static ChromosomeCollection<City> GetBeginningPopulation(int numberOfCities, int populationSize)
        {
            ChromosomeCollection<City> population = new ChromosomeCollection<City>();

            IList<City> cities = GetCitiesOnCircle(numberOfCities);

            for (int i = 0; i < populationSize; i++)
            {
                Chromosome<City> chromosome = new Chromosome<City>(
                    cities.OrderBy(c => Guid.NewGuid()).ToList());

                population.Add(chromosome);
            }

            return population;
        }

        /// <summary>
        /// Gets the desired number of virtual cities whose coordinates lie on a circle.
        /// </summary>
        /// <param name="numberOfCities">The number of cities.</param>
        /// <returns>Returns the desired number of virtual cities whose coordinates lie on a circle.</returns>
        private static List<City> GetCitiesOnCircle(int numberOfCities)
        {
            List<City> cities = new List<City>();

            double twoPi = Math.PI * 2;
            double segment = twoPi / numberOfCities;

            for (int i = 0; i < numberOfCities; i++)
            {
                double x = Math.Cos(i * segment);
                double y = Math.Sin(i * segment);

                City point = new City(
                    name: i.ToString(CultureInfo.CurrentCulture),
                    x: x,
                    y: y);

                cities.Add(point);
            }

            return cities;
        }

        /// <summary>
        /// Gets the fitness for the given chromosome.
        /// </summary>
        /// <param name="chromosome">The chromosome.</param>
        /// <returns>Returns the fitness for the given chromosome.</returns>
        private static double GetFitness(Chromosome<City> chromosome)
        {
            if (chromosome == null || chromosome.Genes.Count < 2)
            {
                return 0;
            }

            double totalDistance = 0;

            City previousCity = chromosome.Genes[0];

            for (int i = 1; i < chromosome.Genes.Count; i++)
            {
                City currentCity = chromosome.Genes[i];

                totalDistance += Math.Sqrt(
                    Math.Pow(currentCity.X - previousCity.X, 2) +
                    Math.Pow(currentCity.Y - previousCity.Y, 2));

                previousCity = currentCity;
            }

            // The shorter the distance the better, 
            // so return the inverse of the distance as the fitness.
            return 1 / totalDistance;
        }
    }
}