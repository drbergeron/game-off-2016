using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using ShakeotDay.Core.Models;
using ShakeotDay.Core.Repositories;
using MoreLinq;

namespace ShakeotDay.Core.Repositories
{
    public class GameEngineRepository
    {

        private SqlConnection _conn;
        private GameRepository _gameRepo;
        private DiceRepository _diceRepo;
        private ShakeValueRepository _shake;

        public GameEngineRepository(string connStr)
        {
            _conn = new SqlConnection(connStr);
            _gameRepo = new GameRepository(connStr);
            _diceRepo = new DiceRepository(connStr);
            _shake = new ShakeValueRepository(connStr);
        }

        /// <summary>
        /// Use this method to check to see if a game can have any additional rolls on it
        /// </summary>
        /// <param name="gameId"></param>
        /// <returns></returns>
        public bool CanRoll (long gameId)
        {
            var thisGame = _gameRepo.GetGameById(gameId).Result;
            var gameType = _gameRepo.GetGameType(thisGame).Result;

            //if you're allowed to take any more rolls
            if(gameType.RollsPerGame > thisGame.RollsTaken)
            {
                return true;
            }else
                return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="gameId"></param>
        /// <param name="diceQty"></param>
        /// <returns></returns>
        public async Task<DiceHand> PerformRoll(long userId, long gameId, DiceHand handIn)
        {
            var thisGame = await _gameRepo.GetGameById(gameId);
            var gType = await _gameRepo.GetGameType(thisGame.TypeId);

            if(thisGame.RollsTaken < gType.RollsPerGame)
            {
                handIn.RollNonHeldDice();
                var updt = _gameRepo.UpdateGameRollsTaken(thisGame.Id, (thisGame.RollsTaken + 1));
                _diceRepo.SaveHand(handIn, userId, gameId, (thisGame.RollsTaken + 1));
                //TODO: check results of update, handle if error?
                handIn.RollNumber = (thisGame.RollsTaken + 1);

                return handIn;
            }
            else
                return new DiceHand();
        }

        /// <summary>
        /// will return the last hand rolled into a DiceHand object for a particular game, should be used on page/game load
        /// </summary>
        /// <param name="gameId"></param>
        /// <returns></returns>
        public async Task<DiceHand> GetHandFromGame(long gameId)
        {
            var game = await _gameRepo.GetGameById(gameId);
            var roll = await _diceRepo.GetNextRollNumberForGame(gameId);
            var hand = await _diceRepo.GetHandForGame(gameId, roll);

            return hand;
        }

        public async Task<GameWinType> EvaluateGame(DiceHand handIn)
        {
            var dice = handIn.Hand;
            var shakevalue = await _shake.GetShakeOfDay(DateTime.Now.Year, DateTime.Now.DayOfYear);

            var pairs = dice.Count(x => x.dieValue == shakevalue);
          
            //future use?
            var sumNonRolls = dice.Where(x => x.dieValue != shakevalue).Sum(x => x.dieValue);

            switch (pairs)
            {
                case 3:
                    return GameWinType.three;
                case 4:
                    return GameWinType.four;
                case 5:
                    return GameWinType.five;
                default:
                    return GameWinType.loss;
            }
            
        }
    }
}
