// <copyright file="FieldDefinition.cs" company="Spike">
//     Copyright (c) . All rights reserved.
// </copyright>

namespace Spikes.FileParser
{
    /// <summary>
    ///  Field Definition class.
    /// </summary>
    public class FieldDefinition
    {
        /// <summary>
        /// Gets or sets the column number.
        /// </summary>
        /// <value>The column number.</value>
        public int ColumnNumber { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this field is required.
        /// </summary>
        /// <value><c>True</c> if this field is required; otherwise, <c>false</c>.</value>
        public bool IsRequired { get; set; }

        /// <summary>
        /// Gets or sets the name of the field.
        /// </summary>
        /// <value>The name of the field.</value>
        public string FieldName { get; set; }

        /// <summary>
        /// Gets or sets the length of the field.
        /// </summary>
        /// <value>The length of the field.</value>
        public int FieldLength { get; set; }
    }
}
