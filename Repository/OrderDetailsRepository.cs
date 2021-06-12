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
    public class OrderDetailsRepository : Repository<OrderDetails> , IOrderDetailsRepository
    {
        private readonly ApplicationDbContext db;

        public OrderDetailsRepository(ApplicationDbContext db) : base(db) 
        {
            this.db = db;
        }
        public void save()
        {
            db.SaveChanges();
        }

        public void Update(OrderDetails orderDetails)
        {
            db.OrderDetails.Update(orderDetails);
        }
    }
}
