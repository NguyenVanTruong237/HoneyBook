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
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
        private ApplicationDbContext _db;
        public OrderHeaderRepository(ApplicationDbContext db) : base (db) 
        {
            _db = db;
        }
        public void Update(OrderHeader orderHeader)   
        {
            _db.Update(orderHeader);
        }
    }
}
