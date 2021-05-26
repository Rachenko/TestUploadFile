using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TestUpload.Interface;
using TestUpload.Models;

namespace TestUpload.Controllers
{
    public class HomeController : Controller
    {
        private readonly IValidationProcess _IValidationProcess;
        private readonly ITransactionProcess _ITransactionProcess;
        private readonly DatabaseContext _context;
        public HomeController(DatabaseContext context,IValidationProcess IValidationProcess, ITransactionProcess ITransactionProcess)
        {
            this._IValidationProcess = IValidationProcess;
            this._ITransactionProcess = ITransactionProcess;
            this._context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        [HttpPost]
        public IActionResult Index(IFormFile files)
        {
            List<TransactionModel> dataList = new List<TransactionModel>();
            try
            {
                if (files != null)
                {
                    if (_IValidationProcess.CheckFileSizeOver1MB(files))
                    {
                        var errorModel = new ResultModel()
                        {
                            responseCode = "400",
                            responseMessage = "BadRequest",
                            messageError = "File size not over 1 MB."
                        };
                        return View("Index", errorModel);
                    }
                    if (files.Length > 0)
                    {
                        var fileName = Path.GetFileName(files.FileName);
                        var fileExtension = Path.GetExtension(fileName);
                        if (!_IValidationProcess.CheckFileType(fileExtension))
                        {
                            var errorModel = new ResultModel()
                            {
                                responseCode = "400",
                                responseMessage = "BadRequest",
                                messageError = "Not support format " + fileExtension
                            };
                            return View("Index", errorModel);
                        }
            
                        try
                        {
                            if (fileExtension == ".csv")
                            {
                                dataList = _IValidationProcess.ReadFileCSV(files);
                            }
                            else if (fileExtension == ".xml")
                            {
                                dataList = _IValidationProcess.ReadFileXML(files);
                            }
                        }
                        catch (Exception ex)
                        {
                            var errorModel = new ResultModel()
                            {
                                responseCode = "400",
                                responseMessage = "BadRequest",
                                messageError = ex.Message
                            };
                            return View("Index", errorModel);
                        }
                      

                        if (dataList.Count > 0)
                        {
                            _ITransactionProcess.Save(dataList, fileExtension, fileName);
                        }

                    }
                }
                else
                {
                    var errorModel = new ResultModel()
                    {
                        responseCode = "400",
                        responseMessage = "BadRequest",
                        messageError = "File Not found.Please select file before Upload."
                    };
                    return View("Index", errorModel);
                }
            }
            catch (Exception ex)
            {
                var errorModel = new ResultModel()
                {
                    responseCode = "500",
                    responseMessage = "InternalError",
                    messageError = ex.Message
                };
                return View("Index", errorModel);
            }
            var successModel = new ResultModel()
            {
                responseCode = "200",
                responseMessage = "Save to DB Success " + dataList.Count.ToString() + " Record(s)",
                messageError = ""
            };
            return View("Index", successModel);
        }
        [HttpGet]
        [Route("GetAllTransactions")]
        public async Task<ActionResult> GetAllTransactions()
        {
            List<TransactionOutputModel> result = new List<TransactionOutputModel>();
            try
            {
                 result = _ITransactionProcess.GetAll();
            }
            catch(Exception ex)
            {
                var errorModel = new ResultModel()
                {
                    responseCode = "500",
                    responseMessage = "InternalError",
                    messageError = ex.Message
                };
                return BadRequest(errorModel);
            }
 
            return Ok(result);

        }
        [HttpGet]
        [Route("GetTransactionsByCurrency/{currencyCode}")]
        public async Task<ActionResult> GetTransactionsByCurrency(string currencyCode)
        {
            List<TransactionOutputModel> result = new List<TransactionOutputModel>();
            try
            {
                result = _ITransactionProcess.GetByCurrency(currencyCode);
            }
            catch (Exception ex)
            {
                var errorModel = new ResultModel()
                {
                    responseCode = "500",
                    responseMessage = "InternalError",
                    messageError = ex.Message
                };
                return BadRequest(errorModel);
            }

            return Ok(result);

        }
        [HttpGet]
        [Route("GetTransactionsDateRange/{dateFrom}/{dateTo}")]
        public async Task<ActionResult> GetTransactionsDateRange(DateTime dateFrom,DateTime dateTo)
        {
            List<TransactionOutputModel> result = new List<TransactionOutputModel>();
            try
            {
                result = _ITransactionProcess.GetByDateRange(dateFrom, dateTo);
            }
            catch (Exception ex)
            {
                var errorModel = new ResultModel()
                {
                    responseCode = "500",
                    responseMessage = "InternalError",
                    messageError = ex.Message
                };
                return BadRequest(errorModel);
            }

            return Ok(result);

        }
        [HttpGet]
        [Route("GetTransactionsByStatus/{status}")]
        public async Task<ActionResult> GetTransactionsByStatus(string status)
        {
            List<TransactionOutputModel> result = new List<TransactionOutputModel>();
            try
            {
                result = _ITransactionProcess.GetByStatus(status);
            }
            catch (Exception ex)
            {
                var errorModel = new ResultModel()
                {
                    responseCode = "500",
                    responseMessage = "InternalError",
                    messageError = ex.Message
                };
                return BadRequest(errorModel);
            }

            return Ok(result);

        }

    }

}

