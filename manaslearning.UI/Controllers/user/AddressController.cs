using manaslearning.Business.Interfaces;
using manaslearning.UI.Helpers;
using manaslearning.UI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace manaslearning.UI.Controllers.User
{
    [Route("api/u/[controller]")]
    [ApiController]
    [Authorize]
    public class AddressController : ControllerBase
    {
        private readonly IAddressRepository _addressRepository;
        private readonly IAddressTypeRepository _addressTypeRepository;        
        private readonly ILogger _logger;

        public AddressController(
            IAddressRepository addressRepository,
            IAddressTypeRepository addressTypeRepository,            
            ILogger<AddressController> logger)
        {
            this._addressRepository = addressRepository;
            this._addressTypeRepository = addressTypeRepository;            
            this._logger = logger;
        }

        // GET api/address
        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            long _userId = 0;
            try
            {
                _userId = HttpContext.GetUserId();
                int organizationId = HttpContext.GetOrganizationId();
                var addresses = await _addressRepository.GetAddressesAsync(organizationId);
                return Ok(addresses.Where(a => !a.IsDeleted).Select(a => new AddressDto
                {
                    Oid = a.Oid,
                    AddressTypeId = a.AddressTypeId,
                    AddressLine1 = a.AddressLine1,
                    AddressLine2 = a.AddressLine2,
                    AddressLine3 = a.AddressLine3,
                    City = a.City,
                    State = a.State,
                    Country = a.Country,
                    Name = a.Name,
                    CreatedDate = a.CreatedDate,
                    ModifiedDate = a.ModifiedDate,
                    IsDefault = a.IsDefault,
                    ZipCode = a.ZipCode
                }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}. Source=AddressController.GetAsync();UserId={_userId}");
                return this.StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        // GET api/address?Oid={oid}
        [HttpGet("{oid:Guid}")]
        public async Task<IActionResult> GetAsync(Guid oid)
        {
            long _userId = 0;
            try
            {
                _userId = HttpContext.GetUserId();
                int organizationId = HttpContext.GetOrganizationId();
                if (_userId == 0 || organizationId == 0)
                    return this.Unauthorized();                

                var address = await _addressRepository.GetAddressByOidAsync(organizationId, oid);
                if (address == null)
                    return this.NotFound();

                return Ok(new AddressDto
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
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}. Source=AddressController.GetAsync();UserId={_userId}, Oid={oid}");
                return this.StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        // POST api/address        
        [HttpPost]
        public async Task<IActionResult> PostAsync([FromForm] AddressRequestModel model)
        {
            long _userId = 0;
            try
            {
                _userId = HttpContext.GetUserId();
                int organizationId = HttpContext.GetOrganizationId();
                if (_userId == 0 || organizationId == 0)
                    return this.Unauthorized();

                if (model == null)
                    return this.BadRequest();

                // HACK:
                model.AddressTypeId = 1; // For now we have one address type

                var validationResult = await IsAddressModelValidAsync(model, false, organizationId);
                if (!validationResult.Item1)
                {
                    _logger.LogError(validationResult.Item2);
                    return this.BadRequest();
                }               

                var result = await _addressRepository.CreateAddressAsync(
                        _userId,
                        organizationId,
                        model.AddressTypeId,
                        model.Name,
                        model.AddressLine1,
                        model.AddressLine2,
                        model.AddressLine3,
                        model.City,
                        model.State,
                        model.Country,
                        model.ZipCode,
                        model.IsDefault
                    );

                if (result == -1) // -1: Error
                    return this.StatusCode((int)HttpStatusCode.InternalServerError);
                if (result == -2) // -2: Address already exists
                    return this.BadRequest();

                var address = await _addressRepository.GetAddressAsync(organizationId, result);

                return Ok(new AddressDto
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
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}. Source=AddressController.PostAsync();UserId={_userId}");
                return this.StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        // PUT api/address       
        [HttpPut]
        public async Task<IActionResult> PutAsync([FromForm] AddressRequestModel model)
        {
            long _userId = 0;
            try
            {
                _userId = HttpContext.GetUserId();
                int organizationId = HttpContext.GetOrganizationId();

                if (_userId == 0 || organizationId == 0)
                    return this.Unauthorized();

                // HACK:
                model.AddressTypeId = 1; // For now we have one address type
               
                var validationResult = await IsAddressModelValidAsync(model, true, organizationId);
                if (!validationResult.Item1)
                {
                    _logger.LogError($"Invalid Model. {validationResult.Item2}. Source=User.AddressController.PutAsync(), UserId={_userId}");
                    return this.BadRequest();
                }              

                var result = await _addressRepository.UpdateAddressAsync(
                        model.Oid.Value,
                        _userId,
                        organizationId,
                        model.AddressTypeId,
                        model.Name,
                        model.AddressLine1,
                        model.AddressLine2,
                        model.AddressLine3,
                        model.City,
                        model.State,
                        model.Country,
                        model.ZipCode,
                        model.IsDefault
                    );

                if (result == -1)
                    return this.StatusCode((int)HttpStatusCode.InternalServerError);
                else if (result == -2)
                    return this.BadRequest();

                var address = await _addressRepository.GetAddressByOidAsync(organizationId, model.Oid.Value);
                if (address == null)
                    return this.NotFound();

                return Ok(new AddressDto
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
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}. Source=AddressController.PutAsync();Oid={model.Oid.Value},UserId={_userId}");
                return this.StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        // DELETE api/address?Oid={oid}        
        [HttpDelete]
        public async Task<IActionResult> DeleteAsync(Guid oid)
        {
            long _userId = 0;
            try
            {
                _userId = HttpContext.GetUserId();
                int organizationId = HttpContext.GetOrganizationId();

                if (_userId == 0 || organizationId == 0)
                    return this.Unauthorized();

                // TODO: validate address
                var result = await _addressRepository.DeleteAddressAsync(organizationId, oid);

                if (result == -1)
                    return this.StatusCode((int)HttpStatusCode.InternalServerError);
                else if (result == -2)
                    return this.BadRequest();

                return Ok("success");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}. Source=AddressController.DeleteAsync();UserId={_userId}, Oid={oid}");
                return this.StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        /// Determines AddressRequestModel model is valid.
        /// </summary>
        /// <param name="model"></param>
        /// <returns>bool</returns>
        private async Task<Tuple<bool, string>> IsAddressModelValidAsync(AddressRequestModel model, bool isPutRequest, int organizationId)
        {
            string nameRegex = @"^[\w\s]";
            string addressLineRegex = @"^[\w\s-.,/]";
            string addressCityRegex = @"^[\w\s]";
            string addressStateRegex = @"^[a-z A-Z]";
            string addressCountryRegex = @"^[a-z A-Z]";

            if (isPutRequest)
            {
                // Address Oid
                if (model.Oid == null || model.Oid.Equals(Guid.Empty))
                    return new Tuple<bool, string>(false, $"AddressOid Null or Empty");
            }

            // Name
            if (string.IsNullOrWhiteSpace(model.Name) || model.Name.Length > 100 || !Regex.IsMatch(model.Name, nameRegex, RegexOptions.CultureInvariant, TimeSpan.FromSeconds(2)))
                return new Tuple<bool, string>(false, $"Name :{model.Name}");

            // Address Line1
            if (string.IsNullOrWhiteSpace(model.AddressLine1) || model.AddressLine1.Length > 200 || !Regex.IsMatch(model.AddressLine1, addressLineRegex, RegexOptions.CultureInvariant, TimeSpan.FromSeconds(2)))
                return new Tuple<bool, string>(false, $"AddressLine1: {model.AddressLine1}");

            // Address Line2
            if (!string.IsNullOrWhiteSpace(model.AddressLine2) && (model.AddressLine2.Length > 200 || !Regex.IsMatch(model.AddressLine2, addressLineRegex, RegexOptions.CultureInvariant, TimeSpan.FromSeconds(2))))
                return new Tuple<bool, string>(false, $"AddressLine2: {model.AddressLine2}");

            // Address Line3
            if (!string.IsNullOrWhiteSpace(model.AddressLine3) && (model.AddressLine3.Length > 200 || !Regex.IsMatch(model.AddressLine3, addressLineRegex, RegexOptions.CultureInvariant, TimeSpan.FromSeconds(2))))
                return new Tuple<bool, string>(false, $"AddressLine3: {model.AddressLine3}");

            // City
            if (string.IsNullOrWhiteSpace(model.City) || model.City.Length > 100 || !Regex.IsMatch(model.City, addressCityRegex, RegexOptions.CultureInvariant, TimeSpan.FromSeconds(2)))
                return new Tuple<bool, string>(false, $"City: {model.City}");

            // State
            if (string.IsNullOrWhiteSpace(model.State) || model.State.Length > 200 || !Regex.IsMatch(model.State, addressStateRegex, RegexOptions.CultureInvariant, TimeSpan.FromSeconds(2)))
                return new Tuple<bool, string>(false, $"State: {model.State}");

            // Country
            if (string.IsNullOrWhiteSpace(model.Country) || model.Country.Length > 200 || !Regex.IsMatch(model.Country, addressCountryRegex, RegexOptions.CultureInvariant, TimeSpan.FromSeconds(2)))
                return new Tuple<bool, string>(false, $"Country: {model.Country}");

            // ZipCode
            if (string.IsNullOrWhiteSpace(model.ZipCode) || model.ZipCode.Length < 6 || model.ZipCode.Length > 10 || !long.TryParse(model.ZipCode, out long _zip))
                return new Tuple<bool, string>(false, $"ZipCode: {model.ZipCode}");

            // Address Type
            if (model.AddressTypeId <= 0)
                return new Tuple<bool, string>(false, $"AddressTypeId: {model.AddressTypeId}");

            var addressTypes = await _addressTypeRepository.GetAddressTypesAsync();
            if (addressTypes == null | !addressTypes.Any(at => at.AddressTypeId == model.AddressTypeId))
                return new Tuple<bool, string>(false, $"AddressTypeId: {model.AddressTypeId}");

            if (isPutRequest)
            {
                var addresses = await _addressRepository.GetAddressesAsync(organizationId);
                if (addresses != null && addresses.Count() > 0 && !addresses.Any(a => a.Oid == model.Oid))
                    return new Tuple<bool, string>(false, $"AddressOid: {model.Oid}");
            }

            return new Tuple<bool, string>(true, string.Empty);
        }
    }
}
