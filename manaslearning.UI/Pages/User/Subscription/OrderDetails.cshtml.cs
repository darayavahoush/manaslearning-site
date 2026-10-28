using manaslearning.Business.Interfaces;
using manaslearning.Models;
using manaslearning.UI.Helpers;
using manaslearning.UI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace manaslearning.UI.Pages.User.Subscription
{
    public class OrderDetailsModel : PageModel
    {
        public BreadcrumbViewModel BreadcrumbViewModel { get; set; }
        public OrderModel Order { get; set; }

        private readonly ILogger _logger;
        private readonly IOrderRepository _orderRepository;           

        private long _userId;

        public OrderDetailsModel(
            ILogger<OrderDetailsModel> logger,
            IOrderRepository orderRepository)
        {
            _logger = logger;
            _orderRepository = orderRepository;            
        }

        public async Task<IActionResult> OnGetAsync(Guid orderOid)
        {
            try
            {
                _userId = HttpContext.GetUserId();
                if (_userId == 0)
                    return LocalRedirect(UrlConstants.UNAUTHORIZED);

                var order = await _orderRepository.GetOrderInfoByOidAsync(_userId, orderOid);

                if (order ==  null)
                    return LocalRedirect(UrlConstants.USER_NOT_FOUND);

                SetBreadcrumb(order.OrderNumber);

                Order = new OrderModel()
                {
                    OrderNumber = order.OrderNumber,
                    PurchasedDate = order.CreatedDate.ToString(StringConstants.DATE_FORMAT),
                    PurchasedTime = order.CreatedDate.ToString("T"),
                    ProductName = order.AssessmentName,
                    ItemPrice = decimal.Round(order.ItemPrice, 2),
                    GSTRate = decimal.Round(order.GSTRate, 2),
                    Quantity = order.Quantity,
                    GSTAmount = decimal.Round(order.GSTAmount, 2),
                    Subtotal = decimal.Round(order.ItemPrice * order.Quantity, 2),
                    TotalAmount = decimal.Round(order.TotalAmount, 2),
                    BillingAddressLine1 = order.BillingAddressLine1,
                    BillingAddressLine2 = order.BillingAddressLine2,
                    BillingAddressLine3 = order.BillingAddressLine3,
                    BillingName = order.BillingName,
                    BillingCity = order.BillingCity,
                    BillingCountry = order.BillingCountry,
                    BillingState = order.BillingState,
                    BillingZipCode = order.BillingZipCode,
                    CurrencySymbol = string.IsNullOrWhiteSpace(order.CurrencySymbol) ? "₹" : order.CurrencySymbol
                };

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error={ex.Message}, Source=Subscription.OrderDetailsModel.OnGetAsync(), UserId={_userId}, OrderOid={orderOid}");
                return LocalRedirect(UrlConstants.USER_ERROR);
            }
        }

        private void SetBreadcrumb(string orderNumber)
        {
            BreadcrumbViewModel = new BreadcrumbViewModel
            {
                Items = new BreadcrumbItem[] {
                    new BreadcrumbItem{ Title = "Home", Href = UrlConstants.USER_DASHBOARD },
                    new BreadcrumbItem{ Title = "Orders & Payments", Href = UrlConstants.USER_ORDERS_AND_PAYMENTS },
                    new BreadcrumbItem{ Title = string.IsNullOrWhiteSpace(orderNumber) ? "Order: #" : orderNumber }
                },
                IsResponsive = true
            };
        }

        public class OrderModel
        {
            public string OrderNumber { get; set; }
            public string PurchasedDate { get; set; }
            public string PurchasedTime { get; set; }
            public string ProductName { get; set; }
            public string CurrencySymbol { get; set; }
            public int Quantity { get; set; }
            public decimal ItemPrice { get; set; }
            public decimal GSTRate { get; set; }
            public decimal GSTAmount { get; set; }
            public decimal Subtotal { get; set; }
            public decimal TotalAmount { get; set; }
            public string BillingName { get; set; }
            public string BillingAddressLine1 { get; set; }
            public string BillingAddressLine2 { get; set; }
            public string BillingAddressLine3 { get; set; }
            public string BillingCity { get; set; }
            public string BillingState { get; set; }
            public string BillingCountry { get; set; }
            public string BillingZipCode { get; set; }
        }
    }
}
