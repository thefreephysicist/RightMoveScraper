using RightMove;
using System;
using System.Collections.Generic;
using System.Text;

namespace RightMoveApp.Model
{
	public class RightMoveViewItem
	{
		private RightMoveSearchItem _item;

		public RightMoveViewItem(RightMoveSearchItem rightMoveSearchItem)
		{
			_item = rightMoveSearchItem;
		}

		public int Id
		{
			get
			{
				return _item.RightMoveId;
			}
		}

		public string Desc
		{
			get
			{
				return _item.Desc;
			}
		}

		public string HouseInfo
		{
			get
			{
				return _item.HouseInfo;
			}
		}

		public string Address
		{
			get
			{
				return _item.Address;
			}
		}

		public string Agent
		{
			get
			{
				return _item.Agent;
			}
		}

		public DateTime Date
		{
			get
			{
				return _item.Date;
			}
		}

		public int Price
		{
			get
			{
				return _item.Price;
			}
		}

		public string Url
		{
			get { return _item.Url; }
		}
	}
}
