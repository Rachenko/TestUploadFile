using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using TestUpload.Interface;
using TestUpload.Models;

namespace TestUpload.Process
{
    public class TransactionProcess : ITransactionProcess
    {
        private static HttpClient _client;
        private static DatabaseContext _context;
        private const string strXML = ".xml";
        private const string strCSV = ".csv";
        public TransactionProcess(DatabaseContext context,HttpClient client)
        {
            _client = client;
            _context = context;
        }
        public void Save(List<TransactionModel> dataList,string sourceType, string fileName)
        {
            try
            {
                foreach (var data in dataList)
                {
                    var objfiles = new TblTest()
                    {
                        TransactionId = data.transactionId,
                        TransactionDate = data.transactionDate,
                        CurrencyCode = data.paymentDetail.currencyCode,
                        Amount = data.paymentDetail.amount,
                        Status = data.status,
                        SourceType = sourceType,
                        CreateDate = DateTime.Now,
                        FileName = fileName,

                    };
                    _context.tblTest.Add(objfiles);
                    _context.SaveChanges();
                    
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }           
        }
        public List<TransactionOutputModel> GetAll()
        {
            List<TransactionOutputModel> result = new List<TransactionOutputModel>();
            try
            {
                var list = _context.tblTest.AsEnumerable();
                result = list.Select(d => new TransactionOutputModel()
                { id = d.TransactionId,
                    payment = d.Amount + " " + d.CurrencyCode,
                    Status = GetStatusCode(d)
                }).ToList();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        public List<TransactionOutputModel> GetByCurrency(string currencyCode)
        {
            List<TransactionOutputModel> result = new List<TransactionOutputModel>();
            try
            {
                var list = _context.tblTest.AsEnumerable().Where(q=>q.CurrencyCode == currencyCode);
                result = list.Select(d => new TransactionOutputModel()
                {
                    id = d.TransactionId,
                    payment = d.Amount + " " + d.CurrencyCode,
                    Status = GetStatusCode(d)
                }).ToList();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        public List<TransactionOutputModel> GetByDateRange(DateTime dateFrom,DateTime dateTo)
        {
            List<TransactionOutputModel> result = new List<TransactionOutputModel>();
            try
            {
                var list = _context.tblTest.AsEnumerable().Where(q => q.TransactionDate >= dateFrom && q.TransactionDate <= dateTo);
                result = list.Select(d => new TransactionOutputModel()
                {
                    id = d.TransactionId,
                    payment = d.Amount + " " + d.CurrencyCode,
                    Status = GetStatusCode(d)
                }).ToList();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        public List<TransactionOutputModel> GetByStatus(string status)
        {
            List<TransactionOutputModel> result = new List<TransactionOutputModel>();
            try
            {
                var list = _context.tblTest.AsEnumerable().Where(q => q.Status == status);
                result = list.Select(d => new TransactionOutputModel()
                {
                    id = d.TransactionId,
                    payment = d.Amount + " " + d.CurrencyCode,
                    Status = GetStatusCode(d)
                }).ToList();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        private string GetStatusCode(TblTest data)
        {
            string result = "";
            try
            {
                if (data.SourceType == strCSV)
                {
                    if (data.Status == "Approved")
                    {
                        result = "A";
                    }
                    else if (data.Status == "Failed")
                    {
                        result = "R";
                    }
                    else if (data.Status == "Finished")
                    {
                        result = "D";
                    }

                }
                else if (data.SourceType == strXML)
                {
                    if (data.Status == "Approved")
                    {
                        result = "A";
                    }
                    else if (data.Status == "Rejected")
                    {
                        result = "R";
                    }
                    else if (data.Status == "Done")
                    {
                        result = "D";
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
    }
}
