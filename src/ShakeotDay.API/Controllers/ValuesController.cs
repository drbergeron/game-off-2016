using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ShakeotDay.Core.Models;
using Microsoft.Extensions.Options;
using ShakeotDay.Core.Repositories;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;

namespace ShakeotDay.API.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private readonly DiceRepository _repo;
        private readonly GameEngineRepository _engine;
        private readonly JsonSerializerSettings _serializerSettings;

        public ValuesController(IOptions<ConnectionStrings> conn)
        {
            _repo = new DiceRepository(conn.Value.DefaultConnection);
            _engine = new GameEngineRepository(conn.Value.DefaultConnection);
            _serializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };
        }

        // GET api/values
        [HttpGet]
        public IActionResult Get()
        {
            var testHand = new DiceHand(new List<Dice>()
            {
                new Dice(1),
                new Dice(1),
                new Dice(2),
                new Dice(2),
                new Dice(2)
            }, 3);
            return Ok(_engine.EvaluateGame(testHand).Result);
        }

        // GET api/values/5
       
        [HttpGet("{id}")]
        [Authorize(Policy = "DisneyUser")]
        public IActionResult Get(int id)
        {
            var response = new
            {
                made_it = "Welcome Mickey!"
            };

            var json = JsonConvert.SerializeObject(response, _serializerSettings);
            return new OkObjectResult(json);
        }
    

        
        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
