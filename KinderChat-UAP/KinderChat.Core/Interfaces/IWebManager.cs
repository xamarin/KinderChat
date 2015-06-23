using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using KinderChat.Core.Managers;

namespace KinderChat.Core.Interfaces
{
    public interface IWebManager
    {
        bool IsNetworkAvailable { get; }
        Task<WebManager.Result> PostData(Uri uri, MultipartContent header, StringContent content);

        Task<WebManager.Result> PutData(Uri uri, StringContent json);

        Task<WebManager.Result> DeleteData(Uri uri, StringContent json);

        Task<WebManager.Result> GetData(Uri uri);
    }
}
