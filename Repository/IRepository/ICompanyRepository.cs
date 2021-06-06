using BookStore.Models;
using RepositoryGenral.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore.Repository.IRepository
{
   public interface ICompanyRepository : IRepository<Company>
    {
        void Update(Company company);
    }
}
