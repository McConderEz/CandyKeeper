﻿using CandyKeeper.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandyKeeper.DAL.Entities
{
    public class ProductTypeEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public ICollection<ProductEntity> Products { get; set; } = [];
    }
}
