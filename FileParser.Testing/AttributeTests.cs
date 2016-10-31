
namespace Spikes.FileParser.Testing
{
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Attribute_Definitions;

    [TestClass]
    public class AttributeTests
    {
        [TestMethod]
        public void GetTheExtensionForASpecificFileType()
        {
            var importType = FileType.Report;
            var extension = FileAttributeUtility.GetExtensionType(importType);

            Assert.AreEqual(@".rep", extension);
        }

        [TestMethod]
        public void GetTheFileTypeForASpecificExtension()
        {
            var extension = @".rep";
            var fileType = FileAttributeUtility.ParseEnumFromExtension(extension, FileType.Undefined);

            Assert.AreEqual(FileType.Report, fileType);
            Assert.AreEqual(FileType.Report, fileType);
        }

        [TestMethod]
        public void GetTheContractTypeUsedForHeaderRows()
        {
            var importType = FileType.Report;
            var headerContract = importType.GetHeaderContract();

            Assert.AreEqual(typeof(ReportImportHeader), headerContract);
        }

        [TestMethod]
        public void GetTheContractTypeUsedForDataRows()
        {
            var importType = FileType.Report;
            var dataContract = importType.GetDataContract();

            Assert.AreEqual(typeof(ReportImportData), dataContract);
        }

        [TestMethod]
        public void GetTheContractTypeUsedForFooterRows()
        {
            var importType = FileType.Report;
            var footerContract = importType.GetFooterContract();

            Assert.AreEqual(typeof(ReportImportFooter), footerContract);
        }

        [TestMethod]
        public void GetAllAvailableImportFiles()
        {
            var availableFiles = FileAttributeUtility.GetAllFileDefinitions<FileType>(true);

            Assert.IsNotNull(availableFiles);
            Assert.AreEqual(1, availableFiles.Count());
            Assert.AreEqual(FileType.Report.ToString(), availableFiles.First().Name);
        }

        [TestMethod]
        public void GetTheImportColumnDetailsForAnImportContract()
        {
            var headerContract = FileType.Report.GetHeaderContract();
            var headerColumnDefinitions = FileAttributeUtility.GetImportColumnsDetails(headerContract);

            Assert.IsNotNull(headerColumnDefinitions);
            Assert.AreEqual(1, headerColumnDefinitions.Count());
            Assert.AreEqual(0, headerColumnDefinitions.First().Position);
            Assert.AreEqual(true, headerColumnDefinitions.First().IsRequired);
        }

    }
    
    #region sample enum manager

        public enum FileType
        {
            Undefined = 0,

            [ImportExtension(".rep")]
            [ImportHeader(typeof(ReportImportHeader))]
            [ImportData(typeof(ReportImportData))]
            [ImportFooter(typeof(ReportImportFooter))]
            Report = 1,
        }

        [ImportType(ImportTypes.Delimited)]
        public class ReportImportHeader
        {
            [ColumnIndex(0)]
            [IsRequired(true)]
            [ColumnDescription("This is the general report code")]
            public string ReportCode { get; set; }
        }

        [ImportType(ImportTypes.Delimited)]
        public class ReportImportData
        {
            [ColumnIndex(0)]
            public int ItemId { get; set; }

            [ColumnIndex(1)]
            public int Quantity { get; set; }
        }

        [ImportType(ImportTypes.Delimited)]
        public class ReportImportFooter
        {
            [ColumnIndex(0)]
            public string ContentHash { get; set; }
        }

    #endregion
}
