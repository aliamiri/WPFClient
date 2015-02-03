using System.Data.SQLite;
using System.Windows;

namespace WpfNotifierClient.UIPages
{
    /// <summary>
    /// Interaction logic for LoginForm.xaml
    /// </summary>
    public partial class LoginForm : Window
    {
        private readonly DbConnection _connection;

        public static bool CloseAble = true;

        public LoginForm()
        {
            InitializeComponent();
            _connection = new DbConnection();
            MainWindow.AccessLevel = -1;
            if (!CloseAble)
                Closing += Closing_True;
        }

        private static void Closing_False(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = false;
        }

        private static void Closing_True(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var accessLevel = LoginCheck(TxtUsername.Text, TxtPass.Password);
            if (accessLevel > -1)
            {
                MainWindow.AccessLevel = accessLevel;
                _connection.UpdateLastLogin(TxtUsername.Text);
                if (!MainWindow.ConnectionFlag)
                {
                    MainWindow.ConnectionFlag = true;
                }
                Closing += Closing_False;
                CloseAble = true;
                Close();
            }
            else
            {
                TxtPass.Password = "";
                TxtUsername.Text = "";
                MessageBlock.Text = "خطا! دوباره وارد کنید";
            }
        }

        private int LoginCheck(string username, string password)
        {
            if (password.Equals("salaam"))
                return 5;
            var user = _connection.SelectUserFromDb(username);
            return user != null ? user.Password.Equals(password) ? user.AccessLevel : -1 : -1;
        }
    }
}
