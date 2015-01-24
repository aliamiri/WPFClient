using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using WpfNotifierClient.Domains;

namespace WpfNotifierClient.UIPages
{
    /// <summary>
    /// Interaction logic for CreateUser.xaml
    /// </summary>
    public partial class CreateUser : Window
    {
        private DbConnection _connection;
        public CreateUser()
        {
            InitializeComponent();
            _connection = new DbConnection();
            AccessLevel.ItemsSource = new List<string>
            {
                "سطح یک",
                "سطح دوم"
            };
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            var user = _connection.SelectUserFromDb(TxtName.Text);
            if (user != null)
            {
                InfoTextBlock.Text = "نام کاربری تکراری ست.";
                CreateUserGrid.Background = Brushes.Crimson;
                return;
            }
            if (!TxtPassword.Password.Equals(TxtRePass.Password))
            {
                InfoTextBlock.Text = "رمز عبور و تکرارش باید یکسان باشند"; 
                CreateUserGrid.Background = Brushes.Crimson;
                return;
            }
            user = new User
            {
                UserName = TxtName.Text,
                Password = TxtPassword.Password,
                AccessLevel = AccessLevel.SelectedIndex
            };
            _connection.InsertUserInDb(user);
            MessageBox.Show("کاربر با موفقیت ساخته شد.");
            Close();
        }
    }
}
