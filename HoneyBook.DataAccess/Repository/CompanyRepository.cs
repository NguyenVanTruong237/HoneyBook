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
    class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        private ApplicationDbContext _db;
        public CompanyRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Update(Company company)
        {
            var objInDb = _db.Companies.FirstOrDefault(c =>c.Id == company.Id);
            objInDb.Name = company.Name;
            objInDb.StreetAddress = company.StreetAddress;
            objInDb.City = company.City;
            objInDb.State = company.State;
            objInDb.PostalCode = company.PostalCode;
            objInDb.PhoneNumber = company.PhoneNumber;
            objInDb.IsAuthorizedCompany = company.IsAuthorizedCompany;
        }
    }
}
