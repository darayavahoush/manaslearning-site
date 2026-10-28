using manaslearning.Business.Interfaces;
using manaslearning.Models;
using manaslearning.UI.Helpers;
using manaslearning.UI.Models;
using manaslearning.UI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace manaslearning.UI.Pages.User.Subscription
{
    [Authorize]
    public class CheckoutModel : PageModel
    {
        private long _userId;
        private int _organizationId;
        private readonly ILogger<CheckoutModel> _logger;
        
        private readonly IAddressRepository _addressRepository;
        private readonly IOrderRepository _orderRepository;       
        private readonly IItemRepository _itemRepository;
        
        public AddressDto Address { get; private set; }
        public ItemInfo Item { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }

        public bool IsAddressAvailable { get; private set; } = false;
        public string StatusMessage { get; private set; } = string.Empty;

        /// <summary>
        /// Billing Address Oid
        /// </summary>
        [BindProperty] public Guid Param1 { get; set; }

        /// <summary>
        /// Item Oid
        /// </summary>
        [BindProperty] public Guid Param2 { get; set; }

        /// <summary>
        /// Quantity
        /// </summary>
        [BindProperty] public int Param3 { get; set; }

        public CheckoutModel(
            ILogger<CheckoutModel> logger,            
            IAddressRepository addressRepository,
            IOrderRepository orderRepository,
            IItemRepository itemRepository            
            )
        {
            this._logger = logger;            
            this._addressRepository = addressRepository;
            this._orderRepository = orderRepository;
            this._itemRepository = itemRepository;            
        }

        public async Task<IActionResult> OnGetAsync(Guid itemOid, int quantity)
        {
            try
            {
                this.StatusMessage = string.Empty;

                _userId = HttpContext.GetUserId();
                _organizationId = HttpContext.GetOrganizationId();
                if (_userId == 0 || _organizationId == 0)
                    return LocalRedirect(UrlConstants.UNAUTHORIZED);

                if (itemOid.Equals(Guid.Empty))
                    return LocalRedirect(UrlConstants.USER_NOT_FOUND);

                if (quantity <= 0)
                    return LocalRedirect(UrlConstants.USER_NOT_FOUND);

                Param2 = itemOid;
                Param3 = quantity;

                var addresses = await _addressRepository.GetAddressesAsync(_organizationId);
                if (addresses == null || !addresses.Any(a => !a.IsDeleted))
                {
                    this.IsAddressAvailable = false;
                    Address = null;
                }
                else
                {
                    this.IsAddressAvailable = true;
                    var address = addresses.Where(a => !a.IsDeleted).FirstOrDefault();
                    Address = new AddressDto
                    {
                        Oid = address.Oid,
                        AddressTypeId = address.AddressTypeId,
                        AddressLine1 = address.AddressLine1,
                        AddressLine2 = address.AddressLine2,
                        AddressLine3 = address.AddressLine3,
                        City = address.City,
                        State = address.State,
                        Country = address.Country,
                        Name = address.Name,
                        CreatedDate = address.CreatedDate,
                        ModifiedDate = address.ModifiedDate,
                        IsDefault = address.IsDefault,
                        ZipCode = address.ZipCode
                    };
                }

                this.Item = await _itemRepository.GetItemInfoByOidAsync(itemOid);
                if (this.Item == null)
                    return LocalRedirect(UrlConstants.USER_NOT_FOUND);

                CalculateTotalAmount();

                return this.Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error={ex.Message}, Source=Subscription.CheckoutModel.OnGetAsync(), UserId={_userId}, ItemOid={itemOid}, Quantity={quantity}");
                return LocalRedirect(UrlConstants.USER_ERROR);
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            string errorSourceWithInputParams = $"Source=Subscription.CheckoutModel.OnPostAsync(), BillingAddressOid={Param1}, ItemOid={Param2}, Quantity={Param3}";

            this.StatusMessage = string.Empty;
            try
            {
                _userId = HttpContext.GetUserId();
                _organizationId = HttpContext.GetOrganizationId();
                if (_userId == 0 || _organizationId == 0)
                    return LocalRedirect(UrlConstants.UNAUTHORIZED);

                errorSourceWithInputParams = $"Source=Subscription.CheckoutModel.OnPostAsync(), UserId={_userId}, BillingAddressOid={Param1}, ItemOid={Param2}, Quantity={Param3}";

                if (!ModelState.IsValid || ModelState.Count != 3)
                {
                    _logger.LogError($"Error=CheckoutModel is invalid. {errorSourceWithInputParams}");
                    return LocalRedirect(UrlConstants.USER_ERROR);
                }

                var validationResult = await IsModelValidAsync(_organizationId);
                if (!validationResult.Item1)
                {
                    this.StatusMessage = "Bad Request. Please try again.";
                    _logger.LogError($"Error=CheckoutModel is invalid. {validationResult.Item2}, {errorSourceWithInputParams}");
                    await ReloadPageDataAsync();
                    return this.Page();
                }

                string orderNumber = $"{DateTime.UtcNow.ToString("yyyyMMdd")}ML{GenerateRandom(4)}{GenerateRandom(4)}";

                var orderResult = await _orderRepository.CreateOrderAsync(
                        _userId,
                        _organizationId,
                        Param1,
                        Param2,
                        1, // HACK: for now INR is the currency
                        Param3,
                        orderNumber
                    );

                if (orderResult == -1) // -1: ERROR
                {
                    this.StatusMessage = "Error occured. Please try again.";
                    await ReloadPageDataAsync();
                    return this.Page();
                }
                else if (orderResult == -2) // -2: Invalid Billing Address
                {
                    this.StatusMessage = "Error occured. Please check the address and try again.";
                    _logger.LogError($"Error=BillingAddressOid does not exist. {errorSourceWithInputParams}");
                    await ReloadPageDataAsync();
                    return this.Page();
                }
                else if (orderResult == -3) // -3: Invalid Item
                {
                    this.StatusMessage = "Error occured. Please refresh the page and try again.";
                    _logger.LogError($"Error=ItemOid does not exist. {errorSourceWithInputParams}");
                    await ReloadPageDataAsync();
                    return this.Page();
                }
                else if (orderResult == -4) // -4: Invalid Currency
                {
                    this.StatusMessage = "Error occured. Please refresh the page and try again.";
                    _logger.LogError($"Error=Currency does not exist. {errorSourceWithInputParams}");
                    await ReloadPageDataAsync();
                    return this.Page();
                }

                var orderInfo = await _orderRepository.GetOrderInfoAsync(_userId, orderResult);
                if (orderInfo == null)
                {
                    _logger.LogError($"Error=OrderInfo can not be null. {errorSourceWithInputParams}");
                    return LocalRedirect(UrlConstants.USER_ERROR);
                }

                this.Item = await _itemRepository.GetItemInfoByOidAsync(Param2);
                if (this.Item == null)
                    return LocalRedirect(UrlConstants.USER_NOT_FOUND);

                // TODO: Move url to constants
                // string mid, string orderId, string paymentUrl, string txnToken
                return LocalRedirect($"/u/showpaymentpage?orderOid={orderInfo.Oid}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error={ex.Message}, {errorSourceWithInputParams}");
                return LocalRedirect(UrlConstants.USER_ERROR);
            }
        }
        /// <summary>
        /// Calculate Total Amount
        /// </summary>
        private void CalculateTotalAmount()
        {
            if (this.Item != null)
            {
                this.SubTotal = this.Item.ItemPrice * Param3;
                this.Tax = SubTotal * (this.Item.GSTRate / 100);
                this.Total = Math.Round(SubTotal + Tax, 2) + 0.00m;
            }
            else
            {
                this.SubTotal = this.Tax = this.Total = 0.00m;
            }
        }

        private async Task<Tuple<bool, string>> IsModelValidAsync(int organizationId)
        {
            // Address
            if (Param1 == null || Param1.Equals(Guid.Empty))
                return new Tuple<bool, string>(false, $"Billing Address Oid: {Param1}");

            var billingAddress = await _addressRepository.GetAddressByOidAsync(organizationId, Param1);
            if (billingAddress == null)
                return new Tuple<bool, string>(false, $"Billing Address Oid: {Param1} does not exist");

            // Item
            if (Param2 == null || Param2.Equals(Guid.Empty))
                return new Tuple<bool, string>(false, $"ItemOid: {Param2}");

            var item = await _itemRepository.GetItemByOidAsync(Param2);
            if (item == null)
                return new Tuple<bool, string>(false, $"ItemOid: {Param2} does not exist");

            // Quantity
            if (Param3 <= 0)
                return new Tuple<bool, string>(false, $"Quantity: {Param3}");

            // TODO: Add currency validations.

            return new Tuple<bool, string>(true, string.Empty);
        }        

        /// <summary>
        /// Reload page data incase of data validation errors
        /// </summary>
        private async Task ReloadPageDataAsync()
        {
            // Item
            this.Item = await _itemRepository.GetItemInfoByOidAsync(Param2);

            // Address
            if (Param1 == null || Param1.Equals(Guid.Empty))
            {
                this.IsAddressAvailable = true;
                this.Address = null;
                return;
            }

            var address = await _addressRepository.GetAddressByOidAsync(_organizationId, Param1);
            if (address == null)
            {
                this.IsAddressAvailable = true;
                this.Address = null;
                return;
            }

            this.IsAddressAvailable = true;
            this.Address = new AddressDto
            {
                Oid = address.Oid,
                AddressTypeId = address.AddressTypeId,
                AddressLine1 = address.AddressLine1,
                AddressLine2 = address.AddressLine2,
                AddressLine3 = address.AddressLine3,
                City = address.City,
                State = address.State,
                Country = address.Country,
                Name = address.Name,
                CreatedDate = address.CreatedDate,
                ModifiedDate = address.ModifiedDate,
                IsDefault = address.IsDefault,
                ZipCode = address.ZipCode
            };
        }        

        private string GenerateRandom(int length)
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
    }
}
