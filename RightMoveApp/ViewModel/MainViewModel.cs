using RightMove;
using RightMoveApp.Model;
using RightMoveApp.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using AngleSharp;

namespace RightMoveApp.ViewModel
{
	public class MainViewModel : INotifyPropertyChanged
	{
		public MainViewModel()
		{
			RightMoveList = new ObservableCollection<RightMoveViewItem>();
			SearchCommand = new RelayCommand(ExecuteSearch, CanExecuteSearch);
			OpenLink = new RelayCommand(ExecuteOpenLink, CanExecuteOpenLink);
		}

		private void ExecuteOpenLink(object obj)
		{
			if (RightMoveSelectedItem is null)
			{
				return;
			}

			var sInfo = new System.Diagnostics.ProcessStartInfo(RightMoveSelectedItem.Url)
			{
				UseShellExecute = true,
			};
			System.Diagnostics.Process.Start(sInfo);
		}

		private bool CanExecuteOpenLink(object arg)
		{
			return true;
		}

		public ObservableCollection<RightMoveViewItem> RightMoveList 
		{
			get;
			set;
		}

		public RightMoveViewItem RightMoveSelectedItem
		{
			get;
			set;
		}
		
		/// <summary>
		/// Radius entries
		/// </summary>
		public Dictionary<double, string> RadiusEntries
		{
			get;
			set;
		} = new Dictionary<double, string>()
		{
			{0, "This area only" },
			{ 0.25, "Within 1/4 mile" },
			{ 0.5, "Within 1/2 mile" },
			{ 1, "Within 1 mile" },
			{ 3, "Within 3 miles" },
			{ 5, "Within 5 miles" },
			{ 10, "Within 10 miles" },
			{ 15, "Within 15 miles" },
			{ 20, "Within 20 miles" },
			{ 30, "Within 30 miles" },
			{ 40, "Within 40 miles" }
		};

		public List<int> Prices
		{
			get;
			set;
		} = new List<int>()
		{
			0,
			50000,
			60000,
			70000,
			80000,
			90000,
			100000,
			110000,
			120000,
			125000,
			130000,
			150000,
			200000,
			250000,
			300000
		};

		public List<int> Bedrooms
		{
			get;
			set;
		} = new List<int>()
		{
			0,
			1,
			2,
			3,
			4,
			5
		};

		public double SelectedRadius
		{
			get;
			set;
		} = 0;

		public int MinSelectedBedrooms
		{
			get;
			set;
		} = 2;

		public int MaxSelectedBedrooms
		{
			get;
			set;
		} = 3;

		public int MinSelectedPrice
		{
			get;
			set;
		} = 150000;

		public int MaxSelectedPrice
		{
			get;
			set;
		} = 300000;

		public string AreaCode
		{
			get;
			set;
		} = "OL6";

		public SortType SortTypeSelected
		{
			get;
			set;
		} = SortType.NewestListed;

		public List<string> SearchString
		{
			get
			{
				return RightMoveCodes.OutcodeDictionary.Select(o => o.Key).ToList();
			}
		}

		public string SelectedString
		{
			get;
			set;
		}

		public SearchParams SearchParams
		{
			get
			{
				SearchParams searchParams = new SearchParams()
				{
					Location = AreaCode,
					MinBedrooms = MinSelectedBedrooms,
					MaxBedrooms = MaxSelectedBedrooms,
					MinPrice = MinSelectedPrice,
					MaxPrice = MaxSelectedPrice,
					Sort = SortTypeSelected
				};

				return searchParams;
			}
		}

		public ICommand SearchCommand
		{
			get;
			set;
		}

		public ICommand OpenLink
		{
			get;
			set;
		}
		
		public ICommand SortCommand
		{
			get;
			set;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private void ExecuteSearch(object parameter)
		{
			RightMoveParser parser = new RightMoveParser(SearchParams);
			Task.Run(async () => await parser.SearchAsync()).Wait();

			RightMoveList.Clear();

			foreach (var res in parser.Results) 
			{
				RightMoveViewItem item = new RightMoveViewItem(res);
				RightMoveList.Add(item);
			}
		}

		private bool CanExecuteSearch(object parameter)
		{
			return true;
		}

		// This method is called by the Set accessor of each property.  
		// The CallerMemberName attribute that is applied to the optional propertyName  
		// parameter causes the property name of the caller to be substituted as an argument.  
		private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
