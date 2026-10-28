using manaslearning.Business.Interfaces;
using manaslearning.UI.Helpers;
using manaslearning.UI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace manaslearning.UI.Controllers.User
{
    [Route("api/u/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly ICurrencyRepository _currencyRepository;
        private readonly IPaymentStatusRepository _paymentStatusRepository;
        private readonly ILogger _logger;

        public PaymentController(
            IPaymentRepository paymentRepository,
            IOrderRepository orderRepository,         
            ICurrencyRepository currencyRepository,
            IPaymentStatusRepository paymentStatusRepository,
            ILogger<PaymentController> logger)
        {
            this._paymentRepository = paymentRepository;
            this._orderRepository = orderRepository;            
            this._currencyRepository = currencyRepository;            
            this._paymentStatusRepository = paymentStatusRepository;
            this._logger = logger;
        }

        // POST api/payment        
        [HttpPost]
        public async Task<IActionResult> PostAsync([FromForm] PaymentRequestModel model)
        {
            long _userId = 0;
            try
            {
                _userId = HttpContext.GetUserId();

                if (!IsPaymentModelValid(model, _userId))
                    return this.BadRequest();

                var result = await _paymentRepository.CreatePaymentAsync(
                        _userId,
                        model.OrderOid,
                        model.CurrencyCode,
                        model.PaymentTransactionOid                        
                    );               

                if (result == -1)
                    return this.StatusCode((int)HttpStatusCode.InternalServerError);
                else if (result <= -2)
                    return this.BadRequest();

                return Ok("Success");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}. Source=PaymentController.PostAsync();UserId={_userId};OrderOid={model.OrderOid}");
                return this.StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        //TODO : Create a new action to update the status

        /// <summary>
        /// Determines PaymentRequestModel model is valid.
        /// </summary>
        /// <param name="model"></param>
        /// <returns>bool</returns>
        private bool IsPaymentModelValid(PaymentRequestModel model, long userId)
        {
            var order = _orderRepository.GetOrderByOidAsync(userId, model.OrderOid).Result;
            if (order == null)
                return false;

            return true;
        }
    }
}
