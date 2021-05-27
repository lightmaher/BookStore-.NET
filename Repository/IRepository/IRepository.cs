using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BookStore.Repository.IRepository
{
    interface IRepository<T> where T : class
    {
        T GetT(int id);
        IEnumerable<T> GetAll(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>,IOrderedQueryable<T>> OrderBy = null,
            string includeproprites = null
            );
        T GetFirstOrDefault(
            Expression<Func<T, bool>> filter = null,
            string includeproprites = null
            );

        void add(T entity);
        void remove(int id);
        void remove(T entity);
        void removeRange(IEnumerable<T> entitis);
    }
}
