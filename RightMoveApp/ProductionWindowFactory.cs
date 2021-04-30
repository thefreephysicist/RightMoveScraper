using System;
using System.Collections.Generic;
using System.Text;
using RightMoveApp.ViewModel;

namespace RightMoveApp
{
	public class ProductionWindowFactory : IWindowFactory
	{
		public void CreateWindow()
		{
			ImageWindow window = new ImageWindow()
			{
				DataContext = new ImageViewModel()
			};
			window.Show();
		}
	}
}
