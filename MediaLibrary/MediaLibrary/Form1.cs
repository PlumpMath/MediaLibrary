using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MediaLibrary
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();


        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            int variable = 1 + 1;
        }

        private void btnSelectFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Video Files (*.mp4, *.avi)|*.mp4;*.avi";

            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string fileToPlay = ofd.FileName;
                txtSekectedFile.Text = fileToPlay;
                AXVLC.VLCPlugin alxplugin1 = new AXVLC.VLCPlugin();
                alxplugin1.addTarget("file:///" + fileToPlay, null, AXVLC.VLCPlaylistMode.VLCPlayListInsert, 0);
                alxplugin1.play();
            }
        }
    }
}
