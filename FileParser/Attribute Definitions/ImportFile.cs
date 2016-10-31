
namespace Spikes.FileParser.Attribute_Definitions
{
    using System;
    
    public class ImportFile
    {
        /// <summary>
        /// Gets or sets the correlation identifier.
        /// </summary>
        /// <value>The correlation identifier.</value>
        public int CorrelationId { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the header contract.
        /// </summary>
        /// <value>The header contract</value>
        public Type HeaderContract { get; set; }

        /// <summary>
        /// Gets or sets the data contract.
        /// </summary>
        /// <value>The data contract</value>
        public Type DataContract { get; set; }

        /// <summary>
        /// Gets or sets the footer contract.
        /// </summary>
        /// <value>The footer contract</value>
        public Type FooterContract { get; set; }

        /// <summary>
        /// Gets or sets the extension.
        /// </summary>
        /// <value>The extension</value>
        public string Extension { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance has header.
        /// </summary>
        /// <value><c>true</c> if this instance has header; otherwise, <c>false</c></value>
        public bool HasHeader
        {
            get
            {
                return this.HeaderContract != null;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has data.
        /// </summary>
        /// <value><c>true</c> if this instance has data; otherwise, <c>false</c></value>
        public bool HasData
        {
            get
            {
                return this.DataContract != null;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has footer.
        /// </summary>
        /// <value><c>true</c> if this instance has footer; otherwise, <c>false</c></value>
        public bool HasFooter
        {
            get
            {
                return this.FooterContract != null;
            }
        }
    }
}
