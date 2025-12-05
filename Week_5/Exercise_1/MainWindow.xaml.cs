using System;
using System.Threading;
using System.Windows;

namespace Exercise_1
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            StartButton.IsEnabled = false;

            Thread thread = new Thread(SlowWork);
            thread.Start();
        }

        private void SlowWork()
        {
            DateTime startTime = DateTime.Now;

            Dispatcher.Invoke(() =>
            {
                StartTimeLabel.Content = $"Start Time: {startTime:HH:mm:ss.fff}";
                NumberLabel.Content = "Number: 0";
                EndTimeLabel.Content = "End Time: Not finished yet";
            });

            for (int i = 1; i <= 100; i++)
            {
                Thread.Sleep(100);

                Dispatcher.Invoke(() =>
                {
                    NumberLabel.Content = $"Number: {i}";
                });
            }

            DateTime endTime = DateTime.Now;

            Dispatcher.Invoke(() =>
            {
                EndTimeLabel.Content = $"End Time: {endTime:HH:mm:ss.fff}";
                StartButton.IsEnabled = true;
            });
        }
    }
}

