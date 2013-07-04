//-----------------------------------------------------------------------
// <copyright file="City.cs" company="Seth Flowers">
//     All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace TravelingSalesmanGA
{
    /// <summary>
    /// Represents a city to which the salesman must travel.
    /// </summary>
    internal struct City
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="City"/> struct.
        /// </summary>
        /// <param name="name">The name of the city.</param>
        /// <param name="x">The x coordinate of the city.</param>
        /// <param name="y">The y coordinate of the city.</param>
        internal City(string name, double x, double y)
            : this()
        {
            this.Name = name;
            this.X = x;
            this.Y = y;
        }

        /// <summary>
        /// Gets the name of the city.
        /// </summary>
        /// <value>
        /// The name of the city.
        /// </value>
        internal string Name { get; private set; }

        /// <summary>
        /// Gets the X coordinate of the city.
        /// </summary>
        /// <value>
        /// The X coordinate of the city.
        /// </value>
        internal double X { get; private set; }

        /// <summary>
        /// Gets the Y coordinate of the city.
        /// </summary>
        /// <value>
        /// The Y coordinate of the city.
        /// </value>
        internal double Y { get; private set; }
    }
}