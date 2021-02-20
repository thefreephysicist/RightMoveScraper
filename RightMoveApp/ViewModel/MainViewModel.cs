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
			RightMoveList.Clear();

			SearchCommand = new RelayCommand(ExecuteSearch, CanExecuteSearch);
			OpenLink = new RelayCommand(ExecuteOpenLink, CanExecuteOpenLink);
		}

		/// <summary>
		/// Gets the observable collection of <see cref="RightMoveViewItem"/>
		/// </summary>
		public ObservableCollection<RightMoveViewItem> RightMoveList 
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the selected <see cref="RightMoveViewItem"/>
		/// </summary>
		public RightMoveViewItem RightMoveSelectedItem
		{
			get;
			set;
		}

		#region Combobox properties


		/// <summary>
		/// Radius entries bound to combobox
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

		/// <summary>
		/// Prices bound to combo box
		/// </summary>
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

		/// <summary>
		/// Bedrooms bound to combobox
		/// </summary>
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

		#endregion

		#region Selected items

		/// <summary>
		/// Gets or sets the selected radius
		/// </summary>
		public double SelectedRadius
		{
			get;
			set;
		} = 0;

		/// <summary>
		/// Gets or sets the minimum selected bedrooms
		/// </summary>
		public int MinSelectedBedrooms
		{
			get;
			set;
		} = 2;

		/// <summary>
		/// Gets or sets the maximum selected bedrooms
		/// </summary>
		public int MaxSelectedBedrooms
		{
			get;
			set;
		} = 3;

		/// <summary>
		/// Gets or sets the minimum selected price
		/// </summary>
		public int MinSelectedPrice
		{
			get;
			set;
		} = 150000;

		/// <summary>
		/// Gets or sets the maximum selected price
		/// </summary>
		public int MaxSelectedPrice
		{
			get;
			set;
		} = 300000;

		/// <summary>
		/// Gets or sets the area code
		/// </summary>
		public string AreaCode
		{
			get;
			set;
		} = "OL6";

		#endregion

		public SortType SortTypeSelected
		{
			get;
			set;
		} = SortType.NewestListed;

		/// <summary>
		/// Gets the search string
		/// </summary>
		public List<string> SearchString
		{
			get
			{
				return RightMoveCodes.OutcodeDictionary.Select(o => o.Key).ToList();
			}
		}

		/// <summary>
		/// Gets the <see cref="SearchParams"/>
		/// </summary>
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
					Sort = SortTypeSelected,
					Radius = SelectedRadius
				};

				return searchParams;
			}
		}

		#region Commands

		/// <summary>
		/// Gets or sets the search command
		/// </summary>
		public ICommand SearchCommand
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the open link command
		/// </summary>
		public ICommand OpenLink
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the sort command
		/// </summary>
		public ICommand SortCommand
		{
			get;
			set;
		}


		#endregion

		#region event handlers

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		/// <summary>
		/// Execute open link command
		/// </summary>
		/// <param name="obj">the object</param>
		private void ExecuteOpenLink(object obj)
		{
			if (RightMoveSelectedItem is null)
			{
				return;
			}

			OpenWebpage(RightMoveSelectedItem);
		}

		/// <summary>
		/// Open a webpage for the <see cref="RightMoveViewItem"/>
		/// </summary>
		/// <param name="item">The <see cref="RightMoveViewItem"/></param>
		private void OpenWebpage(RightMoveViewItem item)
		{
			var sInfo = new System.Diagnostics.ProcessStartInfo(item.Url)
			{
				UseShellExecute = true,
			};

			System.Diagnostics.Process.Start(sInfo);
		}

		/// <summary>
		/// Can execute open link command
		/// </summary>
		/// <param name="arg">the argument</param>
		/// <returns>true if can execute, false otherwise</returns>
		private bool CanExecuteOpenLink(object arg)
		{
			return true;
		}
		
		/// <summary>
		/// The execute search command
		/// </summary>
		/// <param name="parameter"></param>
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

		/// <summary>
		/// THe can execute search command
		/// </summary>
		/// <param name="parameter">the parameter</param>
		/// <returns></returns>
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
