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
using System.Text;
using System.Net;
using System.Threading;
using System.Text.RegularExpressions;
using System.IO;
using System.Windows.Forms;
using System.Reflection;
using System.Drawing;

namespace AlbumArtDownloader
{
    public delegate void TaskEventHandler(object o, TaskEventArgs e);
    public delegate void ThreadEventHandler(object o, ThreadEventArgs e);

    public class ScriptTask
    {
        public int rescount;
        internal int estimate;
        ArtDownloader artdownloader;
        public ScriptTask(Script s, Task t, ArtDownloader a)
        {
            script = s;
            task = t;
            artdownloader = a;
        }
        public bool IsDebug
        {
            get
            {
                return task.IsDebug;
            }
        }
        public void AddThumb(string thumb, string name, int width, int height, object callback)
        {
            if (task.ShouldAbort())
                throw new AbortedException();
            long size = new long();
            Stream http = ArtDownloader.GetHTTPStream(thumb, ref size);
            AlbumArtDownloader.ArtDownloader.ThumbRes result = new AlbumArtDownloader.ArtDownloader.ThumbRes();
            result.Width = width;
            result.Height = height;
            result.CallbackData = callback;
            result.OwnerTask = task;
            result.ScriptOwner = script;
            if (callback == null)
            {
                result.FullSize = new MemoryStream();
                artdownloader.GetStreamToStream(result.FullSize, http, 0, false);
                result.Thumb = System.Drawing.Bitmap.FromStream(result.FullSize);
                result.FullSize.Seek(0, SeekOrigin.Begin);
            }
            else
                result.Thumb = System.Drawing.Bitmap.FromStream(http);
            result.Name = name;
            task.AddResult(result);
            ++rescount;
            artdownloader.mainForm.BeginInvoke(artdownloader.mainForm.taskupdate, task, script, estimate == 0 ? string.Format("{0}", rescount) : string.Format("{0}/{1}", rescount, estimate));
            if (rescount >= Properties.Settings.Default.MaxResults)
                throw new AbortedException();
        }
        public void AddThumb(Stream thumb, string name, int width, int height, object callback)
        {
            if (task.ShouldAbort())
                throw new AbortedException();
            AlbumArtDownloader.ArtDownloader.ThumbRes result = new AlbumArtDownloader.ArtDownloader.ThumbRes();
            result.Width = width;
            result.Height = height;
            result.CallbackData = callback;
            if (callback == null)
            {
                result.FullSize = new MemoryStream();
                artdownloader.GetStreamToStream(result.FullSize, thumb, 0, false);
                result.Thumb = System.Drawing.Bitmap.FromStream(result.FullSize);
                result.FullSize.Seek(0, SeekOrigin.Begin);
            }
            else
                result.Thumb = System.Drawing.Bitmap.FromStream(thumb);
            result.OwnerTask = task;
            result.ScriptOwner = script;
            result.Name = name;
            task.AddResult(result);
            ++rescount;
            artdownloader.mainForm.BeginInvoke(artdownloader.mainForm.taskupdate, task, script, estimate == 0 ? string.Format("{0}", rescount) : string.Format("{0}/{1}", rescount, estimate));
            if (rescount >= Properties.Settings.Default.MaxResults)
                throw new AbortedException();
        }
        public void AddThumb(System.Drawing.Image thumb, string name, int width, int height, object callback)
        {
            if (task.ShouldAbort())
                throw new AbortedException();
            AlbumArtDownloader.ArtDownloader.ThumbRes result = new AlbumArtDownloader.ArtDownloader.ThumbRes();
            result.Width = width;
            result.Height = height;
            result.Thumb = thumb;
            result.OwnerTask = task;
            result.CallbackData = callback;
            result.ScriptOwner = script;
            result.Name = name;
            task.AddResult(result);
            ++rescount;
            artdownloader.mainForm.BeginInvoke(artdownloader.mainForm.taskupdate, task, script, estimate == 0 ? string.Format("{0}", rescount) : string.Format("{0}/{1}", rescount, estimate));
            if (rescount >= Properties.Settings.Default.MaxResults)
                throw new AbortedException();
        }
        public void DebugHTML(string html)
        {
            if (IsDebug)
            {
                artdownloader.mainForm.BeginInvoke(artdownloader.mainForm.dohtml, false, html);
            }
        }
        public void DebugURL(string html)
        {
            if (IsDebug)
            {
                artdownloader.mainForm.BeginInvoke(artdownloader.mainForm.dohtml, true, html);
            }
        }
        public void SetCountEstimate(int count)
        {
            estimate = count;
        }
        internal Script script;
        internal Task task;
    }
    public class TaskThread
    {
        public Thread thread;
        ManualResetEvent e_abort;
        AutoResetEvent e_wakeup;
        ArtDownloader artdownloader;
        public TaskThread(ArtDownloader a)
        {
            artdownloader = a;
            e_abort = new ManualResetEvent(false);
            e_wakeup = new AutoResetEvent(false);
            thread = new Thread(new ThreadStart(this.Run));
            a.RegisterThread(this);
        }
        public void Start()
        {
            thread.Start();
        }
        public void End()
        {
            e_abort.Set();
            WakeUp();
        }
        public void WakeUp()
        {
            e_wakeup.Set();
        }
        void Run()
        {
            while (!e_abort.WaitOne(0, false))
            {
                ScriptTask t = artdownloader.FetchTask();
                if (t != null)
                {
                    try
                    {
                        if (t.script.Enabled)
                        {
                            t.script.GetThumbs(t, t.task.Artist, t.task.Album);
                            lock (t.task.results)
                            {
                                string s = string.Format("{0}/{1}", t.rescount, t.rescount);
                                artdownloader.mainForm.BeginInvoke(artdownloader.mainForm.taskupdate, t.task, t.script, s);
                            }
                        }
                        else
                        {
                            artdownloader.mainForm.BeginInvoke(artdownloader.mainForm.taskupdate, t.task, t.script, "Script disabled");
                        }
                    }
                    catch (AbortedException)
                    {
                        continue;
                    }
                    catch (Exception e)
                    {
                        artdownloader.mainForm.BeginInvoke(artdownloader.mainForm.taskupdate, t.task, t.script, e.Message);
                    }
                    finally
                    {
                        --t.task.ScriptsToGo;
                    }
                }
                else
                    e_wakeup.WaitOne();
            }
            artdownloader.UnRegisterThread(this);
        }
    }
    public class TaskEventArgs : EventArgs
    {
        public Task task;
        public TaskEventArgs(Task t)
        {
            task = t;
        }

    }
    public class ThreadEventArgs : EventArgs
    {
        public int count;
        public ThreadEventArgs(int c)
        {
            count = c;
        }

    }
    public class AbortedException : Exception
    {
    }
    public class Task
    {
        bool isdebug;
        public bool IsDebug
        {
            get
            {
                return isdebug;
            }
        }
        public Task(ArtDownloader a, string artist, string album, string filesave, bool showExisting, bool showFolder, bool bDebug)
        {
            e_abort = new ManualResetEvent(false);
            _Artist = artist;
            _Album = album;
            _showExisting = showExisting;
            _showFolder = showFolder;
            status = State.Waiting;
            artdownloader = a;
            savestr = filesave;
            isdebug = bDebug;
            _Done = false;
            scripts = new List<Script>();
            results = new List<AlbumArtDownloader.ArtDownloader.ThumbRes>();

        }
        ArtDownloader artdownloader;
        enum State
        {
            Waiting,
            Searching,
            Done
        };
        string savestr;
        State status;
        public Task(ArtDownloader a, string artist, string album, string filesave, bool showExisting, bool showFolder, ListViewItem item)
        {
            e_abort = new ManualResetEvent(false);
            _Artist = artist;
            artdownloader = a;
            _Album = album;
            _showExisting = showExisting;
            _showFolder = showFolder;
            listviewitem = item;
            status = State.Waiting;
            savestr = filesave;
            _Done = false;
            scripts = new List<Script>();
            results = new List<AlbumArtDownloader.ArtDownloader.ThumbRes>();
        }
        string _Artist;
        string _Album;
        bool _showExisting;
        bool _showFolder;
        public List<Script> scripts;
        public void Abort()
        {
            e_abort.Set();
        }
        private int _ScriptsToGo;
        public int ScriptsToGo
        {
            get
            {
                lock (this)
                    return _ScriptsToGo;
            }
            set
            {
                lock (this)
                {
                    _ScriptsToGo = value;
                    if (_ScriptsToGo == 0)
                    {
                        artdownloader.mainForm.CoolBeginInvoke(artdownloader.mainForm.taskdone, this);
                    }
                }
            }
        }

        public bool ShouldAbort()
        {
            return e_abort.WaitOne(0, false);
        }
        public string Status
        {
            get
            {
                lock (this)
                {
                    switch (status)
                    {
                        case State.Waiting:
                            return "Waiting";
                        case State.Searching:
                            return "Searching";
                        case State.Done:
                            return "Done";
                        default:
                            return "Unknown";
                    }
                }
            }
        }
        public string Artist
        {
            get
            {
                return _Artist;
            }
        }
        public string FileSave
        {
            get
            {
                lock (this)
                {
                    return savestr;
                }
            }
        }
        public string Album
        {
            get
            {
                return _Album;
            }
        }
        public bool ShowExisting
        {
            get
            {
                return _showExisting;
            }
        }
        public bool ShowFolder
        {
            get
            {
                return _showFolder;
            }
        }
        bool _Done;
        public bool Done
        {
            get
            {
                lock (this)
                {
                    return _Done;
                }
            }
        }
        bool _Running;
        public bool Running
        {
            get
            {
                lock (this)
                {
                    return _Running;
                }
            }
            set
            {
                lock (this)
                {
                    _Running = value;
                }
            }
        }


        public void AddResult(AlbumArtDownloader.ArtDownloader.ThumbRes result)
        {
            if (result.Name.ToLowerInvariant() == (Album.ToLowerInvariant())) result.Exact = true;
            lock (results)
            {
                results.Add(result);

            }
            lock (artdownloader)
            {
                if (artdownloader.SelectedTask == this)
                    artdownloader.SendThumb(result);
            }
        }
        public ListViewItem listviewitem;
        public System.Collections.Generic.List<AlbumArtDownloader.ArtDownloader.ThumbRes> results;
        ManualResetEvent e_abort;
    }

    public class Script
    {
        public ListViewGroup group;
        public ColumnHeader listcol;
        ArtDownloader artdownloader;

        string _Name;
        string _Version;
        string _Creator;
        bool _Enabled = true;
        int _SortPosition;

        public string Name
        {
            get
            {
                return _Name;
            }
        }
        public string Version
        {
            get
            {
                return _Version;
            }
        }
        public string Creator
        {
            get
            {
                return _Creator;
            }
        }

        public bool Enabled
        {
            get
            {
                return _Enabled;
            }
            set
            {
                _Enabled = value;
            }
        }

        public int SortPosition
        {
            get
            {
                return _SortPosition;
            }
            set
            {
                _SortPosition = value;
            }
        }


        MethodInfo getThumbs, getResult;
        internal Script(ArtDownloader a, Type thetype)
        {
            lock (this)
            {
                _Name = thetype.Name;
                artdownloader = a;

                getThumbs = thetype.GetMethod("GetThumbs", BindingFlags.Static | BindingFlags.Public);
                getResult = thetype.GetMethod("GetResult", BindingFlags.Static | BindingFlags.Public);
                
                PropertyInfo nameinfo = thetype.GetProperty("SourceName", BindingFlags.Static | BindingFlags.Public);
                _Name = nameinfo.GetValue(null, null).ToString();
                
                nameinfo = thetype.GetProperty("SourceVersion", BindingFlags.Static | BindingFlags.Public);
                _Version = nameinfo.GetValue(null, null).ToString();

                try
                {
                    nameinfo = thetype.GetProperty("SourceCreator", BindingFlags.Static | BindingFlags.Public);
                    _Creator = nameinfo.GetValue(null, null).ToString();
                }
                catch
                {
                    _Creator = "";
                }


                if (getThumbs == null || getResult == null)
                    throw new Exception("Script must implement GetThumbs() And GetResult()");

                group = a.mainForm.AddGroup(this);
                listcol = a.mainForm.AddCol(this);
            }

        }

        internal void GetThumbs(ScriptTask t, string artist, string album)
        {
            try
            {
                getThumbs.Invoke(null, new object[] { t, artist, album });
            }
            catch (Exception e)
            {
                if (e.GetType() == typeof(System.Reflection.TargetInvocationException))
                {
                    throw e.InnerException;
                }
            }
        }
        internal object GetResult(object data)
        {
            try
            {
                return getResult.Invoke(null, new object[] { data });
            }
            catch (Exception e)
            {
                if (e.GetType() == typeof(System.Reflection.TargetInvocationException))
                {
                    throw e.InnerException;
                }
            }
            return null;
        }

		public override string ToString()
		{
			return Name;
		}
    }
    public class ArtDownloader
    {
        public class ThumbRes
        {
            private MemoryStream _FullSize;
            public MemoryStream FullSize
            {
                get { return _FullSize; }
                set { _FullSize = value; }
            }
            private System.Drawing.Image _Thumb;
            public System.Drawing.Image Thumb
            {
                get { return _Thumb; }
                set { _Thumb = value; }
            }
            private object _CallbackData;
            public object CallbackData
            {
                get { return _CallbackData; }
                set { _CallbackData = value; }
            }

            private Task _OwnerTask;
            public Task OwnerTask
            {
                get { return _OwnerTask; }
                set { _OwnerTask = value; }
            }

            private Int32 _Width;
            public Int32 Width
            {
                get { return _Width; }
                set { _Width = value; }
            }
            private Int32 _Height;
            public Int32 Height
            {
                get { return _Height; }
                set { _Height = value; }
            }

            private Script _ScriptOwner;
            public Script ScriptOwner
            {
                get { return _ScriptOwner; }
                set { _ScriptOwner = value; }
            }

            private string _Name;
            public string Name
            {
                get { return _Name; }
                set { _Name = value; }
            }

            private bool _Exact;
            public bool Exact
            {
                get { return _Exact; }
                set { _Exact = value; }
            }
        }
        Task selectedtask;

        public Task SelectedTask
        {
            get
            {
                lock (this)
                {
                    return selectedtask;
                }
            }
            set
            {
                lock (this)
                {
                    selectedtask = value;
                    lock (selectedtask.results)
                    {
                        foreach (ThumbRes r in selectedtask.results)
                        {
                            SendThumb(r);
                        }
                    }
                }
            }
        }

        public void SendThumb(ThumbRes r)
        {
            lock (r)
                mainForm.BeginInvoke(mainForm.thumbupdate, r);
        }

        public MainForm mainForm;
        ManualResetEvent e_abort;
        List<Task> tasks;
        public event TaskEventHandler TaskAdded;
        public event TaskEventHandler TaskRemoved;
        public event ThreadEventHandler ThreadCountChanged;
        private System.Collections.Generic.List<TaskThread> threads;
        public List<Script> scripts;
        public List<Thread> otherthreads;

        public void AddTask(Task t)
        {
            lock (scripts)
            {
                t.scripts.AddRange(scripts);
                t.ScriptsToGo = scripts.Count;
            }
            lock (tasks)
            {
                tasks.Add(t);
            }
            if (TaskAdded != null)
            {
                TaskAdded(this, new TaskEventArgs(t));
            }
            lock (threads)
            {
                foreach (TaskThread tt in threads)
                {
                    tt.WakeUp();
                }
            }
        }
        public void SetThreads()
        {
            if (!e_abort.WaitOne(0, false))
            {
                int difference = (int)Properties.Settings.Default.ThreadCount - threads.Count;
                lock (threads)
                {
                    while (difference < 0)
                    {
                        TaskThread victim = threads[0];
                        victim.End();
                        ++difference;
                    }
                    while (difference > 0)
                    {
                        TaskThread t = new TaskThread(this);
                        t.Start();
                        --difference;
                    }
                }
            }
            else
            {
                lock (threads)
                {
                    foreach (TaskThread tt in threads)
                    {
                        tt.End();
                    }
                }
            }
        }
        public void RemoveTask(Task t)
        {
            lock (tasks)
            {
                t.Abort();
                if (tasks.Contains(t))
                    tasks.Remove(t);
            }
            if (TaskRemoved != null)
            {
                TaskRemoved(this, new TaskEventArgs(t));
            }
        }
        public void LoadScripts()
        {
            string path = System.Windows.Forms.Application.StartupPath + "\\scripts\\scriptcache.dll";
            Assembly ass = Assembly.LoadFile(path);
            Type[] types = ass.GetTypes();

            foreach (Type m in types)
            {
                if (m.Namespace == "CoverSources")
                {
                    try
                    {
                        scripts.Add(new Script(this, m));
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(string.Format("Loading Source named \'{0}\' failed because: {1}", m.Name, e.Message));
                    }
                }
            }
            // string[] files = System.IO.Directory.GetFiles(System.Windows.Forms.Application.StartupPath + "\\scripts", "*.py");
            /*     foreach (string file in files)
                 {
                     if (System.IO.Path.GetFileName(file)[0] != '_')
                     {
                         Script s;
                         try
                         {
                             s = new Script(this, file);
                             scripts.Add(s);
             
                         }
                         catch (Exception e)
                         {
                             MessageBox.Show(e.Message, System.IO.Path.GetFileNameWithoutExtension(file));
                         }
                     }

                 }*/
            mainForm.ScriptsLoaded(scripts);

            lock (tasks)
            {
                foreach (Task t in tasks)
                {
                    t.scripts.AddRange(scripts);
                }
            }

        }
        public void StatusUpdate(Task t)
        {

        }

        public ArtDownloader(MainForm form)
        {
            threads = new System.Collections.Generic.List<TaskThread>();
            mainForm = form;
            e_abort = new ManualResetEvent(false);
            // artists=new string();
            // albums=new string();
            tasks = new List<Task>();
            scripts = new List<Script>();
            otherthreads = new List<Thread>();
        }
        public void Abort()
        {
            e_abort.Set();
            lock (threads)
            {
                foreach (TaskThread t in threads)
                {
                    t.End();
                }
                lock (tasks)
                {
                    foreach (Task tt in tasks)
                    {
                        tt.Abort();
                    }
                }
            }
        }
        static public System.IO.Stream GetHTTPStream(string url, ref long size)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            size = response.ContentLength;
            return response.GetResponseStream();
        }


        public bool ShouldAbort()
        {
            return (e_abort.WaitOne(0, false));
        }
        public struct SaveData
        {
            public ThumbRes indexn;
            public string filetargetn;
        }
        public void SaveArt(ThumbRes result, string filetarget)
        {
            Thread t;
            lock (this)
            {
                SaveData s = new SaveData();
                s.indexn = result;
                s.filetargetn = filetarget;
                t = new Thread(new ParameterizedThreadStart(this.DoSaveArt));
                otherthreads.Add(t);
                t.Start(s);
            }
        }

        private void DoSaveArt(object o)
        {
            FileStream f = null;
            try
            {
                SaveData s = (SaveData)o;
                ThumbRes thumb;
                thumb = s.indexn;
                lock (thumb)
                {
					PopupateFullSizeStream(thumb);
                    if (s.filetargetn != null)
                        f = File.Create(s.filetargetn);
                    Image i = ProcessStream(ref f, s, thumb);
                    if (i != null)
                    {
                        mainForm.CoolBeginInvoke(mainForm.previewgot, i, thumb);
                        mainForm.CoolBeginInvoke(mainForm.statupdate, "Complete", null, true, Thread.CurrentThread);
                    }
                    else
                    {
                        mainForm.CoolBeginInvoke(mainForm.statupdate, "File Saved.", thumb.OwnerTask, true, Thread.CurrentThread);
                    }


                }

            }
            catch (AbortedException)
            {

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Retrieving Art Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                if (f != null)
                {
                    f.Close();
                }
            }


#if false
            try
            {
                SaveData s = (SaveData)o;
                ThumbRes baseurl;
                System.IO.FileStream f = null;
                System.Drawing.Image i = null;
                baseurl = s.indexn;
                if (s.filetargetn != null)
                    f = System.IO.File.Create(s.filetargetn);
                try
                {
                    mainForm.CoolInvoke(mainForm.statupdate, "Retrieving image...", false, false, Thread.CurrentThread);
                    mainForm.CoolInvoke(mainForm.produpdate, (int)(0));
                    object output = baseurl.script.GetResult(baseurl.callbackdata);
                    if (output is System.IO.Stream)
                    {
                        Stream ss = output as Stream;
                        if (f != null) GetStreamToFile(f, ss, ss.Length);
                        else
                        {
                            i = new System.Drawing.Bitmap(ss);
                        }
                    }
                    else if (output is String)
                    {
                        long size = new long();
                        Stream ss = GetHTTPStream(output as String, ref size);
                        if (f != null) GetStreamToFile(f, ss, size);
                        else
                        {
                            i = new System.Drawing.Bitmap(ss);
                        }
                    }
                }
                catch (Exception e)
                {
                    if (f != null)
                        f.Close();
                    throw e;
                }
                if (f != null)
                    f.Close();
                mainForm.CoolInvoke(mainForm.produpdate, (int)(-1));
                if (i != null)
                    mainForm.CoolBeginInvoke(mainForm.previewgot, i, baseurl.name);
                mainForm.CoolBeginInvoke(mainForm.statupdate, f == null ? "Complete." : "File Saved.", false, true, Thread.CurrentThread); 

            }
            catch (AbortedException)
            {
                mainForm.CoolNoThrowBeginInvoke(mainForm.statupdate, "Aborted.", false, true, Thread.CurrentThread);
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message);
                mainForm.CoolInvoke(mainForm.statupdate, "Failed with error.", false, true, Thread.CurrentThread);


            }
#endif
            ThreadEnded(System.Threading.Thread.CurrentThread);
        }

        public void PopupateFullSizeStream(Object thumb)
		{
			if ((thumb as ThumbRes).FullSize == null)
			{
                //mainForm.CoolInvoke(mainForm.statupdate, "Retrieving image...", null, false, Thread.CurrentThread);
                //mainForm.CoolInvoke(mainForm.produpdate, (int)(0));
                object output = (thumb as ThumbRes).ScriptOwner.GetResult((thumb as ThumbRes).CallbackData);
				if (output is System.IO.Stream)
				{
					long length = new long();

					Stream ss = output as Stream;

					if (ss.CanSeek)
						length = ss.Length;
					else
						length = 0;

					if (length > 50 * 1024 * 1024)
					{
						throw new Exception("Script returned preposterous Stream (greater than 50MB.)");
					}
                    (thumb as ThumbRes).FullSize = new MemoryStream((int)(length));
                    GetStreamToStream((thumb as ThumbRes).FullSize, ss, length, true);

				}
				else if (output is String)
				{
					long size = new long();

					Stream ss = GetHTTPStream(output as String, ref size);

					if (!ss.CanSeek)
						size = 0;

					if (size > 50 * 1024 * 1024)
					{
						throw new Exception("Server returned preposterous Content-Length (greater than 50MB.)");
					}
                    (thumb as ThumbRes).FullSize = new MemoryStream((int)(size));
                    GetStreamToStream((thumb as ThumbRes).FullSize, ss, size, true);

				}
			}
		}

        public void PopupateFullSizeStream(ThumbRes thumb)
        {
            if (thumb.FullSize == null)
            {
                mainForm.CoolInvoke(mainForm.statupdate, "Retrieving image...", null, false, Thread.CurrentThread);
                mainForm.CoolInvoke(mainForm.produpdate, (int)(0));
                object output = thumb.ScriptOwner.GetResult(thumb.CallbackData);
                if (output is System.IO.Stream)
                {
                    long length = new long();

                    Stream ss = output as Stream;

                    if (ss.CanSeek)
                        length = ss.Length;
                    else
                        length = 0;

                    if (length > 50 * 1024 * 1024)
                    {
                        throw new Exception("Script returned preposterous Stream (greater than 50MB.)");
                    }
                    thumb.FullSize = new MemoryStream((int)(length));
                    GetStreamToStream(thumb.FullSize, ss, length, true);

                }
                else if (output is String)
                {
                    long size = new long();

                    Stream ss = GetHTTPStream(output as String, ref size);

                    if (!ss.CanSeek)
                        size = 0;

                    if (size > 50 * 1024 * 1024)
                    {
                        throw new Exception("Server returned preposterous Content-Length (greater than 50MB.)");
                    }
                    thumb.FullSize = new MemoryStream((int)(size));
                    GetStreamToStream(thumb.FullSize, ss, size, true);

                }
            }
        }

        private System.Drawing.Image ProcessStream(ref FileStream f, SaveData s, ThumbRes thumb)
        {
            if (f != null)
            {
                thumb.FullSize.Seek(0, SeekOrigin.Begin);
                GetStreamToStream(f, thumb.FullSize, thumb.FullSize.Length, false);
                return null;
            }
            else
            {
                thumb.FullSize.Seek(0, SeekOrigin.Begin);
                return System.Drawing.Bitmap.FromStream(thumb.FullSize);
            }

        }

        public void GetStreamToStream(Stream f, Stream ss, long size, bool sendprogress)
        {
            byte[] buf = new byte[4096];
            int read;
            int total = 0;
            while ((read = ss.Read(buf, 0, buf.Length)) != 0)
            {
                f.Write(buf, 0, read);
                total += read;
                if (total > 50 * 1024 * 1024)
                {
                    throw new Exception("Stream is ridiculously long (50MB read so far). Stopping before something bad happens.");
                }
                if (sendprogress && size != 0)
                    mainForm.CoolInvoke(mainForm.produpdate, (int)(total * 100 / size));
            }
        }

        public bool ThreadsActive()
        {
            lock (threads)
            {
                if (threads.Count > 0) return true;
            }
            lock (otherthreads)
            {
                if (otherthreads.Count > 0) return true;
            }
            return false;
        }

        public void ThreadEnded(Thread from)
        {
            lock (otherthreads)
            {
                //   System.Diagnostics.Debug.Assert(threads.Contains(from));
                otherthreads.Remove(from);
                if (e_abort.WaitOne(0, false) && !ThreadsActive())
                    mainForm.BeginInvoke(mainForm.threadended);
            }
        }

        internal ScriptTask FetchTask()
        {
            lock (tasks)
            {
                foreach (Task t in tasks)
                {
                    lock (t)
                    {
                        lock (t.scripts)
                        {
                            if (t.scripts.Count > 0)
                            {
                                Script s = t.scripts[0];
                                t.scripts.Remove(s);
                                return new ScriptTask(s, t, this);
                            }
                        }
                    }
                }
            }
            return null;
        }

        internal void UnRegisterThread(TaskThread taskThread)
        {
            int newcount = 0;
            lock (threads)
            {
                threads.Remove(taskThread);
                newcount = threads.Count;
                if (!ThreadsActive() && e_abort.WaitOne(0, false))
                {
                    mainForm.BeginInvoke(mainForm.threadended);
                }
            }
        }

        internal void RegisterThread(TaskThread taskThread)
        {
            int newcount = 0;
            lock (threads)
            {
                threads.Add(taskThread);
                newcount = threads.Count;
            }
            lock (ThreadCountChanged)
            {
                if (ThreadCountChanged != null)
                {
                    //  ThreadCountChanged.(this, new ThreadEventArgs(newcount));
                }
            }
        }

        internal void CompileScripts()
        {
            try
            {
                string path = System.Windows.Forms.Application.StartupPath;
                string[] files = System.IO.Directory.GetFiles(System.Windows.Forms.Application.StartupPath + "\\scripts", "*.boo");
                System.Collections.Generic.Dictionary<string, DateTime> oldtimes = new System.Collections.Generic.Dictionary<string, DateTime>();
                string[] fileinfolist = Properties.Settings.Default.ScriptInfo.Split('|');
                foreach (string ss in fileinfolist)
                {
                    string[] filedetail = ss.Split(':');
                    if (filedetail.Length == 2)
                    {
                        long parseres;
                        if (long.TryParse(filedetail[1], out parseres))
                            oldtimes[filedetail[0]] = new DateTime(parseres, DateTimeKind.Utc);
                    }
                }
                bool compileneeded = files.Length != fileinfolist.Length;
                if (!compileneeded)
                {
                    if (!System.IO.File.Exists(System.Windows.Forms.Application.StartupPath + "\\scripts\\scriptcache.dll"))
                    {
                        compileneeded = true;
                    }
                }
                List<string> newkey = new List<string>();
                foreach (string file in files)
                {
                    DateTime spec = System.IO.File.GetLastWriteTimeUtc(file);
                    if (!oldtimes.ContainsKey(System.IO.Path.GetFileName(file).ToLowerInvariant()) || oldtimes[System.IO.Path.GetFileName(file)] != spec)
                    {
                        compileneeded = true;
                    }
                    newkey.Add(System.IO.Path.GetFileName(file) + ":" + spec.Ticks);
                }
                if (compileneeded)
                {
                    ScriptCompilerForm ss = new ScriptCompilerForm(string.Join("|", newkey.ToArray()));
                    ss.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }
    }
}