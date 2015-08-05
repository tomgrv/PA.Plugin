using PA.Plugin.Threads.Attributes;
using PA.Plugin.Threads.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Threading;

namespace PA.Plugin.Threads
{
    [PartNotDiscoverable]
    [DefaultProperty("LoopSpan")]
    public partial class PluginThread : PluginBase, IPluginThread
    {
        private Thread _tLoop;
        private Semaphore _sLoop;

        public EventHandler<EventArgs> RunStarted;
        public EventHandler<EventArgs> RunLoop;
        public EventHandler<EventArgs> RunCompleted;

        public TimeSpan LoopSpan { get; private set; }
        public TimeSpan WaitSpan { get; private set; }
        public bool IsPaused { get; private set; }
        public bool IsRunning { get; private set; }
        public bool IsFirstLoop { get; private set; }

        public PluginThread() :
            base()
        {
            int maxInstances1 = PluginBase.GetAttribute<PluginThreadAttribute>(this.GetType()).MaxInstances;
            int maxInstances2 = PluginBase.GetAttribute<PluginThreadAttribute>(typeof(PluginThread)).MaxInstances;

            if (PluginThread.idList == null)
            {
                PluginThread.idList = new List<IPluginThread>(maxInstances2);
            }

            List<IPluginThread> list = new List<IPluginThread>(PluginThread.idList);

            if (list.FindAll(p => p.GetType().IsSubclassOf(this.GetType())).Count >= maxInstances1)
            {
                throw new IndexOutOfRangeException("Cannot start more than " + maxInstances1.ToString() + " " + this + " threads");
            }
            else if (PluginThread.idList.Count >= maxInstances2)
            {
                throw new IndexOutOfRangeException("Cannot start more than " + maxInstances2.ToString() + " " + this + " threads");
            }


            // List thread
            PluginThread.idList.Add((IPluginThread)this);

            // Init Thread
            this._tLoop = new Thread(new ThreadStart(this.Loop));
            this._tLoop.Name = this.GetType().FullName;
            this._sLoop = new Semaphore(1, 1);
            this.LoopSpan = new TimeSpan(0, 1, 0);
            this.IsRunning = false;
            this.IsPaused = false;
            this.IsFirstLoop = true;

        }

        #region IPluginThread Members

        private static List<IPluginThread> idList;

        [Browsable(false)]
        public int ThreadId
        {
            get { return this.DesignMode ? -1 : idList.IndexOf(this as IPluginThread); }
        }

        public bool Start()
        {
            this.IsPaused = false;

            if (!this._tLoop.IsAlive)
            {
                Trace.TraceInformation(this + " is starting...");

                this._tLoop.Start();

                Trace.TraceInformation(this + " is started.");
            }

            return true;
        }

        public bool Stop()
        {
            this.IsRunning = false;

            if (this._tLoop.IsAlive)
            {
                Trace.TraceInformation(this + " is stopping...");

                if (this._tLoop.ThreadState == System.Threading.ThreadState.WaitSleepJoin)
                {
                    this._tLoop.Abort();
                }
                else
                {
                    this._tLoop.Join();
                }

                Trace.TraceInformation(this + " is stopped");

            }

            return true;
        }

        public bool Pause()
        {
            if (this._tLoop.IsAlive)
            {
                this.IsPaused = true;
            }

            return true;
        }

        #endregion

        [Obsolete]
        private void Loop()
        {
            this.IsRunning = true;
            this.OnRunStarted();

            while (true)
            {
                if (!this.IsRunning)
                {
                    break;
                }

                DateTime now = DateTime.Now;

                if (!this.IsPaused)
                {
                    this._sLoop.WaitOne();
                    {
                        try
                        {
                            this.Execute();
                        }
                        catch (Exception ex)
                        {
                            Trace.TraceError(this + " (" + ex.Source + ") is stopping after exception: " + ex.Message + "\n" + (ex.InnerException is Exception ? ex.InnerException.Message : ex.StackTrace), EventLogEntryType.Error);
                            this.IsRunning = false;
                        }
                    }
                    this._sLoop.Release();
                }
                else
                {
                    this.Wait(TimeSpan.FromMinutes(10));
                }


                if (this.IsRunning)
                {
                    if (this.WaitSpan == TimeSpan.Zero)
                    {
                        Thread.Sleep(this.LoopSpan - (now - DateTime.Now));
                    }
                    else
                    {
                        Thread.Sleep(this.WaitSpan);
                    }
                }

                this.WaitSpan = TimeSpan.Zero;
                this.IsFirstLoop = false;
            }

            this.OnRunCompleted();
        }

         [Obsolete]
        public virtual void OnRunStarted()
        {
            if (this.RunStarted != null)
            {
                this.RunStarted(this, new EventArgs());
            }
        }

         [Obsolete]
        public virtual void OnRunCompleted()
        {
            if (this.RunCompleted != null)
            {
                this.RunCompleted(this, new EventArgs());
            }
        }

        // public virtual void Execute(IJobExecutionContext context)
        //{
        //    this.Execute();
        //}

        [Obsolete]
        public virtual void Execute()
        {
            if (this.RunLoop != null)
            {
                this.RunLoop(this, new EventArgs());
            }
        }

         [Obsolete]
        protected void Wait(TimeSpan ws)
        {
            this.WaitSpan = ws;
        }

         [Obsolete]
        protected void Wait(DateTime wd)
        {
            bool flag = !(wd < DateTime.Now);
            if (flag)
            {
                this.WaitSpan = wd - DateTime.Now;
                return;
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }
        }
    }
}
