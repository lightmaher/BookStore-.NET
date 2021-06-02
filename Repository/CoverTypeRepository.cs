using BookStore.DataAccess.Data;
using BookStore.Models;
using BookStore.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore.Repository
{
    public class CoverTypeRepository : Repository<CoverType>, ICoverType
    {
        private readonly ApplicationDbContext dbContext;

        public CoverTypeRepository( ApplicationDbContext dbContext ) : base(dbContext)
        {
            this.dbContext = dbContext;
        }
        public void Update(CoverType coverType)
        {
            if ( coverType != null)
            {
                dbContext.coverTypes.Update(coverType);
            }
        }
    }
}
