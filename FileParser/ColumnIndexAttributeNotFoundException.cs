// <copyright file="ColumnIndexAttributeNotFoundException.cs" company="Spike">
//     Copyright (c) . All rights reserved.
// </copyright>

namespace Spikes.FileParser
{
    using System;
    
    /// <summary>
    /// Column Index Attribute Not Found Exception class.
    /// </summary>
    public class ColumnIndexAttributeNotFoundException : Exception
    {

        public ColumnIndexAttributeNotFoundException(string name)
            : base(string.Format("ColumnIndex is a required attribute on the import class definition: Missing on Field {0}", name))
        {
        }
    }
}
