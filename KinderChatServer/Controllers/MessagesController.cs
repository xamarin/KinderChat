using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using KinderChatServer.DAL;
using KinderChatServer.Models;
using KinderChatServer.Tools;

namespace KinderChatServer.Controllers
{
    [Authorize]
    public class MessagesController : ApiController
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
        
        [HttpGet]
        public IQueryable<IOrderedEnumerable<Message>> GetConversation(int fromUserId, string deviceId)
        {
            var user = UserManager.FindByName(User.Identity.Name);
            // Get messages sent to the current user from the id specified.
            return
                _db.Messages.Where(d => d.FromUserId == fromUserId && d.DeviceId == deviceId && d.ToUserId == user.UserId)
                    .GroupBy(c => c.ToUserId)
                    .Select(node => node.OrderByDescending(msg => msg.TimeStamp));
        }

        [HttpGet]
        public IQueryable<Message> GetLastMessage(int fromUserId, string deviceId)
        {
            var user = UserManager.FindByName(User.Identity.Name);
            return _db.Messages.Where(
                d => d.FromUserId == fromUserId && d.DeviceId == deviceId && d.ToUserId == user.UserId)
                .GroupBy(c => c.ToUserId)
                .Select(node => node.OrderByDescending(msg => msg.TimeStamp).LastOrDefault());
        }

        [HttpGet]
        public List<User> GetConversationList()
        {
            var user = UserManager.FindByName(User.Identity.Name);
            var fromUserIds = _db.Messages.Where(d => d.ToUserId == user.UserId).Select(node => node.FromUserId);
            var users = _db.Users.Where(p => fromUserIds.Contains(p.Id)).ToList();
            return users;
        }

        /* 
        While messages are sent via a live websocket connection, 
        if we wanted to save a message directly to the database, we can call
        this API.
        */
        [HttpPost]
        [ResponseType(typeof(Message))]
        public IHttpActionResult PostMessage(int toUserId, string deviceId, string encryptedMessage)
        {
            var user = UserManager.FindByName(User.Identity.Name);
            var message = new Message()
            {
                DeviceId = deviceId,
                EncryptedMessage = encryptedMessage,
                ToUserId = toUserId,
                FromUserId = user.UserId
            };

            if (user.UserId == message.ToUserId)
            {
                return BadRequest("Can't send message to self.");
            }
            user.KinderPoints++;
            _db.UserDevices.AddOrUpdate(user);
            _db.Messages.Add(message);
            _db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = message.Id }, message);
        }

        [HttpPost]
        [ResponseType(typeof(Message))]
        public IHttpActionResult PostMessages(int toUserId, List<string> deviceIds, List<string> encryptedMessages)
        {
            // This would be better as a binded model... but whatever. This is easier for now :\
            var user = UserManager.FindByName(User.Identity.Name);
            for (var i = 0; i < deviceIds.Count; i++)
            {
                var message = new Message()
                {
                    DeviceId = deviceIds[i],
                    EncryptedMessage = encryptedMessages[i],
                    ToUserId = toUserId,
                    FromUserId = user.UserId
                };

                if (user.UserId == message.ToUserId)
                {
                    return BadRequest("Can't send message to self.");
                }
                user.KinderPoints++;
                _db.Messages.Add(message);
            }
            _db.UserDevices.AddOrUpdate(user);
            _db.SaveChanges();

            return Ok();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}