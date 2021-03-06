﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace eshopAPI.Models
{
    public class Attribute
    {
        [Key]
        public long ID { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } // Bussiness key (should be unique?)
    }
}
