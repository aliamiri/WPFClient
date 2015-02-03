using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using NLog;
using WpfNotifierClient.Domains;
using Timer = System.Timers.Timer;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Triggers;
using ICSharpCode.SharpZipLib.Zip;

namespace WpfNotifierClient.UIPages
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {

        #region 

        public delegate void ReadTcpDelegate();

        private readonly List<TrxInfo> _trxInfos = new List<TrxInfo>();

        private readonly TaskbarNotifier _taskbarNotifier;

        private int _index = 0;
        private readonly DbConnection _connection;
        private readonly TcpClient _tcpClient = new TcpClient();
        private string _bufferStrings = "";
        public static bool ConnectionFlag = false;
        public static int AccessLevel = 0;

        private Logger _logger = LogManager.GetCurrentClassLogger();

        #endregion


        //currently this function is useless but I'll use this function in the future
        private void CheckFistTime()
        {
            var appPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            if (appPath == null) return;
            var configFile = Path.Combine(appPath, "App.config");
            var isFirstTime = ConfigurationManager.AppSettings["isFirstTime"];
            if (!isFirstTime.Equals("true")) return;
            var configFileMap = new ExeConfigurationFileMap { ExeConfigFilename = configFile };
            Configuration config = ConfigurationManager.OpenMappedExeConfiguration(configFileMap,
                ConfigurationUserLevel.None);
            config.AppSettings.Settings["isFirstTime"].Value = "false";
            config.Save(ConfigurationSaveMode.Full);
            //TODO configuration for database
            //TODO show a window to set merchant name and this type of ...
        }

        public MainWindow()
        {
            _logger.Info("Program starts");
            try
            {
                _logger.Info("Initilization");

                _connection = new DbConnection();
                _connection.CreateConnection();

                new LoginForm().ShowDialog();
                if (AccessLevel == -1)
                {
                    Close();
                    return;
                }
                InitializeComponent();
                if (AccessLevel < 5)
                {
                    CreateUserItem.Visibility = Visibility.Collapsed;
                    LoginReportItem.Visibility = Visibility.Collapsed;
                }
                ReconnectButton.Visibility = Visibility.Hidden;
                StartTcp();
            }
            catch (Exception exc)
            {
                _logger.Trace("initilization :" + exc.Message);
            }
            _taskbarNotifier = new TaskbarNotifier();

            DgTrxInfo.ItemsSource = _trxInfos;
            // CheckFistTime();
        }

        #region 
        //This region is for sending log files data to server side

        public void UploadFtpFile()
        {
            var fileName = Path.Combine(Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData), @"asan\logs\AsanPardakhtLogs.zip");
            try
            {
                var pureFileName = new FileInfo(fileName).Name;
                var uploadUrl = String.Format("ftp://{0}/{1}/{2}", "localhost", "uploadHere", pureFileName);
                var req = (FtpWebRequest) FtpWebRequest.Create(uploadUrl);
                req.Proxy = null;
                req.Method = WebRequestMethods.Ftp.UploadFile;
                //req.Credentials = new NetworkCredential("a.akhondian", "Aa123456%");
                req.UseBinary = true;
                req.UsePassive = true;
                //req.UseDefaultCredentials = true;
                byte[] data = File.ReadAllBytes(fileName);
                req.ContentLength = data.Length;
                Stream stream = req.GetRequestStream();
                stream.Write(data, 0, data.Length);
                stream.Close();
            }
            catch (Exception ex)
            {
                _logger.Error("UploadFtpFile:", ex.Message);
            }
        }

        private void ZipLogFiles()
        {
            var logFolder = Path.Combine(Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData), @"asan\logs");
            var zipFileAddr = Path.Combine(Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData), @"asan\logs\AsanPardakhtLogs.zip");
            string[] args = {logFolder, zipFileAddr};
            var filenames = Directory.GetFiles(args[0]);
            using (var s = new ZipOutputStream(File.Create(args[1])))
            {
                s.SetLevel(9); // 0 - store only to 9 - means best compression
                var buffer = new byte[4096];
                foreach (var file in filenames)
                {
                    if (!file.EndsWith(".log")) continue;
                    var entry = new ZipEntry(Path.GetFileName(file)) {DateTime = DateTime.Now};
                    s.PutNextEntry(entry);
                    using (var fs = File.OpenRead(file))
                    {
                        int sourceBytes;
                        do
                        {
                            sourceBytes = fs.Read(buffer, 0, buffer.Length);
                            s.Write(buffer, 0, sourceBytes);
                        } while (sourceBytes > 0);
                    }
                }
                s.Finish();
                s.Close();
            }
        }

        private void UploadLogFiles()
        {
            ZipLogFiles();
            UploadFtpFile();
        }

        #endregion


        #region 
        //This region is for TCP connection and reading data and commands from server

        private InputContainer InputParser()
        {
            var container = new InputContainer();
            try
            {
                var indexOf = _bufferStrings.IndexOf('|');
                if (indexOf == -1) return null;

                var input = _bufferStrings.Substring(0, indexOf - 1);
                var startsWithC = input.StartsWith("c");
                _bufferStrings = _bufferStrings.Remove(1, indexOf + 3);
                var values = input.Split(new[] {";"}, StringSplitOptions.None);
                if (startsWithC)
                {
                    var commands = new Commands();
                    int intIn;
                    if (int.TryParse(values[0], out intIn))
                        commands.Priority = intIn;
                    if (int.TryParse(values[1], out intIn))
                        commands.Command = (CommandTypes) Enum.ToObject(typeof (CommandTypes), intIn);
                    container.Command = commands;
                    container.Type = true;
                    return container;
                }
                else
                {
                    var trxInfo = new TrxInfo();
                    int intIn;
                    if (int.TryParse(values[0], out intIn))
                        trxInfo.Amount = intIn;
                    trxInfo.TrxDate = new PersianDateTime(DateTime.Parse(values[1]));
                    trxInfo.CardNo = values[2];
                    _logger.Trace("result of parse is : " + trxInfo.Details);
                    container.Info = trxInfo;
                    container.Type = false;
                    return container;
                }
            }
            catch (Exception e)
            {
                _logger.Error("parser error : " + e.Message);
                return null;
            }
        }

        //TODO check redundant data

        private string ReadAsync()
        {
            while (ConnectionFlag)
            {
                _logger.Trace("we start reading from tcp connection");
                var stm = _tcpClient.GetStream();

                var bb = new byte[100];

                var k = stm.Read(bb, 0, 100);
                var stbldr = new StringBuilder();
                for (var i = 0; i < k; i++)
                    stbldr.Append(Convert.ToChar(bb[i]));

                var readAsync = stbldr.ToString();

                _bufferStrings += readAsync;
                _logger.Trace("result is : " + readAsync);
            }
            return null;
        }

        public string AddToListsAsync()
        {
            while (ConnectionFlag)
            {
                var inputContainer = InputParser();
                if (inputContainer != null)
                {
                    var trxInfo = inputContainer.Info;
                    if (trxInfo != null)
                    {
                        _index++;
                        if (_index > 10)
                        {
                            _trxInfos.RemoveAt(0);
                            _index--;
                        }
                        _logger.Trace("show the result to user");
                        _trxInfos.Add(trxInfo);
                        DgTrxInfo.ItemsSource = null;
                        DgTrxInfo.ItemsSource = _trxInfos;
                        _taskbarNotifier.Show("خرید جدید", trxInfo.Details, 500, 1000, 500);
                        _connection.InsertTrxInDb(trxInfo);
                    }
                    var commands = inputContainer.Command;
                    if (commands != null)
                    {
                        if (commands.Command == CommandTypes.SendLogToServer)
                            UploadLogFiles();
                        if (commands.Command == CommandTypes.LogOffApplication)
                            SignOut();
                        //TODO consider other commands that may exist :)
                    }
                    _logger.Trace("call me again and again till the end of the time :D");
                }
                Thread.Sleep(100);
            }
            return null;
        }

        public async void AsyncCalls()
        {
            try
            {
                _logger.Trace("start of AsyncCalls, keep track of index to just show 10 last results");
                var readAsync = Task<string>.Factory.StartNew(ReadAsync);
                await readAsync;
                var addTolistAsync = Task<string>.Factory.StartNew(AddToListsAsync);
                await addTolistAsync;
            }
            catch (Exception exception)
            {
                _logger.Error("an exception occured : " + exception.Message);
            }
        }

        private void StartTcp()
        {
            var serverIp = ConfigurationManager.AppSettings["serverIp"];
            var serverPort = int.Parse(ConfigurationManager.AppSettings["serverPort"]);
            _logger.Info("before connecting to TCP with address : " + serverIp + " on port : " + serverPort);
            try
            {
                if (!_tcpClient.Connected)
                    _tcpClient.Connect(serverIp, serverPort);
                _logger.Info("successfully connected to TCP, start reading data");
                AutoCheckItem.Visibility = Visibility.Collapsed;
                AsyncCalls();
            }
            catch (SocketException ex)
            {
                ConnectionFlag = false;
                _logger.Error("TCP Connection failed with this error : " + ex.Message);
                MessageBox.Show("امکان ارتباط با سرور برقرار نمیباشد. اتصال شبکه خود را چک کنید.");
                MessageBox.Show("از برنامه در حالت آفلاین استفاده خواهید کرد");
                ReconnectButton.Visibility = Visibility.Visible;
            }
        }

        #endregion


        private void Reconnect_Click(object sender, RoutedEventArgs e)
        {
            ReconnectButton.Visibility = Visibility.Hidden;
            ConnectionFlag = true;
            StartTcp();
        }

        #region 
        //Menu Items

        private void MenuItem_lock(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Collapsed;
            LoginForm.CloseAble = false;
            new LoginForm().ShowDialog();
            if (AccessLevel == -1) Close();
            if (AccessLevel < 5)
            {
                CreateUserItem.Visibility = Visibility.Collapsed;
                LoginReportItem.Visibility = Visibility.Collapsed;
            }
            Visibility = Visibility.Visible;
        }

        private void MenuItem_IntervalReport(object sender, RoutedEventArgs e)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo(1065);
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;
            new IntervalReportWindow().Show();
        }

        private void SignOut()
        {
            ConnectionFlag = false;
            Visibility = Visibility.Collapsed;
            new LoginForm().ShowDialog();
            if (AccessLevel == -1) Close();
            Visibility = Visibility.Visible;
            if (AccessLevel < 5)
            {
                CreateUserItem.Visibility = Visibility.Collapsed;
                LoginReportItem.Visibility = Visibility.Collapsed;
            }
            StartTcp();
        }

        private void MenuItem_signOut(object sender, RoutedEventArgs e)
        {
            SignOut();
        }

        private void MenuItem_createNewUser(object sender, RoutedEventArgs e)
        {
            new CreateUser().Show();
        }

        private void MenuItem_LoginReport(object sender, RoutedEventArgs e)
        {
            new LoginReport().Show();
        }

        private void MenuItem_AutomaticCheck(object sender, RoutedEventArgs e)
        {
            ConnectAutomaticly();
        }

        #endregion


        public delegate void ConnectTcpDelegate();

        private void ConnectAutomaticly()
        {
            // Create a timer with a two second interval.
//            _aTimer = new Timer(10000);
            // Hook up the Elapsed event for the timer. 
//            _aTimer.Elapsed += OnTimedEvent;
//            _aTimer.Enabled = true;
//            _b = AutoCheckItem.IsChecked;

            ISchedulerFactory schedulerFactory = new StdSchedulerFactory();
            _scheduler = schedulerFactory.GetScheduler();
            _scheduler.Start();
            MessageBox.Show("Starting Scheduler");

            AddJob();
        }

        private static IScheduler _scheduler;
        private bool _b;

        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            //if (!ConnectionFlag && _b)
            //    StartTcp();
            //else
            //    _aTimer.Enabled = false;

        }

        public static void AddJob()
        {
            IMyJob myJob = new MyJob(); //This Constructor needs to be parameterless
            var jobDetail = new JobDetailImpl("Job1", "Group1", myJob.GetType());
            var trigger = new CronTriggerImpl("Trigger1", "Group1", "0/30 * 8-17 * * ?");
                //run every minute between the hours of 8am and 5pm
            _scheduler.ScheduleJob(jobDetail, trigger);
        }

        protected override void OnClosed(EventArgs e)
        {
            if(_scheduler != null )
                if(_scheduler.IsStarted)
                    _scheduler.Shutdown();
            base.OnClosed(e);
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    internal class MyJob : IMyJob
    {
        public void Execute(IJobExecutionContext context)
        {
            MessageBox.Show("In MyJob class");
            DoMoreWork();
        }

        public void DoMoreWork()
        {
            MessageBox.Show("Do More Work");
        }
    }

    internal interface IMyJob : IJob
    {
    }

}
