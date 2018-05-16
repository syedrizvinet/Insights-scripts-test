﻿using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Knapcode.ExplorePackages.Logic
{
    public class FileStorageService
    {
        private readonly PackageFilePathProvider _filePathProvider;
        private readonly PackageBlobNameProvider _blobNameProvider;
        private readonly ExplorePackagesSettings _settings;
        private readonly ILogger<FileStorageService> _logger;

        private readonly Lazy<CloudBlobContainer> _lazyBlobContainer;

        public FileStorageService(
            PackageFilePathProvider filePathProvider,
            PackageBlobNameProvider blobNameProvider,
            ExplorePackagesSettings settings,
            ILogger<FileStorageService> logger)
        {
            _filePathProvider = filePathProvider;
            _blobNameProvider = blobNameProvider;
            _settings = settings;
            _logger = logger;

            _lazyBlobContainer = new Lazy<CloudBlobContainer>(GetBlobContainer);
        }

        private CloudBlobContainer BlobContainer => _lazyBlobContainer.Value;

        public async Task StoreMZipStreamAsync(string id, string version, Func<Stream, Task> writeAsync)
        {
            var filePath = _filePathProvider.GetLatestMZipFilePath(id, version);
            await SafeFileWriter.WriteAsync(filePath, writeAsync, _logger);

            var blobName = _blobNameProvider.GetLatestMZipBlobName(id, version);
            await StoreFileToBlobAsync(filePath, blobName);
        }

        public Task<Stream> GetMZipStreamOrNullAsync(string id, string version)
        {
            var filePath = _filePathProvider.GetLatestMZipFilePath(id, version);
            return Task.FromResult(GetStreamOrNull(filePath));
        }

        public Task DeleteMZipStreamAsync(string id, string version)
        {
            var filePath = _filePathProvider.GetLatestMZipFilePath(id, version);
            DeleteFile(filePath);
            return Task.CompletedTask;
        }

        public async Task StoreNuspecStreamAsync(string id, string version, Func<Stream, Task> writeAsync)
        {
            var filePath = _filePathProvider.GetLatestNuspecFilePath(id, version);
            await SafeFileWriter.WriteAsync(filePath, writeAsync, _logger);

            var blobName = _blobNameProvider.GetLatestNuspecPath(id, version);
            await StoreFileToBlobAsync(filePath, blobName);
        }

        public Task<Stream> GetNuspecStreamOrNullAsync(string id, string version)
        {
            var filePath = _filePathProvider.GetLatestNuspecFilePath(id, version);
            return Task.FromResult(GetStreamOrNull(filePath));
        }

        public Task DeleteNuspecStreamAsync(string id, string version)
        {
            var filePath = _filePathProvider.GetLatestNuspecFilePath(id, version);
            DeleteFile(filePath);
            return Task.CompletedTask;
        }

        private async Task StoreFileToBlobAsync(string filePath, string blobName)
        {
            if (BlobContainer == null)
            {
                return;
            }

            var blob = BlobContainer.GetBlockBlobReference(blobName);

            _logger.LogInformation("  PUT {BlobUri}", blob.Uri);
            await blob.UploadFromFileAsync(filePath);
        }

        private CloudBlobContainer GetBlobContainer()
        {
            if (_settings.StorageConnectionString == null || _settings.StorageContainerName == null)
            {
                return null;
            }

            var account = CloudStorageAccount.Parse(_settings.StorageConnectionString);
            var client = account.CreateCloudBlobClient();
            var container = client.GetContainerReference(_settings.StorageContainerName);

            return container;
        }

        private static void DeleteFile(string filePath)
        {
            try
            {
                File.Delete(filePath);
            }
            catch (FileNotFoundException)
            {
            }
            catch (DirectoryNotFoundException)
            {
            }
        }

        private static Stream GetStreamOrNull(string filePath)
        {
            Stream stream = null;
            try
            {
                return new FileStream(filePath, FileMode.Open);
            }
            catch (FileNotFoundException)
            {
                return null;
            }
            catch (DirectoryNotFoundException)
            {
                return null;
            }
            catch
            {
                stream?.Dispose();
                throw;
            }
        }
    }
}
