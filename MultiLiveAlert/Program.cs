using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace MultiLiveAlert
{
    class Program
    {
        [STAThread]
        static void Main()
        {
            var app = new AppNoStartupUri
            {
                ShutdownMode = ShutdownMode.OnExplicitShutdown
            };
            app.InitializeComponent();
            SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext());

            var p = new Program();
            p.ExitRequested += (sender, e) =>
            {
                app.Shutdown();
            };

            var t = p.StartAsync();
            Handle(t);
            app.Run();
        }
        static async void Handle(Task t)
        {
            try
            {
                await t;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
        public async Task StartAsync()
        {
            ViewModel.IMainViewModel viewModel = new ViewModel.MainViewModel();
            viewModel.CloseRequested += ViewModel_CloseRequested;

            void windowClosed(object sender, EventArgs e)
            {
                viewModel.RequestClose();
            }

            await viewModel.InitializeAsync();

            var mainForm = new View.MainView();
            mainForm.Closed += windowClosed;
            mainForm.DataContext = viewModel;
            mainForm.Show();
        }

        public event EventHandler<EventArgs> ExitRequested;
        void ViewModel_CloseRequested(object sender, EventArgs e)
        {
            OnExitRequested(EventArgs.Empty);
        }

        protected virtual void OnExitRequested(EventArgs e)
        {
            ExitRequested?.Invoke(this, e);
        }
    }
}
