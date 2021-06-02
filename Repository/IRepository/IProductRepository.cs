﻿using BookStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore.Repository.IRepository
{
   public interface IProductRepository : IRepository<Product>
    {
        void Update(Product product);
    }
}
