using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore.Repository.IRepository
{
   public interface IUnitOfWork : IDisposable
    {
        ICategoryRepository Category { get; }
        ICoverType coverType { get; }
        IProductRepository product { get; }
       ICompanyRepository company { get;}
        IApplicationUserRepository ApplicationUser { get; }

        ISP_Call SP_Call { get; }
        void save();

    }
}
