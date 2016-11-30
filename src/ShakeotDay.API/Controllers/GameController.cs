using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ShakeotDay.Core.Models;
using Microsoft.Extensions.Options;
using ShakeotDay.Core.Repositories;
using System.Net;
using ShakeotDay.API.Models;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ShakeotDay.API.Controllers
{
    [Route("api/[controller]")]
    public class GameController : Controller
    {
        private GameRepository _gameRepo;
        private GameEngineRepository _engine;
        private DiceRepository _diceRepo;
        private ShakeValueRepository _shake;
        private WalletRepository _wallets;

        public GameController(IOptions<ConnectionStrings> connIn)
        {
            _gameRepo = new GameRepository(connIn.Value.DefaultConnection);
            _engine = new GameEngineRepository(connIn.Value.DefaultConnection);
            _diceRepo = new DiceRepository(connIn.Value.DefaultConnection);
            _shake = new ShakeValueRepository(connIn.Value.DefaultConnection);
            _wallets = new WalletRepository(connIn.Value.DefaultConnection);
        }

        // GET: api/values
        [HttpGet("default/{id}")]
        public IActionResult GetDefaultGames(long id)
        {
            var getGamesTask = _gameRepo.GetManyGames(id);
            var Games = getGamesTask.Result;
            if (Games.Count() == 0)
                return NoContent();
            else
                return Ok(Games);
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public IActionResult GetSingleGame(long id)
        {
            var getgameTask = _gameRepo.GetGameById(id);
            getgameTask.Wait();
            var gameObj = getgameTask.Result;

            return Ok(gameObj);
        }

        // POST api/values
        [HttpPost("{GameType}/new/{UserId}")]
        public IActionResult CreateNewGame(long UserId, GameTypeEnum GameType)
        {

            var cntTask = _gameRepo.UserGamesPlayedToday(UserId);
            var cnt = cntTask.Result;
            if (cnt != 0) return new BadRequestObjectResult(new ShakeException(ShakeError.AlreadyPlayedToday, "You have already played a game today."));


            var gameTask = _gameRepo.NewGame(UserId, GameType);
            gameTask.Wait();
            var gameId = gameTask.Result.Single();

            var success = _wallets.SubtractABuck(UserId).Result;
            if (!success)
                return new NoContentResult();

            var getgameTask = _gameRepo.GetGameById(gameId);
            getgameTask.Wait();
            var gameObj = getgameTask.Result;

            return Ok(gameObj);
        }

        /// <summary>
        /// Should be used to close games before finishing them only, the rules engine should close games via repo if conditions
        /// have been satisfied.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="id"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        [HttpPut("{id}/user/{user}")]
        public IActionResult CloseGame(long user, long id, [FromBody] GameResult result)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                //figure out if a winning hand here, then pass into closegame
                var res = _gameRepo.CloseGame(id, result.winType);
                return Ok(res);
            }
            catch (Exception e)
            {
                return StatusCode(500,(new ShakeException(ShakeError.Other, $"There was an Error closing the game. \n Err: {e.InnerException}")));
            }
        }

        [HttpGet("{gameid}/Hand")]
        public IActionResult GetGameHand(long gameid)
        {
            if (!ModelState.IsValid)
            {
                throw new Exception("invalid model state");
            }

            return Ok(_engine.GetHandFromGame(gameid).Result);
        }

        [HttpPost("{gameid}/Roll/{userId}")]
        public IActionResult RollDice(long gameid, long userId, [FromBody]DiceHand handIn)
        {
            //get user id from login ?
            if (!ModelState.IsValid || handIn.Hand.Count != 5)
            {
                throw new Exception("invalid model state");
            }
            //if(handIn.)

            var newHand = _engine.PerformRoll(userId, gameid, handIn).Result;

            //if the roll comes back as nothing
            if(newHand.Hand.Count == 0)
            {
                return Ok(new ShakeException(ShakeError.NoMoreRollsAllowed, $"You are out of rolls for game {gameid}"));
            }

            //TODO: move this into it's own route??
            var type = _gameRepo.GetGameType(GameTypeEnum.ShakeOfTheDay).Result;
            if(newHand.RollNumber == type.RollsPerGame)
            {
              
                var wintype = _engine.EvaluateGame(handIn).Result;
                var t = _gameRepo.CloseGame(gameid, wintype).Result;

                if(wintype != GameWinType.loss)
                {
                    bool result;
                    switch (wintype)
                    {
                        case GameWinType.three:
                            result = _wallets.TransferAmountFromJackpot(userId, 1).Result;
                            break;
                        case GameWinType.four:
                            result = _wallets.TransferAmountFromJackpot(userId, 5).Result;
                            break;
                        case GameWinType.five:
                            result = _wallets.TransferJackpot(userId).Result;
                            break;
                        default:
                            break;
                    }
                }
            }

            return Ok(newHand);
        }

        [HttpGet("ShakeOfTheDay/{view:int?}")]
        public IActionResult GetTodayShakeValue(int? view = 0)
        {
            //view 0 = today
            //view 1 = tomorrow
            int val = -1;
            if (view == 0)
            {
                val = _shake.GetShakeOfDay(DateTime.Now.Year, DateTime.Now.DayOfYear).Result;
            }else
            {
                var tomorrow = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                tomorrow = tomorrow.AddDays(1);
                val = _shake.GetShakeOfDay(tomorrow.Year, tomorrow.DayOfYear).Result;
            }

            return Ok(val);
        }

        [HttpGet("ShakeOfTheDay/{year:int}/{day:int}")]
        public IActionResult GetSpecificShakeValue(int year, int day)
        {
            //view 0 = today
            //view 1 = tomorrow
            int val = -1;
            val = _shake.GetShakeOfDay(year, day).Result;
           
            return Ok(val);
        }

        [HttpGet("Wallets/{user}")]
        public IActionResult GetWallet(long user)
        {
            var wal = _wallets.GetOrCreateWallet(user).Result;

            return Ok(wal);
        }
    }
}
