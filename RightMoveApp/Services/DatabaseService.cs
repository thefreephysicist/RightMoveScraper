using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using Dapper;
using Microsoft.Extensions.Options;
using RightMove;

namespace RightMoveApp.Services
{
	public interface IDatabaseService
	{
		List<RightMoveProperty> LoadProperties();

		// void SaveProperties(List<RightMoveProperty> properties);
		void SaveProperty(RightMoveProperty property);
	}

	public class DatabaseService : IDatabaseService
	{
		private AppSettings _appSettings;
		
		public DatabaseService(IOptions<AppSettings> options)
		{
			_appSettings = options.Value;
		}
		
		public List<RightMoveProperty> LoadProperties()
		{
			using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
			{
				var output = cnn.Query<RightMoveProperty>("select * from Property", new DynamicParameters());
				return output.ToList();
			}
		}
		
		public void SaveProperty(RightMoveProperty property)
		{
			using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
			{
				cnn.Execute("insert into Property (RightMoveId, Link, HouseInfo, Desc, Agent, Date, Price) " +
				            "values (@RightMoveId, @Link, @HouseInfo, @Desc, @Agent, @Date, @Price)", property);
			}
		}
		
		private string LoadConnectionString(string id = "Default")
		{
			return _appSettings.ConnectionString;
		}
	}
}
