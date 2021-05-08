using RightMove;
using RightMoveApp.Model;
using RightMoveApp.ViewModel.Commands;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight;
using Microsoft.Extensions.Options;
using RightMoveApp.Helpers;
using RightMoveApp.Services;

namespace RightMoveApp.ViewModel
{
	public class MainViewModel : ViewModelBase
	{
		private readonly NavigationService _navigationService;

		private RightMoveSearchItemCollection _rightMoveList;
		private string _info;

		private int _selectedImageIndex;
		private BitmapImage _displayedImage;
		private RightMoveProperty _rightMovePropertyFullSelectedItem;
		CancellationTokenSource _tokenSource = new CancellationTokenSource();

		public MainViewModel(NavigationService navigationService)
		{
			_navigationService = navigationService;
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
			get => _rightMovePropertyFullSelectedItem;
			set
			{
				Set(ref _rightMovePropertyFullSelectedItem, value);
				PrevImageCommand.RaiseCanExecuteChanged();
				NextImageCommand.RaiseCanExecuteChanged();
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="SearchParams"/>
		/// </summary>
		public SearchParams SearchParams
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the displayed image
		/// </summary>
		public BitmapImage DisplayedImage
		{
			get => _displayedImage;
			set => Set(ref _displayedImage, value);
		}

		/// <summary>
		/// Gets or sets a value indicating whether searching is occuring
		/// </summary>
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
		
		// Tried to get this AsyncCommand to work but it wouldn't
		/*
		public AsyncCommand<object> UpdateImages
		{
			get;
			set;
		}
		*/
		
		/// <summary>
		/// Update images command
		/// </summary>
		public CommandAsync<object> UpdateImages { get; set; }

		/// <summary>
		/// Gets or sets the NextImageCommand
		/// </summary>
		public AsyncCommand<object> NextImageCommand
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the PrevImageCommand
		/// </summary>
		public AsyncCommand<object> PrevImageCommand
		{
			get;
			set;
		}

		#endregion

		#region Command methods

		/// <summary>
		/// Show image window
		/// </summary>
		/// <param name="obj"></param>
		/// <returns>Task to load image window</returns>
		private Task ExecuteLoadImageWindowAsync(object obj)
		{
			return _navigationService.ShowDialogAsync(App.WindowKeys.ImageWindow, RightMoveSelectedItem.RightMoveId);
		}

		/// <summary>
		/// Can execute load image window
		/// </summary>
		/// <param name="arg"></param>
		/// <returns></returns>
		private bool CanExecuteLoadImageWindow(object arg)
		{
			return RightMoveSelectedItem != null;
		}

		private async Task ExecuteUpdateImagesNewAsync(object arg, CancellationToken cancellationToken)
		{
			System.Diagnostics.Debug.WriteLine(RightMoveSelectedItem.RightMoveId);
			
			await UpdateRightMovePropertyFullSelectedItem(cancellationToken);
			var updateImageTask = UpdateImage(_selectedImageIndex, cancellationToken);
			
			try
			{
				// await t;
				await updateImageTask;
			}
			catch (OperationCanceledException e)
			{
				Console.WriteLine($"{nameof(OperationCanceledException)} thrown with message: {e.Message}");
			}
			finally
			{
				// cancellationToken.Dispose();
			}
		}

		private async Task TempTask(CancellationToken cancellationToken)
		{
			await UpdateRightMovePropertyFullSelectedItem(cancellationToken);
			await UpdateImage(_selectedImageIndex, cancellationToken);
		}

		private async Task ExecuteUpdateImagesAsync(object arg)
		{
			System.Diagnostics.Debug.WriteLine(RightMoveSelectedItem.RightMoveId);
			System.Diagnostics.Debug.WriteLine("Started running");
			try
			{
				_tokenSource.Cancel();
				_tokenSource = new CancellationTokenSource();

				CancellationToken cancellationToken = _tokenSource.Token;
				// Task t = TempTask(cancellationToken);
				await TempTask(cancellationToken);
				System.Diagnostics.Debug.WriteLine("next statement");
				
			}
			catch (OperationCanceledException ex)
			{
				System.Diagnostics.Debug.WriteLine("Operation cancelled");
			}
			finally
			{
				// dispose of token if I ever figure it out!
				System.Diagnostics.Debug.WriteLine("Finished running");
			}
		}

		private async Task ExecuteUpdateImagesAsync(object arg, CancellationToken cancellationToken)
		{
			System.Diagnostics.Debug.WriteLine(RightMoveSelectedItem.RightMoveId);

			bool _isRunning = true;
			System.Diagnostics.Debug.WriteLine("Started running");
			try
			{
				// CancellationToken cancellationToken = _tokenSource.Token;
				// Task t = TempTask(cancellationToken);
				await TempTask(cancellationToken);
				System.Diagnostics.Debug.WriteLine("next statement");

			}
			catch (OperationCanceledException ex)
			{
				System.Diagnostics.Debug.WriteLine("Operation cancelled");
			}
			finally
			{
				_isRunning = false;
				// dispose of token if I ever figure it out!
				System.Diagnostics.Debug.WriteLine("Finished running");
			}
		}

		private async Task UpdateImage(int selectedIndex, CancellationToken cancellationToken)
		{
			try
			{
				await UpdateImage(RightMovePropertyFullSelectedItem, selectedIndex, cancellationToken);
			}
			catch (OperationCanceledException e)
			{
				Console.WriteLine($"{nameof(OperationCanceledException)} thrown with message: {e.Message}");
			}
			finally
			{
				// _tokenSource.Dispose();
			}
		}
		
		private async Task UpdateRightMovePropertyFullSelectedItem(CancellationToken cancellationToken)
		{
			_selectedImageIndex = 0;
			RightMovePropertyPageParser parser = new RightMovePropertyPageParser(RightMoveSelectedItem.RightMoveId);
			await parser.ParseRightMovePropertyPageAsync(cancellationToken);
			if (cancellationToken.IsCancellationRequested)
			{
				cancellationToken.ThrowIfCancellationRequested();
			}

			RightMovePropertyFullSelectedItem = parser.RightMoveProperty;
		}

		private async Task UpdateImage(RightMoveProperty rightMoveProperty, int selectedIndex, CancellationToken cancellationToken)
		{
			byte[] imageArr = rightMoveProperty.GetImage(selectedIndex);
			if (imageArr is null)
			{
				return;
			}

			if (cancellationToken.IsCancellationRequested)
			{
				cancellationToken.ThrowIfCancellationRequested();
			}
			
			var bitmapImage = ImageHelper.ToImage(imageArr);
			
			// freeze as accessed from non UI thread
			bitmapImage.Freeze();
			
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
			SearchAsyncCommand = new AsyncRelayCommandOld(ExecuteSearch, CanExecuteSearch);
			OpenLink = new RelayCommand(ExecuteOpenLink, CanExecuteOpenLink);
			LoadImageWindow = new AsyncRelayCommand(ExecuteLoadImageWindowAsync, CanExecuteLoadImageWindow);
			SearchParams = new SearchParams();
			// UpdateImages = new CommandAsync<object>(ExecuteUpdateImagesNewAsync, CanExecuteUpdateImages);
			// UpdateImages = new AsyncCommand<object>(ExecuteUpdateImagesAsync, CanExecuteUpdateImages);
			UpdateImages = new CommandAsync<object>(ExecuteUpdateImagesAsync, CanExecuteUpdateImages);
			PrevImageCommand = new AsyncCommand<object>(ExecuteUpdatePrevImageAsync, CanExecuteUpdatePrevImage);
			NextImageCommand = new AsyncCommand<object>(ExecuteUpdateNextImageAsync, CanExecuteUpdateNextImage);
		}

		private bool CanExecuteUpdateNextImage(object obj)
		{
			return RightMovePropertyFullSelectedItem != null && _selectedImageIndex != RightMovePropertyFullSelectedItem.ImageUrl.Length - 1;
		}

		private async Task ExecuteUpdateNextImageAsync(object arg1)
		{
			_selectedImageIndex++;
			CancellationTokenSource tokenSource = new CancellationTokenSource();
			var token = tokenSource.Token;
			await UpdateImage(RightMovePropertyFullSelectedItem, _selectedImageIndex, token);
		}

		private bool CanExecuteUpdatePrevImage(object obj)
		{
			if (RightMovePropertyFullSelectedItem is null)
			{
				return false;
			}

			return _selectedImageIndex > 0;
		}

		private async Task ExecuteUpdatePrevImageAsync(object arg1)
		{
			_selectedImageIndex--;
			CancellationTokenSource tokenSource = new CancellationTokenSource();
			var token = tokenSource.Token;
			await UpdateImage(RightMovePropertyFullSelectedItem, _selectedImageIndex, token);
		}
	}
}
