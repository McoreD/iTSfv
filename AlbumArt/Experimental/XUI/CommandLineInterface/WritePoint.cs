using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlbumArtDownloader
{
	/// <summary>
	/// Holds a console position to allow subsequent writing at that position
	/// without altering the current position.
	/// </summary>
	public class WritePoint
	{
		/// <summary>
		/// Creates a write point for the current cursor position
		/// </summary>
		public WritePoint() : this(Console.CursorLeft, Console.CursorTop) {}
		/// <summary>
		/// Creates a write point for the specified position
		/// </summary>
		public WritePoint(int x, int y)
		{
			X = x;
			Y = y;
		}

		public int X { get; set; }
		public int Y { get; set; }

		/// <summary>
		/// Sets the console cursor to this write point, and creates a disposable object
		/// that, when disposed, restores the console cursor to the current point.
		/// </summary>
		public IDisposable WriteAt()
		{
			return new WriteAtDisposable(this);
		}

		private class WriteAtDisposable : IDisposable
		{
			private WritePoint mWritePoint;
			private int mOriginX, mOriginY;
			public WriteAtDisposable(WritePoint writePoint)
			{
				mWritePoint = writePoint;
				//Keep current cursor position to restore
				mOriginX = Console.CursorLeft;
				mOriginY = Console.CursorTop;

				Console.SetCursorPosition(writePoint.X, writePoint.Y);
			}

			public void Dispose()
			{
				//Restore original cursor position
				Console.SetCursorPosition(mOriginX, mOriginY);
			}
		}
	}
}
