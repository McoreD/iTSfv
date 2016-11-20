/*  This file is part of Album Art Downloader.
 *  CoverDownloader is free software; you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation; either version 2 of the License, or
 *  (at your option) any later version.
 *
 *  CoverDownloader is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with CoverDownloader; if not, write to the Free Software             
 *  Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA  */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.IO;
using Boo.Lang.Compiler.Pipelines;
using System.IO;
using System.Reflection;

namespace AlbumArtDownloader
{
    public partial class ScriptCompilerForm : Form
    {
        string key;
        public ScriptCompilerForm(string newkey)
        {
            key = newkey;
            InitializeComponent();
        }

        private void ScriptCompiler_Load(object sender, EventArgs e)
        {
            this.CenterToParent();
            backgroundWorkerScriptCompiler.RunWorkerAsync();
        }
        struct Result
        {
            public Result(bool b1, bool b2)
            {
                success = b1;
                warnings = b2;
            }
            public bool success;
            public bool warnings;
        }
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                e.Result = new Result(true, false);
                backgroundWorkerScriptCompiler.ReportProgress(0, "Searching for scripts...");
                string path = System.Windows.Forms.Application.StartupPath;
                string[] files = System.IO.Directory.GetFiles(System.Windows.Forms.Application.StartupPath + "\\scripts", "*.boo");
                List<FileInput> goodfiles = new List<FileInput>();
                List<string> readablefiles = new List<string>();
                List<string> refs = new List<string>();

                foreach (string file in files)
                {

                    StreamReader sr = new StreamReader(File.OpenRead(file));
                    string firstline = sr.ReadLine();
                    if (firstline.StartsWith("# refs: ") && firstline.Length > 8)
                    {
                        string refstext = firstline.Substring(8);
                        refs.AddRange(refstext.Split(' '));
                    }
                    goodfiles.Add(new FileInput(file));
                    readablefiles.Add(System.IO.Path.GetFileName(file));
                    sr.Close();
                }



                backgroundWorkerScriptCompiler.ReportProgress(0, string.Format("Found {0} files, [{1}]...", goodfiles.Count, string.Join(", ", readablefiles.ToArray())));
                backgroundWorkerScriptCompiler.ReportProgress(0, string.Format("Loading references; [{0}]...", string.Join(", ", refs.ToArray())));
                string target = System.Windows.Forms.Application.StartupPath + "\\scripts\\scriptcache.dll";
                BooCompiler c = new BooCompiler();
                c.Parameters.LibPaths.Add(Application.StartupPath);
                c.Parameters.Ducky = true;
                c.Parameters.OutputType = CompilerOutputType.Library;
                c.Parameters.Debug = false;
                CompilerContext z = null;
                c.Parameters.Pipeline = new CompileToFile();
                c.Parameters.OutputAssembly = target;
                foreach (string ss in refs)
                {
                    c.Parameters.References.Add(c.Parameters.LoadAssembly(ss, true));
                }
                foreach (FileInput f in goodfiles)
                {
                    c.Parameters.Input.Add(f);
                }
                backgroundWorkerScriptCompiler.ReportProgress(0, "Compiling...");

                bool bWarnings = false;
                //   c.Compile();
                z = c.Run();
                if (z != null)
                {
                    if (z.GeneratedAssembly != null)
                    {
                        foreach (CompilerWarning cw in z.Warnings)
                        {
                            backgroundWorkerScriptCompiler.ReportProgress(0, string.Format("Warning: {0}", cw.Message));
                            bWarnings = true;

                        }
                        backgroundWorkerScriptCompiler.ReportProgress(0, "Complete");
                    }
                    else
                    {
                        foreach (CompilerWarning cw in z.Warnings)
                        {
                            bWarnings = true;
                            backgroundWorkerScriptCompiler.ReportProgress(0, string.Format("Warning: {0}", cw.Message));

                        }
                        foreach (CompilerError ce in z.Errors)
                        {
                            backgroundWorkerScriptCompiler.ReportProgress(0, string.Format("Error: {0} - {1}", ce.LexicalInfo, ce.Message));
                        }
                        throw new Exception("Failed.");
                    }
                }
                else throw new Exception("Weird error. No CompilerContext");
                e.Result = new Result(true, bWarnings);
            }
            catch (Exception eee)
            {
                backgroundWorkerScriptCompiler.ReportProgress(0, string.Format("Error: {0}", eee.Message));
                e.Result = new Result(false, true);
            }

        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            textBoxStatus.AppendText(((string)e.UserState) + "\r\n");
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (((Result)(e.Result)).success == true)
            {
                Properties.Settings.Default.ScriptInfo = key;
                Properties.Settings.Default.Save();
                if (!((Result)(e.Result)).warnings)
                    this.Close();
                else
                {
                    progressBarCompiler.Style = ProgressBarStyle.Continuous;
                    progressBarCompiler.Value = 100;
                    buttonClose.Enabled = true;
                    buttonRetry.Enabled = true;
                }
            }
            else
            {
                buttonClose.Enabled = false;
                buttonRetry.Enabled = true;
                progressBarCompiler.Style = ProgressBarStyle.Continuous;
                progressBarCompiler.Value = 0;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBoxStatus.Clear();
            buttonClose.Enabled = false;
            buttonRetry.Enabled = false;
            progressBarCompiler.Style = ProgressBarStyle.Marquee;
            backgroundWorkerScriptCompiler.RunWorkerAsync();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
