// <copyright file="FieldLengthAttributeNotFoundException.cs" company="Spike">
//     Copyright (c) . All rights reserved.
// </copyright>

namespace Spikes.FileParser
{
    using System;

    /// <summary>
    /// Field length Attribute Not Found Exception class.
    /// </summary>
    public class FieldLengthAttributeNotFoundException : Exception
    {

        public FieldLengthAttributeNotFoundException(string name)
            : base(string.Format("FieldLength is a required attribute on a fixed length import class definition: Missing on Field {0}", name))
        {
        }
    }
}
