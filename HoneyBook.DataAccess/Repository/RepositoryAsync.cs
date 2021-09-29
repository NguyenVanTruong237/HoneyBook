﻿using HoneyBook.DataAccess.Data;
using HoneyBook.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HoneyBook.DataAccess.Repository
{
    public class RepositoryAsync<T> : IRepositoryAsync<T> where T: class
    {
        private readonly ApplicationDbContext _db;
        internal DbSet<T> dbSet;
        public RepositoryAsync(ApplicationDbContext db)
        {
            _db = db;
            this.dbSet = _db.Set<T>();
        }
        public async Task AddAsync(T entity)
        {
             await dbSet.AddAsync(entity);
        }

        public async Task<T> GetAsync(int id)
        {
            return await dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProPerties = null)
        {
            IQueryable<T> query = dbSet;
            if (filter != null)         // nếu 1 object không null thì trả data về query
            {
                query = query.Where(filter);    
            }
            if (includeProPerties != null)
            {
                foreach (var includeProp in includeProPerties.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries))  
                {
                    query = query.Include(includeProp); //include để thêm data từ class khác
                }
            }
            if (orderBy != null)
            {
                return await orderBy(query).ToListAsync();     //nếu list data trong oderby không null thì list đó về query
            }
            return await query.ToListAsync();
        }

        public async Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> filter = null, string includeProPerties = null)
        {
            IQueryable<T> query = dbSet;

            if (filter != null)         // nếu 1 object không null thì trả data về query
            {
                query = query.Where(filter);
            }
            if (includeProPerties != null)
            {
                foreach (var includeProp in includeProPerties.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries))   //cắt data từng thuộc tính của class về query
                {
                    query = query.Include(includeProp);
                }
            }
            return await query.FirstOrDefaultAsync();
        }

        public async Task RemoveAsync(int id)
        {
            T entity = await dbSet.FindAsync(id);
            await RemoveAsync(entity);
        }

        public async Task RemoveAsync(T entity)
        {
            dbSet.Remove(entity);
        }

        public async Task RemoveRangeAsync(IEnumerable<T> entity)
        {
            dbSet.RemoveRange(entity);
        }
    }
}
