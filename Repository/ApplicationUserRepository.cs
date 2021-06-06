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
    public class ApplicationUserRepository : Repository<ApplicationUser> , IApplicationUserRepository
    {
        private readonly ApplicationDbContext db;

        public ApplicationUserRepository(ApplicationDbContext db) : base(db) 
        {
            this.db = db;
        }
        public void save()
        {
            db.SaveChanges();
        }
    }
}
