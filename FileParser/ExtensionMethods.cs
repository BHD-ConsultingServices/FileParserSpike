// <copyright file="ExtensionMethods.cs" company="Spike">
//     Copyright (c) . All rights reserved.
// </copyright>

namespace Spikes.FileParser
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    
    /// <summary>
    ///  Extension Methods.
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Splits to list.
        /// </summary>
        /// <param name="data">The data to be split.</param>
        /// <returns>List of strings.</returns>
        public static List<string> SplitToList(this string data, char delimiter)
        {
            var splitData = data.Split(delimiter);
            var splitlist = new List<string>();
            var sb = new StringBuilder();
            var inQuotes = false;
            foreach (var s in splitData)
            {
                sb.Append(s);
                if (!inQuotes)
                {
                    if (s.StartsWith("'") || s.StartsWith("\""))
                    {
                        sb.Append(delimiter);
                        inQuotes = true;
                    }
                }
                else
                {
                    if (s.EndsWith("'") || s.EndsWith("\""))
                    {
                        inQuotes = false;
                    }
                    else
                    {
                        sb.Append(delimiter);
                    }
                }

                if (!inQuotes)
                {
                    var line = sb.ToString();
                    if (line.StartsWith("'") || line.StartsWith("\""))
                    {
                        line = line.Substring(1);
                    }

                    if (line.EndsWith("'") || line.EndsWith("\""))
                    {
                        line = line.Substring(0, line.Length - 1);
                    }

                    splitlist.Add(line.Trim());
                    sb.Clear();
                }
            }

            return splitlist;
        }

        /// <summary>
        /// Splits to array.
        /// </summary>
        /// <param name="data">The data to be split.</param>
        /// <returns>Array of strings.</returns>
        public static string[] SplitToArray(this string data, char delimiter)
        {
            var splitData = data.Split(delimiter);
            var splitlist = new List<string>();
            var sb = new StringBuilder();
            var inQuotes = false;
            foreach (var s in splitData)
            {
                sb.Append(s);
                if (!inQuotes)
                {
                    if (s.StartsWith("'") || s.StartsWith("\""))
                    {
                        sb.Append(",");
                        inQuotes = true;
                    }
                }
                else
                {
                    if (s.EndsWith("'") || s.EndsWith("\""))
                    {
                        inQuotes = false;
                    }
                    else
                    {
                        sb.Append(",");
                    }
                }

                if (!inQuotes)
                {
                    var line = sb.ToString();
                    if (line.StartsWith("'") || line.StartsWith("\""))
                    {
                        line = line.Substring(1);
                    }

                    if (line.EndsWith("'") || line.EndsWith("\""))
                    {
                        line = line.Substring(0, line.Length - 1);
                    }

                    splitlist.Add(line);
                    sb.Clear();
                }
            }

            return splitlist.ToArray();
        }

        /// <summary>
        /// Parses to list.
        /// </summary>
        /// <param name="data">The data to be parsed.</param>
        /// <param name="fieldDefinitions">The field definitions.</param>
        /// <returns>List of strings.</returns>
        public static List<string> ParseToList(this string data, IEnumerable<FieldDefinition> fieldDefinitions)
        {
            var cursor = new Cursor();
            var list = new List<string>();
            foreach (var definition in fieldDefinitions.OrderBy(o => o.ColumnNumber))
            {
                var str = ReadNextField(data, definition.FieldLength, cursor);
                list.Add(str);
            }

            return list;
        }

        /// <summary>
        /// Reads the next field.
        /// </summary>
        /// <param name="line">The data line.</param>
        /// <param name="length">The field length.</param>
        /// <param name="cursor">The read cursor.</param>
        /// <returns>The field value.</returns>
        private static string ReadNextField(string line, int length, Cursor cursor)
        {
            var parsedSection = line.Substring(cursor.Location, length);
            cursor.Location += length;
            return parsedSection;
        }

        /// <summary>
        /// The cursor class.
        /// </summary>
        private class Cursor
        {
            /// <summary>
            /// Gets or sets the location of the cursor within the data.
            /// </summary>
            /// <value>The cursor location.</value>
            public int Location { get; set; }
        }
    }
}
