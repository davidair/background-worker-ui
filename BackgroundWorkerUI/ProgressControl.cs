using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace BackgroundWorkerUI
{
    /// <summary>
    /// The ProgressControl class wraps the BackgroundWorker class and adds UI with a progress bar,
    /// a textbox for details and a button to start/stop the worker.
    /// </summary>
    public partial class ProgressControl : UserControl
    {
        /// <summary>
        /// Occurs when the Start button is clicked.
        /// </summary>
        public event DoWorkEventHandler DoWork;

        /// <summary>
        /// Provides the input for the background worker.
        /// </summary>
        public event Func<object> StartClicked;

        private bool _running;
        private string _buttonText;
        /// <summary>
        /// Gets or sets the start button text (default is &Start).
        /// </summary>
        [Category("Settings")]
        public string ButtonText {
            get { return _buttonText; }
            set { _buttonText = value;
                if (this.buttonStart != null) {
                    this.buttonStart.Text = _buttonText;
                } }
        }

        /// <summary>
        /// Gets or sets the cancel text (when the background work is in progress).
        /// </summary>
        [Category("Settings")]
        public string CancelText { get; set; }

        public ProgressControl()
        {
            this.ButtonText = "&Start";
            this.CancelText = "&Cancel";
            InitializeComponent();
        }

        private void ProgressControl_Load(object sender, EventArgs e)
        {
            this.buttonStart.Text = this.ButtonText;
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (this.DoWork != null)
            {
                this.DoWork(sender, e);
            }
        }

        public void ReportProgress(int progress, string value)
        {
            this.backgroundWorker.ReportProgress(progress, value);
        }

        public bool CancellationPending
        {
            get
            {
                return this.backgroundWorker.CancellationPending;
            }
        }

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.progressBar.Value = e.ProgressPercentage;
            if (e.UserState != null)
            {
                this.textBoxProgress.AppendText(String.Format("{0} {1}{2}", DateTime.Now, e.UserState, Environment.NewLine));
            }
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.progressBar.Value = 100;
            if (e.Cancelled)
            {
                MessageBox.Show("Cancelled");
                this.textBoxProgress.AppendText("Cancelled");
            }
            else if (e.Error != null)
            {
                MessageBox.Show("Error: " + e.Error.Message);
                this.textBoxProgress.AppendText(e.Error.Message);
                this.textBoxProgress.AppendText(Environment.NewLine);
                this.textBoxProgress.AppendText(e.Error.StackTrace);
            }
            else
            {
                MessageBox.Show("Done");
                this.textBoxProgress.AppendText("Done");
            }

            this.buttonStart.Text = this.ButtonText;
            this.buttonStart.Enabled = true;
            this.progressBar.Value = 0;
            _running = false;
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            if (!_running)
            {
                if (this.StartClicked != null)
                {

                    this.textBoxProgress.Clear();
                    _running = true;
                    this.buttonStart.Text = this.CancelText;
                    this.backgroundWorker.RunWorkerAsync(this.StartClicked());
                }
            }
            else
            {
                this.buttonStart.Enabled = false;
                this.backgroundWorker.CancelAsync();
            }
        }
    }
}
