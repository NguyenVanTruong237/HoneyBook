using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HoneyBook.DataAccess.Repository.IRepository
{
    public interface IRepositoryAsync<T> where T :class //Ràng buộc T là class
    {
        Task<T> GetAsync(int id);  //Get id trong class T
        Task<IEnumerable<T>> GetAllAsync(          //Get list object T
            Expression<Func<T, bool>> filter = null,        //định nghĩa filter(data của class T) = null  sẽ là true 
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,    //Định nghĩa orderby(1 list object class T) = null sẽ là true
            string includeProPerties = null         
            );
        Task<T> GetFirstOrDefaultAsync(
            Expression<Func<T, bool>> filter = null,        
            string includeProPerties = null
            );
        Task AddAsync(T entity);
        Task RemoveAsync(int id);
        Task RemoveAsync(T entity);
        Task RemoveRangeAsync(IEnumerable<T> entity);
    }
}
