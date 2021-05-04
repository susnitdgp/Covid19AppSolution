using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Covid19App.Web.Models;
using Covid19App.Web.Utility;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;

namespace Covid19App.Web.Services
{
    public class FeedbackRepository : IFeedbackRepository
    {
       

        private IConfiguration configuration;

        private CloudTable table = null;

        private CloudBlobClient cloudBlobClient = null;
        private CloudQueueClient cloudQueueClient = null;

        public FeedbackRepository(IConfiguration _configuration)
        {
            configuration = _configuration;

            //Read Connection String from AZURE portal
            //var connString=configuration.GetConnectionString("AzureTableStorageConnString");
            //Read Application Settings from Azure Portal
            //var a = configuration["ANCM_ADDITIONAL_ERROR_PAGE_LINK"];

            Boolean useHttps = true;
            var accountName = configuration["StoargeAccountName"];
            var keyValue = configuration["StoargeAccoutKey"];

            var storageCredentials = new StorageCredentials(accountName, keyValue);
            var storageAccount = new CloudStorageAccount(storageCredentials, useHttps);

            CloudTableClient client = storageAccount.CreateCloudTableClient();


            table = client.GetTableReference("Feedback");

            cloudBlobClient = storageAccount.CreateCloudBlobClient();

            cloudQueueClient = storageAccount.CreateCloudQueueClient();
            

        }

        public void Add(FeedbackTableModel feedback, byte[] binaryContent)
        {

            FeedbackUility utility = new FeedbackUility();
            var extension = Path.GetExtension(feedback.FileName);
            

            //Azure Blob Storage Insertion
            try
            {
                //string data = Base64Decode(binaryContent);
                //byte[] bytes = Encoding.UTF8.GetBytes(data);
                var _task = Task.Run(() => this.UploadFileToBlobAsync(feedback.FileName, binaryContent, utility.getMimeType(extension)));
                _task.Wait();
                string fileUrl = _task.Result;

                feedback.BlobUrl = fileUrl;

                //return fileUrl;
            }
            catch (Exception ex)
            {
                throw (ex);
            }

            //AzureStoarge Table Insertion

            Task<bool> abc=table.CreateIfNotExistsAsync();
            bool result = abc.Result;

           
            TableOperation insertOperation = TableOperation.InsertOrMerge(feedback);
            table.ExecuteAsync(insertOperation);
            
            

            //this.sendMessageToQueue("Inserted");

        }


        public void Add(FeedbackTableModel feedback)
        {

            
            
            //AzureStoarge Table Insertion

            Task<bool> abc = table.CreateIfNotExistsAsync();
            bool result = abc.Result;


            TableOperation insertOperation = TableOperation.InsertOrMerge(feedback);
            table.ExecuteAsync(insertOperation);



            //this.sendMessageToQueue("Inserted");

        }

        public IEnumerable<FeedbackTableModel> GetByTiles(string tiles,string subtiles)
        {

            TableQuery<FeedbackTableModel> feedbackQuery = new TableQuery<FeedbackTableModel>()
                .Where
                (TableQuery.CombineFilters(
                    TableQuery.GenerateFilterCondition("TilesName", QueryComparisons.Equal, tiles),
                    TableOperators.And, TableQuery.GenerateFilterCondition("SubTilesName", QueryComparisons.Equal, subtiles)));

            

            TableContinuationToken token = null;
            var entities = new List<FeedbackTableModel>();
            do
            {
                var queryResult = table.ExecuteQuerySegmentedAsync(feedbackQuery, token);
                entities.AddRange(queryResult.Result);
                token = queryResult.Result.ContinuationToken;

            } while (token != null);

            return entities;

        }

        public IEnumerable<FeedbackTableModel> GetAll()
        {

            TableContinuationToken token = null;
            var entities = new List<FeedbackTableModel>();
            do
            {
                var queryResult = table.ExecuteQuerySegmentedAsync(new TableQuery<FeedbackTableModel>(), token);
                entities.AddRange(queryResult.Result);
                token = queryResult.Result.ContinuationToken;

            } while (token != null);

            return entities;
        }

        private async Task<string> UploadFileToBlobAsync(string strFileName, byte[] fileData, string fileMimeType)
        {
            try
            {

                string strContainerName = "covid-feedback-conntainer";
                CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(strContainerName);
                string fileName = this.GenerateFileName(strFileName);

                if (await cloudBlobContainer.CreateIfNotExistsAsync())
                {
                    await cloudBlobContainer.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
                }

                if (fileName != null && fileData != null)
                {
                    CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(fileName);
                    cloudBlockBlob.Properties.ContentType = fileMimeType;
                    await cloudBlockBlob.UploadFromByteArrayAsync(fileData, 0, fileData.Length);
                    return cloudBlockBlob.Uri.AbsoluteUri;
                }
                return "";
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        private string GenerateFileName(string fileName)
        {
            string strFileName = string.Empty;
            string[] strName = fileName.Split('.');
            strFileName = DateTime.Now.ToLocalTime().ToString("yyyy-MM-dd") + "/" + DateTime.Now.ToLocalTime().ToString("yyyyMMdd\\THHmmssfff") + "." + strName[strName.Length - 1];
            return strFileName;
        }

        private static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
            
        }

        private void sendMessageToQueue(string message)
        {

            CloudQueue cloudQueue = cloudQueueClient.GetQueueReference("covid-file-upload-queue");
            CloudQueueMessage queueMessage = new CloudQueueMessage("Hello, Message Created by Console Application");
            cloudQueue.AddMessageAsync(queueMessage);

        }

    }
}
