using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProgressUI
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            this.textBoxPath.Text = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            if (this.folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                this.textBoxPath.Text = this.folderBrowserDialog.SelectedPath;
            }
        }

        private void progressControl_DoWork(object sender, DoWorkEventArgs e)
        {
            string path = (string)e.Argument;
            string[] files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
            int percentage = 0;
            int index = 0;
            foreach (string file in files)
            {
                if (this.progressControl.CancellationPending)
                {
                    break;
                }
                percentage = 100 * index / files.Length;
                this.progressControl.ReportProgress(percentage, file);
                index++;
                Thread.Sleep(50);
            }
        }

        private object progressControl_StartClicked()
        {
            return this.textBoxPath.Text;
        }
    }
}
