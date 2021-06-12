﻿using BookStore.DataAccess.Data;
using BookStore.Models;
using BookStore.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore.Repository
{

    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Category = new CategoryRepository(_db);
            SP_Call = new SP_Call(_db);
            coverType = new CoverTypeRepository(_db);
            product = new ProductRepository(_db);
            company = new CompanyRepository(_db);
            ApplicationUser = new ApplicationUserRepository(_db);
            shoppingCart = new ShoppingCartRepository(_db);
            orderHeader = new OrderHeaderRepository(_db);
            orderDetails = new OrderDetailsRepository(_db);
        }
        public ISP_Call SP_Call { get; private set; }
        public IApplicationUserRepository ApplicationUser { get; private set; }

        public ICoverType coverType { get; private set; }
        public ICompanyRepository company { get; private set; }
        public ICategoryRepository Category { get; private set; }
        public IShoppingCartRepository shoppingCart { get; private set; }
        public IOrderHeaderRepository orderHeader { get; private set; }
        public IOrderDetailsRepository orderDetails { get; private set; }
        public IProductRepository product { get; private set; }

      
        public void Dispose()
        {
            _db.Dispose();
        }

        public void save()
        {
            _db.SaveChanges();
        }
    }
}
