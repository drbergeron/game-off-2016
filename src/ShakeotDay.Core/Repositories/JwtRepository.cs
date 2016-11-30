using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;

namespace ShakeotDay.Core.Repositories
{
    public class JwtRepository
    {

        private string _connstr;
        protected internal SqlConnection _conn;

        public JwtRepository(string connString)
        {
            _connstr = connString;
            _conn = new SqlConnection(_connstr);
        }
    }
}
