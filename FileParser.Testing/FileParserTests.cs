
namespace Spikes.FileParser.Testing
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Attribute_Definitions;
    using Builder;

    [TestClass]
    public class FileParserTests
    {
        [TestMethod]
        public void SimpleDelimitedImportTest()
        {
            string path = @"c:\temp\SimpleDelimitedImportTest.csv";
            BuildSimpleDelimitedFile(path, 100);
            var parser = new SimpleDelimitedParser(path);
            var response = parser.PerformImport<DelimitedTestData>();
            Assert.IsTrue(response.WasSuccessful);
            Assert.AreEqual(100, response.Data.Count());
            Assert.AreEqual("SomeData10", response.Data.Where(n => n.DataField1 == 10).Single().DataField2);
            Assert.AreEqual(EnumTest.option2, response.Data.Where(n => n.DataField1 == 10).Single().DataField3);
            Assert.AreEqual(EnumTest.option1, response.Data.Where(n => n.DataField1 == 11).Single().DataField3);
        }

        [TestMethod]
        public void LargeDelimitedImportNoReturnDataTest()
        {
            string path = @"c:\temp\LargeDelimitedImportNoReturnDataTest.csv";
            BuildSimpleDelimitedFile(path, 5000);
            var parser = new SimpleDelimitedParser(path);
            parser.ShouldReturnData = false;
            var response = parser.PerformImport<DelimitedTestData>();
            Assert.IsTrue(response.WasSuccessful);
            Assert.AreEqual(0, response.Data.Count());
            Assert.AreEqual(5000, parser.ProcessedRecordsCounter);
        }

        [TestMethod]
        public void SimpleLargeFileDelimitedImportTest()
        {
            string path = @"c:\temp\SimpleDelimitedImportTest.csv";
            BuildSimpleDelimitedFile(path, 100);
            var parser = new SimpleDelimitedParser(path);
            var response = parser.PerformImport<DelimitedTestData>();
            Assert.IsTrue(response.WasSuccessful);
            Assert.AreEqual(100, response.Data.Count());
            Assert.AreEqual("SomeData10", response.Data.Where(n => n.DataField1 == 10).Single().DataField2);
        }

        [TestMethod]
        public void DelimitedImportWithValidationTest()
        {
            string path = @"c:\temp\DelimitedImportWithValidationTest.csv";
            BuildSimpleDelimitedFile(path, 100);
            var parser = new DelimitedParserWithValidation(path);
            parser.ErrorHandling = ErrorOptions.OnErrorSkipToNextRow;
            var response = parser.PerformImport<DelimitedTestData>();
            Assert.IsFalse(response.WasSuccessful);
            Assert.AreEqual(81, response.Data.Count());
            Assert.AreEqual("SomeData10", response.Data.Where(n => n.DataField1 == 10).Single().DataField2);
        }

        [TestMethod]
        public void DelimitedImportWithValidationHaltOnErrorTest()
        {
            string path = @"c:\temp\DelimitedImportWithValidationTest.csv";
            BuildSimpleDelimitedFile(path, 100);
            var parser = new DelimitedParserWithValidation(path);
            parser.ErrorHandling = ErrorOptions.OnErrorHalt;
            var response = parser.PerformImport<DelimitedTestData>();
            Assert.IsFalse(response.WasSuccessful);
            Assert.AreEqual(7, response.Data.Count());
            Assert.AreEqual("SomeData2", response.Data.Where(n => n.DataField1 == 2).Single().DataField2);
        }

        [TestMethod]
        public void DelimitedImportWithOnlyValidationTest()
        {
            string path = @"c:\temp\DelimitedImportWithOnlyValidationTest.csv";
            BuildSimpleDelimitedFile(path, 100);
            var parser = new DelimitedParserWithValidation(path);
            parser.ErrorHandling = ErrorOptions.OnErrorSkipToNextRow;
            var response = parser.PerformValidation<DelimitedTestData>();
            Assert.IsFalse(response.WasSuccessful);
            Assert.AreEqual(0, response.Data.Count());
            Assert.AreEqual(19, response.ErrorCollection.Count());
            Assert.AreEqual("Validation error in data row 74: Business rule validation error. Invalid character '7' in data", response.ErrorCollection.ElementAt(10));
        }

        [TestMethod]
        public void DelimitedImportWithMissingRequiredDataTest()
        {
            string path = @"c:\temp\DelimitedImportWithMissingRequiredDataTest.csv";
            BuildSimpleDelimitedFileMissingData(path, 100);
            var parser = new SimpleDelimitedParser(path);
            parser.ErrorHandling = ErrorOptions.OnErrorSkipToNextRow;
            var response = parser.PerformImport<DelimitedTestData>();
            Assert.IsFalse(response.WasSuccessful);
            Assert.AreEqual(99, response.Data.Count());
            Assert.AreEqual(1, response.ErrorCollection.Count());
        }

        [TestMethod]
        [ExpectedException(typeof(ColumnIndexAttributeNotFoundException))]
        public void BadDataDefinitionDelimitedImportTest()
        {
            string path = @"c:\temp\BadDataDefinitionDelimitedImportTest.csv";
            BuildSimpleDelimitedFile(path, 100);
            var parser = new SimpleDelimitedParser(path);
            var response = parser.PerformImport<DelimitedBadTestData>();
        }

        [TestMethod]
        public void SimpleDelimitedWithColumnNamesImportTest()
        {
            string path = @"c:\temp\SimpleDelimitedWithColumnNamesImportTest.csv";
            BuildSimpleDelimitedFileWithColumnNames(path, 100);
            var parser = new SimpleDelimitedParser(path, HeaderOptions.FirstRowColumnNames);
            
            var response = parser.PerformImport<DelimitedTestData>();
            Assert.IsTrue(response.WasSuccessful);
            Assert.AreEqual(100, response.Data.Count());
            Assert.AreEqual("SomeData10", response.Data.Where(n => n.DataField1 == 10).Single().DataField2);
        }

        [TestMethod]
        public void SimpleDelimitedWithHeaderImportTest()
        {
            string path = @"c:\temp\SimpleDelimitedWithHeaderImportTest.csv";
            BuildDelimitedFileWithHeader(path, 100);
            var parser = new SimpleDelimitedParser(path);
            var response = parser.PerformImport<DelimitedTestHeader,DelimitedTestData>();
            Assert.IsTrue(response.WasSuccessful);
            Assert.AreEqual(100, response.Header.NumberOfRecords);
            Assert.AreEqual(100, response.Data.Count());
            Assert.AreEqual("SomeData10", response.Data.Where(n => n.DataField1 == 10).Single().DataField2);
        }

        [TestMethod]
        public void SimpleDelimitedWithHeaderAndFooterImportTest()
        {
            string path = @"c:\temp\SimpleDelimitedWithHeaderAndFooterImportTest.csv";
            BuildDelimitedFileWithHeaderAndFooter(path, 100);
            var parser = new SimpleDelimitedParser(path);
            var response = parser.PerformImport<DelimitedTestHeader, DelimitedTestData, DelimitedTestFooter>();
            Assert.IsTrue(response.WasSuccessful);
            Assert.AreEqual(100, response.Header.NumberOfRecords);
            Assert.AreEqual(100, response.Data.Count());
            Assert.AreEqual("SomeData10", response.Data.Where(n => n.DataField1 == 10).Single().DataField2);
            Assert.AreEqual(4950, response.Footer.Checksum);
        }

        [TestMethod]
        public void SimpleDelimitedWithHeaderAndFooter_ReadHeaderTest()
        {
            string path = @"c:\temp\SimpleDelimitedWithHeaderAndFooter_ReadHeaderTest.csv";
            BuildDelimitedFileWithHeaderAndFooter(path, 100);
            var parser = new SimpleDelimitedParser(path);
            var response = parser.ReadHeader<DelimitedTestHeader>();
            Assert.AreEqual(100, response.NumberOfRecords);
        }

        [TestMethod]
        public void SimpleDelimitedWithHeaderAndFooter_ReadFooterTest()
        {
            string path = @"c:\temp\SimpleDelimitedWithHeaderAndFooter_ReadFooterTest.csv";
            BuildDelimitedFileWithHeaderAndFooter(path, 100);
            var parser = new SimpleDelimitedParser(path);
            var response = parser.ReadFooter<DelimitedTestFooter>();
            Assert.AreEqual(4950, response.Checksum);
        }

        [TestMethod]
        public void SimpleFixedLengthImportTest()
        {
            string path = @"c:\temp\SimpleFixedLengthImportTest.csv";
            BuildSimpleFixedLengthFile(path, 100);
            var parser = new SimpleFixedLengthParser(path);
            var response = parser.PerformImport<FixedTestData>();
            Assert.IsTrue(response.WasSuccessful);
            Assert.AreEqual(100, response.Data.Count());
            Assert.AreEqual("SomeData10", response.Data.Where(n => n.Number == 10).Single().Data.Trim());
        }

        [TestMethod]
        [ExpectedException(typeof(FieldLengthAttributeNotFoundException))]
        public void BadDataDefinitionFixedLengthImportTest()
        {
            string path = @"c:\temp\BadDataDefinitionFixedLengthImportTest.csv";
            BuildSimpleFixedLengthFile(path, 100);
            var parser = new SimpleFixedLengthParser(path);
            var response = parser.PerformImport<FixedBadTestData>();
        }

        [TestMethod]
        public void SimpleFixedLengthWithColumnNamesImportTest()
        {
            string path = @"c:\temp\SimpleFixedLengthWithColumnNamesImportTest.csv";
            BuildSimpleFixedLengthFileWithColumnNames(path, 100);
            var parser = new SimpleFixedLengthParser(path, HeaderOptions.FirstRowColumnNames);

            var response = parser.PerformImport<FixedTestData>();
            Assert.IsTrue(response.WasSuccessful);
            Assert.AreEqual(100, response.Data.Count());
            Assert.AreEqual("SomeData10", response.Data.Where(n => n.Number == 10).Single().Data.Trim());
        }

        [TestMethod]
        public void SimpleFixedLengthWithHeaderImportTest()
        {
            string path = @"c:\temp\SimpleFixedLengthWithHeaderImportTest.csv";
            BuildFixedLengthFileWithHeader(path, 100);
            var parser = new SimpleFixedLengthParser(path);
            var response = parser.PerformImport<FixedTestHeader, FixedTestData>();
            Assert.IsTrue(response.WasSuccessful);
            Assert.AreEqual(100, response.Header.Count);
            Assert.AreEqual(100, response.Data.Count());
            Assert.AreEqual("SomeData10", response.Data.Where(n => n.Number == 10).Single().Data.Trim());
        }

        [TestMethod]
        public void SimpleFixedLengthWithHeaderAndFooterImportTest()
        {
            string path = @"c:\temp\SimpleFixedLengthWithHeaderAndFooterImportTest.csv";
            BuildFixedLengthFileWithHeaderAndFooter(path, 100);
            var parser = new SimpleFixedLengthParser(path);
            var response = parser.PerformImport<FixedTestHeader, FixedTestData, FixedTestFooter>();
            Assert.IsTrue(response.WasSuccessful);
            Assert.AreEqual(100, response.Header.Count);
            Assert.AreEqual(100, response.Data.Count());
            Assert.AreEqual("SomeData10", response.Data.Where(n => n.Number == 10).Single().Data.Trim());
            Assert.AreEqual(4950, response.Footer.Checksum);
        }

        [TestMethod]
        public void SimpleOtherSemicolonDelimitedCheck()
        {
            var path = @"c:\temp\SemicolonSeperatedFile.csv";
            var numberOfItems = 20;
            new DelimitedFileBuilder(';').CreateDefaultHeader(numberOfItems).CreateDataLines(numberOfItems).CreateDefaultFooter(450).BuildFile(path);

            var parser = new SimpleDelimitedParser(path);
            var response = parser.PerformImport<DelimitedTestHeader, DelimitedTestData, DelimitedTestFooter>();

            Assert.IsTrue(response.WasSuccessful);
            Assert.AreEqual(numberOfItems, response.Header.NumberOfRecords);
            Assert.AreEqual(numberOfItems, response.Data.Count());
        }

        [TestMethod]
        public void SimpleOtherTabDelimitedCheck()
        {
            var path = @"c:\temp\TabSeperatedFile.csv";
            var numberOfItems = 20;
            new DelimitedFileBuilder('\t').CreateDefaultHeader(numberOfItems).CreateDataLines(numberOfItems).CreateDefaultFooter(450).BuildFile(path);

            var parser = new SimpleDelimitedParser(path);
            var response = parser.PerformImport<DelimitedTestHeader, DelimitedTestData, DelimitedTestFooter>();

            Assert.IsTrue(response.WasSuccessful);
            Assert.AreEqual(numberOfItems, response.Header.NumberOfRecords);
            Assert.AreEqual(numberOfItems, response.Data.Count());
        }

        [TestMethod]
        public void SimplePipeDelimitedWithEmptyLinesCheck()
        {
            var path = @"c:\temp\PipeSeperatedFileWithBlanks.csv";
            new DelimitedFileBuilder('|').CreateDefaultHeader(12).CreateDataLines(12).CreateEmptyDataLines(3,2).CreateDefaultFooter(450).BuildFile(path);

            var parser = new SimpleDelimitedParser(path);
            var response = parser.PerformImport<DelimitedTestHeader, DelimitedTestData, DelimitedTestFooter>();

            Assert.IsFalse(response.WasSuccessful);
            Assert.AreEqual(12, response.Header.NumberOfRecords);
        }

        #region Build delimited test files
        private void BuildSimpleDelimitedFile(string filePath, int numberOfRecords)
        {
            FileInfo fi = new FileInfo(filePath);
            if(!Directory.Exists(fi.DirectoryName))
            {
                Directory.CreateDirectory(fi.DirectoryName);
            }

            TextWriter tw = new StreamWriter(filePath, false);
            for (int index = 0; index < numberOfRecords; index++ )
            {
                tw.WriteLine(string.Format("{0},SomeData{1},{2}",index, index, IsOdd(index)? EnumTest.option1.ToString(): ((int)EnumTest.option2).ToString()));
            }
            tw.Close(); 
        }

        private bool IsOdd(int value)
        {
            return value % 2 != 0;
        }

        private void BuildSimpleDelimitedFileMissingData(string filePath, int numberOfRecords)
        {
            FileInfo fi = new FileInfo(filePath);
            if (!Directory.Exists(fi.DirectoryName))
            {
                Directory.CreateDirectory(fi.DirectoryName);
            }

            TextWriter tw = new StreamWriter(filePath, false);
            for (int index = 0; index < numberOfRecords; index++)
            {
                if(index == 20)
                {
                    tw.WriteLine(string.Format("{0},", index));
                    continue;
                }
                if(index == 30)
                {
                    tw.WriteLine(string.Format(",SomeData{0}", index));
                    continue;
                }
                tw.WriteLine(string.Format("{0},SomeData{1}", index, index));
            }
            tw.Close();
        }

        private void BuildSimpleDelimitedFileWithColumnNames(string filePath, int numberOfRecords)
        {
            FileInfo fi = new FileInfo(filePath);
            if (!Directory.Exists(fi.DirectoryName))
            {
                Directory.CreateDirectory(fi.DirectoryName);
            }

            TextWriter tw = new StreamWriter(filePath, false);
            tw.WriteLine("Age,Data");
            for (int index = 0; index < numberOfRecords; index++)
            {
                tw.WriteLine(string.Format("{0},SomeData{1}", index, index));
            }
            tw.Close();
        }

        private void BuildDelimitedFileWithHeader(string filePath, int numberOfRecords)
        {
            FileInfo fi = new FileInfo(filePath);
            if (!Directory.Exists(fi.DirectoryName))
            {
                Directory.CreateDirectory(fi.DirectoryName);
            }

            TextWriter tw = new StreamWriter(filePath, false);
            int sum = 0;
            tw.WriteLine(string.Format("2016-01-01, {0}", numberOfRecords));
            for (int index = 0; index < numberOfRecords; index++)
            {
                sum += index;
                tw.WriteLine(string.Format("{0},SomeData{1}", index, index));
            }
            tw.Close();
        }

        private void BuildDelimitedFileWithHeaderAndFooter(string filePath, int numberOfRecords)
        {
            FileInfo fi = new FileInfo(filePath);
            if (!Directory.Exists(fi.DirectoryName))
            {
                Directory.CreateDirectory(fi.DirectoryName);
            }

            TextWriter tw = new StreamWriter(filePath, false);
            int sum = 0;
            tw.WriteLine(string.Format("2016-01-01, {0}", numberOfRecords));
            for (int index = 0; index < numberOfRecords; index++)
            {
                sum += index;
                tw.WriteLine(string.Format("{0},SomeData{1}", index, index));
            }
            tw.WriteLine(string.Format("{0}", sum));
            tw.Close();
        }
        #endregion

        #region Build fixed length test files
        private void BuildSimpleFixedLengthFile(string filePath, int numberOfRecords)
        {
            FileInfo fi = new FileInfo(filePath);
            if (!Directory.Exists(fi.DirectoryName))
            {
                Directory.CreateDirectory(fi.DirectoryName);
            }
            
            TextWriter tw = new StreamWriter(filePath, false);
            for (int index = 0; index < numberOfRecords; index++)
            {
                tw.WriteLine(string.Format("{0}{1}{2}", DateTime.Now.AddDays(index).ToString("yyyy-MM-dd"), string.Format("SomeData{0}",index).PadRight(20,' '),index.ToString().PadLeft(3,'0')));
            }
            tw.Close();
        }

        private void BuildSimpleFixedLengthFileWithColumnNames(string filePath, int numberOfRecords)
        {
            FileInfo fi = new FileInfo(filePath);
            if (!Directory.Exists(fi.DirectoryName))
            {
                Directory.CreateDirectory(fi.DirectoryName);
            }

            TextWriter tw = new StreamWriter(filePath, false);
            tw.WriteLine("Date    Data                Cnt");
            for (int index = 0; index < numberOfRecords; index++)
            {
                tw.WriteLine(string.Format("{0}{1}{2}", DateTime.Now.AddDays(index).ToString("yyyy-MM-dd"), string.Format("SomeData{0}", index).PadRight(20, ' '), index.ToString().PadLeft(3, '0')));
            }
            tw.Close();
        }

        private void BuildFixedLengthFileWithHeader(string filePath, int numberOfRecords)
        {
            FileInfo fi = new FileInfo(filePath);
            if (!Directory.Exists(fi.DirectoryName))
            {
                Directory.CreateDirectory(fi.DirectoryName);
            }

            TextWriter tw = new StreamWriter(filePath, false);
            tw.WriteLine(string.Format("{0}{1}", DateTime.Now.ToString("yyyy-MM-dd"), numberOfRecords.ToString().PadLeft(3, '0')));
            for (int index = 0; index < numberOfRecords; index++)
            {
                tw.WriteLine(string.Format("{0}{1}{2}", DateTime.Now.AddDays(index).ToString("yyyy-MM-dd"), string.Format("SomeData{0}", index).PadRight(20, ' '), index.ToString().PadLeft(3, '0')));
            }
            tw.Close();
        }

        private void BuildFixedLengthFileWithHeaderAndFooter(string filePath, int numberOfRecords)
        {
            FileInfo fi = new FileInfo(filePath);
            if (!Directory.Exists(fi.DirectoryName))
            {
                Directory.CreateDirectory(fi.DirectoryName);
            }

            TextWriter tw = new StreamWriter(filePath, false);
            var sum = 0;
            tw.WriteLine(string.Format("{0}{1}", DateTime.Now.ToString("yyyy-MM-dd"), numberOfRecords.ToString().PadLeft(3, '0')));
            for (int index = 0; index < numberOfRecords; index++)
            {
                sum += index;
                tw.WriteLine(string.Format("{0}{1}{2}", DateTime.Now.AddDays(index).ToString("yyyy-MM-dd"), string.Format("SomeData{0}", index).PadRight(20, ' '), index.ToString().PadLeft(3, '0')));
            }
            tw.WriteLine(string.Format("{0}{1}", sum.ToString().PadLeft(5,'0'), numberOfRecords.ToString().PadLeft(3, '0')));
            tw.Close();
        }
        #endregion





    }

    #region Delimited parser implementation and DTOs
    public class SimpleDelimitedParser : AbstractFileParser
    {
        public SimpleDelimitedParser(string filePath)
            : base(filePath, HeaderOptions.NoHeader, false, true)
        {
            this.ErrorHandling = ErrorOptions.OnErrorSkipToNextRow;
        }

        public SimpleDelimitedParser(string filePath, HeaderOptions option)
            : base(filePath, option, false, true)
        {
            this.ErrorHandling = ErrorOptions.OnErrorHalt;
        }

        public bool ShouldReturnData
        {
            get
            {
                return base.shouldReturnDataCollection;
            }

            set
            {
                base.shouldReturnDataCollection = value;
            }
        }

        protected override void ProcessHeader<T>(T header)
        {
            DelimitedTestHeader hdr = header as DelimitedTestHeader;
            base.processedRecordsCounter++;
            Console.WriteLine("Header Date:{0}, Count={1}", hdr.Date.ToString("yyyyMMdd"), hdr.NumberOfRecords);
        }

        protected override void ProcessData<T>(T data)
        {
            DelimitedTestData record = data as DelimitedTestData;
            base.processedRecordsCounter++;
            Console.WriteLine("{0}:{1}", record.DataField1, record.DataField2);
        }

        protected override void ProcessFooter<T>(T footer)
        {
            DelimitedTestFooter record = footer as DelimitedTestFooter;
            base.processedRecordsCounter++;
            Console.WriteLine("Checksum:{0}", record.Checksum);
        }
    }

    public class DelimitedParserWithValidation : AbstractFileParser
    {
        public DelimitedParserWithValidation(string filePath)
            : base(filePath, HeaderOptions.NoHeader, false, true)
        {
            this.ErrorHandling = ErrorOptions.OnErrorHalt;
        }

        public DelimitedParserWithValidation(string filePath, HeaderOptions option)
            : base(filePath, option, false, true)
        {
            this.ErrorHandling = ErrorOptions.OnErrorHalt;
        }
        
        protected override List<string> ValidateData<T>(T data)
        {
            var errors = new List<string>();
            DelimitedTestData record = data as DelimitedTestData;
            if (record.DataField2.Contains("7"))
            {
                errors.Add("Business rule validation error. Invalid character '7' in data");
            }
            return errors;
        }

        protected override void ProcessHeader<T>(T header)
        {
            DelimitedTestHeader hdr = header as DelimitedTestHeader;
            Console.WriteLine("Header Date:{0}, Count={1}", hdr.Date.ToString("yyyyMMdd"), hdr.NumberOfRecords);
        }

        protected override void ProcessData<T>(T data)
        {
            DelimitedTestData record = data as DelimitedTestData;
            Console.WriteLine("{0}:{1}", record.DataField1, record.DataField2);
        }

        protected override void ProcessFooter<T>(T footer)
        {
            DelimitedTestFooter record = footer as DelimitedTestFooter;
            Console.WriteLine("Checksum:{0}", record.Checksum);
        }
    }

    [ImportType(ImportTypes.Delimited)]
    public class DelimitedTestData
    {
        private char delimiter;
        public DelimitedTestData() { }
        public DelimitedTestData(char? delimiter = null)
        {
            this.delimiter = delimiter ?? ',';
        }

        [ColumnIndex(0)]
        public int DataField1 { get; set; }

        [ColumnIndex(1)]
        [IsRequired(false)]
        public string DataField2 { get; set; }

        [ColumnIndex(2)]
        public EnumTest DataField3 { get; set; }
        public override string ToString()
        {
            return string.Format("{0}{1}{2}{3}{4}", DataField1, delimiter, DataField2, delimiter, DataField3 == EnumTest.option1 ? DataField3.ToString() : ((int)DataField3).ToString());
        }
    }

    public enum EnumTest
    {
        unknown = 0,
        option1 = 1,
        option2 = 2
    }

    [ImportType(ImportTypes.Delimited)]
    public class DelimitedTestHeader
    {
        private char delimiter;
        public DelimitedTestHeader() {}
        public DelimitedTestHeader(char? delimiter = null)
        {
            this.delimiter = delimiter ?? ',';
        }

        [ColumnIndex(1)]
        public int NumberOfRecords { get; set; }

        [ColumnIndex(0)]
        public DateTime Date { get; set; }

        public override string ToString()
        {
            return string.Format("{0}{1}{2}", Date, delimiter, NumberOfRecords);
        }
    }

    [ImportType(ImportTypes.Delimited)]
    public class DelimitedTestFooter
    {
        [ColumnIndex(0)]
        public int Checksum { get; set; }

        public override string ToString()
        {
            return string.Format("{0}", Checksum);
        }
    }

    [ImportType(ImportTypes.Delimited)]
    public class DelimitedBadTestData
    {
        [ColumnIndex(1)]
        public int DataField1 { get; set; }

        public string DataField2 { get; set; }
    }

    #endregion

    #region Fixed length parser implementation and DTOs
    public class SimpleFixedLengthParser : AbstractFileParser
    {
        public SimpleFixedLengthParser(string filePath)
            : base(filePath, HeaderOptions.NoHeader, false, true)
        {
            this.ErrorHandling = ErrorOptions.OnErrorHalt;
        }

        public SimpleFixedLengthParser(string filePath, HeaderOptions option)
            : base(filePath, option, false, true)
        {
            this.ErrorHandling = ErrorOptions.OnErrorHalt;
        }

        protected override void ProcessHeader<T>(T header)
        {
            FixedTestHeader hdr = header as FixedTestHeader;
            Console.WriteLine("Header Date:{0}, Count={1}", hdr.Date.ToString("yyyyMMdd"), hdr.Count);
        }

        protected override void ProcessData<T>(T data)
        {
            FixedTestData record = data as FixedTestData;
            Console.WriteLine("{0}:{1}:{2}", record.Number,record.Date.ToLongDateString(), record.Data);
        }

        protected override void ProcessFooter<T>(T footer)
        {
            FixedTestFooter record = footer as FixedTestFooter;
            Console.WriteLine("Checksum:{0} Count{1}", record.Checksum, record.Count);
        }
    }

    [ImportType(ImportTypes.FixedLength)]
    public class FixedTestData
    {
        [ColumnIndex(0)]
        [FieldLength(10)]
        public DateTime Date { get; set; }

        [ColumnIndex(1)]
        [FieldLength(20)]
        [IsRequired(false)]
        public string Data { get; set; }

        [ColumnIndex(2)]
        [FieldLength(3)]
        [IsRequired(true)]
        public int Number { get; set; }
    }

    [ImportType(ImportTypes.FixedLength)]
    public class FixedBadTestData
    {
        [ColumnIndex(0)]
        [FieldLength(10)]
        public DateTime Date { get; set; }

        [ColumnIndex(1)]
        public string Data { get; set; }

        [ColumnIndex(2)]
        [FieldLength(3)]
        public int Number { get; set; }
    }

    [ImportType(ImportTypes.FixedLength)]
    public class FixedTestHeader
    {
        [ColumnIndex(0)]
        [FieldLength(10)]
        public DateTime Date { get; set; }


        [ColumnIndex(1)]
        [FieldLength(3)]
        public int Count { get; set; }
    }

    [ImportType(ImportTypes.FixedLength)]
    public class FixedTestFooter
    {
        [ColumnIndex(0)]
        [FieldLength(5)]
        public int Checksum { get; set; }


        [ColumnIndex(1)]
        [FieldLength(3)]
        public int Count { get; set; }
    }
    #endregion
}
