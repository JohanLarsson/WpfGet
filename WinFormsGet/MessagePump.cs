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
        private SynchronizationContext context;

        public MessagePump()
        {
            var setupComplete = new ManualResetEvent(false);
            this.thread = new Thread(() =>
            {
                EventHandler idleHandler = null;

                idleHandler = (s, e) =>
                {
                    // handle Application.Idle just once
                    this.context = SynchronizationContext.Current;
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
            this.Send(Application.ExitThread);
            this.thread.Join();
        }

        public void Send(Action action)
        {
            this.context.Send(_ => action(), null);
        }

        public void Post<T>(Action<T> action, T state)
        {
            this.context.Post(s => action((T) s), state);
        }

        public void Send<T>(Action<T> action, T state)
        {
            this.context.Send(s => action((T)s), state);
        }
    }
}