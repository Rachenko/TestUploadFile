using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestUpload.Models
{
    [Table("tblTest")]
    public class TblTest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [MaxLength(50)]
        public string TransactionId { get; set; }

        public DateTime TransactionDate { get; set; }
    
        public Decimal Amount { get; set; }
        [MaxLength(10)]
        public string CurrencyCode { get; set; }
        [MaxLength(50)]
        public string Status { get; set; }
        [MaxLength(50)]
        public string SourceType { get; set; }
        public DateTime CreateDate { get; set; }
        [MaxLength(100)]
        public string FileName { get; set; }
    }
}