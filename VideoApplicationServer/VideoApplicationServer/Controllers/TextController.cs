using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace VideoApplicationServer.Controllers
{
    public class TextController : ApiController
    {
        [HttpGet]
        public IHttpActionResult GetText()
        {
            return Content(System.Net.HttpStatusCode.OK, "pqrst", Configuration.Formatters.JsonFormatter);
        }
    }
}