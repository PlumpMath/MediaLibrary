using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;

namespace MediaLibrary
{
    public partial class Form1 : Form
    {
        AXVLC.VLCPlugin alxplugin1 = new AXVLC.VLCPlugin();
        SQLiteConnection filesLibraryConnection;
        const string DBName = "LibraryFiles.sqlite";
        const int DBVersion = 1;
        public Form1()
        {
            InitializeComponent();
            if (!System.IO.File.Exists(DBName))
            {
                SQLiteConnection.CreateFile(DBName);
            }
            filesLibraryConnection = new SQLiteConnection("Data Source="+ DBName + ";Version=3;");
            filesLibraryConnection.Open();
            string commandString = "SELECT name FROM sqlite_master WHERE type=\'table\' AND name=\'Version\'";
            SQLiteCommand command = new SQLiteCommand(commandString, filesLibraryConnection);
            SQLiteDataReader reader = command.ExecuteReader();
            if(reader.HasRows) {
                commandString = "SELECT number FROM Version order by number desc";
                command = new SQLiteCommand(commandString, filesLibraryConnection);
                reader = command.ExecuteReader();
                while(reader.Read())
                    if (!(reader.GetInt32(0) == DBVersion))
                    {
                        Application.Exit();
                        // TODO Implement Upgrader
                    }
            }
            else {
                commandString = "CREATE TABLE Version (number INT32)";
                command = new SQLiteCommand(commandString, filesLibraryConnection);
                command.ExecuteNonQuery();
                commandString = "insert into Version (number) values (" + DBVersion + ")";
                command = new SQLiteCommand(commandString, filesLibraryConnection);
                command.ExecuteNonQuery();
            }
        }

        private void btnSelectFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Video Files (*.mp4, *.avi)|*.mp4;*.avi";

            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string fileToPlay = ofd.FileName;
                txtSekectedFile.Text = fileToPlay;
                alxplugin1.addTarget("file:///" + fileToPlay, null, AXVLC.VLCPlaylistMode.VLCPlayListInsert, 0);
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            alxplugin1.stop();
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            alxplugin1.pause();
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            alxplugin1.play();
        }
    }
}
