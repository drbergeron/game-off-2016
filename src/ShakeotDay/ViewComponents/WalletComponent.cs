using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShakeotDay.Core.Models;
using ShakeotDay.Core.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity;
using ShakeotDay.Web.Models;
using ShakeotDay.ViewModels;

namespace ShakeotDay.ViewComponents
{
    public class WalletViewComponent : ViewComponent
    {
        // GET: Game
        private IOptions<ConnectionStrings> _options;
        private readonly UserManager<ApplicationUser> _manager;

        public WalletViewComponent(IOptions<ConnectionStrings> optIn, UserManager<ApplicationUser> manager)
        {
            _options = optIn;
            _manager = manager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = _manager.GetUserAsync(HttpContext.User).Result;
            var friendlyID = user?.FriendlyUserId ?? -1;
            var api = new API.Controllers.GameController(_options);

            if (friendlyID == 0)
            {
                return View<WalletsViewModel>("~/Shared/_WalletPartial", null);
            }

            var userWallResp = api.GetWallet(friendlyID);
            var userWall = (ObjectResult)(userWallResp.GetType() == typeof(NoContentResult) ? new ObjectResult(new Game()) : userWallResp);

            var jackpotResp = api.GetWallet((int)WalletRepository.Jackpot.Wallet);
            var jackpot = (ObjectResult)(jackpotResp.GetType() == typeof(NoContentResult) ? new ObjectResult(new Game()) : jackpotResp);

            return View(new WalletsViewModel() { JackpotWallet = (Wallet)jackpot.Value, UserWallet = (Wallet)userWall.Value });
        }
    }
}