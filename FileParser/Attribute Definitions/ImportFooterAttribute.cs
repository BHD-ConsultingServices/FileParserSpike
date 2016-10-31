// <copyright file="ImportFooterAttribute.cs" company="Spike">
//     Copyright (c) . All rights reserved.
// </copyright>

namespace Spikes.FileParser.Attribute_Definitions
{
    using System;

    /// <summary>
    /// This class defines the header attribute for a specific import type
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class ImportFooterAttribute : Attribute, IImportContract
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImportFooterAttribute"/> class.
        /// </summary>
        /// <param name="footerClassType">Type of the header class.</param>
        public ImportFooterAttribute(Type footerClassType)
        {
            this.FooterClassType = footerClassType;
        }

        /// <summary>
        /// Gets or sets the type of the header class.
        /// </summary>
        /// <value>
        /// The type of the header class.
        /// </value>
        public Type FooterClassType { get; set; }

        /// <summary>
        /// Gets the type of the contract.
        /// </summary>
        /// <returns>The contract type</returns>
        public Type GetContractType()
        {
            return this.FooterClassType;
        }
    }
}
