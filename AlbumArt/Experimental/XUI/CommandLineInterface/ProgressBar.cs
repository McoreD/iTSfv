using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlbumArtDownloader
{
	/// <summary>
	/// Draws a console progress bar
	/// </summary>
	public class ProgressBar
	{
		private int mWidth;
		private WritePoint mPosition;
		private double mMinimum = 0D;
		private double mMaximum = 1D;
		private double mValue = 0D;

		public ProgressBar() : this(25) { }
		public ProgressBar(int width) : this(new WritePoint(), width) { }
		public ProgressBar(WritePoint position, int width)
		{
			mPosition = position;
			mWidth = width;
			Redraw();
		}
		public int Width
		{
			get { return mWidth; }
			set
			{
				if (value < 0) throw new ArgumentException("Width can not be less than zero");
				Clear();
				mWidth = value;
				Redraw();
			}
		}

		public double Minimum
		{
			get { return mMinimum; }
			set
			{
				if (value >= Maximum) throw new ArgumentException("Minimum must be less than Maximum");
				mMinimum = value;
				mValue = Math.Max(mValue, mMinimum); //Coerce value to new minimum
				Redraw();
			}
		}

		public double Maximum
		{
			get { return mMaximum; }
			set
			{
				if (value <= Minimum) throw new ArgumentException("Maximum must be greater than Minimum");
				mMaximum = value;
				mValue = Math.Min(mValue, mMaximum); //Coerce value to new maximum
				Redraw();
			}
		}

		public double Value
		{
			get { return mValue; }
			set
			{
				if (value > Maximum || value < Minimum) throw new ArgumentException("Value must be between Minimum and Maximum");
				mValue = value;
				Redraw();
			}
		}

		public void Redraw()
		{
			if (Width < 5)
				return; //Can't draw anything at width less than 5

			using(mPosition.WriteAt())
			{
				Console.Write("[");
				if(Width < 8)
				{
					//Reduced width mode: write percentage, max of 99
					if(Width > 5) Console.Write(" "); //Padding
					int percentage = Math.Min(99, (int)Math.Round(100D * (Value - Minimum) / (Maximum - Minimum)));
					if (percentage < 10) Console.Write(" "); //Keep number width constant
					Console.Write(percentage);
					Console.Write("%");
					if(Width > 6) Console.Write(" "); //Additional padding
				}
				else
				{
					//Normal mode: write progress bar
					int filledArea = (int)Math.Round((double)(Width - 2) * (Value - Minimum) / (Maximum - Minimum));
					Console.Write(new String('■', filledArea));
					//Write empty area
					Console.Write(new String(' ', Width - filledArea - 2));
				}
				Console.Write("]");
			}
		}

		public void Clear()
		{
			using(mPosition.WriteAt())
			{
				//Write empty area
				Console.Write(new String(' ', Width));
			}
		}
	}
}
