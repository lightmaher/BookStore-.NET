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
    public class CategoryRepository : Repository<Category> , ICategoryRepository
    {
        private readonly ApplicationDbContext db;

        public CategoryRepository(ApplicationDbContext db) : base(db) 
        {
            this.db = db;
        }
        public void Update(Category category)
        {
            db.categories.Update(category);
        }
        public void save()
        {
            db.SaveChanges();
        }
    }
}
