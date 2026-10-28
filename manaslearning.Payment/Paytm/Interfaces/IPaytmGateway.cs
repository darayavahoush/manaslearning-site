using manaslearning.Payment.Paytm.Models;
using System;

namespace manaslearning.Payment
{
    public interface IPaytmGateway
    {
        string InitiateTransactionAPI(string orderOid, string currency, string amount, string callbackUrl, long userId);

        Tuple<PaytmResponse.Root, bool> GetPaymentResponse(string orderId);

        void SetPaytmConfiguration(PaytmConfiguration paytmConfiguration);
    }
}