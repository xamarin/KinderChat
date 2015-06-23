using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using KinderChatServer.DAL;
using KinderChatServer.Models;
using KinderChatServer.Tools;

namespace KinderChatServer.Controllers
{
    [Authorize]
    public class AdminController : ApiController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        [HttpGet]
        public IHttpActionResult ViewRegistrations()
        {
            var result = _db.Users.GroupBy(o => new
            {
                o.Joined.Month, o.Joined.Year, o.Joined.Day
            }).Select(g => new RegDate
            {
                Month = g.Key.Month,
                Year = g.Key.Year,
                Day = g.Key.Day,
                Total = g.Count()
            })
        .OrderByDescending(a => a.Year)
        .ThenByDescending(a => a.Month)
        .ToList();
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult ViewFlagUsers(int id)
        {
            return Ok(_db.Users.Where(node => node.UserFlags.Any(test => test.FlagId == id)));
        }

        [HttpGet]
        public IHttpActionResult TotalAvatars()
        {
            return Ok(_db.Avatars.Count(node => node.Type == AvatarType.User));
        }

        [HttpGet]
        public IHttpActionResult GetPopularNames()
        {
            var result = _db.Users.GroupBy(o => new
            {
                o.NickName
            }).Select(node => new PopularNames
            {
                Nickname = node.Key.NickName,
                Count = node.Count()
            }).OrderByDescending(a => a.Count).ToList();
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult GetPopularAvatars()
        {
            var result = _db.Users.GroupBy(o => new
            {
                o.Avatar
            }).Select(node => new PopularAvatars
            {
                AvatarId = node.Key.Avatar.Id,
                Count =  node.Count()
            }).ToList();
            return Ok(result);
        }

        public class PopularNames
        {
            public string Nickname { get; set; }

            public int Count { get; set; }
        }

        public class PopularAvatars
        {
            public int AvatarId { get; set; }

            public int Count { get; set; }
        }

        public class RegDate
        {
            public int Year { get; set; }
            public int Month { get; set; }
            public int Day { get; set; }
            public string MonthName => CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(Month);
            public int Total { get; set; }
        }
    }
}
