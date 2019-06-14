
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace JWTCommonLibForDotNetCore.Controllers
{
    [Route("api/[controller]")]
    public class TestController : Controller
    {
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "test6", "test5" };
        }
    }
}