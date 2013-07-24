//-----------------------------------------------------------------------
// <copyright file="EventArgs.cs" company="Seth Flowers">
//     All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace GeneticAlgorithm
{
    using System;

    /// <summary>
    /// A generic event argument.
    /// </summary>
    /// <remarks>Why does this not already exist in the BCL?</remarks>
    /// <typeparam name="T">The type of data that exists in this argument.</typeparam>
    public class EventArgs<T> : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventArgs{T}"/> class.
        /// </summary>
        /// <param name="data">The data.</param>
        public EventArgs(T data)
        {
            this.Data = data;
        }

        /// <summary>
        /// Gets or sets the data for the event.
        /// </summary>
        /// <value>
        /// The data for the event.
        /// </value>
        public T Data { get; set; }
    }
}