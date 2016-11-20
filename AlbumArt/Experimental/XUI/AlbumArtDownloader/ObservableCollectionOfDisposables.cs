using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace AlbumArtDownloader
{
	/// <summary>
	/// An observable collection that owns all its contents, and when one of its
	/// items is removed, it is disposed of.
	/// </summary>
	internal class ObservableCollectionOfDisposables<T> : ObservableCollection<T>
		where T : IDisposable
	{
		private HashSet<T> mDetachingItems = new HashSet<T>();

		protected override void ClearItems()
		{
			foreach (T item in this)
			{
				item.Dispose();
			}
			base.ClearItems();
		}
		protected override void RemoveItem(int index)
		{
			T item = this[index];
			
			base.RemoveItem(index);

			if (item != null)
			{
				if (!mDetachingItems.Remove(item))
				{
					item.Dispose();
				}
			}
		}

		/// <summary>
		/// Removes an item fro the collection without disposing of it.
		/// Responsibility for the disposal of the item is taken by the caller.
		/// </summary>
		/// <returns>True if <paramref name="item"/> was present in the collection</returns>
		public bool Detach(T item)
		{
			mDetachingItems.Add(item);
			if (!Remove(item))
			{
				//Failed to remove item, so it is no longer being detached.
				mDetachingItems.Remove(item);
				return false;
			}
			return true;
		}
	}
}
