using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace RightMove
{
	/// <summary>
	/// A collection of right move items
	/// </summary>
	public class RightMoveSearchItemCollection : IEnumerable<RightMoveSearchItem>
	{
		private int _id = 0;
		
		private readonly HashSet<RightMoveSearchItem> _hash;

		public RightMoveSearchItemCollection()
		{
			_hash = new HashSet<RightMoveSearchItem>();
		}

		public RightMoveSearchItemCollection(int n)
		{
			_hash = new HashSet<RightMoveSearchItem>(n);
		}

		public RightMoveSearchItemCollection(IEnumerable<RightMoveSearchItem> lst)
		{
			_hash = new HashSet<RightMoveSearchItem>(lst);
		}

		public void Add(RightMoveSearchItem item)
		{
			if (_hash.Add(item))
			{
				item.Id = GetNextId();
			}
		}
		
		public void AddRange(RightMoveSearchItemCollection items)
		{
			_hash.UnionWith(items);
		}

		public IEnumerator<RightMoveSearchItem> GetEnumerator()
		{
			return _hash.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		private int GetNextId()
		{
			return ++_id;
		}
	}
}
