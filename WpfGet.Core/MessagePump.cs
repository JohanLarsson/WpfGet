namespace WpfGet.Core
{
    using System;
    using System.Threading;
    using System.Windows.Threading;

    // based on:  http://reedcopsey.com/2011/11/28/launching-a-wpf-window-in-a-separate-thread-part-1/
    internal class MessagePump : IDisposable
    {
        private readonly Thread thread;
        private SynchronizationContext context;

        public MessagePump()
        {
            var setupComplete = new ManualResetEvent(false);
            this.thread = new Thread(() =>
            {
                SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));
                this.context = SynchronizationContext.Current;
                setupComplete.Set();
                Dispatcher.Run();
            });

            this.thread.SetApartmentState(ApartmentState.STA);
            this.thread.IsBackground = true;
            this.thread.Start();
            setupComplete.WaitOne();
        }

        /// <summary>Shutdown the STA thread</summary>
        public void Dispose()
        {
            this.Send(Dispatcher.ExitAllFrames);
            this.thread.Join();
        }

        public void Post<T>(Action<T> action, T state)
        {
            this.context.Post(s => action((T) s), state);
        }

        private void Send(Action action)
        {
            this.context.Send(_ => action(), null);
        }
    }
}