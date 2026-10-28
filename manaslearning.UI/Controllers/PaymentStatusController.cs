using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;

namespace manaslearning.UI.Controllers.User
{
    [Route("api/u/[controller]")]
    [ApiController]   
    public class PaymentStatusController : ControllerBase
    {
        private readonly ILogger _logger;

        public PaymentStatusController(            
            ILogger<PaymentStatusController> logger)
        {            
            this._logger = logger;
        }

        
        // POST api/paymentstatus       
        [HttpPost]
        public async Task<IActionResult> PostAsync([FromForm] PaymentResponse model)
        {
            try
            {
                return Redirect(string.Format("~/u/paymentstatus/{0}", model.ORDERID));               
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}. Source=PaymentStatusController.PostAsync();PaymentTransactionOid={model.ORDERID}");
                return this.StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        public class PaymentResponse
        {
            public string CURRENCY { get; set; }
            public string GATEWAYNAME { get; set; }
            public string RESPMSG { get; set; }
            public string BANKNAME { get; set; }
            public string PAYMENTMODE { get; set; }
            public string MID { get; set; }
            public string RESPCODE { get; set; }
            public string TXNID { get; set; }
            public string TXNAMOUNT { get; set; }
            public string ORDERID { get; set; }
            public string STATUS { get; set; }
            public string BANKTXNID { get; set; }
            public string TXNDATE { get; set; }
            public string CHECKSUMHASH { get; set; }
        }
    }
}
