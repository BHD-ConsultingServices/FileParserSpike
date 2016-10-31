// <copyright file="FileAttributeUtility.cs" company="Spike">
//     Copyright (c) . All rights reserved.
// </copyright>

namespace Spikes.FileParser.Attribute_Definitions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// The file atribute utility helps retrieving file related attribute configuration
    /// </summary>
    public static class FileAttributeUtility
    {
        /// <summary>
        /// Parses the enum from extension.
        /// </summary>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
        /// <param name="extension">The extension.</param>
        /// <param name="defaultEnum">The default enum.</param>
        /// <returns>The enum from the extension</returns>
        public static TEnum ParseEnumFromExtension<TEnum>(string extension, TEnum defaultEnum)
        {
            ValidateTypeAsEnum<TEnum>();
            foreach (Enum enumItem in Enum.GetValues(typeof(TEnum)))
            {
                if (!string.Equals(
                    extension,
                    GetExtensionType(enumItem),
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
        /// Gets the header contract.
        /// </summary>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
        /// <param name="enumValue">The enum value.</param>
        /// <returns>The header contract</returns>
        public static Type GetHeaderContract<TEnum>(this TEnum enumValue)
        {
            return GetContractType<TEnum, ImportHeaderAttribute>(enumValue);
        }

        /// <summary>
        /// Gets the content contract.
        /// </summary>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
        /// <param name="enumValue">The enum value.</param>
        /// <returns>The contect contract</returns>
        public static Type GetDataContract<TEnum>(this TEnum enumValue)
        {
            return GetContractType<TEnum, ImportDataAttribute>(enumValue);
        }

        /// <summary>
        /// Gets all file definitions.
        /// </summary>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
        /// <param name="ignorePositionZero">if set to <c>true</c> [ignore position zero].</param>
        /// <returns>Get list of file definitions</returns>
        public static IEnumerable<ImportFile> GetAllFileDefinitions<TEnum>(bool ignorePositionZero = false)
        {
            var availableFileTypes = Enum.GetValues(typeof(TEnum));
            var fileList = new List<ImportFile>();

            foreach (var file in availableFileTypes)
            {
                if (ignorePositionZero && ((int)file == 0))
                {
                    continue;
                }

                fileList.Add(file.GetFileDefinitions());
            }

            return fileList;
        }

        /// <summary>
        /// Gets the file definitions.
        /// </summary>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
        /// <param name="enumValue">The enum value.</param>
        /// <returns>Get file definitions</returns>
        private static ImportFile GetFileDefinitions<TEnum>(this TEnum enumValue)
        {
            var extension = GetExtensionType(enumValue as Enum);
            var header = GetHeaderContract(enumValue as Enum);
            var data = GetDataContract(enumValue as Enum);
            var footer = GetFooterContract(enumValue as Enum);
            var name = enumValue.ToString();
            var correlationId = Convert.ToInt32(enumValue);

            return new ImportFile
            {
                Name = name,
                HeaderContract = header,
                DataContract = data,
                FooterContract = footer,
                Extension = extension,
                CorrelationId = correlationId
            };
        }

        /// <summary>
        /// Gets the footer contract.
        /// </summary>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
        /// <param name="enumValue">The enum value.</param>
        /// <returns>the footer contract</returns>
        public static Type GetFooterContract<TEnum>(this TEnum enumValue)
        {
            return GetContractType<TEnum, ImportFooterAttribute>(enumValue);
        }

        /// <summary>
        /// Gets the import columns.
        /// </summary>
        /// <param name="theImportContract">The import contract.</param>
        /// <returns>The import columns in a contract class</returns>
        public static IEnumerable<ImportColumn> GetImportColumnsDetails(Type theImportContract)
        {
            var columnList = new List<ImportColumn>();

            foreach (var property in theImportContract.GetProperties())
            {
                var indexAttribute = property.GetCustomAttributes(typeof(ColumnIndexAttribute), false);
                var isRequiredAttribute = property.GetCustomAttributes(typeof(IsRequiredAttribute), false);
                var fieldLengthAttribute = property.GetCustomAttributes(typeof(FieldLengthAttribute), false);
                var descriptionAttribute = property.GetCustomAttributes(typeof(ColumnDescriptionAttribute), false);
                
                if (indexAttribute.Any() || isRequiredAttribute.Any() || fieldLengthAttribute.Any())
                {
                    var newColumn = new ImportColumn();

                    newColumn.Name = property.Name;

                    if (indexAttribute.Any())
                    {
                        newColumn.Position = (indexAttribute.First() as ColumnIndexAttribute).Index;
                    }

                    if (isRequiredAttribute.Any())
                    {
                        newColumn.IsRequired = (isRequiredAttribute.First() as IsRequiredAttribute).IsRequired;
                    }

                    if (fieldLengthAttribute.Any())
                    {
                        newColumn.FieldLength = (fieldLengthAttribute.First() as FieldLengthAttribute).Length;
                    }

                    if (descriptionAttribute.Any())
                    {
                        newColumn.Description = (descriptionAttribute.First() as ColumnDescriptionAttribute).Description;
                    }

                    columnList.Add(newColumn);
                }
            }

            return columnList.OrderBy(item => item.Position);
        }

        /// <summary>
        /// Gets the type of the contract.
        /// </summary>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
        /// <typeparam name="TAttrib">The type of the attribute.</typeparam>
        /// <param name="enumValue">The enum value.</param>
        /// <returns>Contract type from attribute</returns>
        private static Type GetContractType<TEnum, TAttrib>(this TEnum enumValue)
             where TAttrib : Attribute, IImportContract
        {
            ValidateTypeAsEnum<TEnum>();

            var enumInstance = enumValue.GetType().GetField(enumValue.ToString());
            var attributes = enumInstance.GetCustomAttributes(typeof(TAttrib), false);
            if (!attributes.Any()) return null;

            var importContract = attributes.First() as IImportContract;
            return importContract != null ? importContract.GetContractType() : null;
        }
        
        /// <summary>
        /// Gets the type of the extention.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>THe file extension from attribute</returns>
        public static string GetExtensionType(Enum value)
        {
            var enumItem = value.GetType().GetField(value.ToString());
            
            var attributes = (ImportExtensionAttribute[])enumItem.GetCustomAttributes(typeof(ImportExtensionAttribute), false);
            
            foreach (var attribute in attributes)
            {
                var importContract = attribute;
                if (importContract == null) continue;

                return importContract.Extension;
            }

            return null;
        }

        /// <summary>
        /// Validates the type as enum.
        /// </summary>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
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
