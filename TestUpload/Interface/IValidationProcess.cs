using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestUpload.Models;

namespace TestUpload.Interface
{
    public interface IValidationProcess
    {
        bool CheckFileSizeOver1MB(IFormFile files);
        bool CheckFileType(string fileExtension);

        List<TransactionModel> ReadFileCSV(IFormFile files);

        List<TransactionModel> ReadFileXML(IFormFile files);
    }
}
