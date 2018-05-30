﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using eshopAPI.DataAccess;
using eshopAPI.Models;
using eshopAPI.Models.ViewModels;
using eshopAPI.Models.ViewModels.Admin;
using eshopAPI.Requests;
using eshopAPI.Utils;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace eshopAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/admin/discount")]
    public class DiscountController : ODataController
    {
        private readonly IDiscountRepository _discountRepository;

        public DiscountController(IDiscountRepository DiscountRepository)
        {
            _discountRepository = DiscountRepository;
        }

        [HttpGet]
        [EnableQuery]
        public async Task<IQueryable<AdminDiscountVM>> Get()
        {
            return await _discountRepository.GetAdminDiscounts();
        }

        [Transaction]
        [HttpPost]
        public async Task<IActionResult> Post(DiscountRequest request)
        {
            if (request.ItemID.HasValue && (await _discountRepository.GetDiscountForItem(request.ItemID.Value)) != null)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, new ErrorResponse(ErrorReasons.BadRequest, "Item already has a discount."));
            }
            if (request.CategoryID.HasValue && (await _discountRepository.GetDiscountForCategory(request.CategoryID.Value)) != null)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, new ErrorResponse(ErrorReasons.BadRequest, "Category already has a discount."));
            }
            if (request.SubCategoryID.HasValue && (await _discountRepository.GetDiscountForCategory(request.SubCategoryID.Value)) != null)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, new ErrorResponse(ErrorReasons.BadRequest, "SubCategory already has a discount."));
            }
            if ((request.ItemID.HasValue && request.SubCategoryID.HasValue && request.CategoryID.HasValue) ||
                (request.ItemID.HasValue && request.CategoryID.HasValue) ||
                (request.ItemID.HasValue && request.SubCategoryID.HasValue) ||
                (request.SubCategoryID.HasValue && request.CategoryID.HasValue))
            {
                return StatusCode((int)HttpStatusCode.BadRequest, new ErrorResponse(ErrorReasons.BadRequest, "Only one: Category or SubCategory or Item can have discount."));
            }
            await _discountRepository.InsertDiscount(new Discount
            {
                Name = request.Name,
                Value = request.Value,
                IsPercentages = request.IsPercentages,
                To = request.To,
                ItemID = request.ItemID,
                CategoryID = request.CategoryID,
                SubCategoryID = request.SubCategoryID
            });
            return StatusCode((int)HttpStatusCode.NoContent);
        }

        [Transaction]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var discount = await _discountRepository.GetDiscountByID(id);
            if (discount == null)
            {
                return StatusCode((int)HttpStatusCode.NoContent);
            }
            await _discountRepository.RemoveDiscount(discount);
            return StatusCode((int)HttpStatusCode.NoContent);
        }
    }
}