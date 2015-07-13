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
            string sql = "CREATE TABLE Videos (filename VARCHAR(40), path VARCHAR(256), lastPosition INT32, isNetwork BOOLEAN)";
            openConnection();
            SQLiteCommand command = new SQLiteCommand(sql, filesLibraryConnection);
            command.ExecuteNonQuery();
            closeConnection();
        }

        public void createMusicTable()
        {
            string sql = "CREATE TABLE Music (filename VARCHAR(40), path VARCHAR(256), lastPosition INT32, isNetwork BOOLEAN)";
            openConnection();
            SQLiteCommand command = new SQLiteCommand(sql, filesLibraryConnection);
            command.ExecuteNonQuery();
            closeConnection();
        }

        public void addNonNetworkVideos(string[] paths)
        {
            string sql = "INSERT INTO \'Videos\' (filename, path, lastPosition, isNetwork) VALUES (@name, @filepath, @position, @network)";
            openConnection();
            SQLiteTransaction transaction = filesLibraryConnection.BeginTransaction();
            foreach (string path in paths) {
                SQLiteCommand command = new SQLiteCommand(sql, filesLibraryConnection);
                //TODO: Find a way to get filename from the path
                command.Parameters.AddWithValue("@name", "name");
                command.Parameters.AddWithValue("@filepath",path);
                command.Parameters.AddWithValue("@position", 0);
                command.Parameters.AddWithValue("@network", 0);
                command.ExecuteNonQuery();
            }
            transaction.Commit();
            closeConnection();
        }

        public void addNetworkVideo(string path, string name)
        {
            string sql = "INSERT INTO \'Videos\' (filename, path, lastPosition, isNetwork) VALUES (@name, @filepath, @position, @network)";
            openConnection();
            SQLiteCommand command = new SQLiteCommand(sql, filesLibraryConnection);
            command.Parameters.AddWithValue("@name", name);
            command.Parameters.AddWithValue("@filepath", path);
            command.Parameters.AddWithValue("@position", 0);
            command.Parameters.AddWithValue("@network", 1);
            command.ExecuteNonQuery();
            closeConnection();
        }

        public List<MediaFile> getAllVideos()
        {
            string sql = "SELECT * FROM \'Videos\'";
            List<MediaFile> ret = new List<MediaFile>();
            openConnection();
            SQLiteCommand command = new SQLiteCommand(sql, filesLibraryConnection);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                MediaFile file = new MediaFile(reader.GetString(0), reader.GetString(1), reader.GetInt32(2), reader.GetBoolean(3), true);
                ret.Add(file);
            }
            closeConnection();
            return ret;
        }

        public List<MediaFile> getAllMusic()
        {
            string sql = "SELECT * FROM \'Music\'";
            List<MediaFile> ret = new List<MediaFile>();
            openConnection();
            SQLiteCommand command = new SQLiteCommand(sql, filesLibraryConnection);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                MediaFile file = new MediaFile(reader.GetString(0), reader.GetString(1), reader.GetInt32(2), reader.GetBoolean(3), false);
                ret.Add(file);
            }
            closeConnection();
            return ret;
        }

        public void setVideoLastPosition(int seconds, string path)
        {
            string sql = "UPDATE videos SET lastPosition=" + seconds + "WHERE path=\'" + path + "\'";
            openConnection();
            SQLiteCommand command = new SQLiteCommand(sql, filesLibraryConnection);
            command.ExecuteNonQuery();
            closeConnection();
        }

        public void setMusicLastPosition(int seconds, string path)
        {
            string sql = "UPDATE Music SET lastPosition=" + seconds + "WHERE path=\'" + path + "\'";
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
