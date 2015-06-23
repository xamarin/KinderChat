using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KinderChat.ServerClient.Entities;

namespace KinderChat.ServerClient.Interfaces
{
    public interface IAvatarManager
    {
        Task<List<Avatar>> GetStaticAvatars();
    }
}
