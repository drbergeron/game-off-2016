using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShakeotDay.API.Controllers;
using ShakeotDay.Core.Models;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;
using ShakeotDay.ViewModels;
using ShakeotDay.Web.Models;
using Microsoft.AspNetCore.Identity;
using ShakeotDay.Core.Repositories;

namespace ShakeotDay.Controllers
{
    [Authorize]
    public class GameController : Controller
    {

        // GET: Game
        private IOptions<ConnectionStrings> _conn;
        // Stores UserManager
        private readonly UserManager<ApplicationUser> _manager;

        public GameController(IOptions<ConnectionStrings> optIn, UserManager<ApplicationUser> manager)
        {
            _conn = optIn;
            _manager = manager;
        }
        
        public IActionResult Index(bool error, ShakeException ex)
        {
            
            ViewData["Error"] = error ? true : false; 
            ViewData["Type"] = ex?.ErrorType ?? "";
            ViewData["errorMsg"] = ex?.ErrorMessage ?? "";
            ViewData["errorNo"] = ex?.ErrorNumber ?? -1;


            var user = _manager.GetUserAsync(HttpContext.User).Result;
            var friendlyID = user?.FriendlyUserId ?? -1;

            var con = new API.Controllers.GameController(_conn);
            var respAct = con.GetDefaultGames(friendlyID);
            //if the response was noContent (no games), then create an empty ObjectResult, else cast returned fame as ObjectResult
            var resp = (ObjectResult)(respAct.GetType() == typeof(NoContentResult) ? new ObjectResult(new List<Game>()) : respAct);

            return View(resp.Value);
        }
        
        // GET: Game/Details/5
        public ActionResult Details(long id)
        {
            var user = _manager.GetUserAsync(HttpContext.User).Result;
            var friendlyID = user?.FriendlyUserId;

            ViewData["UserId"] = friendlyID ?? -1;
            ViewData["access_token"] = "";

            var api = new API.Controllers.GameController(_conn);

            var respGame = api.GetSingleGame(id);
            var gameOr = (ObjectResult)(respGame.GetType() == typeof(NoContentResult) ? new ObjectResult(new Game()) : respGame);

            var respHand = api.GetGameHand(id);
            var hand = (ObjectResult)(respHand.GetType() == typeof(NoContentResult) ? new ObjectResult(new DiceHand()) : respHand);

            var game = (Game)gameOr.Value;

            //get today's value
            var shakeValueResp = api.GetSpecificShakeValue(game.Year, game.Day);
            var shakeValue = (ObjectResult)(shakeValueResp.GetType() == typeof(NoContentResult) ? new ObjectResult(-1) : shakeValueResp);

            var gameHand = new GameHand(game, (DiceHand)hand.Value, (int)shakeValue.Value);
            return View(gameHand);
        }
  
        // GET: Game/Create
        public ActionResult Create()
        {
            var user = _manager.GetUserAsync(HttpContext.User).Result;
            var friendlyID = user?.FriendlyUserId ?? -1;

            var api = new API.Controllers.GameController(_conn);
            var respAct = api.CreateNewGame(friendlyID, GameTypeEnum.ShakeOfTheDay);

            var resp = (ObjectResult)(respAct.GetType() == typeof(NoContentResult) ? new ObjectResult(new Game()) : respAct);

            if(resp.StatusCode == 400)
            {
                var err = (ShakeException)resp.Value;
                return RedirectToAction("Index", new { error = true, ex = err });
            }
            else
                return RedirectToAction("Index", new { error = false});
        }

        // POST: Game/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Game/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Game/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Game/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Game/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}