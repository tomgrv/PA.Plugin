using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PA.Plugin
{
    public partial class PluginRunner : Component
    {

        abstract class AsyncWrapper : IDisposable
        {
            public abstract object Execute();
            public abstract void Dispose();
        }

        class AsyncWrapper<T, U> : AsyncWrapper
            where T : class, IPluginOperation
            where U : class
        {
            private T p;
            private U o;

            public AsyncWrapper(T p, U o)
            {
                this.p = p.Clone() as T;
                this.o = o is U ? o : Activator.CreateInstance<U>();
            }

            public override object Execute()
            {
                return this.p.Execute<T>(o) as object;
            }

            #region IDisposable Membres

            public override void Dispose()
            {
                this.p.Dispose();
            }

            #endregion
        }

        public PluginRunner()
        {
            InitializeComponent();
            this.DelayedCalls = new Queue<AsyncWrapper>();
        }

        #region IPluginRunner Membres

        [Browsable(false)]
        public bool IsBusy { get { return this.DelayedCalls.Count > 0; } }

        [Category("Plugin Management")]
        public event RunWorkerCompletedEventHandler Done;

        [Category("Plugin Management")]
        public event EventHandler Started;

        [Category("Plugin Management")]
        public bool ContinueOnError { get; set; }

        public void RunAsync<T, U>(T p, U args)
            where T : class, IPluginOperation
            where U : class
        {
            this.DelayedCalls.Enqueue(new AsyncWrapper<T, U>(p, args));
            this.RunNext();
        }

        public virtual void Cancel()
        {
            this.DelayedCalls.Clear();
            this.AsyncOperation.CancelAsync();
        }

        #endregion

        #region Async

        private Queue<AsyncWrapper> DelayedCalls;

        private void RunNext()
        {
            if (!this.AsyncOperation.IsBusy && this.DelayedCalls.Count > 0)
            {
                this.AsyncOperation.RunWorkerAsync(this.DelayedCalls.Dequeue());

                if (this.Started != null)
                {
                    this.Started(this, EventArgs.Empty);
                }

            }
        }

        private void AsyncOperation_DoWork(object sender, DoWorkEventArgs e)
        {
            if (e.Argument is AsyncWrapper)
            {
                e.Result = (e.Argument as AsyncWrapper).Execute();
            }
        }

        private void AsyncOperation_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.OnRunWorkerCompleted(e);
        }

        protected virtual void OnRunWorkerCompleted(RunWorkerCompletedEventArgs e)
        {
            if (this.Done != null)
            {
                this.Done(this, e);
            }

            if (this.ContinueOnError || e.Error == null)
            {
                RunNext();
            }
        }

        #endregion


    }
}
