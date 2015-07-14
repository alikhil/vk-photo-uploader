using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using VkNet.Model;
namespace VK_Photo_Uploader.Pages
{
    /// <summary>
    /// Логика взаимодействия для Tabs.xaml
    /// </summary>
    public partial class Tabs : Page
    {
        public static List<User> Friends { get; set; }
        public static List<Group> Groups { get; set; }
        public Tabs()
        {
            InitializeComponent();
            InitializeComponent();
            VKPhotoUploader.OnTotalProgressChange += VKPhotoUploader_OnTotalProgressChange;
            VKPhotoUploader.OnImageUploadProgressChange += VKPhotoUploader_OnImageUploadProgressChange;
        }

        private string[] FileNames = new string[] { };
           

        void VKPhotoUploader_OnImageUploadProgressChange(System.Net.UploadProgressChangedEventArgs e)
        {
            Dispatcher.Invoke(() => {
                ImageProgressBar.Maximum = 100;
                ImageProgressBar.Value = e.ProgressPercentage;
            });
        }

        void VKPhotoUploader_OnTotalProgressChange(int uploaded, int total)
        {
            Dispatcher.Invoke(() => {
                UploadStatus.Text = String.Format("Загружено фотографий {0} из {1}", uploaded, total);
            });
        }

        private void ChosePhotosBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.DefaultExt = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            ofd.Filter = "Фотографии |*.jpg;*.png;*.jpeg";
            ofd.Multiselect = true;
            var res = ofd.ShowDialog();
            if(res.Value)
            {
                FileNames = ofd.FileNames;
                PhotoStatus.Text = "Выбрано фотографий - " + FileNames.Length;
            }
        }

        private async void UploadBtn_Click(object sender, RoutedEventArgs e)
        {
            int tabId = TabsControl.SelectedIndex;
            if (FileNames.Length == 0)
            {
                MessageBox.Show("Выберите фотографии!");
                return;
            }
            switch (tabId)
            {
                //Группы
                case 0:
                    if(GroupNameCBox.SelectedIndex == -1)
                    {
                        MessageBox.Show("Выберите группу!");
                        return;
                    }
                    await Validator.Try(Groups[GroupNameCBox.SelectedIndex], FileNames, PublishFromGroupChBox.IsChecked.Value, MessageTBox.Text);
                    break;
                    //Друзья
                case 1:
                    if (FriendNameCBox.SelectedIndex == -1)
                    {
                        MessageBox.Show("Выберите друга!");
                        return;
                    }
                    await Validator.Try(Friends[FriendNameCBox.SelectedIndex], FileNames,MessageTBox.Text);
                    break;
                    //По screenName
                case 2:
                    if (String.IsNullOrEmpty(ScreenNameTBox.Text))
                    {
                        MessageBox.Show("Укажите короткое имя!");
                        return;
                    }
                    await Validator.Try(ScreenNameTBox.Text, FileNames, MessageTBox.Text, PublishFromGroupChBox.IsChecked.Value);
                    break;
            }
           
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;
            Thread t = new Thread(new ThreadStart(() => {
                Friends = VKPhotoUploader.GetFriends();
                Groups = VKPhotoUploader.GetGroups();
                Dispatcher.Invoke(() => {
                    FriendNameCBox.ItemsSource = Friends;
                    GroupNameCBox.ItemsSource = Groups;
                    Cursor = Cursors.Arrow;
                });
            }));
            t.Start();
        }
    }
}
