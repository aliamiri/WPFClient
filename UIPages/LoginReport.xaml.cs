using System.Windows;

namespace WpfNotifierClient.UIPages
{
    /// <summary>
    /// Interaction logic for LoginReport.xaml
    /// </summary>
    public partial class LoginReport : Window
    {
        public LoginReport()
        {
            InitializeComponent();
            var connection = new DbConnection();
            var users = connection.SelectAllUsers();
            DgLoginInfo.ItemsSource = users;
        }
    }
}
