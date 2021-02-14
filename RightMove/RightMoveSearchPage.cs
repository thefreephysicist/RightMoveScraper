using System.Collections;
using System.Collections.Generic;

namespace RightMove
{
	public class RightMoveSearchPage : IEnumerable<RightMoveSearchItem>
	{
		/// <summary>
		/// Gets the list of <see cref="RightMoveSearchItem"/> on the page
		/// </summary>
		public List<RightMoveSearchItem> RightMoveSearchItems
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the current page
		/// </summary>
		public int CurrentPage
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the page count
		/// </summary>
		public int PageCount
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the results count
		/// </summary>
		public int ResultsCount
		{
			get;
			set;
		}

		public IEnumerator<RightMoveSearchItem> GetEnumerator()
		{
			return RightMoveSearchItems.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return RightMoveSearchItems.GetEnumerator();
		}
	}
}
