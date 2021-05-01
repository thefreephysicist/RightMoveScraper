using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RightMoveApp.Services;
using RightMoveApp.ViewModel;

namespace RightMoveApp
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private readonly IHost host;
		public static IServiceProvider ServiceProvider { get; private set; }
		
		public static class Windows
		{
			public const string MainWindow = nameof(MainWindow);
			public const string ImageWindow = nameof(ImageWindow);
		}
		
		public App()
		{
			host = Host.CreateDefaultBuilder()
				.ConfigureServices((context, services) =>
				{
					ConfigureServices(context.Configuration, services);
				})
				.Build();

			ServiceProvider = host.Services;
		}

		private void ConfigureServices(IConfiguration configuration,
			IServiceCollection services)
		{
			services.Configure<AppSettings>(configuration.GetSection(nameof(AppSettings)));
			services.AddScoped<ISampleService, SampleService>();
			services.AddScoped<IWindowService, WindowService>();

			services.AddScoped<NavigationService>(serviceProvider =>
			{
				var navigationService = new NavigationService(serviceProvider);
				navigationService.Configure(Windows.MainWindow, typeof(MainWindow));
				navigationService.Configure(Windows.ImageWindow, typeof(ImageWindow));
				return navigationService;
			});
			
			// ...
			// Register all ViewModels
			services.AddSingleton<MainViewModel>();
			services.AddTransient<ImageViewModel>();
			
			// Register all the windows of the application
			services.AddSingleton<MainWindow>();
			services.AddTransient<ImageWindow>();
		}

		protected override async void OnStartup(StartupEventArgs e)
		{
			PresentationTraceSources.Refresh();
			PresentationTraceSources.DataBindingSource.Listeners.Add(new ConsoleTraceListener());
			PresentationTraceSources.DataBindingSource.Listeners.Add(new DebugTraceListener());
			PresentationTraceSources.DataBindingSource.Switch.Level = SourceLevels.Warning | SourceLevels.Error;

			await host.StartAsync();

			/*
			var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
			mainWindow.Show();
			*/
			var navigationService = ServiceProvider.GetRequiredService<NavigationService>();
			await navigationService.ShowAsync(Windows.MainWindow);
			
			base.OnStartup(e);
		}

		protected override async void OnExit(ExitEventArgs e)
		{
			using (host)
			{
				await host.StopAsync(TimeSpan.FromSeconds(5));
			}

			base.OnExit(e);
		}
	}

	public class DebugTraceListener : TraceListener
	{
		public override void Write(string message)
		{
		}

		public override void WriteLine(string message)
		{
			// Debugger.Break();
		}
	}
}
