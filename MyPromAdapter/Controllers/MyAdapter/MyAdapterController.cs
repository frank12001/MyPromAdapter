using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace MyPromAdapter.Controllers.MyAdapter
{
    [Route("api/[controller]")]
    [ApiController]
    public class MyAdapterController : ControllerBase
    {
        // POST api/values
        [HttpPost("{id}")]
        public ActionResult<string> Post(int id,[FromBody] AlertmanagerMsg body)
        {
            ActionResult<string> result = Ok("OK");
            switch (id)
            {
                case 1:
                    HttpAction[] actions = new HttpActionFactory(body).GetActions();
                    result = Ok(JsonConvert.SerializeObject(body));
                    break;
                default:
                    result = BadRequest();
                    break;
            }
            Console.WriteLine($"post body:{JsonConvert.SerializeObject(body)}");
            return result;
        }
    }
}