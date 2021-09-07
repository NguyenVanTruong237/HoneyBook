using HoneyBook.DataAccess.Data;
using HoneyBook.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoneyBook.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Category = new CategoryRepository(db);
            CoverType = new CoverTypeRepository(db);
            SP_Call = new SP_Call(db);

        }

        public ICategoryRepository Category { get; private set; }

        public ISP_Call SP_Call { get; private set; }

        public ICoverTypeRepository CoverType { get; private set; }

        public void Dispose()
        {
            _db.Dispose();
        }
        public void save()
        {
            _db.SaveChanges();
        }
    }
}
