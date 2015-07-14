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
using System.Windows;
using System.Windows.Threading;
using VkNet.Model;
using VkNet.Enums;
using VkNet.Model.Attachments;
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
            catch (Exception e)
            {
                Dispatcher.CurrentDispatcher.Invoke(() => {
                    MessageBox.Show(String.Format("Message: {0}", e.Message));
                });
                return false;
            }
        }

        public static VkObject IdentifyScreenName(string screenName)
        {
            return Api.Utils.ResolveScreenName(screenName);
        }

        public static User FindUserById(long id)
        {
            return Api.Users.Get(id, ProfileFields.CanPost);
        }
        public static Group FindGroupById(long id)
        {
            var res = Api.Groups.Get(id, true, fields:GroupsFields.CanPost);
            return res != null && res.Count > 0 ? res[0] : null;
        }
        public static async Task UploadImages(string screenName, string [] files, string message, bool fromGroup, VkNet.Enums.VkObjectType type, long ownerId)
        {
            string result = "OK";
            try
            {
                var serverUp = Api.Photo.GetWallUploadServer(ownerId);
                var url = serverUp.UploadUrl;

                var coll = new List<Photo>().AsEnumerable();
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
                try
                {
                    var post = Api.Wall.Post(ownerId * (type == VkNet.Enums.VkObjectType.Group ? -1 : 1), false, fromGroup, message, coll, signed: true);
                }
                catch
                {
                    if(fromGroup)
                        Api.Wall.Post(ownerId * (type == VkNet.Enums.VkObjectType.Group ? -1 : 1), false, !fromGroup, message, coll, signed: true);
                }
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

            Dispatcher.CurrentDispatcher.Invoke(() => {
                MessageBox.Show(result == "OK" ? "Фотографии успешно загружены" : result);
            });
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

        public static List<User> GetFriends()
        {
           var friends = Api.Friends.Get(Api.UserId.Value, ProfileFields.CanPost);
           return new List<User>(friends);
        }
        public static List<Group> GetGroups()
        {
            var groups = Api.Groups.Get(Api.UserId.Value, true, fields: GroupsFields.CanPost);
            return new List<Group>(groups);
        }
    }
}
