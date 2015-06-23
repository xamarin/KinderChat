using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using KinderChatServer.DAL;
using KinderChatServer.Tools;

namespace KinderChatServer.Controllers
{
    [Authorize]
    public class FlagController : ApiController
    {
        /*
        "Flags" are ways to tag users who have commited bad acts. These acts are stored in the database
        so they can be updated with new acts whenever the admin feels like adding new ones. 
        */
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        public IHttpActionResult GetFlags()
        {
            return Ok(_db.Flags);
        }

        public IHttpActionResult GetFlagsForAccusedUser(int id)
        {
            return Ok(_db.UserFlags.Where(node => node.AccusedUserId == id));
        }

        public IHttpActionResult GetFlagsFromAccuserUser(int id)
        {
            return Ok(_db.UserFlags.Where(node => node.AccuserUserId == id));
        }
    }
}
