using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace RightMoveApp.ViewModel
{
	public class ViewModelLocator
	{
		public ViewModelLocator()
		{
		}

		public MainViewModel MainViewModel
			=> App.ServiceProvider.GetRequiredService<MainViewModel>();

		public ImageViewModel ImageViewModel
			=> App.ServiceProvider.GetRequiredService<ImageViewModel>();
	}
}
