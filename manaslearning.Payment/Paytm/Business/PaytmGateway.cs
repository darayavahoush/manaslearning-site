using manaslearning.Payment.Paytm.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Paytm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace manaslearning.Payment
{
    public class PaytmGateway : IPaytmGateway
    {       
        private readonly ILogger _logger;
        private PaytmConfiguration _paytmConfiguration;
        public PaytmGateway(ILogger<PaytmGateway> logger)
        {                        
            this._logger = logger;
        }

        public void SetPaytmConfiguration(PaytmConfiguration paytmConfiguration)
        {
            _paytmConfiguration = paytmConfiguration;
        }

        public string InitiateTransactionAPI(string orderOid, string currency, string amount, string callbackUrl, long userId)
        {
            string errorSourceWithInputParams = $"Source=PaytmGateway.InitiateTransactionAPI(), OrderOid={orderOid}";
            try
            {
                var paytmConfigurationValidation = _paytmConfiguration.IsValid();
                if (!paytmConfigurationValidation.Item1)
                {
                    _logger.LogError($"{ errorSourceWithInputParams }, ErrorMessage={ paytmConfigurationValidation.Item2 }");
                    throw new Exception("Paytm configuration Bad request");
                }

                Dictionary<string, object> body = new Dictionary<string, object>();
                Dictionary<string, string> head = new Dictionary<string, string>();
                Dictionary<string, object> requestBody = new Dictionary<string, object>();

                Dictionary<string, string> txnAmount = new Dictionary<string, string>();
                txnAmount.Add("value", amount.ToString());
                txnAmount.Add("currency", currency);
                Dictionary<string, string> userInfo = new Dictionary<string, string>();
                userInfo.Add("custId", userId.ToString());
                body.Add("requestType", _paytmConfiguration.RequestType);
                body.Add("mid", _paytmConfiguration.MId);
                body.Add("websiteName", _paytmConfiguration.WebsiteName);
                body.Add("orderId", orderOid.ToString());
                body.Add("txnAmount", amount);
                body.Add("userInfo", userInfo);
                body.Add("callbackUrl", callbackUrl);

                string paytmChecksum = Checksum.generateSignature(JsonConvert.SerializeObject(body), _paytmConfiguration.MKey);
                if (paytmChecksum == null)
                {
                    _logger.LogError($"{ errorSourceWithInputParams }, ErrorMessage=Paytm CheckSum is null");
                    throw new Exception("Paytm Checksum Bad request");
                }

                head.Add("signature", paytmChecksum);
                requestBody.Add("body", body);
                requestBody.Add("head", head);

                string postingData = JsonConvert.SerializeObject(requestBody);
                string url = $"{_paytmConfiguration.InitiateTransactionAPI}?mid={_paytmConfiguration.MId}&orderId={orderOid}";

                //TODO : Use HttpClient instead of HttpWebRequest
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
                webRequest.Method = "POST";
                webRequest.ContentType = "application/json";
                webRequest.ContentLength = postingData.Length;
                using (StreamWriter requestWriter = new StreamWriter(webRequest.GetRequestStream()))
                {
                    requestWriter.Write(postingData);
                }
                string responseData = string.Empty;
                using (StreamReader responseReader = new StreamReader(webRequest.GetResponse().GetResponseStream()))
                {
                    responseData = responseReader.ReadToEnd();
                }
                var response = JsonConvert.DeserializeObject<PaytmRequest.Root>(responseData);
                return response.body.txnToken;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error={ex.Message}, {errorSourceWithInputParams}");
                throw;
            }
        }

        public Tuple<PaytmResponse.Root, bool> GetPaymentResponse(string orderId)
        {
            string errorSourceWithInputParams = $"Source=PaytmGateway.GetPaymentResponse(), OrderOid={orderId}";
            try
            {
                Dictionary<string, string> body = new Dictionary<string, string>();
                Dictionary<string, string> head = new Dictionary<string, string>();
                Dictionary<string, Dictionary<string, string>> requestBody = new Dictionary<string, Dictionary<string, string>>();

                string mid = _paytmConfiguration.MId;
                string mkey = _paytmConfiguration.MKey;

                body.Add("mid", mid);
                body.Add("orderId", orderId);

                string paytmChecksum = Checksum.generateSignature(JsonConvert.SerializeObject(body), mkey);

                head.Add("signature", paytmChecksum);

                requestBody.Add("body", body);
                requestBody.Add("head", head);

                string requestData = JsonConvert.SerializeObject(requestBody);

                string url = _paytmConfiguration.OrderStatusAPI;

                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);

                webRequest.Method = "POST";
                webRequest.ContentType = "application/json";
                webRequest.ContentLength = requestData.Length;

                using (StreamWriter requestWriter = new StreamWriter(webRequest.GetRequestStream()))
                {
                    requestWriter.Write(requestData);
                }

                string responseData = string.Empty;
                using (StreamReader responseReader = new StreamReader(webRequest.GetResponse().GetResponseStream()))
                {
                    responseData = responseReader.ReadToEnd();
                }
                var response = JsonConvert.DeserializeObject<PaytmResponse.Root>(responseData);

                bool isVerifySignature = Checksum.verifySignature(JsonConvert.SerializeObject(response.body), mkey, response.head.signature);
                
                return new Tuple<PaytmResponse.Root, bool>(response, isVerifySignature);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error={ex.Message}, {errorSourceWithInputParams}");
                throw;
            }
        }
    }
}
