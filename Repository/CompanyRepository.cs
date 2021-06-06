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
    public class CompanyRepository : Repository<Company> , ICompanyRepository
    {
        private readonly ApplicationDbContext db;

        public CompanyRepository(ApplicationDbContext db) : base(db) 
        {
            this.db = db;
        }
        public void Update(Company company)
        {
            db.Companies.Update(company);
        }
        public void save()
        {
            db.SaveChanges();
        }
    }
}
