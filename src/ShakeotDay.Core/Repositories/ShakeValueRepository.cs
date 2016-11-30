using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using ShakeotDay.Core.Models;
using Dapper;

namespace ShakeotDay.Core.Repositories
{
    public class ShakeValueRepository
    {
        private string _connstr;
        protected internal SqlConnection _conn;

        public ShakeValueRepository(string connString)
        {
            _connstr = connString;
            _conn = new SqlConnection(_connstr);
        }
        
        public async Task<int> GetShakeOfDay(int Year, int Day, bool reprocess = false)
        {
            var sql = "select ShakeOfTheDay from ShakeValues where Year=@Year and Day=@Day";

            var val = await _conn.QueryFirstOrDefaultAsync<int?>(sql, new { Year = Year, Day = Day });

            if (val == null && !reprocess)
            {
                this.GenerateShakeValues();
                return await GetShakeOfDay(Year, Day, true);
            }
            else
                return val ?? -1;

        }

        public void GenerateShakeValues(int daysAhead = 7)
        {
            var dayIterator = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            var rnd = new Random();

            var sql = "insert into ShakeValues(Year, Day, ShakeOfTheDay) Values (@Year, @Day, @Value)";
            var getsql = "select ShakeOfTheDay from ShakeValues where Year=@Year and Day=@Day";

            for (int i = 0; i < daysAhead; ++i)
            {
                var val = _conn.QueryFirstOrDefault<int?>(getsql, new { Year = dayIterator.Year, Day = dayIterator.DayOfYear });
                if(val == null)
                    _conn.Execute(sql, new { Year = dayIterator.Year, Day = dayIterator.DayOfYear, Value = rnd.Next(1, 7) });

                dayIterator = dayIterator.AddDays(1);
            }
        }
    }
}
