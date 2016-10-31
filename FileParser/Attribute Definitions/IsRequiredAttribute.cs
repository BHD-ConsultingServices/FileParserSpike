// <copyright file="IsRequiredAttribute.cs" company="Spike">
//     Copyright (c) . All rights reserved.
// </copyright>

namespace Spikes.FileParser.Attribute_Definitions
{
    using System;

    /// <summary>
    ///  Is Required Attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class IsRequiredAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IsRequiredAttribute"/> class.
        /// </summary>
        public IsRequiredAttribute(bool isRequired)
        {
            this.IsRequired = isRequired;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this field is required.
        /// </summary>
        /// <value><c>True</c> if the field is required; otherwise, <c>false</c>.</value>
        public bool IsRequired { get; set; }
    }
}
