// <copyright file="Response.cs" company="Spike">
//     Copyright (c) . All rights reserved.
// </copyright>

namespace Spikes.FileParser
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Class Response.
    /// </summary>
    /// <typeparam name="H">The header type.</typeparam>
    /// <typeparam name="D">The data type.</typeparam>
    /// <typeparam name="F">the footer type.</typeparam>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed.")]
    public class Response<H, D, F> : GenericResponse
    {
        /// <summary>
        /// Gets or sets the header.
        /// </summary>
        /// <value>The header value.</value>
        public H Header { get; set; }

        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>The data value.</value>
        public IEnumerable<D> Data { get; set; }

        /// <summary>
        /// Gets or sets the footer.
        /// </summary>
        /// <value>The footer value.</value>
        public F Footer { get; set; }
    }

    /// <summary>
    /// Class Response.
    /// </summary>
    /// <typeparam name="H">The header type.</typeparam>
    /// <typeparam name="D">The data type.</typeparam>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed.")]
    public class Response<H, D> : GenericResponse
    {
        /// <summary>
        /// Gets or sets the header.
        /// </summary>
        /// <value>The header value.</value>
        public H Header { get; set; }

        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>The data value.</value>
        public IEnumerable<D> Data { get; set; }
    }

    /// <summary>
    /// Class Response.
    /// </summary>
    /// <typeparam name="D">The data type</typeparam>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed.")]
    public class Response<D> : GenericResponse
    {
        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>The data value.</value>
        public IEnumerable<D> Data { get; set; }
    }

    /// <summary>
    /// Class Generic Response.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed.")]
    public class GenericResponse
    {
        /// <summary>
        /// Gets or sets a value indicating whether the operation was successful.
        /// </summary>
        /// <value><c>True</c> if the operation was successful; otherwise, <c>false</c>.</value>
        public bool WasSuccessful { get; set; }

        /// <summary>
        /// Gets or sets the error collection.
        /// </summary>
        /// <value>The error collection.</value>
        public IEnumerable<string> ErrorCollection { get; set; }
    }
}