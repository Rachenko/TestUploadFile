using System;
using System.Collections.Generic;

namespace TestUpload.Models
{
    public class TransactionErrorModel
    {
        public string transactionId { get; set; }
        public List<string> messageErrorList { get; set; }
        public bool isError { get; set; }
    }
}