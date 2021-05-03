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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using Microsoft.Extensions.Options;
using RightMoveApp.Services;
using RightMoveApp.UserControls;

namespace RightMoveApp.ViewModel
{
	public class MainViewModel : ViewModelBase
	{
		private readonly IWindowService _windowFactory;
		private readonly NavigationService _navigationService;
		private readonly ISampleService sampleService;
		private readonly AppSettings settings;

		public MainViewModel(NavigationService navigationService,
			ISampleService sampleService, 
			IWindowService windowService,
			IOptions<AppSettings> options)
		{
			_navigationService = navigationService;
			this.sampleService = sampleService;
			_windowFactory = windowService;
			settings = options.Value;

			// RightMoveList = new RightMoveSearchItemCollection();

			SearchAsyncCommand = new AsyncRelayCommand(ExecuteSearch, CanExecuteSearch);
			OpenLink = new RelayCommand(ExecuteOpenLink, CanExecuteOpenLink);
			ViewImages = new AsyncRelayCommand(ExecuteViewImagesAsync, CanExecuteViewImages);
			SearchParams = new SearchParams();

			IsNotSearching = true;
		}

		private string _info; 
		
		public string Info
		{
			get
			{
				return _info;
			}
			set
			{
				Set(ref _info, value);
			}
		}

		private RightMoveSearchItemCollection _rightMoveList;

		public RightMoveSearchItemCollection RightMoveList
		{
			get
			{
				return _rightMoveList;
			}
			set
			{
				Set(ref _rightMoveList, value);
			}
		}
		
		/// <summary>
		/// Gets or sets the selected <see cref="RightMoveViewItem"/>
		/// </summary>
		public RightMoveProperty RightMoveSelectedItem
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the <see cref="SearchParams"/>
		/// </summary>
		public SearchParams SearchParams
		{
			get;
			set;
		}
		
		#region Commands

		/// <summary>
		/// Gets or sets the search command
		/// </summary>
		public ICommand SearchAsyncCommand
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

		public ICommand ViewImages
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

		public bool IsNotSearching { get; set; }

		#endregion

		private Task ExecuteViewImagesAsync(object obj)
		{
			return _navigationService.ShowDialogAsync(App.Windows.ImageWindow, RightMoveSelectedItem.RightMoveId);
		}

		private bool CanExecuteViewImages(object arg)
		{
			return RightMoveSelectedItem != null;
		}
		
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
		private void OpenWebpage(RightMoveProperty item)
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
		private async Task ExecuteSearch(object parameter)
		{
			IsNotSearching = false;
			
			RightMoveParser parser = new RightMoveParser(SearchParams);
			await parser.SearchAsync();

			RightMoveList = parser.Results;

			Info = $"Average price: {parser.Results.AveragePrice.ToString("C2")}";
			
			IsNotSearching = true;
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
	}
}
