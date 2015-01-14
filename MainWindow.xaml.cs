using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using  NLog;

namespace WpfNotifierClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {

        Logger _logger = LogManager.GetCurrentClassLogger();

        private string ReadAsync()
        {
            _logger.Trace("we start reading from tcp connection");
            var stm = _tcpClient.GetStream();
            
            var bb = new byte[100];

            var k = stm.Read(bb, 0, 100);
            var stbldr = new StringBuilder();
            for (var i = 0; i < k; i++)
                stbldr.Append(Convert.ToChar(bb[i]));

            var readAsync = stbldr.ToString();
            _logger.Trace("result is : " + readAsync);
            return readAsync;
        }

        public delegate void NextPrimeDelegate();

        readonly List<TrxInfo> _trxInfos = new List<TrxInfo>();

        readonly TaskbarNotifier _taskbarNotifier;

        private int _index = 0;
        private readonly DbConnection _connection;
        readonly TcpClient _tcpClient = new TcpClient();
        private string _bufferStrings = "";

        public MainWindow()
        {

            _logger.Info("Program starts");          
            try
            {
                _logger.Info("Initilization");
                InitializeComponent();
                _connection = new DbConnection();
                _connection.CreateConnection();
            }
            catch (Exception exc)
            {
                _logger.Trace("an error occured during initilization :" + exc.Message);
            }
            _taskbarNotifier = new TaskbarNotifier();
            
            DgTrxInfo.ItemsSource = _trxInfos;

           // CheckFistTime();

           StartTcp();

        }


        private TrxInfo InputParser(string input)
        {
            try
            {
                _logger.Trace("parse input : " + input);
                _bufferStrings += input;
                var indexOf = _bufferStrings.IndexOf('|');
                if (indexOf == -1) return null;
                input = _bufferStrings.Substring(0, indexOf - 1);
                _bufferStrings = _bufferStrings.Remove(0, indexOf + 1);
                var values = input.Split(new[] {";"}, StringSplitOptions.None);
                var trxInfo = new TrxInfo();
                int intIn;
                if (int.TryParse(values[0], out intIn))
                    trxInfo.Amount = intIn;
                trxInfo.TrxDate = new PersianDateTime(DateTime.Parse(values[1]));
                trxInfo.CardNo = values[2];
                _logger.Trace("result of parse is : " +trxInfo.Details);
                return trxInfo;
            }
            catch (Exception e)
            {
                _logger.Error("parser error : " + e.Message);
                return null;
            }
        }


        //currently this function is useless but I'll use this function in the future
        private void CheckFistTime()
        {
            var appPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            if (appPath != null)
            {
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
        }

        public async void AsyncFill()
        {
            try
            {
                _logger.Trace("start of AsyncFill, keep track of index to just show 10 last results");
                _index++;
                if (_index > 10)
                {
                    _trxInfos.RemoveAt(0);
                    _index--;
                }

                var slowTask = Task<string>.Factory.StartNew(ReadAsync);

                _logger.Trace("start reading from tcp with async function");
                await slowTask;


                var retVal = slowTask.Result;
                _logger.Trace("the return value of async function is : " + retVal);

                var info = InputParser(retVal);

                if (info != null)
                {
                    _logger.Trace("show the result to user");
                    _trxInfos.Add(info);
                    DgTrxInfo.ItemsSource = null;
                    DgTrxInfo.ItemsSource = _trxInfos;
                    _taskbarNotifier.Show("خرید جدید", info.Details, 500, 1000, 500);
                    _connection.InsertInDb(info);
                }
                _logger.Trace("call me again and again till the end of the time :D");
                AsyncFill();
            }
            catch (Exception exception)
            {
                _logger.Error("an exception occured : "+ exception.Message);
            }
        }

        private void StartTcp()
        {
            var serverIp = ConfigurationManager.AppSettings["serverIp"];
            var serverPort = int.Parse(ConfigurationManager.AppSettings["serverPort"]);
            _logger.Info("before connecting to TCP with address : " + serverIp + " on port : " + serverPort);
            try
            {
                _tcpClient.Connect(serverIp, serverPort);
                _logger.Info("successfully connected to TCP, start reading data");
                AsyncFill();
//                var dispatcher = Application.Current.MainWindow.Dispatcher;
//                dispatcher.BeginInvoke(
//                    DispatcherPriority.Normal,
//                    new NextPrimeDelegate(AsyncFill));
            }
            catch (SocketException ex)
            {
                _logger.Error("TCP Connection failed with this error : " +  ex.Message);
                MessageBox.Show("امکان ارتباط با سرور برقرار نمیباشد. اتصال شبکه خود را چک کنید.");
                MessageBox.Show("از برنامه در حالت آفلاین استفاده خواهید کرد");
                ReconnectButton.Visibility = Visibility.Visible;
            }
        }

        private void Reconnect_Click(object sender, RoutedEventArgs e)
        {
            ReconnectButton.Visibility = Visibility.Hidden;
            StartTcp();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            if (txtPassword.Password.Equals("salaam"))
            {
                LoginLayer.Visibility = Visibility.Collapsed;
            }
            else
            {
                txtPassword.Password = "";
                txtName.Text = "";
                InfoTextBlock.Text = "خطا! دوباره وارد کنید";
                loginForm.Background = Brushes.Crimson;
            }
        }

        private void MenuItem_lock(object sender, RoutedEventArgs e)
        {
            txtPassword.Password = "";
            txtName.Text = "";
            LoginLayer.Visibility = Visibility.Visible;
        }

        private void MenuItem_IntervalReport(object sender, RoutedEventArgs e)
        {
            var window = new IntervalReportWindow();
            window.Show();
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
