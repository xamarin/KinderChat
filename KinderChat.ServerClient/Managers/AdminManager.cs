using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using KinderChat.ServerClient.Entities;
using KinderChat.ServerClient.Interfaces;

namespace KinderChat.ServerClient.Managers
{
    public class AdminManager : IAdminManager
    {
        private readonly IWebManager _webManager;
        public AdminManager(IWebManager webManager)
        {
            _webManager = webManager;
        }

        public AdminManager(string authenticationToken)
            : this(new WebManager(authenticationToken))
        {
        }


        public async Task<List<RegDate>> TotalRegistrations()
        {
            var uri = new Uri(EndPoints.ViewRegistrations);
            var result = await _webManager.GetData(uri);
            return JsonConvert.DeserializeObject<List<RegDate>>(result.ResultJson);
        }

        public async Task<List<User>> FlagUserList(int flagId)
        {
            var uri = new Uri(string.Format(EndPoints.ViewFlagUsers, flagId));
            var result = await _webManager.GetData(uri);
            return JsonConvert.DeserializeObject<List<User>>(result.ResultJson);
        }

        public async Task<int> TotalAvatarCount()
        {
            var uri = new Uri(EndPoints.TotalAvatars);
            var result = await _webManager.GetData(uri);
            return Convert.ToInt32(result.ResultJson);
        }

        public async Task<List<PopularNames>> PopularNames()
        {
            var uri = new Uri(EndPoints.GetPopularNames);
            var result = await _webManager.GetData(uri);
            return JsonConvert.DeserializeObject<List<PopularNames>>(result.ResultJson);
        }

        public async Task<List<PopularAvatars>> PopularAvatars()
        {
            var uri = new Uri(EndPoints.GetPopularAvatars);
            var result = await _webManager.GetData(uri);
            return JsonConvert.DeserializeObject<List<PopularAvatars>>(result.ResultJson);
        }
    }
}
