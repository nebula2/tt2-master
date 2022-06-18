using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TT2Master.Shared.Models;

namespace TT2MasterFunc.Util
{
    public static class BlobStorageHelper
    {
        /// <summary>
        /// Returns the latest version stored on server
        /// </summary>
        /// <param name="containerName">name of container</param>
        /// <returns></returns>
        public static async Task<Version> GetLatestVersionExistingOnServerAsync(string conStr, string containerName)
        {
            CloudStorageAccount storageAccount;
            storageAccount = CloudStorageAccount.Parse(conStr);

            CloudBlobClient client;
            client = storageAccount.CreateCloudBlobClient();

            CloudBlobContainer container;
            container = client.GetContainerReference(containerName);

            var dir = await container.ListBlobsSegmentedAsync(null);

            var folders = dir.Results.Where(x => x as CloudBlobDirectory != null).ToList();

            var versions = new List<Version>();

            foreach (var item in folders)
            {
                if(Version.TryParse(item.Uri.Segments.Last().Replace("/",""), out var v))
                {
                    versions.Add(v);
                }
            }

            return versions.Max(x => x);
        }

        /// <summary>
        /// Downloads each file in container that is stored for given version to LocalApplicationData
        /// </summary>
        /// <param name="version">version</param>
        /// <param name="version">asset type</param>
        /// <returns>True if successful else false</returns>
        public static async Task<List<Uri>> SaveAssetsAsync(string conStr, AssetType at)
        {
            var uris = new List<Uri>();

            var cloudStorageAccount = CloudStorageAccount.Parse(conStr);
            var blobClient = cloudStorageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference(at.AzureContainer);
            var dir = container.GetDirectoryReference(at.CurrentVersion.ToString());

            var items = await dir.ListBlobsSegmentedAsync(null);

            // save each thing
            foreach (var item in items.Results)
            {
                var cloudBlob = (CloudBlockBlob)item;

                uris.Add(cloudBlob.Uri);
            }

            return uris;
        }

        /// <summary>
        /// Copies assets between containers
        /// </summary>
        /// <param name="conStr">connection string</param>
        /// <param name="at">asset release request</param>
        /// <returns>True if successful else false</returns>
        public static async Task<bool> CopyAssetsAsync(string conStr, AssetReleaseRequest at)
        {
            var cloudStorageAccount = CloudStorageAccount.Parse(conStr);
            var blobClient = cloudStorageAccount.CreateCloudBlobClient();

            var stagingContainer = blobClient.GetContainerReference(at.StagingContainer);
            var stagingDir = stagingContainer.GetDirectoryReference(at.StagingVersion);
            
            var items = await stagingDir.ListBlobsSegmentedAsync(null);

            // get staging folder content
            foreach (var item in items.Results)
            {
                var cloudBlob = (CloudBlockBlob)item;

                using (Stream stream = new MemoryStream())
                {
                    await cloudBlob.DownloadToStreamAsync(stream);

                    string destinationName = $"{at.ProductionVersion}\\{(item.Uri.Segments.Last().EndsWith(".csv") ? item.Uri.Segments.Last() : item.Uri.Segments.Last() + ".csv")}";

                    await CreateBlobAsync(conStr, destinationName, stream, at.ProductionContainer);
                }

                await cloudBlob.DeleteAsync();
            }

            return true;
        }

        /// <summary>
        /// Creates and uploads a blob
        /// </summary>
        /// <param name="connectionString">connection string</param>
        /// <param name="name">filename including directory</param>
        /// <param name="data">content to write</param>
        /// <param name="containerName">name of container</param>
        /// <returns></returns>
        public static async Task CreateBlobAsync(string connectionString, string name, string data, string containerName)
        {
            CloudStorageAccount storageAccount;
            CloudBlobClient client;
            CloudBlobContainer container;
            CloudBlockBlob blob;

            storageAccount = CloudStorageAccount.Parse(connectionString);

            client = storageAccount.CreateCloudBlobClient();

            container = client.GetContainerReference(containerName);

            await container.CreateIfNotExistsAsync();

            blob = container.GetBlockBlobReference(name);

            using (Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(data)))
            {
                await blob.UploadFromStreamAsync(stream);
            }
        }

        /// <summary>
        /// Creates and uploads a blob
        /// </summary>
        /// <param name="connectionString">connection string</param>
        /// <param name="name">filename including directory</param>
        /// <param name="data">content to write</param>
        /// <param name="containerName">name of container</param>
        /// <returns></returns>
        private async static Task CreateBlobAsync(string connectionString, string name, Stream data, string containerName)
        {
            CloudStorageAccount storageAccount;
            CloudBlobClient client;
            CloudBlobContainer container;
            CloudBlockBlob blob;

            storageAccount = CloudStorageAccount.Parse(connectionString);

            client = storageAccount.CreateCloudBlobClient();

            container = client.GetContainerReference(containerName);

            await container.CreateIfNotExistsAsync();

            blob = container.GetBlockBlobReference(name);

            await blob.UploadFromStreamAsync(data);
        }

        /// <summary>
        /// Checks if version exists on server
        /// </summary>
        /// <param name="conStr">connection string</param>
        /// <param name="version">version to check for</param>
        /// <param name="containerName">name of container</param>
        /// <returns></returns>
        public static async Task<bool> IsVersionExistingOnServerAsync(string conStr, Version version, string containerName)
        {
            CloudStorageAccount storageAccount;
            storageAccount = CloudStorageAccount.Parse(conStr);

            CloudBlobClient client;
            client = storageAccount.CreateCloudBlobClient();

            CloudBlobContainer container;
            container = client.GetContainerReference(containerName);

            var dir = container.GetDirectoryReference(version.ToString());

            var items = await dir.ListBlobsSegmentedAsync(null);

            return items.Results.Any();
        }
    }
}
