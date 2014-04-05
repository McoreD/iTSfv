using HelpersLib;
using iTSfvLib;
using Microsoft.WindowsAPICodePack.Taskbar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace iTSfvGUI
{
    public partial class LogViewer : Form
    {
        public LogViewer()
        {
            InitializeComponent();
            DebugHelper.Logger.MessageAdded += Logger_MessageAdded;
        }

        private void Logger_MessageAdded(string message)
        {
            this.InvokeSafe(() => AppendMessage(message));
        }

        private void AppendMessage(string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                lbLogs.Items.Add(message);
            }
        }

        public void BindWorker(BackgroundWorker worker)
        {
            worker.ProgressChanged += Program.LogViewer.Worker_ProgressChanged;
            worker.RunWorkerCompleted += Program.LogViewer.Worker_RunWorkerCompleted;
        }

        public void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (tspbApp != null && !tspbApp.IsDisposed)
                tspbApp.Value = e.ProgressPercentage;
            TaskbarHelper.TaskbarSetProgressValue(e.ProgressPercentage);
        }

        public void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ReportWriter report = e.Result as ReportWriter;

            if (Program.Config.UI.Checks_MissingTags)
            {
                tsslApp.Text = "Ready. Tracks with missing tags: " + report.TracksMissingTags.Count;
            }
            else
            {
                tsslApp.Text = "Ready.";
            }

            if (Program.Config.ProduceReports)
                report.Write(Program.LogsFolderPath);
        }

        internal void AddFilesWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            tsslApp.Text = "Reading files/folders...";
            tspbApp.ProgressBar.Style = ProgressBarStyle.Marquee;
            TaskbarHelper.TaskbarSetProgressState(TaskbarProgressBarState.Indeterminate);
        }

        internal void AddFilesWorker_RunWorkerCompleted()
        {
            tsslApp.Text = "Ready.";
            tspbApp.ProgressBar.Style = ProgressBarStyle.Continuous;
            TaskbarHelper.TaskbarSetProgressState(TaskbarProgressBarState.Normal);
        }
    }
}