// <copyright file="AbstractFileParser.cs" company="Spike">
//     Copyright (c) . All rights reserved.
// </copyright>

namespace Spikes.FileParser
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Attribute_Definitions;

    /// <summary>
    /// Class Abstract File Parser.
    /// </summary>
    public abstract class AbstractFileParser
    {
        /// <summary>
        /// The file path.
        /// </summary>
        private string filePath;

        /// <summary>
        /// The header option.
        /// </summary>
        private HeaderOptions headerOption = HeaderOptions.NoHeader;

        /// <summary>
        /// Indicates if the file has a footer.
        /// </summary>
        private bool hasFooter = false;

        /// <summary>
        /// The total line count. Including header and footer lines.
        /// </summary>
        protected int totalLineCount = 0;

        /// <summary>
        /// The total number of data rows. Excludes header and footer lines.
        /// </summary>
        private int totalNumberOfDataRows = 0;

        /// <summary>
        /// The data read counter.
        /// </summary>
        private int dataReadCounter = 0;

        /// <summary>
        /// The row counter.
        /// </summary>
        private int rowCounter = 0;

        /// <summary>
        /// The processed records counter.
        /// </summary>
        protected int processedRecordsCounter = 0;

        /// <summary>
        /// The error collection.
        /// </summary>
        private List<string> errorCollection = new List<string>();

        /// <summary>
        /// The header definition.
        /// </summary>
        private IEnumerable<FieldDefinition> headerDefinition;

        /// <summary>
        /// The footer definition.
        /// </summary>
        private IEnumerable<FieldDefinition> footerDefinition;

        /// <summary>
        /// The data definition.
        /// </summary>
        private IEnumerable<FieldDefinition> dataDefinition;

        /// <summary>
        /// Indicates if the file is delimited or fixed length. True for delimited.
        /// </summary>
        private bool isDelimited = true;

        /// <summary>
        /// Indicates if the file has errors.
        /// </summary>
        private bool hasErrors = false;

        /// <summary>
        /// Indicates if a data collection should be returned.
        /// </summary>
        protected bool shouldReturnDataCollection;

        /// <summary>
        /// The delimiter
        /// </summary>
        private char? delimiter = null;

        /// <summary>
        /// The last processed type
        /// </summary>
        private Sections lastProcessedType = Sections.Header;


        /// <summary>
        /// Gets the default delimiter.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="line">The line.</param>
        /// <returns></returns>
        private char GetDefaultDelimiter<T>(string line)
        {
            var delimiterList = new List<char>() {',','|',';','\t'};
            var expectedColumns =  this.GetColumnCount<T>();

            foreach (var seperator in delimiterList)
            {
                var array = line.Split(seperator);
                if (array.Length == expectedColumns)
                {
                    return seperator;
                }
            }
            
            return ',';
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractFileParser" /> class.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="headerOption">The header option.</param>
        /// <param name="hasFooter">If set to <c>true</c> then the file has a footer.</param>
        /// <param name="shouldReturnDataCollection">If set to <c>true</c> then return a data collection.</param>
        /// <param name="delimiter">The delimiter.</param>
        public AbstractFileParser(string filePath, HeaderOptions headerOption, bool hasFooter, bool shouldReturnDataCollection, char? delimiter = null)
        {
            this.filePath = filePath;
            this.hasFooter = hasFooter;
            this.headerOption = headerOption;
            this.ErrorHandling = ErrorOptions.OnErrorHalt;
            this.shouldReturnDataCollection = shouldReturnDataCollection;
            this.delimiter = delimiter;
        }

        /// <summary>
        /// Gets the column count.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>The number of columns</returns>
        public int GetColumnCount<T>()
        {
            var properties = typeof(T).GetProperties();
            var count = 0;

            foreach (var property in properties)
            {
                var colIndexAttribute = this.GetAttributeFrom<ColumnIndexAttribute, T>(property.Name);
                if (colIndexAttribute != null)
                {
                    count++;
                }
            } 
            
            return count;
        }

        /// <summary>
        /// Gets the processed records counter.
        /// </summary>
        /// <value>The processed records counter.</value>
        public int ProcessedRecordsCounter 
        { 
            get
            {
                return processedRecordsCounter;
            }
        }

        /// <summary>
        /// Gets or sets the error handling option.
        /// </summary>
        /// <value>The error handling option.</value>
        public ErrorOptions ErrorHandling { get; set; }

        /// <summary>
        /// Performs the import.
        /// </summary>
        /// <typeparam name="D">Type of the data to be imported.</typeparam>
        /// <returns>The import response.</returns>
        public Response<D> PerformImport<D>()
        {
            this.Init();
            return this.Process<D>(false);
        }

        /// <summary>
        /// Performs the import.
        /// </summary>
        /// <typeparam name="H">Type of the header to be imported.</typeparam>
        /// <typeparam name="D">Type of the data to be imported.</typeparam>
        /// <returns>The import response.</returns>
        public Response<H, D> PerformImport<H, D>()
        {
            this.Init();
            return this.Process<H, D>(false);
        }

        /// <summary>
        /// Performs the import.
        /// </summary>
        /// <typeparam name="H">Type of the header to be imported.</typeparam>
        /// <typeparam name="D">Type of the data to be imported.</typeparam>
        /// <typeparam name="F">Type of the footer to be imported.</typeparam>
        /// <returns>The import response.</returns>
        public Response<H, D, F> PerformImport<H, D, F>()
        {
            this.Init();

            return this.Process<H, D, F>(false);
        }

        /// <summary>
        /// Performs the validation.
        /// </summary>
        /// <typeparam name="D">The type of the data to be validated.</typeparam>
        /// <returns>The validation response.</returns>
        public Response<D> PerformValidation<D>()
        {
            this.Init();
            return this.Process<D>(true);
        }

        /// <summary>
        /// Performs the validation.
        /// </summary>
        /// <typeparam name="H">The type of the header to be validated.</typeparam>
        /// <typeparam name="D">The type of the data to be validated.</typeparam>
        /// <returns>The validation response.</returns>
        public Response<H, D> PerformValidation<H, D>()
        {
            this.Init();
            return this.Process<H, D>(true);
        }

        /// <summary>
        /// Performs the validation.
        /// </summary>
        /// <typeparam name="H">The type of the header to be validated.</typeparam>
        /// <typeparam name="D">The type of the data to be validated.</typeparam>
        /// <typeparam name="F">The type of the footer to be validated.</typeparam>
        /// <returns>The validation response.</returns>
        public Response<H, D, F> PerformValidation<H, D, F>()
        {
            this.Init();
            return this.Process<H, D, F>(true);
        }

        public H ReadHeader<H>()
        {
            this.Init();
            var headerImportType = this.GetImportType<H>() == ImportTypes.Delimited;
            this.isDelimited = headerImportType;
            this.BuildHeaderDefinition<H>();
            bool validateOnly = true;
            H header = default(H);
            using (var file = new StreamReader(this.filePath))
            {
                header = this.ImportHeader<H>(file, validateOnly);
                if (this.hasErrors)
                {
                    return default(H);
                }

            }
            return header;
        }

        public F ReadFooter<F>()
        {
            this.Init();
            var footerImportType = this.GetImportType<F>() == ImportTypes.Delimited;
            this.isDelimited = footerImportType;
            int rowsToSkip = this.totalLineCount - 1;
            int rowCounter = 0;
            this.BuildFooterDefinition<F>();
            bool validateOnly = true;

            F footer = default(F);

            using (var file = new StreamReader(this.filePath))
            {
                var line = string.Empty;
                while (rowCounter < rowsToSkip && (line = file.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        continue;
                    }
                    rowCounter++;
                }
                footer = this.ImportFooter<F>(file, validateOnly);
                if (this.hasErrors)
                {
                    return default(F);
                }
            }

            return footer;
        }

        /// <summary>
        /// Validates the header.
        /// </summary>
        /// <typeparam name="T">The type of the header.</typeparam>
        /// <param name="header">The header to be validated.</param>
        /// <returns>List of validation errors.</returns>
        protected virtual List<string> ValidateHeader<T>(T header)
        {
            return new List<string>();
        }

        /// <summary>
        /// Validates the data.
        /// </summary>
        /// <typeparam name="T">The type of the data.</typeparam>
        /// <param name="data">The data to be validated.</param>
        /// <returns>List of validation errors.</returns>
        protected virtual List<string> ValidateData<T>(T data)
        {
            return new List<string>();
        }

        /// <summary>
        /// Validates the footer.
        /// </summary>
        /// <typeparam name="T">The type of the footer.</typeparam>
        /// <param name="footer">The footer to be validated.</param>
        /// <returns>List of validation errors..</returns>
        protected virtual List<string> ValidateFooter<T>(T footer)
        {
            return new List<string>();
        }

        /// <summary>
        /// Notifies the error.
        /// </summary>
        /// <param name="rowNumber">The row number.</param>
        /// <param name="errors">The errors.</param>
        protected virtual void NotifyDataError<T>(int rowNumber, IEnumerable<string> errors, T data)
        {
            return;
        }

        /// <summary>
        /// Processes the header.
        /// </summary>
        /// <typeparam name="T">The type of the header.</typeparam>
        /// <param name="header">The header to process.</param>
        /// <returns>List of errors.</returns>
        protected abstract void ProcessHeader<T>(T header);

        /// <summary>
        /// Processes the data.
        /// </summary>
        /// <typeparam name="T">The type of the data.</typeparam>
        /// <param name="data">The data to process.</param>
        /// <returns>List of errors.</returns>
        protected abstract void ProcessData<T>(T data);

        /// <summary>
        /// Processes the footer.
        /// </summary>
        /// <typeparam name="T">The type of the footer.</typeparam>
        /// <param name="footer">The footer to process.</param>
        /// <returns>List of errors.</returns>
        protected abstract void ProcessFooter<T>(T footer);

        /// <summary>
        /// Processes the specified validate only.
        /// </summary>
        /// <typeparam name="H">The type of the header to be processed.</typeparam>
        /// <typeparam name="D">The type of the data to be processed.</typeparam>
        /// <param name="validateOnly">If set to <c>true</c> [validate only].</param>
        /// <returns>The process response.</returns>
        private Response<H, D> Process<H, D>(bool validateOnly)
        {
            var headerImportType = this.GetImportType<H>() == ImportTypes.Delimited;
            var dataImportType = this.GetImportType<D>() == ImportTypes.Delimited;
            if (headerImportType != dataImportType)
            {
                throw new ApplicationException("Different import types on different sections is not supported.");
            }

            this.isDelimited = headerImportType;

            this.totalNumberOfDataRows = this.totalLineCount - 1;

            this.BuildHeaderDefinition<H>();
            this.BuildDataDefinition<D>();

            IEnumerable<D> data = new List<D>().AsEnumerable();
            H header = default(H);

            using (var file = new StreamReader(this.filePath))
            {
                header = this.ImportHeader<H>(file, validateOnly);
                if (this.hasErrors && this.ErrorHandling == ErrorOptions.OnErrorHalt)
                {
                    return new Response<H, D>
                    {
                        Data = null,
                        Header = default(H),
                        WasSuccessful = false,
                        ErrorCollection = this.errorCollection.AsEnumerable()
                    };
                }

                data = this.ImportData<D>(file, validateOnly);
            }

            return new Response<H, D>()
            {
                WasSuccessful = !this.hasErrors,
                ErrorCollection = this.errorCollection.AsEnumerable(),
                Header = header,
                Data = data
            };
        }

        /// <summary>
        /// Processes the specified validate only.
        /// </summary>
        /// <typeparam name="D">The type of the data to be processed.</typeparam>
        /// <param name="validateOnly">if set to <c>true</c> [validate only].</param>
        /// <returns>The process response.</returns>
        private Response<D> Process<D>(bool validateOnly)
        {
            this.isDelimited = this.GetImportType<D>() == ImportTypes.Delimited;

            this.totalNumberOfDataRows = this.totalLineCount;
            if (this.headerOption == HeaderOptions.FirstRowColumnNames)
            {
                this.totalNumberOfDataRows = this.totalLineCount - 1;
            }

            this.BuildDataDefinition<D>();

            IEnumerable<D> data = new List<D>().AsEnumerable();

            using (var file = new StreamReader(this.filePath))
            {
                if (this.headerOption == HeaderOptions.FirstRowColumnNames)
                {
                    var headerLine = file.ReadLine();
                    this.rowCounter++;
                }

                data = this.ImportData<D>(file, validateOnly);
            }

            return new Response<D>()
            {
                WasSuccessful = !this.hasErrors,
                ErrorCollection = this.errorCollection.AsEnumerable(),
                Data = data
            };
        }

        /// <summary>
        /// Processes the specified validate only.
        /// </summary>
        /// <typeparam name="H">The type of the header to be processed.</typeparam>
        /// <typeparam name="D">The type of the data to be processed.</typeparam>
        /// <typeparam name="F">The type of the footer to be processed.</typeparam>
        /// <param name="validateOnly">If set to <c>true</c> [validate only].</param>
        /// <returns>The process response.</returns>
        private Response<H, D, F> Process<H, D, F>(bool validateOnly)
        {
            var headerImportType = this.GetImportType<H>() == ImportTypes.Delimited;
            var dataImportType = this.GetImportType<D>() == ImportTypes.Delimited;
            var footerImportType = this.GetImportType<F>() == ImportTypes.Delimited;
            if (headerImportType != dataImportType || headerImportType != footerImportType)
            {
                throw new ApplicationException("Different import types on different sections is not supported.");
            }

            this.isDelimited = headerImportType;

            this.totalNumberOfDataRows = this.totalLineCount - 2;

            this.BuildHeaderDefinition<H>();
            this.BuildDataDefinition<D>();
            this.BuildFooterDefinition<F>();

            H header = default(H);
            F footer = default(F);
            IEnumerable<D> data = new List<D>().AsEnumerable();

            using (var file = new StreamReader(this.filePath))
            {
                header = this.ImportHeader<H>(file, validateOnly);
                if (this.hasErrors && this.ErrorHandling == ErrorOptions.OnErrorHalt)
                {
                    return new Response<H, D, F>
                    {
                        Data = null,
                        Footer = default(F),
                        Header = default(H),
                        WasSuccessful = false,
                        ErrorCollection = this.errorCollection.AsEnumerable()
                    };
                }

                data = this.ImportData<D>(file, validateOnly);

                if (this.hasErrors && this.ErrorHandling == ErrorOptions.OnErrorHalt)
                {
                    return new Response<H, D, F>
                    {
                        Data = null,
                        Footer = default(F),
                        Header = default(H),
                        WasSuccessful = false,
                        ErrorCollection = this.errorCollection.AsEnumerable()
                    };
                }

                footer = this.ImportFooter<F>(file, validateOnly);
            }

            return new Response<H, D, F>()
            {
                WasSuccessful = !this.hasErrors,
                ErrorCollection = this.errorCollection.AsEnumerable(),
                Header = header,
                Data = data,
                Footer = footer
            };
        }

        /// <summary>
        /// Initializes the specified file path.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        private void Init()
        {
            this.Reset();
            this.headerDefinition = new List<FieldDefinition>().AsEnumerable();
            this.footerDefinition = new List<FieldDefinition>().AsEnumerable();
            this.dataDefinition = new List<FieldDefinition>().AsEnumerable();

            if (!File.Exists(this.filePath))
            {
                throw new ApplicationException(string.Format("File '{0}' does not exist", this.filePath));
            }

            var line = string.Empty;
            using (var file = new StreamReader(this.filePath))
            {
                while ((line = file.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        continue;
                    }

                    this.totalLineCount++;
                }
            }
        }

        /// <summary>
        /// Imports the data.
        /// </summary>
        /// <typeparam name="T">The type of the data.</typeparam>
        /// <param name="file">The file to import.</param>
        /// <param name="validateOnly">If set to <c>true</c> [validate only].</param>
        /// <returns>List of data of type T.</returns>
        private IEnumerable<T> ImportData<T>(StreamReader file, bool validateOnly = false)
        {
            var dataList = new List<T>();
            var line = string.Empty;
            var isSuccessful = false;
            this.dataReadCounter = 0;

            while (this.dataReadCounter < this.totalNumberOfDataRows && (line = file.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                this.rowCounter++;
                this.dataReadCounter++;
                T dataObject;
                List<string> lineErrors = new List<string>();
                isSuccessful = this.ParseAndValidateLine(line, Sections.Data, out dataObject, out lineErrors);
                if (isSuccessful && dataObject != null && !validateOnly)
                {
                    this.ProcessData<T>(dataObject);
                    if (this.shouldReturnDataCollection)
                    {
                        dataList.Add(dataObject);
                    }
                }

                if(!isSuccessful)
                {
                    this.NotifyDataError<T>(this.rowCounter, lineErrors.AsEnumerable(), dataObject);
                    if(this.ErrorHandling == ErrorOptions.OnErrorHalt)
                    {
                        break;
                    }
                }
            }

            return dataList.AsEnumerable();
        }

        /// <summary>
        /// Imports the header.
        /// </summary>
        /// <typeparam name="H">The type of the header.</typeparam>
        /// <param name="file">The file.</param>
        /// <param name="validateOnly">if set to <c>true</c> [validate only].</param>
        /// <returns>The header object.</returns>
        private H ImportHeader<H>(StreamReader file, bool validateOnly = false)
        {
            H data;
            var line = file.ReadLine();
            this.rowCounter++;
            List<string> lineErrors = new List<string>();
            var outcome = this.ParseAndValidateLine<H>(line, Sections.Header, out data, out lineErrors);
            if (outcome && data != null && !validateOnly)
            {
                this.ProcessHeader<H>(data);
            }
            
            if(!outcome)
            {
                this.NotifyDataError<H>(this.rowCounter, lineErrors.AsEnumerable(), data);
            }

            return data;
        }

        /// <summary>
        /// Imports the footer.
        /// </summary>
        /// <typeparam name="F">The type of the footer.</typeparam>
        /// <param name="file">The file.</param>
        /// <param name="validateOnly">if set to <c>true</c> [validate only].</param>
        /// <returns>The footer object.</returns>
        private F ImportFooter<F>(StreamReader file, bool validateOnly = false)
        {
            F data;
            var line = file.ReadLine();
            this.rowCounter++;
            List<string> lineErrors = new List<string>();
            var outcome = this.ParseAndValidateLine<F>(line, Sections.Footer, out data, out lineErrors);
            if (outcome && data != null && !validateOnly)
            {
                this.ProcessFooter<F>(data);
            }
            
            if(!outcome)
            {
                this.NotifyDataError<F>(this.rowCounter, lineErrors.AsEnumerable(), data);
            }

            return data;
        }

        /// <summary>
        /// Parses the and validate line.
        /// </summary>
        /// <typeparam name="T">The type of the data.</typeparam>
        /// <param name="line">The line.</param>
        /// <param name="section">The section.</param>
        /// <param name="result">The result.</param>
        /// <returns><c>True</c> if there we no validation or parsing errors, <c>false</c> otherwise.</returns>
        private bool ParseAndValidateLine<T>(string line, Sections section, out T result, out List<string> lineErrors)
        {
            lineErrors = new List<string>();
            var sectionText = section.ToDescription();
            var isEmptyLine = false;

            if (lastProcessedType != section)
            {
                this.delimiter = null;
            }

            this.delimiter = this.delimiter ?? this.GetDefaultDelimiter<T>(line);
            lastProcessedType = section;

            var columnsData = new List<string>();
            try
            {
                switch(section)
                {
                    case Sections.Header:
                        columnsData = this.isDelimited ? line.SplitToList(this.delimiter.Value) : line.ParseToList(this.headerDefinition);
                        break;
                    case Sections.Data:
                        columnsData = this.isDelimited ? line.SplitToList(this.delimiter.Value) : line.ParseToList(this.dataDefinition);
                        break;
                    case Sections.Footer:
                        columnsData = this.isDelimited ? line.SplitToList(this.delimiter.Value) : line.ParseToList(this.footerDefinition);
                        break;
                }
                
                isEmptyLine = columnsData.All(column => string.IsNullOrWhiteSpace(column));
            }
            catch (Exception ex)
            {
                var msg = string.Format("Error parsing {0}: {1}", sectionText, ex.Message);
                lineErrors.Add(msg);
                this.errorCollection.Add(msg);
                this.hasErrors = true;

                result = default(T);
                return false;
            }

            if (isEmptyLine)
            {
                var msg = string.Format("Empty line provided {0}", sectionText);
                lineErrors.Add(msg);
                this.errorCollection.Add(msg);
                this.hasErrors = true;

                result = default(T);
                return false;
            }
            
            result = (T)Activator.CreateInstance(typeof(T));
            var index = 0;

            foreach (var dataItem in columnsData)
            {
                try
                {
                    var fieldName = string.Empty;
                    var fieldDefinition = new FieldDefinition();

                    switch (section)
                    {
                        case Sections.Data:
                            fieldDefinition = this.dataDefinition.Where(x => x.ColumnNumber == index).First();
                            break;
                        case Sections.Footer:
                            fieldDefinition = this.footerDefinition.Where(x => x.ColumnNumber == index).First();
                            break;
                        default:
                            fieldDefinition = this.headerDefinition.Where(x => x.ColumnNumber == index).First();
                            break;
                    }

                    fieldName = fieldDefinition.FieldName;
                    var propertyInfo = result.GetType().GetProperty(fieldName);
                    
                    if (string.IsNullOrWhiteSpace(dataItem) && fieldDefinition.IsRequired)
                    {
                        var msg = string.Format("Error line {0}: Required field missing for Column {1}:{2}", this.rowCounter, index, fieldName);
                        this.hasErrors = true;
                        this.errorCollection.Add(msg);
                        lineErrors.Add(msg);
                        result = default(T);
                        return false;
                    }
                  
                    if (string.IsNullOrWhiteSpace(dataItem))
                    {
                        propertyInfo.SetValue(result, null, null);
                    }
                    else
                    {

                        if (propertyInfo.PropertyType.BaseType == typeof(Enum))
                        {
                            var value = Enum.Parse(propertyInfo.PropertyType,columnsData[index]);
                            propertyInfo.SetValue(result, value, null);
                        }
                        else
                        {
                            if (propertyInfo.PropertyType == typeof(Guid))
                            {
                                var value = Guid.Parse(columnsData[index]);
                                propertyInfo.SetValue(result, value, null);
                            }
                            else
                            {
                                propertyInfo.SetValue(result, Convert.ChangeType(columnsData[index], propertyInfo.PropertyType), null);
                            }
                        }
                    }
                   
                }
                catch (Exception ex)
                {
                    var msg = string.Format("Error line {0}: Column {1}: {2}", this.rowCounter, index, ex.Message);
                    this.hasErrors = true;
                    this.errorCollection.Add(msg);
                    lineErrors.Add(msg);
                    result = default(T);
                    return false;
                }

                index++;
            }

            var errors = new List<string>();
            switch (section)
            {
                case Sections.Header:
                    errors = this.ValidateHeader<T>(result);
                    break;
                case Sections.Data:
                    errors = this.ValidateData<T>(result);
                    break;
                default:
                    errors = this.ValidateFooter<T>(result);
                    break;
            }

            if (errors.Count > 0)
            {
                this.hasErrors = true;
                foreach (var error in errors)
                {
                    lineErrors.Add(string.Format("Validation fail line {0}: {1}", this.rowCounter, error));
                    this.errorCollection.Add(string.Format("Validation fail line {0}: {1}", this.rowCounter, error));
                }
                return false;
            }

            return true;
        }

        /// <summary>
        /// Builds the data definition.
        /// </summary>
        /// <typeparam name="T">The type of the data.</typeparam>
        private void BuildDataDefinition<T>()
        {
            PropertyInfo[] properties = typeof(T).GetProperties();
            foreach (PropertyInfo property in properties)
            {
                this.GetDefinition<T>(property, Sections.Data);
            }
        }

        /// <summary>
        /// Builds the footer definition.
        /// </summary>
        /// <typeparam name="T">The type of the footer.</typeparam>
        private void BuildFooterDefinition<T>()
        {
            PropertyInfo[] properties = typeof(T).GetProperties();
            foreach (PropertyInfo property in properties)
            {
                this.GetDefinition<T>(property, Sections.Footer);
            }
        }

        /// <summary>
        /// Builds the header definition.
        /// </summary>
        /// <typeparam name="T">The type of the header.</typeparam>
        private void BuildHeaderDefinition<T>()
        {
            PropertyInfo[] properties = typeof(T).GetProperties();
            foreach (PropertyInfo property in properties)
            {
                this.GetDefinition<T>(property, Sections.Header);
            }
        }

        /// <summary>
        /// Gets the definition.
        /// </summary>
        /// <typeparam name="T">The type of the data</typeparam>
        /// <param name="property">The property.</param>
        /// <param name="section">The section.</param>
        private void GetDefinition<T>(PropertyInfo property, Sections section)
        {
            List<FieldDefinition> list = new List<FieldDefinition>();
            switch (section)
            {
                case Sections.Data:
                    list = this.dataDefinition.ToList();
                    break;
                case Sections.Header:
                    list = this.headerDefinition.ToList();
                    break;
                case Sections.Footer:
                    list = this.footerDefinition.ToList();
                    break;
            }

            var colIndexAttribute = this.GetAttributeFrom<ColumnIndexAttribute, T>(property.Name);
            if (colIndexAttribute == null)
            {
                throw new ColumnIndexAttributeNotFoundException(property.Name);
            }

            var lengthAttribute = this.GetAttributeFrom<FieldLengthAttribute, T>(property.Name);
            if (lengthAttribute == null && !this.isDelimited)
            {
                throw new FieldLengthAttributeNotFoundException(property.Name);
            }

            var requiredAttribute = this.GetAttributeFrom<IsRequiredAttribute, T>(property.Name);

            var field = new FieldDefinition
            {
                ColumnNumber = colIndexAttribute.Index,
                FieldLength = this.isDelimited ? 0 : lengthAttribute.Length,
                FieldName = property.Name,
                IsRequired = requiredAttribute == null ? true : requiredAttribute.IsRequired
            };

            list.Add(field);
            switch (section)
            {
                case Sections.Data:
                    this.dataDefinition = list.AsEnumerable();
                    break;
                case Sections.Header:
                    this.headerDefinition = list.AsEnumerable();
                    break;
                case Sections.Footer:
                    this.footerDefinition = list.AsEnumerable();
                    break;
            }
        }

        /// <summary>
        /// Gets the attribute from.
        /// </summary>
        /// <typeparam name="T">The attribute type.</typeparam>
        /// <typeparam name="Y">The property type.</typeparam>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>The attribute object.</returns>
        private T GetAttributeFrom<T, Y>(string propertyName) where T : Attribute
        {
            var attrType = typeof(T);
            var property = typeof(Y).GetProperty(propertyName);
            return (T)property.GetCustomAttributes(attrType, false).FirstOrDefault();
        }

        /// <summary>
        /// Gets the type of the import.
        /// </summary>
        /// <typeparam name="T">The class type.</typeparam>
        /// <returns>The Import Type.</returns>
        private ImportTypes GetImportType<T>()
        {
            var attribute = typeof(T).GetCustomAttributes(typeof(ImportTypeAttribute), true).FirstOrDefault() as ImportTypeAttribute;
            if (attribute != null)
            {
                return attribute.ImportType;
            }

            return ImportTypes.Delimited;
        }

        /// <summary>
        /// Resets this instance.
        /// </summary>
        private void Reset()
        {
            this.totalLineCount = 0;
            this.totalNumberOfDataRows = 0;
            this.dataReadCounter = 0;
            this.rowCounter = 0;
            this.errorCollection = new List<string>();
            this.hasErrors = false;
        }
    }
}
