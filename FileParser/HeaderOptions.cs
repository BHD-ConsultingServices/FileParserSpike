// <copyright file="HeaderOptions.cs" company="Spike">
//     Copyright (c) . All rights reserved.
// </copyright>

namespace Spikes.FileParser
{
    /// <summary>
    ///  Header Options enumeration.
    /// </summary>
    public enum HeaderOptions
    {
        /// <summary>
        /// The no header option.
        /// </summary>
        NoHeader = 0,

        /// <summary>
        /// The first row column names option.
        /// </summary>
        FirstRowColumnNames = 1,

        /// <summary>
        /// The complex header type option.
        /// </summary>
        ComplexHeaderType = 2
    }
}
