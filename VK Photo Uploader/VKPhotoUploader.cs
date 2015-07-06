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

using VkNet.Exception;

using VK_Photo_Uploader.Classes;

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

        public static async Task<string> UploadImages(long ownerId, string [] files, string message)
        {
            string result = "OK";
            try
            {
                var serverUp = Api.Photo.GetWallUploadServer(ownerId);
                var url = serverUp.UploadUrl;

                var coll = new List<VkNet.Model.Attachments.Photo>().AsEnumerable();
                foreach (var file in files)
                {
                    var res = await UploadFile(url, file);
                    var response = JsonConvert.DeserializeObject<PhotoResponse>(res);
                    var col = Api.Photo.SaveWallPhoto(response.Photo, Api.UserId, ownerId, response.Server, response.Hash);
                    coll = coll.Concat(col);
                }
                long type = 1;
                var owner = Api.Utils.ResolveScreenName("public" + ownerId);
                if (owner.Id == ownerId)
                    type = -1;
                var post = Api.Wall.Post(ownerId * type, false, true, message, coll, signed:true);
            }
            catch (AccessDeniedException e)
            {
                result = "Доступ запрещен!";
            }
            catch (Exception e)
            {
                if(result == "OK")
                    result = "Не удалось загрузить...";
            }
           
            return result;
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
