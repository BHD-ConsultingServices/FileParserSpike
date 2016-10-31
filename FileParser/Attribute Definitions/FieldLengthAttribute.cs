// <copyright file="FieldLengthAttribute.cs" company="Spike">
//     Copyright (c) . All rights reserved.
// </copyright>

namespace Spikes.FileParser.Attribute_Definitions
{
    using System;

    /// <summary>
    ///  Field Length Attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class FieldLengthAttribute : System.Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FieldLengthAttribute"/> class.
        /// </summary>
        /// <param name="length">The field length.</param>
        public FieldLengthAttribute(int length)
        {
            this.Length = length;
        }

        /// <summary>
        /// Gets or sets the field length.
        /// </summary>
        /// <value>The field length.</value>
        public int Length { get; set; }
    }
}
