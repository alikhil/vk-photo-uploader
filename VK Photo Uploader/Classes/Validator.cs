using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using VkNet;
using VkNet.Model;

namespace VK_Photo_Uploader.Pages
{
    public static class Validator
    {
        public async static Task Try(Group group, string[] fileNames, bool fromGroup = false, string message = null)
        {
            if (!group.CanPost)
            {
                Dispatcher.CurrentDispatcher.Invoke(() => {
                    MessageBox.Show(String.Format("У вас не достаточно прав чтобы загрузить фотографии на стену группы {0}", group.Name),"Ошибка доступа");
                });
                return;
            }
            
            Dispatcher.CurrentDispatcher.Invoke(() => {
                var res = MessageBox.Show(String.Format("Вы действительно хотите загрузить {0} фото на стену группы {1}?", fileNames.Length, group.Name), "Подтверждение", MessageBoxButton.YesNo);
                if (res != MessageBoxResult.Yes)
                    return;
            });
           await VKPhotoUploader.UploadImages(group.ScreenName, fileNames, message, fromGroup, VkNet.Enums.VkObjectType.Group, group.Id);
        }
        public async static Task Try(User user, string[] fileNames, string message = null)
        {
            if (!user.CanPost)
            {
                Dispatcher.CurrentDispatcher.Invoke(() =>
                {
                    MessageBox.Show(String.Format("Пользователь {0} {1} запретил добавление записей на своей странице", user.FirstName, user.LastName), "Ошибка доступа");
                });
                return;
            }
            Dispatcher.CurrentDispatcher.Invoke(() =>
            {
                var res = MessageBox.Show(String.Format("Вы действительно хотите загрузить {0} фото на стену пользователя {1} {2}?", fileNames.Length, user.FirstName, user.LastName), "Подтверждение", MessageBoxButton.YesNo);
                if (res != MessageBoxResult.Yes)
                    return;
            });
            await VKPhotoUploader.UploadImages(user.Domain, fileNames, message, false, VkNet.Enums.VkObjectType.User, user.Id);
        }

        public async static Task Try(string screenName, string[] fileNames, string message, bool fromGroup)
        {
            var vkObject = VKPhotoUploader.IdentifyScreenName(screenName);
            if(vkObject.Type == VkNet.Enums.VkObjectType.User)
            {
                var user = VKPhotoUploader.FindUserById(vkObject.Id.Value);
                await Try(user, fileNames, message);
                return;
            }
            if (vkObject.Type == VkNet.Enums.VkObjectType.Group)
            {
                var group = VKPhotoUploader.FindGroupById(vkObject.Id.Value);
                await Try(group, fileNames, fromGroup, message);
                return;
            }
            Dispatcher.CurrentDispatcher.Invoke(() => {
                MessageBox.Show("Укажите корректный screenName", "Ошибка");
            });
            return;
        }
    }
}
