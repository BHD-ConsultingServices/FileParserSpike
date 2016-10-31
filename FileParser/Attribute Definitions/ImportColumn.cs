// <copyright file="ImportColumn.cs" company="Spike">
//     Copyright (c) . All rights reserved.
// </copyright>

namespace Spikes.FileParser.Attribute_Definitions
{
    public class ImportColumn
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The column name</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is required.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is required; otherwise, <c>false</c>.
        /// </value>
        public bool IsRequired { get; set; }

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>The position</value>
        public int Position { get; set; }

        /// <summary>
        /// Gets or sets the length of the field.
        /// </summary>
        /// <value>
        /// The length of the field.
        /// </value>
        public int FieldLength { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description</value>
        public string Description { get; set; }
    }
}
