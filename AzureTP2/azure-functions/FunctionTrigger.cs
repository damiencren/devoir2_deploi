using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Queues;
using System;
using System.IO;
using System.Threading.Tasks;

namespace BlobToQueueFunction
{
    class Program
    {
        string connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
        private const string containerName = "/default/input-images";
        private const string queueName = "/default/queue";

        static async Task Main(string[] args)
        {
            Console.WriteLine("Start : " + DateTime.Now.ToString());

            // Create Blob Container Client
            BlobServiceClient serviceClient = new BlobServiceClient(connectionString);
            BlobContainerClient blobClient = serviceClient.GetBlobContainerClient(containerName);

            // Monitor the Blob Container for new files
            await foreach (BlobItem blobItem in blobClient.GetBlobsAsync())
            {
                string filename = blobItem.Name;

                // Send filename to Azure Queue
                QueueClient queueClient = new QueueClient(connectionString, queueName);
                await queueClient.SendMessageAsync(filename);

                Console.WriteLine($"Sent file name '{filename}' to queue.");
            }
        }
    }
}
