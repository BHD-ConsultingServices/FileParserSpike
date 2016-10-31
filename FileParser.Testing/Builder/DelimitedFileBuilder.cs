
namespace Spikes.FileParser.Testing.Builder
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    public class DelimitedFileBuilder
    {
        private DelimitedTestHeader header;

        private IList<string> lines;
        private IList<string> Lines
        {
            get { return this.lines ?? (this.lines = new List<string>()); }
            set { lines = value;  }
        }

        private DelimitedTestFooter footer;
        private char delimiter;

        public DelimitedFileBuilder(char? delimiter = null)
        {
            this.delimiter = delimiter ?? ',';
        }

        public DelimitedFileBuilder CreateDefaultHeader(int itemCount)
        {
            this.header = new DelimitedTestHeader(delimiter)
            {
                Date = DateTime.Now,
                NumberOfRecords = itemCount
            };

            return this;
        }
        
        public DelimitedFileBuilder CreateHeader(DelimitedTestHeader headerData)
        {
            this.header = headerData;
            return this;
        }

        public DelimitedFileBuilder CreateEmptyDataLines(int numberOfLines, int numberOfColumns)
        {
            for (var k = 0; k < numberOfLines; k++)
            {
                var line = new StringBuilder();
                for (var l = 0; l < (numberOfColumns -1); l++)
                {
                    line.Append(delimiter);
                }

                this.Lines.Add(line.ToString());
            }

            return this;
        }

        public DelimitedFileBuilder CreateDataLines(int numberOfLines)
        {
            for (var k = 0; k < numberOfLines; k++)
            {
                this.Lines.Add(new DelimitedTestData(delimiter)
                {
                    DataField1 = k,
                    DataField2 = string.Format("SomeData {0}", k),
                    DataField3 = IsOdd(k) ? EnumTest.option1 : EnumTest.option2
                }.ToString());
            }

            return this;
        }

        private bool IsOdd(int value)
        {
            return value % 2 != 0;
        }

        public DelimitedFileBuilder CreateDataLines(DelimitedTestData lineData)
        {
            this.Lines.Add(lineData.ToString());
            return this;
        }

        public DelimitedFileBuilder CreateDefaultFooter(int checksum)
        {
            this.footer = new DelimitedTestFooter
            {
                Checksum = checksum
            };

            return this;
        }

        public DelimitedFileBuilder CreateFooter(DelimitedTestFooter footerData)
        {
            this.footer = footerData;
            return this;
        }

        public void BuildFile(string filePath)
        {
            var fi = new FileInfo(filePath);
            if (!Directory.Exists(fi.DirectoryName))
            {
                Directory.CreateDirectory(fi.DirectoryName);
            }

            TextWriter tw = new StreamWriter(filePath, false);
            
            if (this.header != null)
            {
                tw.WriteLine(this.header.ToString());
            }

            foreach (var line in Lines)
            {
                tw.WriteLine(line);
            }

            if (this.footer != null)
            {
                tw.WriteLine(this.footer.ToString());
            }

            tw.Close(); 
        }
    }
}
