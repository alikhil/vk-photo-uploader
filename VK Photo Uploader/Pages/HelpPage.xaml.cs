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
    /// Логика взаимодействия для HelpPage.xaml
    /// </summary>
    public partial class HelpPage : Page
    {
        public HelpPage()
        {
            InitializeComponent();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void GetIdBtn_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;
            string screenName = ScreenNameTBox.Text;
            if(String.IsNullOrEmpty(ScreenNameTBox.Text))
            {
                MessageBox.Show("Укажите короткое имя");
                return;
            }
            Thread t = new Thread(new ThreadStart(() => {
                var ownerId = VKPhotoUploader.GetOwnerId(screenName);
                Dispatcher.Invoke(() => {
                    OwnerIdTBox.IsEnabled = true;
                    OwnerIdTBox.Text = ownerId;
                    Cursor = Cursors.Arrow;
                });
            }));
            t.Start();
        }
    }
}
