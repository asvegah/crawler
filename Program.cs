using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Crawler
{
    class Program
    {
        //This is a basic example of how to use the crawler
        //In case of a cache miss, it prints out the page's title and 
        //absolute URI, and saves the page data to the filesystem.
        private static List<Page> Pages = new List<Page>();
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Web Crawler");
            Console.WriteLine("");
            Console.WriteLine("USAGE:");
            Console.WriteLine("Crawler <Starting_Url> <Target_Directory> [--options]");
            Console.WriteLine("<Starting_Url> : a valid absolute URL which will be the starting point for the crawler");
            Console.WriteLine("<Target_Directory> : the directory where the page files will be saved");
            Console.WriteLine("");
            Console.WriteLine("OPTIONS:");
            Console.WriteLine("The only option available is --follow-external, which will make the crawler fetch non-local urls as well");
            Console.WriteLine("");
            Console.WriteLine("EXAMPLE: ");
            Console.WriteLine(@"URL: https://monzo.com DIR: C:\Temp EXT: --follow-external");
            Console.WriteLine("");


            //User Input
            Console.WriteLine("Enter URL:");
            string url = Console.ReadLine();
            Console.WriteLine("Enter DIR:");
            string directory = Console.ReadLine();
            Console.WriteLine("External URLs:");
            string external = Console.ReadLine();

            //Check User Input
            if (String.IsNullOrWhiteSpace(url))
            {
                url = ConfigurationManager.AppSettings["startingUrl"].ToString();
            }
            if (String.IsNullOrWhiteSpace(directory))
            {
                directory = ConfigurationManager.AppSettings["logDirectoryPath"].ToString();
            }
            if (String.IsNullOrWhiteSpace(external))
            {
                external = ConfigurationManager.AppSettings["followExternal"].ToString();
            }

            if (Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {             
                Uri startingUri = new Uri(url);
                String targetDirectoryPath = directory;
                bool followExternal = external.Equals("--follow-external");
                Console.WriteLine("Loading from cache...");
                Cache cache = new Cache(startingUri, targetDirectoryPath);
                Console.WriteLine("Cache loaded - {0} pages stored in cache", cache.Count());
                Crawler crawler = new Crawler(cache, false);
                Persister persister = new Persister(targetDirectoryPath, startingUri);          

                //This event is fired when the crawler's process is over
                crawler.WorkComplete += () =>
                {               
                    Environment.Exit(0);
                };

                //This event is fired every time a valid page is downloaded 
                crawler.NewPageFetched += (page) =>
                {
                    Console.WriteLine(page.Title + " - " + page.Uri.AbsoluteUri);
                    persister.Save(page);
                };

                //starts the crawler, on a different thread
                crawler.Crawl(startingUri);
                while (true) { }
            }
            else
            {
                Console.WriteLine("Welcome to Web Crawler");
                Console.WriteLine("Usage:");
                Console.WriteLine("Crawler <starting_uri> <target_directory> [--options]");
                Console.WriteLine("<starting_uri> : a valid absolute URL which will be the starting point for the crawler");
                Console.WriteLine("<target_directory> : the directory where the page files will be saved");
                Console.WriteLine("");
                Console.WriteLine("OPTIONS:");
                Console.WriteLine("The only option available is --follow-external, which will make the crawler fetch non-local urls as well");
                Console.WriteLine("EXAMPLE: ");
                Console.WriteLine(@"URL: https://monzo.com DIR: C:\Temp EXT: --follow-external");
            }        
        }


    }
}
