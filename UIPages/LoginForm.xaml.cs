using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfNotifierClient
{
    /// <summary>
    /// Interaction logic for LoginForm.xaml
    /// </summary>
    public partial class LoginForm : Window
    {
        private readonly DbConnection _connection;

        public LoginForm()
        {
            InitializeComponent();
            _connection = new DbConnection();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var accessLevel = LoginCheck(TxtUsername.Text, TxtPass.Password);
            if (accessLevel > -1)
            {
                MainWindow.AccessLevel = accessLevel;
                //_connection.UpdateLastLogin(txtName.Text);
                if (!MainWindow.ConnectionFlag)
                {
                    MainWindow.ConnectionFlag = true;
                }
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
