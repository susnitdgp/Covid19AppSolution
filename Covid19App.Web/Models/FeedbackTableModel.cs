using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace Covid19App.Web.Models
{
    public class FeedbackTableModel : TableEntity
    {
       

        public FeedbackTableModel(string date,string id) {

            this.RowKey = id;
            this.PartitionKey = date;

        }

        public FeedbackTableModel() { }

        public string TilesName { get; set; }

        public string SubTilesName { get; set; }

        public string Email { get; set; }

        public string EmpId { get; set; }

        public string Comments { get; set; }

        public string BlobUrl { get; set; }

        public string Link { get; set; }

        public string FileName { get; set; }
    }
}
