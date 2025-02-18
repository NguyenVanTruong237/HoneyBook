﻿using Dapper;
using HoneyBook.DataAccess.Data;
using HoneyBook.DataAccess.Repository.IRepository;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoneyBook.DataAccess.Repository
{
    public class SP_Call : ISP_Call
    {
        private readonly ApplicationDbContext _db;
        private static string ConnectionString = "";

        public SP_Call(ApplicationDbContext db)
        {

            _db = db;
            ConnectionString = db.Database.GetDbConnection().ConnectionString;
        }
        public void Dispose() 
        {
            _db.Dispose(); //giải phóng các tài nguyên chiếm giữ - khi đối tượng bị hủy
        }

        /* Guide StoredProcedure:
         * (SQL Stored Procedures guide) h ttps://www.youtube.com/watch?v=Sggdhot-MoM 
         * h ttps://comdy.vn/sql-server/stored-procedure-trong-sql-server/
         */

        public void Execute(string procedureName, DynamicParameters param = null)       //get data theo param, dùng trong update
        {
            using (SqlConnection sqlCon = new SqlConnection(ConnectionString))
            {
                sqlCon.Open();
                sqlCon.Execute(procedureName, param, commandType: System.Data.CommandType.StoredProcedure);  // Exec nameproc theo para truyền vào dùng type StoredProcedure
            }
        }

        public IEnumerable<T> List<T>(string procedureName, DynamicParameters param = null)     // dùng để truy suất tất cả data của table
        {
            using (SqlConnection sqlCon = new SqlConnection(ConnectionString))
            {
                sqlCon.Open();
                return sqlCon.Query<T>(procedureName, param, commandType: System.Data.CommandType.StoredProcedure);
            }
        }

        public Tuple<IEnumerable<T1>, IEnumerable<T2>> list<T1, T2>(string procedureName, DynamicParameters param = null)       
        {
            using (SqlConnection sqlCon = new SqlConnection(ConnectionString))
            {
                sqlCon.Open();
                var result = SqlMapper.QueryMultiple(sqlCon, procedureName, param, commandType: System.Data.CommandType.StoredProcedure);  //Truy suất 2 table cùng lúc
                var item1 = result.Read<T1>().ToList();
                var item2 = result.Read<T2>().ToList();
                if (item1 != null && item2 !=null)
                {
                    return new Tuple<IEnumerable<T1>, IEnumerable<T2>>(item1, item2);
                }
                return new Tuple<IEnumerable<T1>, IEnumerable<T2>>(new List<T1>(), new List<T2>());
            }
        }

        public T OneRecord<T>(string procedureName, DynamicParameters param = null)     //Get 1 hàng của table trong db
        {
            using (SqlConnection sqlCon = new SqlConnection(ConnectionString))
            {
                sqlCon.Open();
                var value = sqlCon.Query<T>(procedureName, param, commandType: System.Data.CommandType.StoredProcedure);
                return (T)Convert.ChangeType(value.FirstOrDefault(), typeof(T));    // value.FirstOrDefault(): trả về data về value theo id truyền vào para, nếu k có thì trả value về null, và ép kiểu về class
            }
        }

        public T Single<T>(string procedureName, DynamicParameters param = null)    //trả về giá trị của cột đầu tiên và dòng đầu tiên   
        {
            using (SqlConnection sqlCon = new SqlConnection(ConnectionString))
            {
                sqlCon.Open();
                return (T)Convert.ChangeType(sqlCon.ExecuteScalar<T>(procedureName, param, commandType: System.Data.CommandType.StoredProcedure), typeof(T));   
            }
        }
    }
}

