using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Controls;

namespace FileBrowser.Model {
	public class FileSystemModel {
		private readonly int _currentFileRow;
		private int _currentFileColumn;
		private readonly List<FileItem>[] _field;
		private readonly List<ListBox> _listBoxes;
		private readonly List<ScrollViewer> _scrollViewers;

		public List<FileItem>[] Field {
			get { return _field; }
		}

		public FileSystemModel(int rowCount, string currentPath, IEnumerable<ListBox> listBoxes, IEnumerable<ScrollViewer> scrollViewers) {
			if (rowCount <= 1) {
				throw new ArgumentException("row count must be 2 or more");
			}

			_currentFileRow = rowCount - 2;
			_currentFileColumn = 0;

			_field = new List<FileItem>[rowCount];
			for (int i = 0; i < rowCount; i++) {
				_field[i] = new List<FileItem>();
			}

			_listBoxes = listBoxes.ToList();
			_scrollViewers = scrollViewers.ToList();

			SetCurrentFile(currentPath);
		}

		private void EraseField() {
			foreach (var row in _field) {
				row.Clear();
			}
		}

		private void UpdateBottomLevel(string parentPath) {
			if (!Directory.Exists(parentPath)) {
				_field[_currentFileRow].Add(new FileItem(this, parentPath));
				return;
			}

			foreach (var file in Directory.GetDirectories(parentPath).Concat(Directory.GetFiles(parentPath))) {
				_field[_currentFileRow + 1].Add(new FileItem(this, file));
				_field[_currentFileRow + 1].Last().Path = file;
			}
		}

		// закидывает в строчку row файлы из директории newPath. используется для того, чтобы обновлять все линии, кроме нижней
		private void UpdateLevel(int row, string newPath) {
			if (Directory.GetParent(newPath) == null) {
				_field[row].Add(new FileItem(this, newPath));
				_field[row].Last().Path = newPath;
				return;
			}

			var parentPath = Directory.GetParent(newPath).FullName;
			var directories = Directory.GetDirectories(parentPath);
			var files = Directory.GetFiles(parentPath);

			if (row == _currentFileRow) {
				for (int i = 0; i < directories.Length; i++) {
					if (directories[i] == newPath) {
						_currentFileColumn = i;
					}
				}
			}

			foreach (var directory in directories.Concat(files)) {
				_field[row].Add(new FileItem(this, directory));
				_field[row].Last().Path = directory;
			}
		}

		public void SetCurrentFile(string path) {
			if ((File.GetAttributes(path) & FileAttributes.Directory) != FileAttributes.Directory) {
				System.Diagnostics.Process.Start(path);
				return;
			}

			if (path == "") {
				return;
			}

			try {
				Directory.GetDirectories(path);
				Directory.GetFiles(path);
			} catch (UnauthorizedAccessException) {
				return;
			}

			EraseField();
			UpdateBottomLevel(path);
			var tmpPath = path;

			for (int row = _currentFileRow; row >= 0; --row) {
				UpdateLevel(row, tmpPath);
				if (Directory.GetParent(tmpPath) == null) {
					break;
				}
				tmpPath = Directory.GetParent(tmpPath).FullName;
			}

			for (int i = 0; i < _field.Length; ++i) {
				_listBoxes[i].Items.Clear();
				foreach (var element in _field[i]) {
					_listBoxes[i].Items.Add(element);
				}
			}

			foreach (var scrollViewer in _scrollViewers) {
				scrollViewer.ScrollToHorizontalOffset(95);
			}
			_scrollViewers[_currentFileRow].ScrollToHorizontalOffset(52 * _currentFileColumn);
		}
	}
}