using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace WpfNotifierClient
{
    class DbConnection
    {
        private static SqlConnection _connection;

        //TODO dont forget NLog

        public void CreateConnection()
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["DbAddr"].ConnectionString;
                _connection =
                    new SqlConnection(connectionString);
            }
            catch (SqlException)
            {
                
            }
        }

        public void InsertInDb(TrxInfo info)
        {
            try
            {
                _connection.Open();

                String query = "INSERT INTO TrxInfo (TrxDate,CardNo,Amount) VALUES(@TrxDate,@CardNo,@Amount)";

                SqlCommand command = new SqlCommand(query, _connection);

                command.Parameters.Add("@CardNo", SqlDbType.VarChar);
                command.Parameters["@CardNo"].Value = info.GetCardNo();
                command.Parameters.Add("@TrxDate", SqlDbType.DateTime);
                command.Parameters["@TrxDate"].Value = info.GetTrxDate();
                command.Parameters.Add("@Amount", SqlDbType.Int);
                command.Parameters["@Amount"].Value = info.GetAmount();
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {

            }
            finally
            {
                _connection.Close();
            }
        }

        public void CheckDbExistance()
        {
            //TODO do some stuff here :D
        }

        public void CreateDb()
        {
            //TODO must change the name and drives and ...
            String str;
            SqlConnection myConn = new SqlConnection(@"Server=localhost\sqlexpress;Database=master;Trusted_Connection=True;");

            str = "CREATE DATABASE PersianSwitch ON PRIMARY " +
                "(NAME = PersianSwitch_Data, " +
                "FILENAME = 'C:\\PersianSwitchData.mdf', " +
                "SIZE = 2MB, MAXSIZE = 10MB, FILEGROWTH = 10%) " +
                "LOG ON (NAME = MyDatabase_Log, " +
                "FILENAME = 'C:\\PersianSwitchLog.ldf', " +
                "SIZE = 1MB, " +
                "MAXSIZE = 5MB, " +
                "FILEGROWTH = 10%)";


            SqlCommand myCommand = new SqlCommand(str, myConn);
            try
            {
                myConn.Open();
                myCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
            }
            finally
            {
                if (myConn.State == ConnectionState.Open)
                {
                    myConn.Close();
                }
            }
        }

    }
}
