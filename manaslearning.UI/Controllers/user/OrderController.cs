using manaslearning.Business.Interfaces;
using manaslearning.Helpers;
using manaslearning.Models;
using manaslearning.Shared;
using manaslearning.T4;
using manaslearning.T4.Templates;
using manaslearning.T4.Templates.Models;
using manaslearning.UI.Helpers;
using manaslearning.UI.Models;
using manaslearning.UI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace manaslearning.UI.Controllers.User
{
    [Route("api/u/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IAddressRepository _addressRepository;
        private readonly ICurrencyRepository _currencyRepository;        
        private readonly IItemRepository _itemRepository;
        private readonly ITemplateGenerater _templateGenerater;
        private readonly IStorageHelper _storageHelper;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public OrderController(
            IOrderRepository orderRepository,
            IAddressRepository addressRepository,
            ICurrencyRepository currencyRepository,
            IItemRepository itemRepository,            
            ITemplateGenerater templateGenerater,
            IStorageHelper storageHelper,
            IConfiguration configuration,
            ILogger<OrderController> logger)
        {
            _orderRepository = orderRepository;
            _addressRepository = addressRepository;
            _currencyRepository = currencyRepository;
            _itemRepository = itemRepository;
            _templateGenerater = templateGenerater;
            _storageHelper = storageHelper;
            _configuration = configuration;
            _logger = logger;
        }

        // GET api/Order
        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            long _userId = 0;
            try
            {
                _userId = HttpContext.GetUserId();

                var orders = await _orderRepository.GetOrdersInfoAsync(_userId);
                return Ok(orders.Select(o => new OrderDto
                {
                    Oid = o.Oid,
                    BillingAddressLine1 = o.BillingAddressLine1,
                    BillingAddressLine2 = o.BillingAddressLine2,
                    BillingAddressLine3 = o.BillingAddressLine3,
                    BillingCity = o.BillingCity,
                    BillingCountry = o.BillingCountry,
                    BillingName = o.BillingName,
                    BillingState = o.BillingState,
                    BillingZipCode = o.BillingZipCode,
                    CurrencyCode = o.CurrencyCode,
                    CreatedDate = o.CreatedDate,
                    CurrencyName = o.CurrencyName,
                    CurrencySymbol = o.CurrencySymbol,
                    Discount = o.Discount,
                    GSTAmount = o.GSTAmount,
                    GSTRate = o.GSTRate,
                    ItemDescription = o.ItemDescription,
                    ItemOid = o.ItemOid,
                    ItemPrice = o.ItemPrice,
                    OrderNumber = o.OrderNumber,
                    OrderStatusName = o.OrderStatusName,
                    Quantity = o.Quantity,
                    TotalAmount = o.TotalAmount
                }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}. Source=OrderController.GetAsync();UserId={_userId}");
                return this.StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        // GET api/Order?Oid={oid}
        [HttpGet("{oid:Guid}")]
        public async Task<IActionResult> GetAsync(Guid oid)
        {
            long _userId = 0;
            try
            {
                _userId = HttpContext.GetUserId();
                var order = await _orderRepository.GetOrderInfoByOidAsync(_userId, oid);
                return Ok(new OrderDto
                {
                    Oid = order.Oid,
                    BillingAddressLine1 = order.BillingAddressLine1,
                    BillingAddressLine2 = order.BillingAddressLine2,
                    BillingAddressLine3 = order.BillingAddressLine3,
                    BillingCity = order.BillingCity,
                    BillingCountry = order.BillingCountry,
                    BillingName = order.BillingName,
                    BillingState = order.BillingState,
                    BillingZipCode = order.BillingZipCode,
                    CurrencyCode = order.CurrencyCode,
                    CreatedDate = order.CreatedDate,
                    CurrencyName = order.CurrencyName,
                    CurrencySymbol = order.CurrencySymbol,
                    Discount = order.Discount,
                    GSTAmount = order.GSTAmount,
                    GSTRate = order.GSTRate,
                    ItemDescription = order.ItemDescription,
                    ItemOid = order.ItemOid,
                    ItemPrice = order.ItemPrice,
                    OrderNumber = order.OrderNumber,
                    OrderStatusName = order.OrderStatusName,
                    Quantity = order.Quantity,
                    TotalAmount = order.TotalAmount
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}. Source=OrderController.GetAsync();UserId={_userId},Oid={oid}");
                return this.StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        // POST api/order        
        [HttpPost]
        public async Task<IActionResult> PostAsync([FromForm] OrderRequestModel model)
        {
            long _userId = 0;
            try
            {
                _userId = HttpContext.GetUserId();
                int organizationId = HttpContext.GetOrganizationId();

                if (model.CurrencyId <= 0 || model.CurrencyId == null)
                {
                    model.CurrencyId = 1;
                }

                if (!IsOrderModelValid(model, organizationId))
                    return this.BadRequest();

                string orderNumber = $"{DateTime.UtcNow.ToString("yyyyMMdd")}ML{GenerateRandom(4)}{GenerateRandom(4)}";

                var result = await _orderRepository.CreateOrderAsync(
                        _userId,
                        organizationId,
                        model.BillingAddressOid,
                        model.ItemOid,
                        model.CurrencyId.Value,
                        model.Quantity,
                        orderNumber
                    );

                if (result == -1) // -1: 
                    return this.StatusCode((int)HttpStatusCode.InternalServerError);
                else if (result <= -2) // -2: 
                    return this.BadRequest();

                var order = await _orderRepository.GetOrderInfoAsync(_userId, result);

                return Ok(new OrderDto
                {
                    Oid = order.Oid,
                    BillingAddressLine1 = order.BillingAddressLine1,
                    BillingAddressLine2 = order.BillingAddressLine2,
                    BillingAddressLine3 = order.BillingAddressLine3,
                    BillingCity = order.BillingCity,
                    BillingCountry = order.BillingCountry,
                    BillingName = order.BillingName,
                    BillingState = order.BillingState,
                    BillingZipCode = order.BillingZipCode,
                    CurrencyCode = order.CurrencyCode,
                    CreatedDate = order.CreatedDate,
                    CurrencyName = order.CurrencyName,
                    CurrencySymbol = order.CurrencySymbol,
                    Discount = order.Discount,
                    GSTAmount = order.GSTAmount,
                    GSTRate = order.GSTRate,
                    ItemDescription = order.ItemDescription,
                    ItemOid = order.ItemOid,
                    ItemPrice = order.ItemPrice,
                    OrderNumber = order.OrderNumber,
                    OrderStatusName = order.OrderStatusName,
                    Quantity = order.Quantity,
                    TotalAmount = order.TotalAmount
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}. Source=OrderController.PostAsync();UserId={_userId}");
                return this.StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        // PATCH api/updateorderstatus       
        [HttpPatch("[Action]")]
        public async Task<IActionResult> UpdateOrderStatusAsync([FromForm] OrderStatusRequestModel model)
        {
            long _userId = 0;
            try
            {
                _userId = HttpContext.GetUserId();

                var result = await _orderRepository.UpdateOrderStatusAsync(
                       _userId,
                       model.OrderOid,
                       model.OrderStatusId
                    );

                if (result == -1)
                    return this.StatusCode((int)HttpStatusCode.InternalServerError);
                else if (result <= -2)
                    return this.BadRequest();

                return Ok("success");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}. Source=OrderController.UpdateOrderStatusAsync();OrderOid={model.OrderOid},UserId={_userId}, OrderStatusId={model.OrderStatusId}");
                return this.StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        // POST api/u/order/SendInvoiceEmail/oid
        [HttpPost("[Action]")]
        public async Task<ActionResult> SendInvoiceEmailAsync([FromQuery] Guid oid)
        {
            long userId = 0;
            try
            {
                userId = HttpContext.GetUserId();
                if (userId == 0)
                    return LocalRedirect(UrlConstants.UNAUTHORIZED);
                var userEmail = HttpContext.GetClaim(ClaimTypes.Email);
                if (string.IsNullOrWhiteSpace(userEmail))
                {
                    _logger.LogError($"Error=User email not found. Source=OrderController.SendInvoiceEmailAsync();OrderOid={oid},UserId={userId}");
                    return BadRequest("BadRequest");
                }
                var orderInfo = await _orderRepository.GetOrderInfoByOidAsync(userId, oid);
                if (orderInfo == null)
                {
                    _logger.LogError($"Error=Order not found. Source=OrderController.SendInvoiceEmailAsync();OrderOid={oid},UserId={userId}");
                    return BadRequest("BadRequest");
                }

                // Trigger email: Send invoice to user
                var message = GetBase64StringMessage(
                    userId: userId,
                    orderOid: orderInfo.Oid);

                var storageAccountName = _configuration.GetValue<string>("StorageAccountName");
                if (string.IsNullOrWhiteSpace(storageAccountName))
                {
                    _logger.LogError($"ErrorMessage='StorageAccountName' secret does not exists in keyvault, Source=OrderController.SendInvoiceEmailAsync(), userId={userId}, orderOid={orderInfo.Oid}");
                    return BadRequest("BadRequest");
                }

                var assessmentExcelReportInputQueueName = _configuration.GetValue<string>("Queues:InputQueue:Invoice");
                if (string.IsNullOrWhiteSpace(assessmentExcelReportInputQueueName))
                {
                    _logger.LogError($"ErrorMessage='InvoiceInputQueueName' secret does not exists in keyvault, Source=OrderController.SendInvoiceEmailAsync(), userId={userId}, orderOid={orderInfo.Oid}");
                    return BadRequest("BadRequest");
                }

                var isSuccess = await _storageHelper.InsertMessageIntoQueueAsync(
                    storageAccountName: storageAccountName,
                    queueName: assessmentExcelReportInputQueueName,
                    message: message,
                    logger: _logger);

                return Ok("success");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error={ex.Message}. Source=OrderController.SendInvoiceEmailAsync();OrderOid={oid},UserId={userId}");
                return BadRequest("BadRequest");
            }
        }

        //GET api/u/order/DownloadInvoice/oid
        [HttpGet("[Action]")]
        public async Task<IActionResult> DownloadInvoiceAsync([FromQuery] Guid oid)
        {
            long _userId = 0;
            try
            {
                _userId = HttpContext.GetUserId();
                if (_userId == 0)
                    return LocalRedirect(UrlConstants.UNAUTHORIZED);
                var userEmail = HttpContext.GetClaim(ClaimTypes.Email);
                if (string.IsNullOrWhiteSpace(userEmail))
                {
                    _logger.LogError($"Error=User email not found. Source=OrderController.DownloadInvocieAsync();OrderOid={oid},UserId={_userId}");
                    return BadRequest("BadRequest");
                }
                var orderInfo = await _orderRepository.GetOrderInfoByOidAsync(_userId, oid);
                if (orderInfo == null)
                {
                    _logger.LogError($"Error=Order not found. Source=OrderController.DownloadInvocieAsync();OrderOid={oid},UserId={_userId}");
                    return BadRequest("BadRequest");
                }                
                var invoice = BuildInvoiceObject(orderInfo);
                var pdfData = _templateGenerater.GenerateInvoicePDF(invoice);                
                return File(pdfData, "application/pdf", $"{orderInfo.OrderNumber}.pdf");               
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error={ex.Message}. Source=OrderController.DownloadInvocieAsync();OrderOid={oid},UserId={_userId}");
                return BadRequest("BadRequest");
            }
        }

        /// <summary>
        /// Determines OrderRequestModel model is valid.
        /// </summary>
        /// <param name="model"></param>
        /// <returns>bool</returns>
        private bool IsOrderModelValid(OrderRequestModel model, int organizationId)
        {
            var item = _itemRepository.GetItemByOidAsync(model.ItemOid);
            if (item == null)
                return false;

            
            var billingAddress = _addressRepository.GetAddressByOidAsync(organizationId, model.BillingAddressOid);
            if (billingAddress == null)
                return false;

            if (model.CurrencyId <= 0)
                return false;

            if (model.Quantity <= 0)
                return false;

            var currencies = _currencyRepository.GetCurrenciesAsync().Result;
            if (!currencies.Any(c => c.CurrencyId == model.CurrencyId))
                return false;

            return true;
        }

        public string GenerateRandom(int length)
        {
            StringBuilder result = new StringBuilder();
            Random random = new Random();
            char letter;
            for (int i = 0; i < length; i++)
            {
                double flt = random.NextDouble();
                int shift = Convert.ToInt32(Math.Floor(25 * flt));
                letter = Convert.ToChar(shift + 65);
                result.Append(letter);
            }
            return result.ToString();
        }

        private Invoice BuildInvoiceObject(OrderInfo orderInfo, bool isEmail = false)
        {
            var customer = new Customer()
            {
                Name = orderInfo.BillingName,
                CustomerAddress = new T4.Templates.Models.Address()
                {
                    Address1 = orderInfo.BillingAddressLine1,
                    Address2 = orderInfo.BillingAddressLine2,
                    Address3 = orderInfo.BillingAddressLine3,
                    City = orderInfo.BillingCity,
                    State = orderInfo.BillingState,
                    Country = orderInfo.BillingCountry,
                    ZipCode = orderInfo.BillingZipCode
                }
            };
            var invoiceItems = new List<InvoiceItem>()
            {
                new InvoiceItem()
                {
                    PurchasedDate = orderInfo.PaymentDate?.ToString(StringConstants.DATE_FORMAT),
                    Price = orderInfo.ItemPrice,
                    SubTotal = orderInfo.ItemPrice * orderInfo.Quantity,
                    Description = orderInfo.ItemDescription,
                    ProductName = orderInfo.AssessmentName,
                    GSTAmount = orderInfo.GSTAmount * orderInfo.Quantity,
                    GSTRate = orderInfo.GSTRate,
                    Quantity = orderInfo.Quantity,
                    Sno = 1, // For now we are considering one item only 
                    TotalAmount = orderInfo.TotalAmount,
                    TotalAmountInWords = decimal.ToInt32(orderInfo.TotalAmount).ToWords(),
                    CurrencySymbol = string.IsNullOrWhiteSpace(orderInfo.CurrencySymbol) ? "₹" : orderInfo.CurrencySymbol
                }
             };
            var invoice = new Invoice()
            {
                InvoiceNumber = orderInfo.OrderNumber,
                InvoiceDate = orderInfo.PaymentDate?.ToString(StringConstants.DATE_FORMAT),
                IsEmail = isEmail,
                Customer = customer,
                InvoiceItems = invoiceItems,
                Company = new Company()                
            };
            invoice.Company.CompanyAddress = new T4.Templates.Models.Address();
            _configuration.GetSection("Company:Info").Bind(invoice.Company);
            _configuration.GetSection("Company:CompanyAddress").Bind(invoice.Company.CompanyAddress);
            return invoice;
        }

        /// <summary>
        /// Get Base64 string message
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="orderOid"></param>
        /// <returns>string</returns>
        private string GetBase64StringMessage(
            long userId,
            Guid orderOid)
        {
            var message = new
            {
                InstanceId = Guid.NewGuid(),
                UserId = userId,
                OrderOid = orderOid,
            };

            var plainText = JsonConvert.SerializeObject(message);
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }
    }
}
