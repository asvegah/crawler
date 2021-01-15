using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Configuration;

namespace Crawler
{
    //This class acts as a rudimentary file system backed storage.
    public class Persister
    {
        private DirectoryInfo _targetFolder;

        private object _lockObject = new object();
        public Persister(String documentFolder, Uri startingUrl)
        {
            //if the targetFolder is not present, it is automatically created.
            _targetFolder = Directory.CreateDirectory(
                Path.Combine(documentFolder, startingUrl.Authority));
        }

        //When a page is saved, two files are created on the filesystem,
        //in the appropriate folder:
        //a <pagehash>.link file, which contains one line only - the 
        //absolute URI of the page and a <pagehash> file, containing the
        //actual body of the page.
        //The write operation is locked in order to avoid 
        //IO problems with possible concurrent writes
        public void Save(Page page)
        {
            String path = Path.Combine(_targetFolder.FullName, page.Hash);
            lock (_lockObject)
            {
                using (var writer = new StreamWriter(path + ".link"))
                {
                    writer.WriteLine(page.Uri.AbsoluteUri);
                }
                page.Document.Save(Path.Combine(_targetFolder.FullName, page.Hash));
            }
        }

        /// <summary>
        /// Creates a HTML report out of the data gathered.
        /// </summary>
        /// <returns></returns>
        public StringBuilder CreateReport(List<Page> pages)
        {
            var sb = new StringBuilder();

            sb.Append("<html><head><title>Web Crawling Report</title><style>");
            sb.Append("table { border: solid 3px black; border-collapse: collapse; }");
            sb.Append("table tr th { font-weight: bold; padding: 3px; padding-left: 10px; padding-right: 10px; }");
            sb.Append("table tr td { border: solid 1px black; padding: 3px;}");
            sb.Append("h1, h2, p { font-family: Rockwell; }");
            sb.Append("p { font-family: Rockwell; font-size: smaller; }");
            sb.Append("h2 { margin-top: 45px; }");
            sb.Append("</style></head><body>");
            sb.Append("<h1>Crawl Report</h1>");

            sb.Append("<h2>Internal Urls - In Order Crawled</h2>");
            sb.Append("<p>These are the links found within the site. This is the order in which they were crawled.</p>");

            sb.Append("<table><tr><th>Sr. No.</th><th>Url</th></tr>");
            var counter = 1;
            foreach (var page in pages)
            {
                sb.Append("<tr><td>");
                sb.Append(counter++);
                sb.Append("</td><td>");
                sb.Append(page.Title);
                sb.Append("</td></tr>");
            }

            sb.Append("</table>");
            sb.Append("</body></html>");
            return sb;
        }

        /// <summary>
        /// Method to write reports and to demonstrate Log functionality.
        /// </summary>
        /// <param name="contents">string contents</param>
        public void WriteReportToDisk(string contents)
        {
            string fileName = ConfigurationManager.AppSettings["logTextFileName"].ToString();
            FileStream fStream = null;
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
                fStream = File.Create(fileName);
            }
            else
            {
                fStream = File.OpenWrite(fileName);
            }

            using (TextWriter writer = new StreamWriter(fStream))
            {
                writer.WriteLine(contents);
                writer.Flush();
            }

            fStream.Dispose();
        }
    }
}
