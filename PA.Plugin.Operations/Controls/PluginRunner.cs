using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using PA.Plugin.Operations.Interfaces;

namespace PA.Plugin
{
    public partial class PluginRunner : Component
    {

        abstract class AsyncWrapper : IDisposable
        {
            public abstract object Execute();
            public abstract void Dispose();
        }

        class AsyncWrapper<T> : AsyncWrapper
            where T : IActionPlugin, ICloneable
        {
            private T p;
            private IDictionary<string, object> contextData;

            public AsyncWrapper(T p, IDictionary<string, object> contextData)
            {
                this.p = (T)p.Clone();
                this.contextData = contextData;
            }

            public override object Execute()
            {
                return this.p.Execute(contextData) as object;
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

        public void RunAsync<T>(T p, IDictionary<string, object> contextData)
            where T : IActionPlugin, ICloneable
        {
            this.DelayedCalls.Enqueue(new AsyncWrapper<T>(p, contextData));
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
