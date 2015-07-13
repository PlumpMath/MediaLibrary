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
            dbConnection.getAllVideos();
            dbConnection.getAllMusic();
        }

        private void btnSelectFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Media Files (*.mp4, *.avi, *.mkv, *.mp3, *.wav)|*.mp4;*.avi;*.mkv;*.mp3;*.wav";
            ofd.Multiselect = true;

            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string[] filesToAdd = ofd.FileNames;
                foreach (string file in filesToAdd) 
                {
                    axVLCPlugin21.playlist.add("file:///" + file);
                }
                dbConnection.addNonNetworkVideos(filesToAdd);
            }
        }


        private void btnAddNetwork_Click(object sender, EventArgs e)
        {
            string name = "name"; //temporary, need to figure out a nice UI way of getting a name of network video
            // switch to new VLC plugin that allows for embedding into form broke URL playing -- need to fix
            string path = txtSelectedFile.Text;
            dbConnection.addNetworkVideo(path, name);
            axVLCPlugin21.playlist.add(path);
            axVLCPlugin21.playlist.play();
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            axVLCPlugin21.playlist.play();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            axVLCPlugin21.playlist.stop();
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            axVLCPlugin21.playlist.pause();
        }
    }
}
