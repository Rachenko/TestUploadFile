using System;
using System.Collections.Generic;
using TestUpload.Models;

namespace TestUpload.Interface
{
    public interface ITransactionProcess
    {
        void Save(List<TransactionModel> dataList, string sourceType , string fileName);
        List<TransactionOutputModel> GetAll();
        List<TransactionOutputModel> GetByCurrency(string currencyCode);
        List<TransactionOutputModel> GetByDateRange(DateTime dateFrom, DateTime dateTo);
        List<TransactionOutputModel> GetByStatus(string status);
    }
}
