// <copyright file="EnumUtility.cs" company="Spike">
//     Copyright (c) . All rights reserved.
// </copyright>

namespace Spikes.FileParser
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    /// <summary>
    /// Class Enumeration Worker.
    /// </summary>
    public static class EnumUtility
    {
        /// <summary>
        ///  Default Enum.
        /// </summary>
        public enum DefaultEnum
        {
            /// <summary>
            /// The not found option.
            /// </summary>
            Notfound
        }

        /// <summary>
        /// Get the description of the specified enumeration.
        /// </summary>
        /// <typeparam name="TEnum">The type of the enumeration.</typeparam>
        /// <param name="enumValue">The enumeration value.</param>
        /// <returns>The enumeration description.</returns>
        public static string ToDescription<TEnum>(this TEnum enumValue)
        {
            ValidateTypeAsEnum<TEnum>();
            return GetEnumDescription((Enum)(object)enumValue);
        }

        /// <summary>
        /// Parses the enumeration.
        /// </summary>
        /// <typeparam name="TEnum">The type of the enumeration.</typeparam>
        /// <param name="value">The value.</param>
        /// <returns>The parsed enumeration value.</returns>
        public static TEnum ParseEnum<TEnum>(string value)
        {
            ValidateTypeAsEnum<TEnum>();
            return (TEnum)Enum.Parse(typeof(TEnum), value, true);
        }

        /// <summary>
        /// Parses the enumeration.
        /// </summary>
        /// <typeparam name="T">The specified type.</typeparam>
        /// <param name="value">The value.</param>
        /// <param name="defaultEnum">The enumeration default.</param>
        /// <returns>The parsed enumeration value.</returns>
        public static T ParseEnum<T>(string value, T defaultEnum)
        {
            try
            {
                return ParseEnum<T>(value);
            }
            catch (Exception)
            {
               return defaultEnum;
            }
        }

        /// <summary>
        /// Parses the enumeration from description.
        /// </summary>
        /// <typeparam name="TEnum">The enumeration type.</typeparam>
        /// <param name="enumDescription">The enumeration description.</param>
        /// <param name="defaultEnum">The default enumeration.</param>
        /// <returns>The enumeration from Description.</returns>
        public static TEnum ParseEnumFromDescription<TEnum>(string enumDescription, TEnum defaultEnum)
        {
            ValidateTypeAsEnum<TEnum>();
            foreach (Enum enumItem in Enum.GetValues(typeof(TEnum))) 
            {
                if (!string.Equals(
                    enumDescription,
                    GetEnumDescription(enumItem),
                    StringComparison.CurrentCultureIgnoreCase))
                {
                    continue;
                }

                object result = enumItem;
                return (TEnum)result;
            }

            return defaultEnum;
        }

        /// <summary>
        /// Parses the enum from value.
        /// </summary>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
        /// <param name="enumValue">The enum value.</param>
        /// <returns>Returns the enum for the value passed in</returns>
        public static TEnum ParseEnumFromValue<TEnum>(int enumValue)
        {
            ValidateTypeAsEnum<TEnum>();
            foreach (Enum enumItem in Enum.GetValues(typeof(TEnum)))
            {
                if (enumValue != Convert.ToInt32(enumItem))
                {
                    continue;
                }

                object result = enumItem;
                return (TEnum)result;
            }

            throw new EnumNotFoundException<TEnum>((TEnum)(object)enumValue);
        }

        /// <summary>
        /// Enumerations to list.
        /// </summary>
        /// <param name="values">The enumeration values.</param>
        /// <param name="description">The description.</param>
        /// <param name="ignoreUndefined">If set to <c>true</c> [ignore undefined].</param>
        /// <returns>Enumerations as a list.</returns>
        public static IEnumerable<EnumItem> EnumToList(Array values, string[] description, bool ignoreUndefined = false)
        {
            var enumList = new List<EnumItem>();

            var len = values.Length;
            if (len < 1)
            {
                return enumList;
            }
        
            for (var i = 0; i <= len - 1; i++)
            {
                if (ignoreUndefined && description[i] == "Undefined")
                {
                    continue;
                }

                enumList.Add(new EnumItem { Id = (int)values.GetValue(i), Description = description[i] });
            }
          

            return enumList;
        }

        /// <summary>
        /// Gets the enum description.
        /// </summary>
        /// <param name="value">The enum value.</param>
        /// <returns>the enum description.</returns>
        private static string GetEnumDescription(Enum value)
        {
            var enumItem = value.GetType().GetField(value.ToString());

            var attributes = (DescriptionAttribute[])enumItem.GetCustomAttributes(typeof(DescriptionAttribute), false);

            return attributes.Length > 0 ? attributes[0].Description : value.ToString();
        }

        /// <summary>
        /// Validates the enumeration.
        /// </summary>
        /// <typeparam name="TEnum">The type of the enumeration.</typeparam>
        private static void ValidateTypeAsEnum<TEnum>()
        {
            var theType = typeof(TEnum);

            if (theType.FullName != "System.Enum" && !theType.IsEnum)
            {
                throw new InvalidCastException(string.Format("Enum Worker operations can only be applied to Enums not [{0}]", theType));
            }
        }
    }
}
