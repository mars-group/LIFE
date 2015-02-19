using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MarsErrorReporting
{
    public partial class ErrorReportingDialog : Form
    {
        private int _btnSwitchCount;

        public string Title
        {
            get { return txtTitle.Text.Trim(); }
        }

        public string Description
        {
            get { return txtDesc.Text.Trim(); }
        }

        public string ReporterName
        {
            get { return txtName.Text.Trim(); }
        }

        public string ReporterMail
        {
            get { return txtMail.Text.Trim(); }
        }

        public ErrorReportingDialog()
        {
            InitializeComponent();
        }

        public ErrorReportingDialog(Exception ex)
            : this()
        {
            txtException.Text = ex.ToString();
        }

        private void txtTitle_TextChanged(object sender, EventArgs e)
        {
            var title = txtTitle.Text.Trim();
            var desc = txtDesc.Text.Trim();

            btnSend.Enabled = !string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(desc);
        }

        private void btnClose_MouseEnter(object sender, EventArgs e)
        {
            if (_btnSwitchCount < 4)
            {
                var closeLoc = btnClose.Location;
                var sendLoc = btnSend.Location;

                btnClose.Location = sendLoc;
                btnSend.Location = closeLoc;

                _btnSwitchCount++;
            }
            else if (_btnSwitchCount == 4)
            {
                MessageBox.Show("Okay, you won...\n\rBut your karma has been lowered for not reporting an issue!!",
                    "The evil cancel button");
                _btnSwitchCount++;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
