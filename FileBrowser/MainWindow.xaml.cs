using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using FileBrowser.Model;

namespace FileBrowser {
	public partial class MainWindow {
		public FileSystemModel Model { get; private set; }

		public MainWindow() {
			InitializeComponent();
			ListBox[] listBoxes = {PreviousLevel, CurrentLevel, NextLevel};
			ScrollViewer[] scrollViewers = {PreviousLevelScrollViewer, CurrentLevelScrollViewer, NextLevelScrollViewer};
			Model = new FileSystemModel(3, "C:\\", listBoxes, scrollViewers);
		}

		private void OnSelectionChanged(object sender, SelectionChangedEventArgs e) {
			var listBox = (ListBox) sender;
			listBox.SelectedItem = null;
		}

		private void ExitButtonOnClick(object sender, MouseButtonEventArgs e) {
			Close();
		}

		private void MinimizeButtonOnClick(object sender, MouseButtonEventArgs e) {
			WindowState = WindowState.Minimized;
		}
	}
}