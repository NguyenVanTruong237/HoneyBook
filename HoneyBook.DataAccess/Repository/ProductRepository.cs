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
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private ApplicationDbContext _db;
        public ProductRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void update(Product product)
        {
            var objInDb = _db.Products.FirstOrDefault(c => c.Id == product.Id);
            if (product.ImageUrl !=null)
            {
                objInDb.ImageUrl = product.ImageUrl;
            }
            objInDb.Title = product.Title;
            objInDb.Description = product.Description;
            objInDb.ISBN = product.ISBN;
            objInDb.Author = product.Author;
            objInDb.ListPrice = product.ListPrice;
            objInDb.Price = product.Price;
            objInDb.Price50 = product.Price50;
            objInDb.Price100 = product.Price100;
            objInDb.CategoryId = product.CategoryId;
            objInDb.CoverTypeId = product.CoverTypeId;
        }
    }
}
