namespace WinFormsGet
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    // based on: http://stackoverflow.com/a/22262976/1069200
    internal class MessagePump : IDisposable
    {
        private readonly Thread thread;
        private TaskScheduler scheduler;

        public MessagePump()
        {
            var setupComplete = new ManualResetEvent(false);
            this.thread = new Thread(() =>
            {
                EventHandler idleHandler = null;

                idleHandler = (s, e) =>
                {
                    // handle Application.Idle just once
                    this.scheduler = TaskScheduler.FromCurrentSynchronizationContext();
                    setupComplete.Set();
                    Application.Idle -= idleHandler;
                };

                // handle Application.Idle just once
                // to make sure we're inside the message loop
                // and SynchronizationContext has been correctly installed
                Application.Idle += idleHandler;
                Application.Run();
            });

            this.thread.SetApartmentState(ApartmentState.STA);
            this.thread.IsBackground = true;
            this.thread.Start();
            setupComplete.WaitOne();
        }

        /// <summary>Shutdown the STA thread</summary>
        public void Dispose()
        {
            Application.Exit();
            this.thread.Join();
        }

        public Task InvokeAsync(Action action)
        {
            return Task.Factory.StartNew(action, CancellationToken.None, TaskCreationOptions.None, this.scheduler);
        }

        public Task<TResult> InvokeAsync<TResult>(Func<TResult> action)
        {
            return Task.Factory.StartNew(action, CancellationToken.None, TaskCreationOptions.None, this.scheduler);
        }
    }
}