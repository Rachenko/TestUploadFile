using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using TestUpload.Interface;
using TestUpload.Models;

namespace TestUpload.Process
{
    public class ValidationProcess : IValidationProcess
    {
        private static HttpClient _client;
        private static DatabaseContext _context;
        private const Int32 MegaByte = 1;

        public ValidationProcess (DatabaseContext context,HttpClient client)
        {
            _client = client;
            _context = context;
        }
        public bool CheckFileSizeOver1MB(IFormFile files)
        {
            bool result = false;

            int MegaBytes = MegaByte * 1024 * 1024;
            var fileSize = files.Length;
            result = fileSize > MegaBytes;
            return result;

        }
        public bool CheckFileType(string fileExtension)
        {
            string[] typeFileList = { ".csv", ".xml" };
            bool result = false;
            if (typeFileList.Any(q=>q == fileExtension))
            {
                result = true;
            }
            return result;
        }
        public List<TransactionModel> ReadFileCSV(IFormFile files)
        {
            List<TransactionModel> resultList = new List<TransactionModel>();
            List<TransactionErrorModel> errorList = new List<TransactionErrorModel>();
            
            try
            {
                var filePath = Path.GetTempFileName();
                using (var stream = System.IO.File.Create(filePath))
                {
                    files.CopyTo(stream);
                }
                using (var reader = new StreamReader(Path.GetFullPath(filePath)))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var values = line.Split(',');
                        TransactionModel data = new TransactionModel();
                        TransactionErrorModel dataError = new TransactionErrorModel();
                        dataError.messageErrorList = new List<string>();
                        bool isError = false;
                        dataError.transactionId = (values[0]);
                        if ((!string.IsNullOrEmpty(values[0])) && (values[0]).Length <= 50)
                        {
                            data.transactionId = (values[0]);
                        }
                        else
                        {
                            isError = true;
                            dataError.messageErrorList.Add("transactionId Length not over 50");
                        }
                  
                        DateTime dt;
                        var checkDate = DateTime.TryParseExact((values[3]), "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture,DateTimeStyles.None,out dt);
                        if ((!string.IsNullOrEmpty(values[3])) && checkDate)
                        {
                            data.transactionDate = Convert.ToDateTime(dt);
                        }
                        else
                        {
                            isError = true;
                            dataError.messageErrorList.Add("TransactionDate not Format");
                        }
                        Decimal dc;
                        var checkAmount = Decimal.TryParse((values[1]), out dc);
                        if ((!string.IsNullOrEmpty(values[1])) && checkAmount)
                        {
                            data.paymentDetail = new PaymentDetail
                            {
                                currencyCode = (values[2]),
                                amount = Convert.ToDecimal((values[1]))
                            };
                        }
                        else
                        {
                            dataError.messageErrorList.Add("Amonut not Format");
                            isError = true;
                        }
                        
                        string[] statusList = { "Approved", "Failed","Finished" };
                        if ((!string.IsNullOrEmpty(values[4])) && statusList.Contains(values[4]))
                        {
                            data.status = (values[4]);
                        }
                        else
                        {
                            dataError.messageErrorList.Add("Status not Format");
                            isError = true;
                        }


                        dataError.isError = isError;
                        errorList.Add(dataError);
                        resultList.Add(data);
                    }
                }
                if (errorList.Any(q=>q.isError))
                {
                    throw new Exception("Unknown format : " + JsonConvert.SerializeObject(errorList.Where(q => q.isError).ToList()));
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return resultList;
        }
        public List<TransactionModel> ReadFileXML(IFormFile files)
        {
            List<TransactionModel> resultList = new List<TransactionModel>();
            List<TransactionErrorModel> errorList = new List<TransactionErrorModel>();

            try
            {
                var filePath = Path.GetTempFileName();
                using (var stream = System.IO.File.Create(filePath))
                {
                    files.CopyTo(stream);
                }
                using (XmlReader reader = XmlReader.Create(filePath))
                {
                    TransactionModel data = new TransactionModel();
                    TransactionErrorModel dataError = new TransactionErrorModel();
                  
                    bool isError = false;
                    while (reader.Read())
                    {
                        if (reader.IsStartElement())
                        {
                   
                            switch (reader.Name.ToString())
                            {
                         
                                case "Transaction":
                                    data = new TransactionModel();
                                    dataError = new TransactionErrorModel();
                                    isError = false;
                                    dataError.messageErrorList = new List<string>();
                                    dataError.transactionId = reader.GetAttribute("id");
                                    string transactionId = reader.GetAttribute("id");
                                    if ((!string.IsNullOrEmpty(transactionId)) && transactionId.Length <= 50)
                                    {
                                        data.transactionId = reader.GetAttribute("id");
                                    }
                                    else
                                    {
                                        isError = true;
                                        dataError.messageErrorList.Add("transactionId Length not over 50");
                                    }
                                    break;
                                    
                                case "TransactionDate":
                                    var ransactionDate = reader.ReadString();
                                    DateTime dt;
                                    var checkDate = DateTime.TryParseExact(ransactionDate, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt);
                                    if ((!string.IsNullOrEmpty(ransactionDate)) && checkDate)
                                    {
                                        data.transactionDate = Convert.ToDateTime(dt);
                                    }
                                    else
                                    {
                                        isError = true;
                                        dataError.messageErrorList.Add("TransactionDate not Format");
                                    }
                                    data.transactionDate = dt;
                                    break;
                                case "Amount":
                                    var amount = reader.ReadString();
                                    Decimal dc;
                                    var checAmount = Decimal.TryParse(amount, out dc);
                                    if ((!string.IsNullOrEmpty(amount)) && checAmount)
                                    {
                                        data.paymentDetail.amount = dc;
                                    }
                                    else
                                    {
                                        dataError.messageErrorList.Add("Amonut not Format");
                                        isError = true;
                                    }
                                    break;
                                case "CurrencyCode":
                                    data.paymentDetail.currencyCode = reader.ReadString();
                                    break;
                                case "Status":
                                    var status = reader.ReadString();
                                    string[] statusList = { "Approved", "Rejected", "Done" };
                                    if ((!string.IsNullOrEmpty(status)) && statusList.Contains(status))
                                    {
                                        data.status = status;
                                    }
                                    else
                                    {
                                        dataError.messageErrorList.Add("Status not Format");
                                        isError = true;
                                    }
                                    dataError.isError = isError;
                                    errorList.Add(dataError);
                                    resultList.Add(data);
                                    break;
                            }
                        }
                    }
                  
                }
                if (errorList.Any(q => q.isError))
                {
                    throw new Exception("Unknown format : " + JsonConvert.SerializeObject(errorList.Where(q => q.isError).ToList()));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return resultList;
        }
    }
}
