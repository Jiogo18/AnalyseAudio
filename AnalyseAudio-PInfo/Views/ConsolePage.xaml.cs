using AnalyseAudio_PInfo.Core.Models;
using AnalyseAudio_PInfo.ViewModels;
using Microsoft.UI;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Media;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AnalyseAudio_PInfo.Views
{
    public sealed partial class ConsolePage : Page
    {
        public ConsoleViewModel ViewModel { get; }

        public ConsolePage()
        {
            ViewModel = App.GetService<ConsoleViewModel>();
            InitializeComponent();
            Logger.Current.Added += Logger_Added;
            (new List<LogData>(Logger.Current.logs)).ForEach(log => Logger_Added(Logger.Current, log));
        }

        private bool IsScrolledToBottom()
        {
            return Scroller.ScrollableHeight - Scroller.VerticalOffset <= 1;
        }

        private async Task ScrollToBottom()
        {
            await Task.Delay(1);
            Scroller.ChangeView(null, Scroller.ExtentHeight, null, true);
        }

        /// <summary>
        /// Add a log to the console element
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Logger_Added(object sender, LogData e)
        {
            string time = e.timestamp.ToString("[HH:mm:ss.fff] ");

            DispatcherQueue?.TryEnqueue(
                Microsoft.UI.Dispatching.DispatcherQueuePriority.Low,
                async () =>
                {
                    Paragraph myParagraph = new();
                    Run run = new() { Text = time + e.message };
                    myParagraph.Inlines.Add(run);
                    switch (e.context)
                    {
                        case LogContext.Warning:
                            // create Paragraph in Yellow
                            myParagraph.Foreground = new SolidColorBrush(Colors.Yellow);
                            break;
                        case LogContext.Error:
                            // in Red
                            myParagraph.Foreground = new SolidColorBrush(Colors.Red);
                            break;
                    }

                    bool scrollBottom = IsScrolledToBottom();
                    txtLogs.Blocks.Add(myParagraph);
                    if (scrollBottom) await ScrollToBottom();
                });
        }
    }
}
