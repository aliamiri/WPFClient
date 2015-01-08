using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using WpfApplication2;

namespace WpfNotifierClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

    
        private string ReadAsync()
        {
            Stream stm = tcpclnt.GetStream();
            ASCIIEncoding asen = new ASCIIEncoding();

            byte[] bb = new byte[100];

            int k = stm.Read(bb, 0, 100);
            StringBuilder stbldr = new StringBuilder();
            for (int i = 0; i < k; i++)
                stbldr.Append(Convert.ToChar(bb[i]));

            return stbldr.ToString();
        }

        public delegate void NextPrimeDelegate(); 

        List<TrxInfo> _trxInfos = new List<TrxInfo>();

        TaskbarNotifier _taskbarNotifier;
        //OleDbConnection con;
        private int _index = 0;
        private DbConnection _connection;
        TcpClient tcpclnt = new TcpClient();
        public MainWindow()
        {
            try
            {
                InitializeComponent();
                _connection = new DbConnection();
                _connection.CreateConnection();
            }
            catch (Exception exc)
            {
                MessageBox.Show("1. " + exc.Message);
            }
            _taskbarNotifier = new TaskbarNotifier();
            
            dgUsers.ItemsSource = _trxInfos;
        }

        public async void CheckNextNumber()
        {
            _index++;
            if (_index > 10)
            {
                _trxInfos.RemoveAt(0);
                _index--;
            }
            var slowTask = Task<string>.Factory.StartNew(() => ReadAsync());

            // for (int j = 0; j < 10; j++ )
            await slowTask;

            string retVal = slowTask.Result;

            TrxInfo info = new TrxInfo();
            info.SetAmount(100);
            info.SetTrxDate(DateTime.Now);
            info.SetCardNo(retVal);

            _trxInfos.Add(info);
            dgUsers.ItemsSource = null;
            dgUsers.ItemsSource = _trxInfos;
            _taskbarNotifier.Show("persian switch", retVal, 500, 1000, 500);

            _connection.InsertInDb(info);

            StartStopButton.Dispatcher.BeginInvoke(
                DispatcherPriority.SystemIdle,
                new NextPrimeDelegate(this.CheckNextNumber));

        }

        private void Start(object sender, RoutedEventArgs e)
        {
            tcpclnt.Connect("127.0.0.1", 2001);

            StartStopButton.Content = "Stop";
            StartStopButton.Dispatcher.BeginInvoke(
                DispatcherPriority.Normal,
                new NextPrimeDelegate(CheckNextNumber));

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            /*
            try
            {
                SqlConnection connection = new SqlConnection(@"Server=localhost\sqlexpress;Database=PersianSwitch;Trusted_Connection=True;");
                connection.Open();
                SqlCommand command = new SqlCommand("SELECT * FROM TrxInfo", connection);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    string name = reader.GetString(1);
                }
                command.Dispose();
                connection.Close();
                connection.Dispose();
            }
            catch (Exception ex)
            {

            }*/
        }
    }
}
