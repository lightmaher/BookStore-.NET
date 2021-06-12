using BookStore.DataAccess.Data;
using BookStore.Models;
using BookStore.Repository.IRepository;
using RepositoryGenral.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore.Repository
{
    public class ShoppingCartRepository : Repository<ShoppingCart> , IShoppingCartRepository
    {
        private readonly ApplicationDbContext db;

        public ShoppingCartRepository(ApplicationDbContext db) : base(db) 
        {
            this.db = db;
        }
       
        public void save()
        {
            db.SaveChanges();
        }

        public void Update(ShoppingCart shoppingCart)
        {
            db.Update(shoppingCart);


        }
    }
}
