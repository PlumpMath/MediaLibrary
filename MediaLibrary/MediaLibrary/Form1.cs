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
        SQLQuery dbConnection;
        public Form1()
        {
            InitializeComponent();
            dbConnection = new SQLQuery(DBName);
            if (!System.IO.File.Exists(DBName))
            {
                dbConnection.createDB();
            }
            if(dbConnection.checkTableExists("Version")) {
                if (dbConnection.getDBVersion() != DBVersion)
                    //TODO implement upgrader
                    Application.Exit();
            }
            else {
                dbConnection.createVersionTable();
                dbConnection.setTableVersion(DBVersion);
            }

            if (!dbConnection.checkTableExists("Videos"))
            {
                dbConnection.createVideosTable();
            }

            if (!dbConnection.checkTableExists("Music"))
            {
                dbConnection.createMusicTable();
            }
        }

        private void btnSelectFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Video Files (*.mp4, *.avi, *.mkv)|*.mp4;*.avi;*.mkv";
            ofd.Multiselect = true;

            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string[] filesToAdd = ofd.FileNames;
                foreach (string file in filesToAdd) 
                {
                    alxplugin1.addTarget("file:///" + file, null, AXVLC.VLCPlaylistMode.VLCPlayListInsert, 0);
                }
                dbConnection.addNonNetworkVideos(filesToAdd);
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
