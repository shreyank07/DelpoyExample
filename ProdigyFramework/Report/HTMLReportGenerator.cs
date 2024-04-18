using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyFramework.Report
{
    public class HTMLReportGenerator
    {
        StringBuilder bufferContent;

        public void Initialize(string project, string organisation, string projName, string testName, string description, string engineer)
        {
            bufferContent = new StringBuilder();
            bufferContent.AppendLine("< html >");
            bufferContent.AppendLine("< head >");
            var assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream("ProdigyFramework.ResourceDictionary.CssStyle.txt"))
            using (var reader = new StreamReader(stream))
            {
                while (!reader.EndOfStream)
                {
                    bufferContent.AppendLine("< style >");
                    bufferContent.AppendLine(reader.ReadLine());
                    bufferContent.AppendLine("</style >");

                }
            }
            bufferContent.AppendLine("</head >");
            bufferContent.AppendLine("<body >");
        }

        public void AddNewTable(string title, string[] headers, List<string[]> contentValues)
        {
            StringBuilder table = new StringBuilder();
            table.AppendLine("< h1 >" + title + " </ h1 >");
            table.AppendLine("< br >");
            table.AppendLine("< div class=\"wrapper\">");
            table.AppendLine("< div class=\"table\">");
            table.AppendLine("< div class=\"row header blue\">");
            foreach (var header in headers)
            {
                table.AppendLine("< div class=\"cell\">");
                table.AppendLine(header);
                table.AppendLine("</div>");
            }
            table.AppendLine("< /div>");

            foreach (var records in contentValues)
            {
                table.AppendLine("< div class=\"row\">");
                for (int contentId = 0; contentId < records.Count(); contentId++)
                {
                    table.AppendLine("< div class=\"cell\" data-title=\"" + headers[contentId] + "\">");
                    table.AppendLine(records[contentId]);
                    table.AppendLine("</div>");
                }
                table.AppendLine("< /div>");
            }
            table.AppendLine("< /div>");
            table.AppendLine("< /div>");

        }

        public void GenerateReport(string path)
        {
            bufferContent.AppendLine("</body >");
            bufferContent.AppendLine("</html>");
        }
    }
}
