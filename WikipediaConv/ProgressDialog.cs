using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WikipediaConv
{
    public partial class ProgressDialog : Form
    {
        /// <summary>
        /// Handles the ProgressChanged event from indexers
        /// </summary>
        /// <param name="sender">Indexer</param>
        /// <param name="e">Progress event</param>
        private delegate void ProgressChangedDelegate(object sender, ProgressChangedEventArgs e);
        /// <summary>
        /// The indexer we're associated with
        /// </summary>
        private ILongTask ltask;
        /// <summary>
        /// Whether indexing is currently being executed
        /// </summary>
        private bool indexingRunning;

        public ProgressDialog(ILongTask indexer)
        {
            InitializeComponent();

            ltask = indexer;

            ltask.ProgressChanged += new ProgressChangedEventHandler(ixr_ProgressChanged);
        }

        private void ProgressDialog_Shown(object sender, EventArgs e)
        {
            ltask.Start();

            indexingRunning = true;
        }

        private void btnDone_Click(object sender, EventArgs e)
        {
            if (indexingRunning)
            {
                btnDone.Enabled = false;

                textBox.AppendText(Properties.Resources.AbortingIndexing + Environment.NewLine);

                ltask.Abort();
            }
            else
            {
                // Due to failure

                DialogResult = DialogResult.Abort;
                Close();
            }
        }

        private void Indexer_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            DecodingProgress ip = (DecodingProgress)e.UserState;
            
            if (!String.IsNullOrEmpty(ip.Message))
            {
                textBox.AppendText(ip.Message + Environment.NewLine);
            }
            if (!String.IsNullOrEmpty(ip.ETA))
            {
                labelETA.Text = String.Format(Properties.Resources.ETA, ip.ETA);
            }

            if (e.ProgressPercentage > 0)
            {
                progressBar.Value = e.ProgressPercentage;
            }

            if (ip.DecodingState == DecodingProgress.State.Failure)
            {
                btnDone.Text = Properties.Resources.CloseIndexingForm;
            }

            if (ip.DecodingState == DecodingProgress.State.Finished)
            {
                indexingRunning = false;

                labelETA.Text = Properties.Resources.IndexingDoneETA;

                DialogResult = btnDone.Enabled ? DialogResult.OK : DialogResult.Abort;

                Close();
            }
        }

        #region Helper methods

        private void ixr_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Invoke(new ProgressChangedDelegate(Indexer_ProgressChanged), sender, e);
        }

        #endregion
    }
}