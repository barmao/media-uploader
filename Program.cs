using System;
using System.IO;
using System.Threading;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.Extensions.Configuration;
using PutObjectRequest = Amazon.S3.Model.PutObjectRequest;

namespace media_uploader
{
    public class Program
    {
        public static string? accessKey;
        public static string? secretKey;
        public static string? endpoint;
        public static string? bucketName;
        public static double timeout;
        public static int errorRetry;

        public static string? directoryToWatch;
        public static string? filter;
        public static bool includeSubdirectories;
        public static Helper helper = new Helper();

        public static void Main(string[] args)
        {
            try
            {
                // Get configurations from appsettings.json
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

                var configuration = builder.Build();

                accessKey = configuration.GetSection("AWS")["AccessKey"];
                secretKey = configuration.GetSection("AWS")["SecretKey"];
                endpoint = configuration.GetSection("AWS")["Endpoint"];
                timeout = Convert.ToDouble(configuration.GetSection("AWS")["Timeout"]);
                errorRetry = Convert.ToInt32(configuration.GetSection("AWS")["MaxErrorRetry"]);
                bucketName = configuration.GetSection("AWS")["BucketName"];

                directoryToWatch = configuration.GetSection("AppSettings")["DirectoryToWatch"];
                filter = configuration.GetSection("AppSettings")["Filter"];
                //includeSubdirectories = (bool) configuration.GetSection("AppSettings")["IncludeSubdirectories"];

                var watcher = new FileSystemWatcher
                {
                    Path = @directoryToWatch,
                    NotifyFilter = NotifyFilters.FileName | NotifyFilters.Size,
                    Filter = filter,
                    IncludeSubdirectories = includeSubdirectories,
                    EnableRaisingEvents = true,

                };
                watcher.Created += new FileSystemEventHandler(OnChanged);


                Console.WriteLine("Press any key to exit.");
                Console.ReadKey();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void OnChanged(object sender, FileSystemEventArgs e)
        {
            try
            {
                int retry = 0;

                while (true)
                {
                    //check if file in use first
                    if (!helper.IsFileInUse(e.FullPath))
                    {

                        Console.WriteLine("");
                        Console.WriteLine("--------------------------------------------------------------------");
                        Console.WriteLine("New file deposited: {0}", Path.GetFileName(e.FullPath));

                        ////////////////////////////////////////////////
                        // move file from monitor folder to S3 bucket
                        ////////////////////////////////////////////////

                        var uploadComplete = Helper.MoveToSpaces(e.FullPath);

                        //upon successful upload, remove the file
                        if (uploadComplete)
                        {
                            Console.WriteLine("File upload successful: {0}", e.FullPath);
                        }
                        else
                        {
                            //If fail upload, retry for a certain period, log errors
                            Console.WriteLine("File upload failed, kindly retry");
                        }

                        retry = 0;
                        break;

                    }
                    else
                    {
                        Console.WriteLine("file in use, retrying");
                        if (retry >= 100000)
                            break;// self preservation, don't get stuck on 1 file.    

                        retry++;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}