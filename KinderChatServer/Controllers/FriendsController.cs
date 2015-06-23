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
    [Authorize]
    public class FriendsController : ApiController
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
        // GET: api/Friends
        public IQueryable<Friend> GetFriends()
        {
            var userdevice = UserManager.FindByName(User.Identity.Name);
            var user = _db.Users.First(node => node.Id == userdevice.UserId);
            return _db.Friends.Where(node => node.UserId == user.Id);
        }

        // GET: api/Friends/5
        [ResponseType(typeof(Friend))]
        public IHttpActionResult GetFriend(int id)
        {
            var userdevice = UserManager.FindByName(User.Identity.Name);
            var user = _db.Users.First(node => node.Id == userdevice.UserId);
            var friend = _db.Friends.FirstOrDefault(node => node.UserId == user.Id && node.Id == id);

            if (friend == null)
            {
                return NotFound();
            }

            return Ok(friend);
        }

        // PUT: api/Friends/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutFriend(int id, Friend friend)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != friend.Id)
            {
                return BadRequest();
            }

            _db.Entry(friend).State = EntityState.Modified;

            try
            {
                _db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FriendExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Friends
        [ResponseType(typeof(Friend))]
        public IHttpActionResult PostFriend(Friend friend)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userdevice = UserManager.FindByName(User.Identity.Name);
            var user = _db.Users.First(node => node.Id == userdevice.UserId);

            if (friend.UserId != user.Id)
            {
                return BadRequest("Bad Request: Can't Set another users friend");
            }

            if (_db.Blocked.Any(node => node.BlockUserId 
            == friend.UserId && node.UserId == friend.FriendUserId))
            {
                return BadRequest("Bad Request: Can't add as friend");
            }

            _db.Friends.Add(friend);
            _db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = friend.Id }, friend);
        }

        // DELETE: api/Friends/5
        [ResponseType(typeof(Friend))]
        public IHttpActionResult DeleteFriend(int id)
        {
            var friend = _db.Friends.Find(id);
            if (friend == null)
            {
                return NotFound();
            }

            _db.Friends.Remove(friend);
            _db.SaveChanges();

            return Ok(friend);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool FriendExists(int id)
        {
            return _db.Friends.Count(e => e.Id == id) > 0;
        }
    }
}