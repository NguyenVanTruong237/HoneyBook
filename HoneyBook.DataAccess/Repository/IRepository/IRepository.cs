using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HoneyBook.DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T :class //Ràng buộc T là class
    {
        T Get(int id);  //Get id trong class T
        IEnumerable<T> GetAll(          //Get list object T
            Expression<Func<T, bool>> filter = null,        //định nghĩa filter(data của class T) = null  sẽ là true 
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,    //Định nghĩa orderby(1 list object class T) = null sẽ là true
            string includeProPerties = null         
            );
        T GetFirstOrDefault(
            Expression<Func<T, bool>> filter = null,        
            string includeProPerties = null
            );
        void Add(T entity);
        void Remove(int id);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entity);
    }
}
