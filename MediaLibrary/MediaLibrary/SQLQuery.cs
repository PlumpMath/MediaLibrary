using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace MediaLibrary
{
    class SQLQuery
    {
        SQLiteConnection filesLibraryConnection;
        static string DataBaseName;

        public SQLQuery(string DBName) {
            DataBaseName = DBName;
            filesLibraryConnection = new SQLiteConnection("Data Source=" + DataBaseName + ";Version=3;");
        }

        public void createDB()
        {
            SQLiteConnection.CreateFile(DataBaseName);
        }

        public bool checkTableExists(string tableName)
        {
            string sql = "SELECT name FROM sqlite_master WHERE type=\'table\' AND name=\'" + tableName + "\'";
            openConnection();
            SQLiteCommand command = new SQLiteCommand(sql, filesLibraryConnection);
            SQLiteDataReader reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                closeConnection();
                return true;
            }
            else
            {
                closeConnection();
                return false;
            }

        }

        public void createVersionTable()
        {
            string sql = "CREATE TABLE Version (number INT32)";
            openConnection();
            SQLiteCommand command = new SQLiteCommand(sql, filesLibraryConnection);
            command.ExecuteNonQuery();
            closeConnection();
        }

        public void setTableVersion(int version)
        {
            // Clear any data that isnt the current version
            string sql = "DELETE FROM Version WHERE number != " + version;
            openConnection();
            SQLiteCommand command = new SQLiteCommand(sql, filesLibraryConnection);
            command.ExecuteNonQuery();

            sql = "SELECT number FROM Version order by number desc";
            command = new SQLiteCommand(sql, filesLibraryConnection);
            SQLiteDataReader reader = command.ExecuteReader();
            if (reader.HasRows)
                //there is a row with the current version, we can return
                return;
            else
            {
                sql = "INSERT INTO Version (number) VALUES (" + version + ")";
                command = new SQLiteCommand(sql, filesLibraryConnection);
                command.ExecuteNonQuery();
            }
            closeConnection();
        }

        public int getDBVersion()
        {
            string sql = "SELECT number FROM Version order by number desc";
            openConnection();
            SQLiteCommand command = new SQLiteCommand(sql, filesLibraryConnection);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                // should only be one row in the table with the table version
                int version = reader.GetInt32(0);
                closeConnection();
                return version;
            }
            // if we get here there was no data to be read, so something was really wrong on create
            closeConnection();
            return -1;
        }

        public void createVideosTable()
        {
            string sql = "CREATE TABLE Videos (filename VARCHAR(40), path VARCHAR(256), lastPosition INT32)";
            openConnection();
            SQLiteCommand command = new SQLiteCommand(sql, filesLibraryConnection);
            command.ExecuteNonQuery();
            closeConnection();
        }

        public void createMusicTable()
        {
            string sql = "CREATE TABLE Music (filename VARCHAR(40), path VARCHAR(256), lastPosition INT32)";
            openConnection();
            SQLiteCommand command = new SQLiteCommand(sql, filesLibraryConnection);
            command.ExecuteNonQuery();
            closeConnection();
        }

        private void openConnection()
        {
            filesLibraryConnection.Open();
        }

        private void closeConnection()
        {
            filesLibraryConnection.Close();
        }

    }
}
