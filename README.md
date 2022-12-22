# .NET Core Console App for uploading files to Digital Ocean Spaces
This .NET Core console application uses the FileSystemWatcher and AWS S3 .NET SDKs to monitor a specified directory for new or updated files, and automatically uploads them to a specified bucket in Digital Ocean Spaces.

## Prerequisites
- .NET Core 3.1 or higher
- An AWS account with access to Digital Ocean Spaces
- The AWS S3 .NET SDK
- The AWS credentials for your AWS account

## Getting Started
1. Clone this repository to your local machine:

    `git clone https://github.com/your-username/digitalocean-spaces-uploader.git`

2. Navigate to the root directory of the repository:

   `cd digitalocean-spaces-uploader`

3. Restore the NuGet packages:

   `dotnet restore`

4. Set the following environment variables with your AWS credentials and desired bucket name:

   - `AWS_ACCESS_KEY_ID`
   - `AWS_SECRET_ACCESS_KEY`
   - `DIGITALOCEAN_SPACES_BUCKET`

5. Run the application:

   `dotnet run`

The application will now start monitoring the current directory for new or updated files, and will automatically upload them to the specified bucket in Digital Ocean Spaces.

## Configuration
The following configuration options can be modified in the appsettings.json file:

- `DirectoryToWatch`: The directory to be monitored for new or updated files.
- `Filter`: The file filter to be used when monitoring the directory.
- `IncludeSubdirectories`: A flag indicating whether subdirectories should be included when monitoring the directory.

## References
- [AWS S3 .NET SDK](https://aws.amazon.com/sdk-for-net/)
- [Digital Ocean Spaces](https://www.digitalocean.com/docs/spaces/)
- [FileSystemWatcher](https://docs.microsoft.com/en-us/dotnet/api/system.io.filesystemwatcher?view=netcore-3.1)
