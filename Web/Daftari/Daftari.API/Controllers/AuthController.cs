using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Daftari.API.Controllers
{
    public class AuthController : ApiController
    {
        [HttpGet]
        [Route("api/auth/test")]
        public string GetTest()
        {
            return "working";
        }
    }
}
