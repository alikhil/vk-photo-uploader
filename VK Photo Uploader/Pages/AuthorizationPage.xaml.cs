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

namespace VK_Photo_Uploader.Pages
{
    /// <summary>
    /// Логика взаимодействия для AuthorizationPage.xaml
    /// </summary>
    public partial class AuthorizationPage : Page
    {
        public AuthorizationPage()
        {
            InitializeComponent();
        }
        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            
            Cursor = Cursors.Wait;
            StatusLabel.Content = "Авторизация...";
            Thread owner = Thread.CurrentThread;
            string login = LoginTBox.Text, pass = PasswordTBox.Password;
            Thread t = new Thread(new ThreadStart(() => {
                var auth = VKPhotoUploader.Authorize(login, pass);
                Dispatcher.Invoke(() =>
                {
                    Cursor = Cursors.Arrow;
                    StatusLabel.Content = !auth ? "Неправильный пароль или логин" : "";
                    if (auth)
                    {
                        UploadPage UploadP = new UploadPage();
                        NavigationService.Navigate(UploadP);
                    }
                    else
                    {
                        PasswordTBox.Password = "";
                    }
                });
                
               

            }));
            t.Start();
        }
    }
}
