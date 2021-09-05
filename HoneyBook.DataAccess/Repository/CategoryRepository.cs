using HoneyBook.DataAccess.Data;
using HoneyBook.DataAccess.Repository.IRepository;
using HoneyBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoneyBook.DataAccess.Repository 
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private ApplicationDbContext _db;
        public CategoryRepository(ApplicationDbContext db) : base (db) //Base db truyền vào ở lớp cha để dùng các chức năng của lớp cha
        {
            _db = db;
        }
        public void Update(Category category)   
        {
            var objfromDb = _db.Categories.FirstOrDefault(s => s.Id == category.Id);
            if (objfromDb != null)
            {
                objfromDb.Name = category.Name;
                _db.SaveChanges();
            }
        }
    }
}
