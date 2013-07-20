Genetic Algorithm
==
An implementation of a Genetic Algorithm.

Wikipedia
--

    In the computer science field of artificial intelligence, a genetic algorithm (GA) 
	is a search heuristic that mimics the process of natural evolution. 
	This heuristic (also sometimes called a metaheuristic) is routinely used to 
	generate useful solutions to optimization and search problems.

See [article](http://en.wikipedia.org/wiki/Genetic_algorithm) for more information.

Code Features
--

- Conforms to the "Microsoft All Rules" static [code analysis](http://msdn.microsoft.com/en-us/library/3z0aeatx.aspx) ruleset.
- Conforms to the [StyleCop](http://stylecop.codeplex.com/) 4.7 ruleset.

Sample
--

The sample shows how you could use a genetic algorithm to provide an approximate solution to the 
[Travelling Salesman Problem](https://en.wikipedia.org/wiki/Travelling_salesman_problem). 
The cities in the problem are in a hypothetical route that lies on a circle.
The solution is obviously traversing the cities one after another around the circle. 
The GA comes up with solutions that are close to the perfect solutions, 
where the cities travelled are almost one after another.
Occasionally, the GA will skip a city and then come back to it.
The algorithm also doesn't place any significance on the starting city, since it doesn't really affect the route taken.
In order to start at a different city, one can just follow the same route 
and jump from the last city in the solution to the first city.
