﻿using eshopAPI.Models.ViewModels.Admin;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eshopAPI.Requests
{
    public class ItemCreateRequest
    {
        public int CategoryID { get; set; }
        public long? SubcategoryID { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string SKU { get; set; }
        public List<string> PictureURLs { get; set; }
        public List<IFormFile> PictureFiles { get; set; }
        public List<AdminAttributeVM> Attributes { get; set; }
    }
}
