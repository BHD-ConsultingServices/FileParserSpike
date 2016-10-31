// <copyright file="ColumnIndexAttribute.cs" company="Spike">
//     Copyright (c) . All rights reserved.
// </copyright>

namespace Spikes.FileParser.Attribute_Definitions
{
    using System;

    /// <summary>
    /// Column Index Attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnIndexAttribute : System.Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnIndexAttribute"/> class.
        /// </summary>
        /// <param name="index">The column index.</param>
        public ColumnIndexAttribute(int index)
        {
            this.Index = index;
        }

        /// <summary>
        /// Gets or sets the column index.
        /// </summary>
        /// <value>The column index.</value>
        public int Index { get; set; }
    }
}
