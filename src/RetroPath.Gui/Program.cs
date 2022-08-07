using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.ReactiveUI;
using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Threading;
using MessageBox.Avalonia.DTO;
using MessageBox.Avalonia.Enums;
using Serilog;

namespace RetroPath.Gui
{
    internal class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        [STAThread]
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.File(System.IO.Path.Combine(Directory.GetCurrentDirectory(), "logs"))
                .CreateLogger();

            TaskScheduler.UnobservedTaskException += (sender, eventArgs) =>
            {
                var errorWindow = MessageBox.Avalonia.MessageBoxManager
                    .GetMessageBoxStandardWindow(new MessageBoxStandardParams{
                        ContentTitle = $"Error: {eventArgs.Exception.GetType()}",
                        ContentMessage = $"The following error has occurred while running RP2.0 with provided inputs:\n{eventArgs.Exception.Message}",
                        ButtonDefinitions = ButtonEnum.Ok,
                        ShowInCenter = true,
                        WindowStartupLocation = WindowStartupLocation.CenterScreen,
                        SizeToContent = SizeToContent.WidthAndHeight,
                        Icon = Icon.Error
                    });
        
                Log.Error(eventArgs.Exception, "Error while running RP2.0 on provided input configuration");
        
                errorWindow.Show().GetAwaiter().GetResult();

                Environment.Exit(2);
            };

            try
            {
                BuildAvaloniaApp()
                    .StartWithClassicDesktopLifetime(args);
            }
            catch (Exception e)
            {
                OnUnhandledException(e).GetAwaiter().GetResult();
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace()
                .UseReactiveUI();

        private static async Task OnUnhandledException(Exception e)
        {
            Log.Error(e, "Unhandled exception: {ExType}", e.GetType());
            Log.CloseAndFlush();
            Environment.Exit(2);
        }
    }
}
