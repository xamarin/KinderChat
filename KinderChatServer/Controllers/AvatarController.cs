using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using KinderChatServer.DAL;
using KinderChatServer.Models;
using KinderChatServer.Tools;

namespace KinderChatServer.Controllers
{
    [Authorize]
    public class AvatarController : ApiController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        public IQueryable<Avatar> GetAvatars()
        {
            return _db.Avatars.Where(node => node.Type != AvatarType.User);
        }
    }
}
