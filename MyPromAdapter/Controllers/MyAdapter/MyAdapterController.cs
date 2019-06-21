using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;

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
                    foreach (var httpAction in actions)
                    {
                        httpAction.Go();
                    }
                    result = Ok(JsonConvert.SerializeObject(body));
                    break;
                default:
//                    RestClient RestClient= new RestClient("http://35.229.191.68:31943");
//                    var request = new RestRequest("api/count/1");
//                    request.AddParameter("namespace", "default");
//                    request.AddParameter("name", "myapp");
//                    //request.AddParameter("replicas", "1");
//                    var res = RestClient.Get(request);
//                    result = Ok(res.IsSuccessful);
                    result = BadRequest();
                    break;
            }
            Console.WriteLine($"post body:{JsonConvert.SerializeObject(body)}");
            return result;
        }
    }
}