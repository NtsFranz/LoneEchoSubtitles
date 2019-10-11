using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace LoneEchoSubtitles
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private TextBlock text;
		private TextBlock statusText;

		private string gameDir = "C:\\Program Files\\Oculus\\Software\\Software\\ready-at-dawn-lone-echo";
		private string logPathBase;
		private string logPath;
		private DirectoryInfo directory;
		StreamReader reader;
		bool logFileFound;
		bool dataFound;
		private FolderBrowserDialog folderBrowserDialog;

		public MainWindow()
		{
			InitializeComponent();
			Start();
		}

		void Start()
		{
			text = (TextBlock)FindName("dialogTextBlock");
			statusText = (TextBlock)FindName("statusTextBlock");
			FindLogFile();
			folderBrowserDialog = new FolderBrowserDialog();

			DispatcherTimer dispatcherTimer = new DispatcherTimer();

			dispatcherTimer.Tick += new EventHandler(Update);
			dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 100);
			dispatcherTimer.Start();
		}

		void Update(object sender, EventArgs e)
		{
			FindLogFile();

			if (logFileFound)
			{
				string line = reader.ReadLine();
				if (line == null) return;

				if (!dataFound)
				{
					statusText.Foreground = new SolidColorBrush(Color.FromRgb(50, 150, 50));
					dataFound = true;
				}

				if (line.Contains("[DIALOGUE]") &&
					!line.Contains("[REQUEST]") &&
					!line.Contains("Aborting dialogue") &&
					!line.Contains("Finishing dialogue"))
				{
					text.Text = line.Substring(line.IndexOf("[DIALOGUE]") + 10);
				}
			}
		}

		void FindLogFile()
		{
			try
			{
				logPathBase = gameDir + "\\_local\\r14logs\\";

				directory = new DirectoryInfo(logPathBase);
				string newLogPath = logPathBase + directory.GetFiles().OrderByDescending(f => f.LastWriteTime).First().ToString();
				if (!newLogPath.Equals(logPath))
				{
					logPath = newLogPath;


					reader = new StreamReader(new FileStream(logPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));

					reader.ReadToEnd();

					statusText.Text = "Using log file: " + logPath;

					dataFound = false;
					logFileFound = true;
				}
			}
			catch (Exception e)
			{
				statusText.Text = "No log file found";
				statusText.Foreground = new SolidColorBrush(Color.FromRgb(150, 50, 50));
				logFileFound = false;
				dataFound = false;
			}
		}

		private void browseFiles_Click(object sender, RoutedEventArgs e)
		{
			DialogResult result = folderBrowserDialog.ShowDialog();
			if (result == System.Windows.Forms.DialogResult.OK)
			{
				gameDir = folderBrowserDialog.SelectedPath;
			}
		}
	}
}
