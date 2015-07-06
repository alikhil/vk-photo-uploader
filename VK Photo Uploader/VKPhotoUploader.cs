using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Exception;
using Newtonsoft.Json;
using System.Net;
using System.IO;

namespace VK_Photo_Uploader
{
    public static class VKPhotoUploader
    {
        private static VkApi Api = new VkApi();

        public static bool Authorize(string email, string password)
        {
            try
            {
                Api.Authorize(Constants.AppId, email, password, Settings.Groups | Settings.Photos | Settings.Wall);
                return true;
            }
            catch(VkApiAuthorizationException ex)
            {
                return false;
            }
        }

        private static async Task<string> UploadFile(string url, string fName)
        {
            WebClient wClient = new WebClient();
            var ans = await wClient.UploadFileTaskAsync(url, "POST", fName);
            string res = System.Text.Encoding.Default.GetString(ans);
            return res;
        }
    }
}
