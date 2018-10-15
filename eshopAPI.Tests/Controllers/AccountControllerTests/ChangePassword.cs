﻿using eshop_webAPI.Controllers;
using eshopAPI.DataAccess;
using eshopAPI.Models;
using eshopAPI.Requests.Account;
using eshopAPI.Services;
using eshopAPI.Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace eshopAPI.Tests.Controllers.AccountControllerTests
{
    public class ChangePassword
    {
        AccountController _controller;
        private readonly ITestOutputHelper _output;

        public ChangePassword(ITestOutputHelper output)
        {
            _output = output;
        }

        [Theory]
        [InlineData(false, false, false)]
        [InlineData(true, false, false)]
        [InlineData(true, true, false)]
        [InlineData(true, true, true)]
        public async void ChangePassword_Test(bool validUser, bool emailConfirmed, bool changeSuccess)
        {
            ShopUser user = null;
            if (validUser)
                user = new ShopUser { Id = "a", EmailConfirmed = emailConfirmed };
            // Setup
            Mock<IShopUserRepository> userRepoMock = new Mock<IShopUserRepository>();
            Mock<UserManager<ShopUser>> userManagerMock = MockUserManager(user, changeSuccess);
            Mock<SignInManager<ShopUser>> signInManagerMock = MockSignInManager(userManagerMock.Object);
            Mock<IEmailSender> emailSenderMock = new Mock<IEmailSender>();
            Mock<ILogger<AccountController>> loggerMock = new Mock<ILogger<AccountController>>();
            Mock<IConfiguration> configurationMock = new Mock<IConfiguration>();

            _controller = new AccountController(userRepoMock.Object,
                userManagerMock.Object,
                signInManagerMock.Object,
                emailSenderMock.Object,
                loggerMock.Object,
                configurationMock.Object);

            _controller.ControllerContext = new ControllerContext();
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();

            // Act
            ChangePasswordRequest request = new ChangePasswordRequest
            {
                CurrentPassword = "password123",
                NewPassword = "password1234"
            };
            var result = await _controller.ChangePassword(request);

            ObjectResult objectResult;
            // Assert
            if (!validUser)
            {
                objectResult = Assert.IsType<ObjectResult>(result);
                Assert.Equal((int)HttpStatusCode.NotFound, objectResult.StatusCode);
                var errorResponse = Assert.IsType<ErrorResponse>(objectResult.Value);
                Assert.Equal(ErrorReasons.NotFound, errorResponse.Reason);
                return;
            }
            if (!emailConfirmed)
            {
                objectResult = Assert.IsType<ObjectResult>(result);
                Assert.Equal((int)HttpStatusCode.BadRequest, objectResult.StatusCode);
                var errorResponse = Assert.IsType<ErrorResponse>(objectResult.Value);
                Assert.Equal(ErrorReasons.AccountIsNotConfirmed, errorResponse.Reason);
                return;
            }
            if (!changeSuccess)
            {
                objectResult = Assert.IsType<ObjectResult>(result);
                Assert.Equal((int)HttpStatusCode.BadRequest, objectResult.StatusCode);
                var errorResponse = Assert.IsType<ErrorResponse>(objectResult.Value);
                Assert.Equal(ErrorReasons.BadRequest, errorResponse.Reason);
                return;
            }
            var statusCodeResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal((int)HttpStatusCode.NoContent, statusCodeResult.StatusCode);
            signInManagerMock.Verify(o => o.SignOutAsync());
        }
        
        Mock<UserManager<ShopUser>> MockUserManager(ShopUser user, bool changeSuccess)
        {
            var mockUserStore = new Mock<IUserStore<ShopUser>>();
            var mockUserEmailStore = mockUserStore.As<IUserEmailStore<ShopUser>>();
            
            var userManagerMock = new Mock<UserManager<ShopUser>>(mockUserStore.Object, null, null, null, null, null, null, null, null);

            userManagerMock.Setup(o => o.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(user);

            userManagerMock.Setup(o => o.ChangePasswordAsync(user, It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(changeSuccess ? IdentityResult.Success : IdentityResult.Failed(new IdentityError { Description = "Failed" }));

            return userManagerMock;
        }

        Mock<SignInManager<ShopUser>> MockSignInManager<ShopUser>(UserManager<ShopUser> userManager) where ShopUser : class
        {
            var context = new Mock<HttpContext>();
            Mock<SignInManager<ShopUser>> signInManagerMock = new Mock<SignInManager<ShopUser>>(
                userManager,
                new HttpContextAccessor { HttpContext = context.Object },
                new Mock<IUserClaimsPrincipalFactory<ShopUser>>().Object,
                null, null, null)
            { CallBase = true };

            signInManagerMock.Setup(o => o.SignOutAsync())
                .Returns(Task.FromResult(0))
                .Verifiable();

            return signInManagerMock;
        }
    }
}