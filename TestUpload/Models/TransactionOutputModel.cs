using System;
using System.ComponentModel.DataAnnotations;

namespace TestUpload.Models
{

    public class TransactionOutputModel
    {
        public string id { get; set; }
        public string payment { get; set; }
        public string Status { get; set; }
    }

}