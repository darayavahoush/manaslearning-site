using manaslearning.Business.Interfaces;
using manaslearning.Models;
using manaslearning.UI.Helpers;
using manaslearning.UI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace manaslearning.UI.Pages.User.Subscription
{
    public class OrdersModel : PageModel
    {
        public IList<OrderModel> Orders { get; set; }
        public BreadcrumbViewModel BreadcrumbViewModel { get; set; }

        private readonly ILogger<OrdersModel> _logger;
        private readonly IOrderRepository _orderRepository;
        private long _userId;

        public OrdersModel(
            ILogger<OrdersModel> logger,
            IOrderRepository orderRepository)
        {
            _logger = logger;
            _orderRepository = orderRepository;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                _userId = HttpContext.GetUserId();
                if (_userId == 0)
                    return LocalRedirect(UrlConstants.UNAUTHORIZED);

                SetBreadcrumb();

                var orders = await _orderRepository.GetOrdersInfoAsync(_userId);
                if (orders != null)
                    SetOrdersListModel(orders);

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error={ex.Message}, Source=Subscription.OrdersModel.OnGetAsync(), UserId={_userId}");
                return LocalRedirect(UrlConstants.USER_ERROR);
            }
        }

        private void SetOrdersListModel(IEnumerable<OrderInfo> ordersInfo)
        {
            var orderedList = ordersInfo.OrderByDescending(x => x.CreatedDate);
            Orders = new List<OrderModel>();

            foreach (var order in orderedList)
            {               
                var orderDate = order.CreatedDate.ToTimeZone(StringConstants.INDIAN_TIMEZONE);
                var orderModel = new OrderModel()
                {
                    OrderOid = order.Oid,
                    PurchasedDate = orderDate.ToString(StringConstants.DATE_FORMAT),
                    PurchasedTime = orderDate.ToString("T"),
                    OrderNumber = order.OrderNumber,
                    ProductName = order.AssessmentName,
                    Quantity = order.Quantity,
                    OrderStatusName = order.OrderStatusName,
                    TotalAmount = decimal.Round(order.TotalAmount, 2),
                    CurrencySymbol = order.CurrencySymbol,
                    PaymentStatusName = order.PaymentStatusName,
                    PaymentTransactionOid = order.PaymentTransactionOid                    
                };
                Orders.Add(orderModel);
            }
        }

        private void SetBreadcrumb()
        {
            BreadcrumbViewModel = new BreadcrumbViewModel
            {
                Items = new BreadcrumbItem[] {
                    new BreadcrumbItem{ Title = "Home", Href = UrlConstants.USER_DASHBOARD },
                    new BreadcrumbItem{ Title = "Orders & Payments" }
                },
                IsResponsive = true
            };
        }
    }

    public class OrderModel
    {
        public Guid OrderOid { get; set; }
        public string PurchasedDate { get; set; }
        public string PurchasedTime { get; set; }
        public string OrderNumber { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal TotalAmount { get; set; }
        public string OrderStatusName { get; set; }
        public string PaymentStatusName { get; set; }
        public string CurrencySymbol { get; set; }
        public Guid PaymentTransactionOid { get; set; }
    }
}
