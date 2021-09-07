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
    public class CoverTypeRepository : Repository<CoverType>, ICoverTypeRepository
    {
        private ApplicationDbContext _db;
        public CoverTypeRepository(ApplicationDbContext db) :base(db)
        {
            _db = db;
        }
        public void update(CoverType covertype)
        {
            var objInDb = _db.CoverTypes.FirstOrDefault(c => c.Id == covertype.Id);
            if (objInDb != null)
            {
                objInDb.Name = covertype.Name;
            }
        }
    }
}
