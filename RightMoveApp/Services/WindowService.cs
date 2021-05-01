using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using RightMoveApp.ViewModel;

namespace RightMoveApp.Services
{
	public class WindowService : IWindowService
	{
		public void CreateImageWindow()
		{
			var imageWindow = App.ServiceProvider.GetRequiredService<ImageWindow>();
			imageWindow.Show();
			/*
			ImageWindow window = new ImageWindow()
			{
				DataContext = new ImageViewModel()
			};
			
			window.Show();
			*/
		}
	}
}
