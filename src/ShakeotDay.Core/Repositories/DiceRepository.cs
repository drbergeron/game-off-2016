using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using ShakeotDay.Core.Models;
using Dapper;

namespace ShakeotDay.Core.Repositories
{
    public class DiceRepository
    {
        private string _connstr;
        protected internal SqlConnection _conn;

        public DiceRepository(string connString)
        {
            _connstr = connString;
            _conn = new SqlConnection(_connstr);
        }

        public async Task<List<Dice>> GetNewDice(long userId, long gameId, int rollNumber, int numberIn = 5)
        {
            var diceOut = new List<Dice>();
            var saves = new List<Task<int>>();
            var RNG = new Random();

            for(int i = 0; i < numberIn; ++i)
            {
                var die = new Dice(RNG);
                die.Roll();
                diceOut.Add(die);

                var t = await SaveRoll(die, userId, gameId, rollNumber);
            }

            return diceOut;
        }

        public async Task<int> GetNextRollNumberForGame(long gameId)
        {
            var sql =
                @"
                    select COALESCE(max(GameRollNumber),0)
                    from DieRolls
                    where GameID = @GameId
                ";

            return await _conn.QueryFirstOrDefaultAsync<int>(sql, new { GameId = gameId });
        }

        public async void SaveHand(DiceHand handIn, long userIn, long gameIn, int rollNumber)
        {
            foreach(var die in handIn.Hand)
            {
                var savetask = await this.SaveRoll(die, userIn, gameIn, rollNumber);
            }
        }

        public async Task<int> SaveRoll(Dice dieIn, long userIn, long gameIn, int rollNumber)
        {
            var dr = new 
            {
                UserId = userIn,
                RollValue = dieIn.dieValue,
                GameId = gameIn,
                RollNum = rollNumber
            };

            var SQL = $@"
                insert into DieRolls(UserId, RollValue, GameId, GameRollNumber)
                Values(@UserId, @RollValue, @GameId, @RollNum)
                ";

            return await _conn.ExecuteAsync(SQL, dr);
        }

        public async Task<DiceHand> GetHandForGame(long gameId, int rollToGet)
        {
            if(rollToGet == 0)
            {
                Random rnd = new Random();
                var dicehand = new DiceHand();
                dicehand.Fill(rnd);
                dicehand.RollNumber = 0;
                return dicehand;
            }

            var sql = @"select RollValue [dieValue], 0 [holding] from DieRolls where GameId = @Gameid and GameRollNumber = @Roll";

            var diceEnum = await _conn.QueryAsync<Dice>(sql, new { Gameid = gameId, Roll = rollToGet });
            var dice = diceEnum.ToList();
            if(dice.Count != 5)
            {
                throw new Exception($"Wrong Number of dice returned from game {gameId} and roll {rollToGet}; expected 5, got {dice.Count}");
            }else
            {
                return new DiceHand(dice, rollToGet);
            }
        }
    }
}
