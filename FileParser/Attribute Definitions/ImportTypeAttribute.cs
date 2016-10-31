// <copyright file="ImportTypeAttribute.cs" company="Spike">
//     Copyright (c) . All rights reserved.
// </copyright>

namespace Spikes.FileParser.Attribute_Definitions
{
    using System;

    /// <summary>
    /// Class Import Type Attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ImportTypeAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImportTypeAttribute"/> class.
        /// </summary>
        /// <param name="importType">Type of the import to be performed on the class.</param>
        public ImportTypeAttribute(ImportTypes importType)
        {
            this.ImportType = importType;
        }

        /// <summary>
        /// Gets or sets the type of the import to be performed on the class.
        /// </summary>
        /// <value>The type of the import to be performed on the class.</value>
        public ImportTypes ImportType { get; set; }
    }
}
