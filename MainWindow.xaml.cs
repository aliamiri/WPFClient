using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms.VisualStyles;
using System.Windows.Threading;

namespace WpfNotifierClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

    
        private string ReadAsync()
        {
            Stream stm = _tcpClient.GetStream();
            ASCIIEncoding asen = new ASCIIEncoding();

            byte[] bb = new byte[100];

            int k = stm.Read(bb, 0, 100);
            StringBuilder stbldr = new StringBuilder();
            for (int i = 0; i < k; i++)
                stbldr.Append(Convert.ToChar(bb[i]));

            return stbldr.ToString();
        }

        public delegate void NextPrimeDelegate();

        readonly List<TrxInfo> _trxInfos = new List<TrxInfo>();

        readonly TaskbarNotifier _taskbarNotifier;

        private int _index = 0;
        private readonly DbConnection _connection;
        readonly TcpClient _tcpClient = new TcpClient();

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

           // CheckFistTime();
        }


        private TrxInfo InputParser(string input)
        {
            var values = input.Split(new string[]{";"}, StringSplitOptions.None);
            var trxInfo = new TrxInfo();
            int intIn;
            if (int.TryParse(values[0],out intIn))
                trxInfo.SetAmount(intIn);
            DateTime timeIn;
            if (DateTime.TryParse(values[1], out timeIn))
                trxInfo.SetTrxDate(timeIn);
            trxInfo.SetCardNo(values[2]);
            return null;
        }

        private void CheckFistTime()
        {
            var appPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            var configFile = Path.Combine(appPath, "App.config");
            var isFirstTime = ConfigurationManager.AppSettings["isFirstTime"];
            if (isFirstTime.Equals("true"))
            {
                var configFileMap = new ExeConfigurationFileMap { ExeConfigFilename = configFile };
                Configuration config = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);
                config.AppSettings.Settings["isFirstTime"].Value = "false";
                config.Save(ConfigurationSaveMode.Full);    
                //TODO configuration for database
                //TODO show a window to set merchant name and this type of ...
            }
        }

        public async void AsyncFill()
        {
            _index++;
            if (_index > 10)
            {
                _trxInfos.RemoveAt(0);
                _index--;
            }
            var slowTask = Task<string>.Factory.StartNew(ReadAsync);
            
            await slowTask;

            string retVal = slowTask.Result;

            var info = InputParser(retVal);

            _trxInfos.Add(info);
            dgUsers.ItemsSource = null;
            dgUsers.ItemsSource = _trxInfos;
            _taskbarNotifier.Show("خرید جدید", info.Details, 500, 1000, 500);

            _connection.InsertInDb(info);

            StartStopButton.Dispatcher.BeginInvoke(
                DispatcherPriority.SystemIdle,
                new NextPrimeDelegate(this.AsyncFill));

        }

        private void Start(object sender, RoutedEventArgs e)
        {
            var serverIp = ConfigurationManager.AppSettings["serverIp"];
            var serverPort = int.Parse(ConfigurationManager.AppSettings["serverPort"]);

            MessageBox.Show(serverIp);
            _tcpClient.Connect(serverIp,serverPort);

            StartStopButton.Dispatcher.BeginInvoke(
                DispatcherPriority.Normal,
                new NextPrimeDelegate(AsyncFill));
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
