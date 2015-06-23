using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using KinderChatServer.DAL;
using KinderChatServer.Models;

namespace KinderChatServer.Controllers
{
    public class BlocksController : ApiController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        // GET: api/Blocks
        public IQueryable<Block> GetBlocked()
        {
            var userdevice = UserManager.FindByName(User.Identity.Name);
            var user = _db.Users.First(node => node.Id == userdevice.UserId);
            return _db.Blocked.Where(node => node.UserId == user.Id);
        }

        // GET: api/Blocks/5
        [ResponseType(typeof(Block))]
        public IHttpActionResult GetBlock(int id)
        {
            var userdevice = UserManager.FindByName(User.Identity.Name);
            var user = _db.Users.First(node => node.Id == userdevice.UserId);
            var block = _db.Blocked.FirstOrDefault(node => node.UserId == user.Id && node.Id == id);
            if (block == null)
            {
                return NotFound();
            }

            return Ok(block);
        }

        // POST: api/Blocks
        [ResponseType(typeof(Block))]
        public IHttpActionResult PostBlock(Block block)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userdevice = UserManager.FindByName(User.Identity.Name);
            var user = _db.Users.First(node => node.Id == userdevice.UserId);

            if (block.UserId != user.Id)
            {
                return BadRequest("Bad Request: Can't remove another users blocked user");
            }

            _db.Blocked.Add(block);
            _db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = block.Id }, block);
        }

        // DELETE: api/Blocks/5
        [ResponseType(typeof(Block))]
        public IHttpActionResult DeleteBlock(int id)
        {
            var block = _db.Blocked.Find(id);
            if (block == null)
            {
                return NotFound();
            }

            _db.Blocked.Remove(block);
            _db.SaveChanges();

            return Ok(block);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool BlockExists(int id)
        {
            return _db.Blocked.Count(e => e.Id == id) > 0;
        }
    }
}