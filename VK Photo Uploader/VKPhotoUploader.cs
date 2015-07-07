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

using VK_Photo_Uploader.Classes;

namespace VK_Photo_Uploader
{
    public static class VKPhotoUploader
    {
        private static VkApi Api = new VkApi();
        public delegate void UploadImageProgressChange(UploadProgressChangedEventArgs e);
        public static event UploadImageProgressChange OnImageUploadProgressChange;

        public delegate void UploadTotalProgessChange(int uploaded, int total);
        public static event UploadTotalProgessChange OnTotalProgressChange;

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

        public static async Task<string> UploadImages(string screenName, string [] files, string message, bool fromGroup)
        {
            string result = "OK";
            try
            {
                var owner = Api.Utils.ResolveScreenName(screenName);
                var ownerId = owner.Id;
                var serverUp = Api.Photo.GetWallUploadServer(ownerId);
                var url = serverUp.UploadUrl;

                var coll = new List<VkNet.Model.Attachments.Photo>().AsEnumerable();
                int cnt = 0, total = files.Length;
                if (OnTotalProgressChange != null)
                    OnTotalProgressChange(cnt, total);
                foreach (var file in files)
                {
                    var res = await UploadFile(url, file);
                    var response = JsonConvert.DeserializeObject<PhotoResponse>(res);
                    var col = Api.Photo.SaveWallPhoto(response.Photo, Api.UserId, ownerId, response.Server, response.Hash);
                    coll = coll.Concat(col);
                    cnt++;
                    if (OnTotalProgressChange != null)
                        OnTotalProgressChange(cnt, total);
                }
                var post = Api.Wall.Post(ownerId * (owner.Type == VkNet.Enums.VkObjectType.Group ? -1 : 1), false, fromGroup, message, coll, signed:true);
            }
            catch (AccessDeniedException e)
            {
                result = "Доступ запрещен!";
            }
            catch (Exception e)
            {
                if(result == "OK")
                    result = "Не удалось загрузить..." + e.Message;
            }
           
            return result;
        }
        
        private static async Task<string> UploadFile(string url, string fName)
        {
            WebClient wClient = new WebClient();
            wClient.UploadProgressChanged += wClient_UploadProgressChanged;   
            var ans = await wClient.UploadFileTaskAsync(url, "POST", fName);
            string res = System.Text.Encoding.Default.GetString(ans);
            return res;
        }

        private static void wClient_UploadProgressChanged(object sender, UploadProgressChangedEventArgs e)
        {
            if (OnImageUploadProgressChange != null)
                OnImageUploadProgressChange(e);
        }

    }
}
