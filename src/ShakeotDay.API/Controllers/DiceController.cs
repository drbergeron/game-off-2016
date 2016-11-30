using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ShakeotDay.Core.Repositories;
using ShakeotDay.Core.Models;
using Microsoft.Extensions.Options;
// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ShakeotDay.API.Controllers
{
    [Route("api/[controller]")]
    public class DiceController : Controller
    {

        private DiceRepository _repo;
        //this will likely never be used as a controller, and will instead be in the game engine repo.
        //
        public DiceController(IOptions<ConnectionStrings> connIn)
        {
            _repo = new DiceRepository(connIn.Value.DefaultConnection);
        }

      
        // GET: api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        
        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
