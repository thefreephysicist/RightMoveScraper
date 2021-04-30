using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Encodings.Web;
using System.Web;

namespace RightMove
{
	public enum SortType
	{
		HighestPrice = 0,
		LowestPrice = 1,
		NewestListed = 6,
		OldestListed = 10
	}

	public class SearchParams
	{
		private class Option
		{
			public const string LocationIdentifier = "locationIdentifier";
			public const string MinBedrooms = "minBedrooms";
			public const string MaxBedrooms = "maxBedrooms";
			public const string MinPrice = "minPrice";
			public const string MaxPrice = "maxPrice";
			public const string PropertyType = "propertyTypes";
			public const string IncludeSSTC = "includeSSTC";
			public const string SortType = "sortType";
			public const string Radius = "radius";
		}

		/// <summary>
	    /// Gets or sets the selected radius
		/// </summary>
		private const double DefaultRadius = 0;

		/// <summary>
		/// Gets or sets the minimum selected bedrooms
		/// </summary>
		private const int DefaultMinBedrooms = 2;

		/// <summary>
		/// Gets or sets the maximum selected bedrooms
		/// </summary>
		private const int DefaultMaxBedrooms = 3;

		/// <summary>
		/// Gets or sets the minimum selected price
		/// </summary>
		private const int DefaultMinPrice = 150000;

		/// <summary>
		/// Gets or sets the maximum selected price
		/// </summary>
		private const int DefaultMaxPrice = 300000;

		/// <summary>
		/// Gets or sets the area code
		/// </summary>
		private const string DefaultAreaCode = "OL6";

		public SortType DefaultSort = SortType.NewestListed;

		private static readonly List<double> AllowedRadiusValues = new List<double>()
		{
			0,
			0.25,
			0.5,
			1,
			3,
			5,
			10,
			15,
			20,
			30,
			40
		};

		private double _radius;

		/// <summary>
		/// Create a new instance of the <see cref="SearchParams"/> class.
		/// </summary>
		public SearchParams()
		{
			MinBedrooms = DefaultMinBedrooms;
			MaxBedrooms = DefaultMaxBedrooms;
			MinPrice = DefaultMinPrice;
			MaxPrice = DefaultMaxPrice;
			Sort = DefaultSort;
			Radius = DefaultRadius;
		}

		/// <summary>
		/// Gets or sets the location
		/// </summary>
		public string OutcodeLocation
		{
			get;
			set;
		}
		
		/// <summary>
		/// Gets or sets the region location
		/// </summary>
		public string RegionLocation
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the minimum number of bedrooms
		/// </summary>
		public int MinBedrooms
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the maximum number of bedrooms
		/// </summary>
		public int MaxBedrooms
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the minimum price
		/// </summary>
		public int MinPrice
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the maximum price
		/// </summary>
		public int MaxPrice
		{
			get;
			set;
		}
		
		/// <summary>
		/// Gets or sets the property type
		/// </summary>
		public List<string> PropertyType
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the sort type
		/// </summary>
		public SortType Sort
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the radius
		/// </summary>
		/// <remarks>There are a set allowed values for radius in <see cref="AllowedRadiusValues"/></remarks>
		public double Radius
		{
			get
			{
				return _radius;
			}
			set
			{
				if (!AllowedRadiusValues.Contains(_radius))
				{
					throw new ArgumentException(nameof(Radius));
				}

				_radius = value;
			}
		}

		/// <summary>
		/// Generates the options string
		/// </summary>
		/// <returns>the options string</returns>
		internal string EncodeOptions()
		{
			Dictionary<string, string> options = new Dictionary<string, string>();

			if (!string.IsNullOrEmpty(OutcodeLocation))
			{
				string outcodeString = GenerateOutcodeOption(OutcodeLocation);
				if (string.IsNullOrEmpty(outcodeString))
				{
					throw new ArgumentException("invalid area code");
				}
				
				options.Add(Option.LocationIdentifier, outcodeString);
			}
			else if (!string.IsNullOrEmpty(RegionLocation))
			{
				string regionString = GenerateRegionOption(RegionLocation);
				if (string.IsNullOrEmpty(regionString))
				{
					throw new ArgumentException("invalid region code");
				}
				options.Add(Option.LocationIdentifier, regionString);
			}

			if (MinBedrooms > 0)
			{
				options.Add(Option.MinBedrooms, MinBedrooms.ToString());
			}

			if (MaxBedrooms > 0 && MaxBedrooms >= MinBedrooms)
			{
				options.Add(Option.MaxBedrooms, MaxBedrooms.ToString());
			}

			if (MinPrice > 0)
			{
				options.Add(Option.MinPrice, MinPrice.ToString());
			}

			if (MaxPrice > 0 && MaxPrice >= MinPrice)
			{
				options.Add(Option.MaxPrice, MaxPrice.ToString());
			}
			
			if (PropertyType != null && PropertyType.Count > 1)
			{
				options.Add(Option.PropertyType, string.Join(",", PropertyType));
			}

			options.Add(Option.SortType, ((int)Sort).ToString());

			if (Radius > 0)
			{
				options.Add(Option.Radius, Radius.ToString(CultureInfo.InvariantCulture));
			}

			return UrlHelper.ConvertDictionaryToEncodedOptions(options);
		}
		
		/// <summary>
		/// Generate outcode option
		/// </summary>
		/// <param name="areacode">the area code</param>
		/// <returns>the outcode option</returns>
		private string GenerateOutcodeOption(string areacode)
		{
			if (!RightMoveCodes.OutcodeDictionary.TryGetValue(OutcodeLocation, out int outcode))
			{
				return null;
			}

			return $"OUTCODE^{outcode}";
		}

		/// <summary>
		/// Generate outcode option
		/// </summary>
		/// <param name="regionCode">the area code</param>
		/// <returns>the region option</returns>
		private string GenerateRegionOption(string regionCode)
		{
			if (!RightMoveCodes.RegionDictionary.TryGetValue(regionCode, out int region))
			{
				return null;
			}

			return $"REGION^{region}";
		}

	}
}
