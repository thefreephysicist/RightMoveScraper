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
		private readonly ISampleService sampleService;
		private readonly AppSettings settings;
		
		public MainWindow(ISampleService sampleService,
			IOptions<AppSettings> settings)
		{
			InitializeComponent();

			this.sampleService = sampleService;
			this.settings = settings.Value;
		}

		private GridViewColumnHeader lastHeaderClicked;
		private ListSortDirection lastDirection = ListSortDirection.Ascending;

		private void GridViewColumnHeaderClickedHandler(object sender, RoutedEventArgs e)
		{
			if (!(e.OriginalSource is GridViewColumnHeader ch)) return;
			var dir = ListSortDirection.Ascending;
			if (ch == lastHeaderClicked && lastDirection == ListSortDirection.Ascending)
				dir = ListSortDirection.Descending;
			Sort(ch, dir);
			lastHeaderClicked = ch; lastDirection = dir;
		}

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
