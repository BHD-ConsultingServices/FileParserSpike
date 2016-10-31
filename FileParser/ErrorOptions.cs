// <copyright file="ErrorOptions.cs" company="Spike">
//     Copyright (c) . All rights reserved.
// </copyright>

namespace Spikes.FileParser
{
    /// <summary>
    ///  Error Options enumeration.
    /// </summary>
    public enum ErrorOptions
    {
        /// <summary>
        /// The on error halt option.
        /// </summary>
        OnErrorHalt = 0,

        /// <summary>
        /// The on error skip to next row option.
        /// </summary>
        OnErrorSkipToNextRow = 1
    }
}
