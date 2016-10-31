// <copyright file="Sections.cs" company="Spike">
//     Copyright (c) . All rights reserved.
// </copyright>

namespace Spikes.FileParser
{
    using System.ComponentModel;

    /// <summary>
    ///  Sections enumeration.
    /// </summary>
    public enum Sections
    {
        /// <summary>
        /// The header section.
        /// </summary>
        [Description("header")]
        Header,

        /// <summary>
        /// The data section.
        /// </summary>
        [Description("data")]
        Data,

        /// <summary>
        /// The footer section.
        /// </summary>
        [Description("footer")]
        Footer
    }
}
