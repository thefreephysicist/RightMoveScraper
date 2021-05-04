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
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight;
using Microsoft.Extensions.Options;
using RightMoveApp.Helpers;
using RightMoveApp.Services;
using RightMoveApp.UserControls;

namespace RightMoveApp.ViewModel
{
	public class MainViewModel : ViewModelBase
	{
		private readonly NavigationService _navigationService;
		private readonly ISampleService _sampleService;
		private readonly AppSettings _settings;

		private RightMoveSearchItemCollection _rightMoveList;
		private string _info;

		private int _selectedImageIndex;
		private BitmapImage _displayedImage;

		public MainViewModel(NavigationService navigationService,
			ISampleService sampleService, 
			IOptions<AppSettings> options)
		{
			_navigationService = navigationService;
			_sampleService = sampleService;
			_settings = options.Value;

			InitializeCommands();

			IsNotSearching = true;
		}

		public bool IsImagesVisible
		{
			get;
			set;
		}
		
		public string Info
		{
			get =>_info;
			set => Set(ref _info, value);
		}

		public RightMoveSearchItemCollection RightMoveList
		{
			get => _rightMoveList;
			set => Set(ref _rightMoveList, value);
		}
		
		/// <summary>
		/// Gets or sets the selected <see cref="RightMoveViewItem"/>
		/// </summary>
		public RightMoveProperty RightMoveSelectedItem
		{
			get;
			set;
		}

		public RightMoveProperty RightMovePropertyFullSelectedItem
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

		public BitmapImage DisplayedImage
		{
			get => _displayedImage;
			set => Set(ref _displayedImage, value);
		}

		public bool IsNotSearching { get; set; }

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

		public ICommand LoadImageWindow
		{
			get;
			set;
		}
		
		public ICommand UpdateImages
		{
			get;
			set;
		}

		#endregion

		#region Command methods

		private Task ExecuteLoadImageWindowAsync(object obj)
		{
			return _navigationService.ShowDialogAsync(App.WindowKeys.ImageWindow, RightMoveSelectedItem.RightMoveId);
		}

		private bool CanExecuteLoadImageWindow(object arg)
		{
			return RightMoveSelectedItem != null;
		}

		private async Task ExecuteUpdateImagesAsync(object arg)
		{
			System.Diagnostics.Debug.WriteLine(RightMoveSelectedItem.RightMoveId);
			RightMovePropertyPageParser parser = new RightMovePropertyPageParser(RightMoveSelectedItem.RightMoveId);
			await parser.ParseRightMovePropertyPageAsync();
			RightMovePropertyFullSelectedItem = parser.RightMoveProperty;
			_selectedImageIndex = 0;
			await UpdateImage(RightMovePropertyFullSelectedItem);
		}

		private async Task UpdateImage(RightMoveProperty rightMoveProperty)
		{
			byte[] imageArr = rightMoveProperty.GetImage(_selectedImageIndex);
			if (imageArr is null)
			{
				return;
			}

			var bitmapImage = ImageHelper.ToImage(imageArr);
			DisplayedImage = bitmapImage;
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

			BrowserHelper.OpenWebpage(RightMoveSelectedItem.Url);
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
		/// The can execute search command
		/// </summary>
		/// <param name="parameter">the parameter</param>
		/// <returns>true if can execute, false otherwise</returns>
		private bool CanExecuteSearch(object parameter)
		{
			return true;
		}

		/// <summary>
		/// Can execute update images
		/// </summary>
		/// <param name="arg">the argument</param>
		/// <returns>true if can execute, false otherwise</returns>
		private bool CanExecuteUpdateImages(object arg)
		{
			return IsImagesVisible && RightMoveSelectedItem != null;
		}

		#endregion
		
		/// <summary>
		/// Initialize a bunch of <see cref="ICommand"/>
		/// </summary>
		private void InitializeCommands()
		{
			SearchAsyncCommand = new AsyncRelayCommand(ExecuteSearch, CanExecuteSearch);
			OpenLink = new RelayCommand(ExecuteOpenLink, CanExecuteOpenLink);
			LoadImageWindow = new AsyncRelayCommand(ExecuteLoadImageWindowAsync, CanExecuteLoadImageWindow);
			UpdateImages = new AsyncRelayCommand(ExecuteUpdateImagesAsync, CanExecuteUpdateImages);
			SearchParams = new SearchParams();
		}
	}
}
