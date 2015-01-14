﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Windows;
using System.Data.SQLite;
using System.IO;
using NLog;


namespace WpfNotifierClient
{
    class DbConnection
    {
        private static SQLiteConnection _connection;

        Logger _logger = LogManager.GetCurrentClassLogger();
        
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

                _connection = new SQLiteConnection("data source=" + destFilePath);

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
            _connection.Close();
        }

        public void InsertInDb(TrxInfo info)
        {
            try
            {
                _logger.Trace("insert data : " +  info.Details);
                _connection.Open();
                var query = "INSERT INTO asanTrxInfo (TrxDate,CardNo,Amount) VALUES('" + info.TrxDate.ToDateTime() + "','" + info.CardNo + "'," + info.Amount + ")";
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

        public List<TrxInfo> SelectFromDb(DateTime start, DateTime end)
        {
            try
            {
                _connection.Open();
                var query = "SELECT * FROM asanTrxInfo where TrxDate>'" + start.Date + "' and TrxDate<'" + end.Date + "';";
                var command = _connection.CreateCommand();
                command.CommandText = query;
                var sqLiteDataReader = command.ExecuteReader();

                List<TrxInfo> retList = new List<TrxInfo>();
                while (sqLiteDataReader.Read())
                {
                    TrxInfo info = new TrxInfo();
                    info.TrxDate = new PersianDateTime(Convert.ToDateTime(sqLiteDataReader.GetString(1)));
                    info.CardNo = sqLiteDataReader.GetString(2);
                    info.Amount = sqLiteDataReader.GetInt32(3);
                    retList.Add(info);
                }

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
    }
}
