using System;
using System.ComponentModel.DataAnnotations;

namespace TestUpload.Models
{

    public class TransactionModel
    {
        private PaymentDetail _paymentDetail = new PaymentDetail();


           [MaxLength(50)]
        public string transactionId { get; set; }
  
        public DateTime transactionDate { get; set; }
        public PaymentDetail paymentDetail
        {
            get { return _paymentDetail; }
            set { _paymentDetail = value; }
        }

        public string status { get; set; }
    }
    public class PaymentDetail
    {
        public string currencyCode { get; set; }

        public Decimal amount { get; set; }
    }
}