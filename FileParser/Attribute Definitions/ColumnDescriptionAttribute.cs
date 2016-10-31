// <copyright file="DescriptionAttribute.cs" company="Spike">
//     Copyright (c) . All rights reserved.
// </copyright>

namespace Spikes.FileParser.Attribute_Definitions
{
    using System;

    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnDescriptionAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnDescriptionAttribute"/> class.
        /// </summary>
        /// <param name="description">The description.</param>
        public ColumnDescriptionAttribute(string description)
        {
            this.Description = description;
        }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description
        /// </value>
        public string Description { get; set; }
    }
}
