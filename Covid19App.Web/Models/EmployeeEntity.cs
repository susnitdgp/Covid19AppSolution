using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace Covid19App.Web.Models
{
    public class EmployeeEntity : TableEntity
    {
        public EmployeeEntity(string lastName, string firstName)
        {
            this.PartitionKey = lastName; this.RowKey = firstName;
        }
        public EmployeeEntity() { }


        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }
}
    
