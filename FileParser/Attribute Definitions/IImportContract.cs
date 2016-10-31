// <copyright file="IImportContract.cs" company="Spike">
//     Copyright (c) . All rights reserved.
// </copyright>

namespace Spikes.FileParser.Attribute_Definitions
{
    using System;

    /// <summary>
    /// The import attribute contracts interface
    /// </summary>
    public interface IImportContract
    {
        /// <summary>
        /// Gets the type of the contract.
        /// </summary>
        /// <returns></returns>
        Type GetContractType();
    }
}
