using manaslearning.Business.Interfaces;
using manaslearning.UI.Controllers.User;
using manaslearning.UI.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace manaslearning.UI.UnitTests.User
{
    public class AddressControllerTest : BaseControllerTest
    {
        private readonly Mock<ILogger<AddressController>> _logger;
        private readonly Mock<IAddressRepository> _addressRepository;
        private readonly Mock<IAddressTypeRepository> _addressTypeRepository;
        private readonly Mock<IProfileRepository> _profileRepository;

        private readonly AddressController _controller;

        public AddressControllerTest()
        {
            _logger = new Mock<ILogger<AddressController>>();
            _addressRepository = new Mock<IAddressRepository>();
            _addressTypeRepository = new Mock<IAddressTypeRepository>();           

            _controller = new AddressController(
                addressRepository: _addressRepository.Object,
                addressTypeRepository: _addressTypeRepository.Object,                
                logger: _logger.Object);
        }

        #region PostAsync

        [Fact]
        public async Task PostAsyncUserIdUnauthorizedTest()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            var controllerContext = new ControllerContext { HttpContext = httpContext };
            _controller.ControllerContext = controllerContext;

            AddressRequestModel model = null;

            // Act
            var response = await _controller.PostAsync(model);

            // Assert
            Assert.NotNull(response);
            var result = response as UnauthorizedResult;
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.Unauthorized, result.StatusCode);
        }

        [Fact]
        public async Task PostAsyncModelNullBadRequestTest()
        {
            // Arrange
            this.SetupIdentity(_controller);

            AddressRequestModel model = null;

            // Act
            var response = await _controller.PostAsync(model);

            // Assert
            Assert.NotNull(response);
            var result = response as BadRequestResult;
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task PostAsyncNameNullBadRequestTest()
        {
            // Arrange
            this.SetupIdentity(_controller);

            var model = new AddressRequestModel();

            // Act
            var response = await _controller.PostAsync(model);

            // Assert
            Assert.NotNull(response);
            var result = response as BadRequestResult;
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task PostAsyncNameLengthBadRequestTest()
        {
            // Arrange
            this.SetupIdentity(_controller);

            var model = new AddressRequestModel
            {
                Name = @"ABCDEFGHIJ"
                        + "ABCDEFGHIJ"
                        + "ABCDEFGHIJ"
                        + "ABCDEFGHIJ"
                        + "ABCDEFGHIJ"
                        + "ABCDEFGHIJ"
                        + "ABCDEFGHIJ"
                        + "ABCDEFGHIJ"
                        + "ABCDEFGHIJ"
                        + "ABCDEFGHIJ"
                        + "Z"
            };

            // Act
            var response = await _controller.PostAsync(model);

            // Assert
            Assert.NotNull(response);
            var result = response as BadRequestResult;
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task PostAsyncNameSpecialCharactersBadRequestTest()
        {
            // Arrange
            this.SetupIdentity(_controller);

            var model = new AddressRequestModel
            {
                Name = "@Vikram"
            };

            // Act
            var response = await _controller.PostAsync(model);

            // Assert
            Assert.NotNull(response);
            var result = response as BadRequestResult;
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task PostAsyncAddressLine1NullBadRequestTest()
        {
            // Arrange
            this.SetupIdentity(_controller);

            var model = new AddressRequestModel
            {
                Name = "Vikram"
            };

            // Act
            var response = await _controller.PostAsync(model);

            // Assert
            Assert.NotNull(response);
            var result = response as BadRequestResult;
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
        }

        //[Fact]
        //public async Task PostAsyncNewAddressOkObjectResultTest()
        //{
        //    // Arrange
        //    this.SetupIdentity(_controller);

        //    var model = new AddressRequestModel
        //    {
        //        Name = "A",
        //        AddressLine1 = "A",
        //        City = "A",
        //        State = "A",
        //        Country = "A",
        //        ZipCode = "111111"
        //    };
        //    _addressRepository.Setup(x => x.CreateAddressAsync(1, model.AddressTypeId,
        //                model.Name,
        //                model.AddressLine1,
        //                model.AddressLine2,
        //                model.AddressLine3,
        //                model.City,
        //                model.State,
        //                model.Country,
        //                model.ZipCode,
        //                model.IsDefault)).ReturnsAsync(1);

        //    // Act
        //    var response = await _controller.PostAsync(model);

        //    // Assert
        //    Assert.NotNull(response);
        //    var result = response as OkObjectResult;
        //    Assert.NotNull(result);
        //}

        #endregion PostAsync
    }
}
