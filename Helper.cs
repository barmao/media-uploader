using Amazon.Runtime;
using Amazon.S3.Transfer;
using Amazon.S3;
using Amazon.S3.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PutObjectRequest = Amazon.S3.Model.PutObjectRequest;

namespace media_uploader
{
    public class Helper
    {
        public bool IsFileInUse(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("'path' cannot be null or empty.", "path");

            try
            {
                using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read)) { }
            }
            catch (IOException)
            {
                return true;
            }

            return false;
        }

        public static bool MoveToSpaces(string fileNameAndPath)
        {
            try
            {
                var fileName = Path.GetFileName(fileNameAndPath);
                string fileExtension = Path.GetExtension(fileNameAndPath);


                var config = new AmazonS3Config
                {
                    ServiceURL = Program.endpoint,
                    Timeout = TimeSpan.FromSeconds(Program.timeout),
                    MaxErrorRetry = 8,
                };

                var amazonS3Client = new AmazonS3Client(Program.secretKey, Program.accessKey, config);

                amazonS3Client.PutObjectAsync(new PutObjectRequest
                {
                    Key = fileName,
                    FilePath= fileNameAndPath,
                    BucketName = Program.bucketName,
                    CannedACL= S3CannedACL.PublicRead,
                }).GetAwaiter().GetResult();

                return true;

            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine("Error encountered ***. Message:'{0}' when writing an object", e.Message);
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine("Unknown encountered on server. Message:'{0}' when writing an object", ex.Message);
                return false;
            }
        }
    }
}
