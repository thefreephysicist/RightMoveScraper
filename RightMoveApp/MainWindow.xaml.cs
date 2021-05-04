using RightMove;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Extensions.Options;
using RightMoveApp.Model;
using RightMoveApp.Services;
using RightMoveApp.UserControls;

namespace RightMoveApp
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private GridViewColumnHeader _lastHeaderClicked;
		private ListSortDirection _lastDirection = ListSortDirection.Ascending;

		public MainWindow()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Grid view column event handler clicked
		/// </summary>
		/// <param name="sender">the sender</param>
		/// <param name="e">the event args</param>
		private void GridViewColumnHeaderClickedHandler(object sender, RoutedEventArgs e)
		{
			if (!(e.OriginalSource is GridViewColumnHeader ch)) return;
			var dir = ListSortDirection.Ascending;
			if (ch == _lastHeaderClicked && _lastDirection == ListSortDirection.Ascending)
				dir = ListSortDirection.Descending;
			Sort(ch, dir);
			_lastHeaderClicked = ch; _lastDirection = dir;
		}

		/// <summary>
		/// Sort by column header
		/// </summary>
		/// <param name="ch">the column header</param>
		/// <param name="dir">the sort direction</param>
		private void Sort(GridViewColumnHeader ch, ListSortDirection dir)
		{
			var bn = (ch.Column.DisplayMemberBinding as Binding)?.Path.Path;
			bn = bn ?? ch.Column.Header as string;
			var dv = CollectionViewSource.GetDefaultView(listView.ItemsSource);
			dv.SortDescriptions.Clear();
			var sd = new SortDescription(bn, dir);
			dv.SortDescriptions.Add(sd);
			dv.Refresh();
		}
	}
}
