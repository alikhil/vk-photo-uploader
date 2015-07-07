using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using Microsoft.Win32;
using System.Text.RegularExpressions;

namespace VK_Photo_Uploader.Pages
{
    /// <summary>
    /// Логика взаимодействия для UploadPage.xaml
    /// </summary>
    public partial class UploadPage : Page
    {
        private string[] FileNames = new string[] { };
        public UploadPage()
        {
            InitializeComponent();
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
            }
        }

        private async void UploadBtn_Click(object sender, RoutedEventArgs e)
        {
            if(String.IsNullOrEmpty(ScreenNameTBox.Text))
            {
                MessageBox.Show("Укажите короткое имя!");
                return;
            }
            if(FileNames.Length == 0)
            {
                MessageBox.Show("Выберите фотографии!");
                return;
            }
            var res = await VKPhotoUploader.UploadImages(ScreenNameTBox.Text, FileNames, MessageTBox.Text);
            MessageBox.Show(res == "OK" ? "Фотографии успешно загружены" : res);
        }

    }
}
