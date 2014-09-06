//
// AwtDrawingExtensions.cs
//
// Author:
//       don <${AuthorEmail}>
//
// Copyright (c) 2014 don
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using Xwt;
using Xwt.Drawing;

namespace Pinta.Core
{
	public static class XwtDrawingExtensions
	{
		// Most of these functions return an affected area
		// This can be ignored if you don't need it

		public static Rectangle DrawRectangle (this Context g, Rectangle r, Color color, int lineWidth)
		{
			// Put it on a pixel line

			if (lineWidth == 1)
				r = new Rectangle (r.X + 0.5, r.Y + 0.5, r.Width-1, r.Height-1);
				

			g.Save ();

			g.Rectangle (r);

			g.SetColor (color);
			g.SetLineWidth (lineWidth);
			//g.LineCap = LineCap.Square;

			Rectangle dirty = r;
			dirty.Inflate (lineWidth / 2, lineWidth / 2);
			g.Stroke ();

			g.Restore ();

			return dirty;
		}

		public static Rectangle FillRectangle (this Context g, Rectangle r, Color color)
		{
			g.Save ();

			g.Rectangle (r);

			g.SetColor (color);

			// TODO: Find equivalent to FillExtents
			Rectangle dirty = r;

			g.Fill ();
			g.Restore ();

			return dirty;
		}

		public static bool ContainsPoint (this Rectangle r, double x, double y)
		{
			if (x < r.X || x >= r.X + r.Width)
				return false;

			if (y < r.Y || y >= r.Y + r.Height)
				return false;

			return true;
		}

		public static Cairo.Color ToCairoColor (this Xwt.Drawing.Color color)
		{
			return new Cairo.Color (color.Red, color.Green, color.Blue, color.Alpha);
		}

		public static Gdk.Color ToGdkColor (this Xwt.Drawing.Color color)
		{
			Gdk.Color c = new Gdk.Color ();
			c.Blue = (ushort)(color.Blue * ushort.MaxValue);
			c.Red = (ushort)(color.Red * ushort.MaxValue);
			c.Green = (ushort)(color.Green * ushort.MaxValue);

			return c;
		}
	}
}

