using iTSfvLib;
using ShareX.HelpersLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace iTSfvGUI
{
    public static class Program
    {
        [DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();

        private static readonly string ApplicationName = Application.ProductName; // keep this top most
        public static readonly Version AssemblyVersion = Assembly.GetExecutingAssembly().GetName().Version;
        private static readonly string LogFileName = ApplicationName + "Log-{0}.log";

        public static CLIManager CLI { get; private set; }
        public static bool IsPortable { get; private set; }

        public static BackgroundWorker SettingsReader = new BackgroundWorker();
        public static Settings Config = null;

        // Windows
        public static ValidatorWizard MainForm = null;
        public static LogViewer LogViewer = null;
        public static XmlLibrary Library = null;

        private static readonly string DefaultPersonalPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), ApplicationName);
        private static readonly string PortablePersonalPath = Path.Combine(Application.StartupPath, ApplicationName);
        internal static readonly string ConfigCoreFileName = ApplicationName + "Settings.json";

        public const string URL_WEBSITE = "https://github.com/McoreD/iTSfv";
        public const string URL_ISSUES = "https://github.com/McoreD/iTSfv/issues";
        public const string URL_UPDATE = "https://raw.githubusercontent.com/McoreD/iTSfv/master/Update.xml";

        public static List<string> LibNames = new List<string>();

        public static SynchronizationContext TreadUI = null;

        public static string PersonalPath
        {
            get
            {
                if (IsPortable)
                {
                    return PortablePersonalPath;
                }

                return DefaultPersonalPath;
            }
        }

        private static string ConfigCoreFilePath
        {
            get
            {
                return Path.Combine(PersonalPath, ConfigCoreFileName);
            }
        }

        public static string LogsFolderPath
        {
            get
            {
                return Path.Combine(PersonalPath, "Logs");
            }
        }

        public static string LogFilePath
        {
            get
            {
                DateTime now = FastDateTime.Now;
                return Path.Combine(LogsFolderPath, string.Format(LogFileName, now.Year, now.Month));
            }
        }

        public static string Title
        {
            get
            {
                Version version = Version.Parse(Application.ProductVersion);
                string title = string.Format(ApplicationName + " {0}.{1}", version.Major, version.Minor);
                if (version.Build > 0) title += "." + version.Build;
                if (IsPortable) title += " Portable";
                return title;
            }
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
            Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            AppDomain.CurrentDomain.AssemblyLoad += new AssemblyLoadEventHandler(CurrentDomain_AssemblyLoad);

            try
            {
                CLI = new CLIManager(args);
                CLI.ParseCommands();

                IsPortable = CLI.IsCommandExist("p", "portable");

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                if (Environment.OSVersion.Version.Major >= 6) SetProcessDPIAware();

                LogViewer = new LogViewer();

                MainForm = new ValidatorWizard();
                SettingsReader.DoWork += SettingsReader_DoWork;
                SettingsReader.RunWorkerCompleted += MainForm.SettingsReader_RunWorkerCompleted;
                SettingsReader.RunWorkerAsync();

                Application.Run(MainForm);
                Program.Config.Save(ConfigCoreFilePath);
                DebugHelper.Logger.SaveLog(LogFilePath);
            }
            finally
            {
            }
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            OnError(e.Exception);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            OnError((Exception)e.ExceptionObject);
        }

        private static void OnError(Exception e)
        {
            new ErrorForm(Application.ProductName, DebugHelper.Logger.ToString(), LogFilePath, Links.URL_ISSUES).ShowDialog();
        }

        private static void SettingsReader_DoWork(object sender, DoWorkEventArgs e)
        {
            Program.Config = Settings.Read(ConfigCoreFilePath);
        }

        private static void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            LibNames.Add(args.LoadedAssembly.FullName);
        }
    }
}