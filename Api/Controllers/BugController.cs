using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Api.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Api.Controllers
{
    public class BugController : BasicApiController
    {
        private readonly DataContext _context;
        public BugController(DataContext context)
        {
            _context = context;
        }
        
        [Authorize]
        [HttpGet("auth")]
        public ActionResult<string> GetSecret()
        {
            return "lol";
        }

        [HttpGet("notfound")]
        public ActionResult<string> GetNotFound()
        {
            var thing = _context.Users.Find(-1);

            if(thing == null)
                return NotFound();

            return Ok(thing);
        }
        
        [HttpGet("servererror")]
        public ActionResult<string> GetServerError()
        {
            var thing = _context.Users.Find(-1);
            return thing.ToString();
        }

        [HttpGet("badrequest")]
        public ActionResult<string> GetBadRequest()
        {
            return BadRequest("Bad request");
        }
    }
}