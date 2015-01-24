using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using NLog;
using WpfNotifierClient.Domains;


namespace WpfNotifierClient
{
    class DbConnection
    {
        private static SQLiteConnection _connection;

        readonly Logger _logger = LogManager.GetCurrentClassLogger();
        
        public void CreateConnection()
        {
            _logger.Info("start to create a connection");
            try
            {
                _logger.Info("get address of AppData folder");
                var localAppData =
                      Environment.GetFolderPath(
                      Environment.SpecialFolder.LocalApplicationData);
                
                var userFilePath
                  = Path.Combine(localAppData, "AsanPardakht");

                _logger.Info("create a directory with our name in AppData folder");
                if (!Directory.Exists(userFilePath))
                    Directory.CreateDirectory(userFilePath);

                var destFilePath = Path.Combine(userFilePath, "persianSwitch.db");

                if (!File.Exists(destFilePath))
                {
                    _connection = new SQLiteConnection("data source=" + destFilePath + ";Version=3");
                    _connection.SetPassword("weAreThe733WeTheChampions");
                    _connection.Close();
                }
                _connection = new SQLiteConnection("data source=" + destFilePath + ";Version=3;Password=weAreThe733WeTheChampions");

                CheckDbExistance();
            }
            catch (Exception exception)
            {
                _logger.Error("an error occured" +  exception.Message);
                throw;
            }
        }

        private void CheckDbExistance()
        {
            _connection.Open();

            var sqliteCmd = _connection.CreateCommand();

            sqliteCmd.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name='asanTrxInfo';";

            var sqliteDatareader = sqliteCmd.ExecuteReader();

            _logger.Info("check if out database exist or not");
            if (!sqliteDatareader.HasRows)
            {
                _logger.Info("if it is a first time create our database");
                sqliteDatareader.Close();
                sqliteCmd.CommandText =
                    "CREATE TABLE asanTrxInfo(id integer primary key, TrxDate DateTime,CardNo varchar(20),Amount integer );";
                sqliteCmd.ExecuteNonQuery();
            }

            if (!sqliteDatareader.IsClosed)
            {
                sqliteDatareader.Close();
            }

            sqliteCmd.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name='asanLocalUsers';";

            sqliteDatareader = sqliteCmd.ExecuteReader();

            _logger.Info("check if out database exist or not");
            if (!sqliteDatareader.HasRows)
            {
                _logger.Info("if it is a first time create our database");
                sqliteDatareader.Close();
                sqliteCmd.CommandText =
                    "CREATE TABLE asanLocalUsers(id integer primary key, username varchar(20) not null unique,password varchar(20),accessLevel integer,lastLogin DateTime);";
                sqliteCmd.ExecuteNonQuery();
            }

                        if (!sqliteDatareader.IsClosed)
            {
                sqliteDatareader.Close();
            }
            _connection.Close();
        }

        public void InsertTrxInDb(TrxInfo info)
        {
            try
            {
                _logger.Trace("insert data : " +  info.Details);
                _connection.Open();
                var date = info.TrxDate.ToDateTime().ToString("yyy-MM-dd HH:mm:ss");
                var query = "INSERT INTO asanTrxInfo (TrxDate,CardNo,Amount) VALUES('" + date + "','" + info.CardNo + "'," + info.Amount + ")";
                var command = _connection.CreateCommand();
                command.CommandText = query;
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                _logger.Error("an exception occured : " + e.Message);
            }
            finally
            {
                _connection.Close();
            }
        }

        public void InsertUserInDb(User user)
        {
            try
            {
                _logger.Trace("insert user : " + user.UserName);
                _connection.Open();
                var query = "INSERT INTO asanLocalUsers (username,password,accessLevel) VALUES('" + user.UserName + "','" + user.Password + "'," + user.AccessLevel+ ")";
                var command = _connection.CreateCommand();
                command.CommandText = query;
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                _logger.Error("an exception occured : " + e.Message);
            }
            finally
            {
                _connection.Close();
            }
        }

        public List<TrxInfo> SelectTrxFromDb(DateTime start, DateTime end)
        {
            try
            {
                _connection.Open();
                var startDate = start.ToString("yyy-MM-dd HH:mm:ss");
                var endDate = end.ToString("yyy-MM-dd HH:mm:ss");
                var query = "SELECT * FROM asanTrxInfo where TrxDate>'" + startDate + "' and TrxDate<'" + endDate + "';";
                var command = _connection.CreateCommand();
                command.CommandText = query;
                var sqLiteDataReader = command.ExecuteReader();

                var retList = new List<TrxInfo>();
                while (sqLiteDataReader.Read())
                {
                    var info = new TrxInfo
                    {
                        TrxDate = new PersianDateTime(Convert.ToDateTime(sqLiteDataReader.GetString(1))),
                        CardNo = sqLiteDataReader.GetString(2),
                        Amount = sqLiteDataReader.GetInt32(3)
                    };
                    retList.Add(info);
                }
                sqLiteDataReader.Close();
                return retList;
            }
            catch (Exception e)
            {
                _logger.Error("an exception occured : " + e.Message);
                return null;
            }
            finally
            {
                _connection.Close();
            }
        }

        public User SelectUserFromDb(string userName)
        {
            try
            {
                _connection.Open();
                var query = "SELECT * FROM asanLocalUsers where username='" + userName+ "';";
                var command = _connection.CreateCommand();
                command.CommandText = query;
                var sqLiteDataReader = command.ExecuteReader();
                
                while (sqLiteDataReader.Read())
                {
                    var user = new User
                    {
                        UserName = sqLiteDataReader.GetString(1),
                        Password = sqLiteDataReader.GetString(2),
                        AccessLevel = sqLiteDataReader.GetInt32(3)
                    };
                    sqLiteDataReader.Close();
                    return user;

                }
                sqLiteDataReader.Close();
                return null;
            }
            catch (Exception e)
            {
                _logger.Error("an exception occured : " + e.Message);
                return null;
            }
            finally
            {   
                _connection.Close();
            }
        }

        public void UpdateLastLogin(string username)
        {
            try
            {
                _logger.Trace("update user login date : " + username);
                _connection.Open();
                var loginDate = DateTime.Now.ToString("yyy-MM-dd HH:mm:ss");
                var query = "UPDATE asanLocalUsers Set lastLogin = '" + loginDate + "' where 1 = 1";// username = '" + username + "';";
                var command = _connection.CreateCommand();
                command.CommandText = query;
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                _logger.Error("an exception occured : " + e.Message);
            }
            finally
            {
                _connection.Close();
            }
        }
    }
}
