using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace CheckInWorker2
{
    public class WorkerRole : RoleEntryPoint
    {
        // The name of your queue
        const string QueueName = "ProcessingQueue";

        // QueueClient is thread-safe. Recommended that you cache 
        // rather than recreating it on every request
        QueueClient Client;
        ManualResetEvent CompletedEvent = new ManualResetEvent(false);

        public override void Run()
        {
            Trace.WriteLine("Starting processing of messages");

            // Initiates the message pump and callback is invoked for each message that is received, calling close on the client will stop the pump.
            Client.OnMessage((receivedMessage) =>
                {
                    try
                    {
                        // Process the message
                        //ProcessQueueMessage(receivedMessage);
                        Trace.WriteLine("Processing Service Bus message: " + receivedMessage.SequenceNumber.ToString());
                    }
                    catch
                    {
                        // Handle any message processing specific exceptions here
                    }
                });

            CompletedEvent.WaitOne();
        }

        //private void ProcessQueueMessage(BrokeredMessage msg)
        //{
        //    Trace.TraceInformation("Processing queue message {0}", msg);

        //    // Queue message contains AdId.
        //    var adId = int.Parse(msg.);
        //    Ad ad = db.Ads.Find(adId);
        //    if (ad == null)
        //    {
        //        throw new Exception(String.Format("AdId {0} not found, can't create thumbnail", adId.ToString()));
        //    }

        //    Uri blobUri = new Uri(ad.ImageURL);
        //    string blobName = blobUri.Segments[blobUri.Segments.Length - 1];

        //    CloudBlockBlob inputBlob = this.imagesBlobContainer.GetBlockBlobReference(blobName);
        //    string thumbnailName = Path.GetFileNameWithoutExtension(inputBlob.Name) + "thumb.jpg";
        //    CloudBlockBlob outputBlob = this.imagesBlobContainer.GetBlockBlobReference(thumbnailName);

        //    using (Stream input = inputBlob.OpenRead())
        //    using (Stream output = outputBlob.OpenWrite())
        //    {
        //        ConvertImageToThumbnailJPG(input, output);
        //        outputBlob.Properties.ContentType = "image/jpeg";
        //    }
        //    Trace.TraceInformation("Generated thumbnail in blob {0}", thumbnailName);

        //    ad.ThumbnailURL = outputBlob.Uri.ToString();
        //    db.SaveChanges();
        //    Trace.TraceInformation("Updated thumbnail URL in database: {0}", ad.ThumbnailURL);

        //    // Remove message from queue.
        //    this.imagesQueue.DeleteMessage(msg);
        //}

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;

            // Create the queue if it does not exist already
            string connectionString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString");
            var namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);
            if (!namespaceManager.QueueExists(QueueName))
            {
                namespaceManager.CreateQueue(QueueName);
            }

            // Initialize the connection to Service Bus Queue
            Client = QueueClient.CreateFromConnectionString(connectionString, QueueName);
            return base.OnStart();
        }

        public override void OnStop()
        {
            // Close the connection to Service Bus Queue
            Client.Close();
            CompletedEvent.Set();
            base.OnStop();
        }
    }
}
