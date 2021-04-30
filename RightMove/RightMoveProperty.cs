﻿using System;
using System.Text;

namespace RightMove
{
	public class RightMoveProperty : IEquatable<RightMoveProperty>
	{
		/// <summary>
		/// Gets or sets the id
		/// </summary>
		public int Id
		{
			get;
			set;
		}
		
		/// <summary>
		/// Gets or sets the right move id
		/// </summary>
		public int RightMoveId 
		{
			get;
			set;
		}

		/// <summary>
		/// Checks if <see cref="other"/> is equal to this instance
		/// </summary>
		/// <param name="other">True if equal, false otherwise</param>
		/// <returns></returns>
		public bool Equals(RightMoveProperty other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return string.Equals(Link, other.Link, StringComparison.InvariantCultureIgnoreCase) && 
			       string.Equals(HouseInfo, other.HouseInfo, StringComparison.InvariantCultureIgnoreCase) && 
			       string.Equals(Address, other.Address, StringComparison.InvariantCultureIgnoreCase) && 
			       string.Equals(Desc, other.Desc, StringComparison.InvariantCultureIgnoreCase) && 
			       string.Equals(Agent, other.Agent, StringComparison.InvariantCultureIgnoreCase) && 
			       Date.Equals(other.Date) && 
			       Price == other.Price;
					
			       // Featured == other.Featured; // we choose not to make featured an equality check
		}

		/// <summary>
		/// Check if two objects are equal
		/// </summary>
		/// <param name="obj"></param>
		/// <returns>true if equal, false otherwise</returns>
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((RightMoveProperty) obj);
		}

		/// <summary>
		/// Get the hash code
		/// </summary>
		/// <returns>returns the hash code<returns>
		/// <remarks></remarks>
		public override int GetHashCode()
		{
			var hashCode = new HashCode();
			hashCode.Add(Link, StringComparer.InvariantCultureIgnoreCase);
			hashCode.Add(HouseInfo, StringComparer.InvariantCultureIgnoreCase);
			hashCode.Add(Address, StringComparer.InvariantCultureIgnoreCase);
			hashCode.Add(Desc, StringComparer.InvariantCultureIgnoreCase);
			hashCode.Add(Agent, StringComparer.InvariantCultureIgnoreCase);
			hashCode.Add(Date);
			hashCode.Add(Price);
			
			// hashCode.Add(Featured);
			return hashCode.ToHashCode();
		}

		public string Link
		{
			get;
			set;
		}

		public string HouseInfo
		{
			get;
			set;
		}

		public string Address 
		{
			get;
			set;
		}

		public string Desc
		{
			get;
			set;
		}

		public string Agent
		{
			get;
			set;
		}

		public DateTime Date
		{
			get;
			set;
		}

		public int Price
		{
			get;
			set;
		}

		public bool Featured
		{
			get;
			set;
		}

		public string Url
		{
			get
			{
				if (string.IsNullOrEmpty(Link))
				{
					return null;
				}

				return $"{RightMoveUrls.RightMoveUrl}{Link}";
			}
		}
		
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();

			sb.Append(string.Format("{0} : {1}\n", nameof(RightMoveId), RightMoveId));
			sb.Append(string.Format("{0} : {1}\n", nameof(HouseInfo), HouseInfo));
			sb.Append(string.Format("{0} : {1}\n", nameof(Address), Address));
			sb.Append(string.Format("{0} : {1}\n", nameof(Date), Date));
			sb.Append(string.Format("{0} : {1}\n", nameof(Link), Link));

			return sb.ToString();
		}
	}
}
