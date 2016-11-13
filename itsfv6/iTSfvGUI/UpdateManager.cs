using ShareX.HelpersLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace iTSfvGUI
{
    public class UpdateManager : IDisposable
    {
        public double UpdateCheckInterval { get; private set; } = 1; // Hour

        private bool firstUpdateCheck = true;
        private System.Threading.Timer updateTimer = null;
        private readonly object updateTimerLock = new object();

        public UpdateManager()
        {
        }

        public UpdateManager(double updateCheckInterval)
        {
            UpdateCheckInterval = updateCheckInterval;
        }

        public void ConfigureAutoUpdate()
        {
            lock (updateTimerLock)
            {
                if (updateTimer == null)
                {
                    updateTimer = new System.Threading.Timer(state => CheckUpdate(), null, TimeSpan.Zero, TimeSpan.FromHours(UpdateCheckInterval));
                }
                else
                {
                    Dispose();
                }
            }
        }

        private void CheckUpdate()
        {
            if (!UpdateMessageBox.DontShow && !UpdateMessageBox.IsOpen)
            {
                UpdateChecker updateChecker = CreateUpdateChecker();
                updateChecker.CheckUpdate();

                if (UpdateMessageBox.Start(updateChecker, firstUpdateCheck) != DialogResult.Yes)
                {
                    TimeSpan interval = TimeSpan.FromHours(24);
                    updateTimer.Change(interval, interval);
                }

                firstUpdateCheck = false;
            }
        }

        public static UpdateChecker CreateUpdateChecker()
        {
            return new GitHubUpdateChecker("McoreD", "iTSfv")
            {
                Proxy = HelpersOptions.CurrentProxy.GetWebProxy()
            };
        }

        public void Dispose()
        {
            if (updateTimer != null)
            {
                updateTimer.Dispose();
                updateTimer = null;
            }
        }
    }
}