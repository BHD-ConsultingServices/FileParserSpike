// <copyright file="ImportExtensionAttribute.cs" company="Spike">
//     Copyright (c) . All rights reserved.
// </copyright>

namespace Spikes.FileParser.Attribute_Definitions
{
    using System;

    /// <summary>
    /// This defines the import extension used
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class ImportExtensionAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImportExtensionAttribute"/> class.
        /// </summary>
        /// <param name="extension">The extension.</param>
        public ImportExtensionAttribute(string extension)
        {
            this.Extension = extension;
        }

        /// <summary>
        /// Gets or sets the extension.
        /// </summary>
        /// <value>
        /// The extension.
        /// </value>
        public string Extension { get; set; }
    }
}
