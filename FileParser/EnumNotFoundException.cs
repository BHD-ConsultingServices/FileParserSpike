
namespace Spikes.FileParser
{
    using System;
    
    public class EnumNotFoundException<T> : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnumNotFoundException{T}"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public EnumNotFoundException(T value)
            : base("Value " + value + " of enum " + typeof(T).Name + " was not found")
        {
        }
    }
}
