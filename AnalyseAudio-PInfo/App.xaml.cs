using AnalyseAudio_PInfo.Activation;
using AnalyseAudio_PInfo.Contracts.Services;
using AnalyseAudio_PInfo.Core.Contracts.Services;
using AnalyseAudio_PInfo.Core.Models;
using AnalyseAudio_PInfo.Core.Services;
using AnalyseAudio_PInfo.Helpers;
using AnalyseAudio_PInfo.Models;
using AnalyseAudio_PInfo.Services;
using AnalyseAudio_PInfo.ViewModels;
using AnalyseAudio_PInfo.Views;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;
using Windows.UI.Popups;

// To learn more about WinUI3, see: https://docs.microsoft.com/windows/apps/winui/winui3/.
namespace AnalyseAudio_PInfo
{
    public partial class App : Application
    {
        // The .NET Generic Host provides dependency injection, configuration, logging, and other services.
        // https://docs.microsoft.com/dotnet/core/extensions/generic-host
        // https://docs.microsoft.com/dotnet/core/extensions/dependency-injection
        // https://docs.microsoft.com/dotnet/core/extensions/configuration
        // https://docs.microsoft.com/dotnet/core/extensions/logging
        private static IHost _host = Host
            .CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                // Default Activation Handler
                services.AddTransient<ActivationHandler<LaunchActivatedEventArgs>, DefaultActivationHandler>();

                // Other Activation Handlers

                // Services
                services.AddSingleton<ILocalSettingsService, LocalSettingsServicePackaged>();
                services.AddSingleton<IThemeSelectorService, ThemeSelectorService>();
                services.AddTransient<INavigationViewService, NavigationViewService>();

                services.AddSingleton<IActivationService, ActivationService>();
                services.AddSingleton<IPageService, PageService>();
                services.AddSingleton<INavigationService, NavigationService>();

                // Core Services
                services.AddSingleton<IFileService, FileService>();

                // Views and ViewModels
                services.AddTransient<SpectrogramViewModel>();
                services.AddTransient<SpectrogramElement>();
                services.AddTransient<SpectrogramWindow>();
                services.AddTransient<SettingsViewModel>();
                services.AddTransient<SettingsPage>();
                services.AddTransient<ConsoleViewModel>();
                services.AddTransient<ConsolePage>();
                services.AddTransient<PianoViewModel>();
                services.AddTransient<PianoPage>();
                services.AddTransient<SpectrogramConfigViewModel>();
                services.AddTransient<SpectrogramConfigPage>();
                services.AddTransient<CaptureViewModel>();
                services.AddTransient<CapturePage>();
                services.AddTransient<ShellPage>();
                services.AddTransient<ShellViewModel>();

                // Configuration
                services.Configure<LocalSettingsOptions>(context.Configuration.GetSection(nameof(LocalSettingsOptions)));
            })
            .Build();

        public static T GetService<T>()
            where T : class
            => _host.Services.GetService(typeof(T)) as T;

        public static Window MainWindow { get; set; } = new Window() { Title = "AppDisplayName".GetLocalized() };

        public App()
        {
            InitializeComponent();
            UnhandledException += App_UnhandledException;
        }

        private void App_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            var dialog = new MessageDialog(e.Message, "Exception occurred");
            _ = dialog.ShowAsync();
        }

        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            base.OnLaunched(args);
            var activationService = App.GetService<IActivationService>();
            await activationService.ActivateAsync(args);

            MainWindow.Closed += MainWindow_Closed;
            Logger.Initialize();
            Manager.Initialize();
        }

        private void MainWindow_Closed(object sender, WindowEventArgs args)
        {
            Manager.Close();
            System.Windows.Forms.Application.Exit();
        }
    }
}
