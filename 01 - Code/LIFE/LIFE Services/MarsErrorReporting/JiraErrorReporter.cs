using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Atlassian.Jira;

namespace MarsErrorReporting
{
    public static class JiraErrorReporter
    {
        private const string JiraUrl = "http://jira.3ten.de";
        private const string JiraProjectName = "MARS";
        private const string JiraUser = "MMDEIssueReporter";
        private const string JiraPassword = "US7vjOK3p9j2rPIACSGw";
        private const string JiraIssueType = "Bug";

        private readonly static Jira JiraClient;

        static JiraErrorReporter()
        {
            JiraClient = new Jira(
                JiraUrl, JiraUser, JiraPassword
            );
        }

        public static Issue ReportError(string fileToAttach = null, Exception ex = null)
        {
            ErrorReportingDialog errorReportingDialog = null;
            Issue issue = null;
            var dialog = new PendingActivityDialog
            {
                Text = "Sending error report...", //Title of the dialog,
                SupportsCancellation = false,
                SupportsProgressVisualization = false,
                StartPosition = FormStartPosition.CenterScreen
            };

            dialog.Load += delegate
            {
                errorReportingDialog = new ErrorReportingDialog(ex);
                errorReportingDialog.ShowDialog();
            };
            dialog.BackgroundWorker.DoWork += delegate
            {
                if (errorReportingDialog.DialogResult == DialogResult.Cancel) return;
                dialog.BackgroundWorker.ReportProgress(0, "Sending error report. Please wait...");
                issue = ReportError(errorReportingDialog.Title, errorReportingDialog.Description, errorReportingDialog.ReporterName, errorReportingDialog.ReporterMail, fileToAttach, ex);
            };
            dialog.BackgroundWorker.RunWorkerCompleted += delegate
            {
                if (issue != null)
                {
                    MessageBox.Show("Your error has been reported and is identified by " + issue.Key, "Reporting successfull", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            };
            dialog.ShowDialog();

            return issue;
        }

        public static Issue ReportError(string title, string description, string reporterName, string reporterMail, string fileToAttach = null, Exception ex = null)
        {
            var descStrBuilder = new StringBuilder();
            descStrBuilder.AppendFormat("*Reported by:* {0} ({1})\n\n", string.IsNullOrEmpty(reporterName) ? SystemInformation.UserName : reporterName, string.IsNullOrEmpty(reporterMail) ? "N/A" : reporterMail);
            descStrBuilder.AppendFormat("{0}\n\n", description);
            if (ex != null) descStrBuilder.AppendFormat("*Stacktrace:*\n{0}", ex.StackTrace);

            var issue = JiraClient.CreateIssue(JiraProjectName);
            issue.Summary = title;
            issue.Type = JiraIssueType;
            issue.Environment = string.Format("*OS:* {0} ({1})\n" +
                                              "*Computer name:* {2}\n" +
                                              "*IP-Address:* {3}",
                                              Environment.OSVersion,
                                              Environment.Is64BitOperatingSystem ? "x64" : "x86",
                                              SystemInformation.ComputerName,
                                              Dns.GetHostEntry(Dns.GetHostName()).AddressList.First(ipa => ipa.AddressFamily == AddressFamily.InterNetwork)
                                );
            issue.Description = descStrBuilder.ToString();
            issue.SaveChanges();

            if (string.IsNullOrEmpty(fileToAttach)) return issue;
            issue.AddAttachment(Path.GetFileName(fileToAttach), File.ReadAllBytes(fileToAttach));
            issue.SaveChanges();
            return issue;
        }

    }
}
