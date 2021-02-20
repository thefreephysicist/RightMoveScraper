using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RightMoveApp.UserControls
{
	/// <summary>
	/// Interaction logic for CustomListView.xaml
	/// </summary>
	public partial class CustomListView : UserControl
	{
		public CustomListView()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Gets or sets the ItemsSource property
		/// </summary>
		public IEnumerable ItemsSource
		{
			get { return (IEnumerable)GetValue(ItemsSourceProperty); }
			set { SetValue(ItemsSourceProperty, value); }
		}
		
		/// <summary>
		/// Gets or sets the SelectedItem property
		/// </summary>
		public object SelectedItem
		{
			get { return GetValue(SelectedItemProperty); }
			set { SetValue(SelectedItemProperty, value); }
		}

		/// <summary>
		/// Set up the SelectedItem dependency property
		/// </summary>
		public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register("SelectedItem", typeof(object), typeof(CustomListView), new FrameworkPropertyMetadata(null)
		{
			BindsTwoWayByDefault = true,
			DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
		});

		/// <summary>
		/// Set up the ItemsSource dependency property
		/// </summary>
		public static readonly DependencyProperty ItemsSourceProperty =
			DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(CustomListView), new PropertyMetadata(new PropertyChangedCallback(OnItemsSourcePropertyChanged)));

		/// <summary>
		/// The on items source proprety changed
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private static void OnItemsSourcePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			var control = sender as CustomListView;
			if (control != null)
			{
				control.OnItemsSourceChanged((IEnumerable) e.OldValue, (IEnumerable) e.NewValue);
			}
		}

		/// <summary>
		/// The ItemsSource changed event handler
		/// </summary>
		/// <param name="oldValue">the old value</param>
		/// <param name="newValue">the new value</param>
		private void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
		{
			// Remove handler for oldValue.CollectionChanged
			var oldValueINotifyCollectionChanged = oldValue as INotifyCollectionChanged;

			if (null != oldValueINotifyCollectionChanged)
			{
				oldValueINotifyCollectionChanged.CollectionChanged -= new NotifyCollectionChangedEventHandler(NewValueINotifyCollectionChanged_CollectionChanged);
			}
			
			// Add handler for newValue.CollectionChanged (if possible)
			var newValueINotifyCollectionChanged = newValue as INotifyCollectionChanged;
			if (null != newValueINotifyCollectionChanged)
			{
				newValueINotifyCollectionChanged.CollectionChanged += new NotifyCollectionChangedEventHandler(NewValueINotifyCollectionChanged_CollectionChanged);
			}
		}

		private void NewValueINotifyCollectionChanged_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			//Do your stuff here.
		}
	}
}
