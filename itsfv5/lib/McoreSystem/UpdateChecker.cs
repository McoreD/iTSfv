using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Net;
using System.Diagnostics;

namespace McoreSystem
{
    public class UpdateChecker
    {
        private string mDownloadURL = "";
        public bool Manual { get; set; }

        public UpdateChecker(string url)
        {
            mDownloadURL = url;
        }

        public void CheckUpdates()
        {
            Thread updateThread = new Thread(UpdateThread);
            updateThread.Start();
        }

        private string[] CheckUpdate()
        {
            string[] returnValue = new string[3];
            WebClient wClient = new WebClient();
            string source = wClient.DownloadString(mDownloadURL);
            returnValue[0] = Regex.Match(source, "(?<=<a href=\").+(?=\" style=\"white)").Value; //Link
            returnValue[1] = Regex.Match(returnValue[0], @"(?<=.+)(?:\d+\.){3}\d+(?=.+)").Value; //Version
            returnValue[2] = Regex.Match(source, "(?<=q=\">).+?(?=</a>)", RegexOptions.Singleline).Value.Replace("\n", "").Replace("\r", "").Trim(); //Summary
            return returnValue;
        }

        private void UpdateThread()
        {
            try
            {
                string[] updateValues = CheckUpdate();
                if (!string.IsNullOrEmpty(updateValues[1]) && new Version(updateValues[1]).CompareTo(new Version(Application.ProductVersion)) > 0)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("New version available");
                    sb.AppendLine();
                    sb.AppendLine("Current version:\t" + Application.ProductVersion);
                    sb.AppendLine("Latest version:\t" + updateValues[1]);
                    sb.AppendLine();
                    sb.AppendLine(updateValues[2].Replace("|", "\n"));
                    sb.AppendLine();
                    sb.AppendLine("Press OK to download latest version.");

                    if (MessageBox.Show(sb.ToString(), Application.ProductName, MessageBoxButtons.OKCancel,
                        MessageBoxIcon.Information) == DialogResult.OK)
                    {
                        Process.Start(updateValues[0]);
                    }
                }
                else if (Manual)
                {
                    MessageBox.Show("No updates are available", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch
            {

            }
        }
    }
}
