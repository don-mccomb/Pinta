// 
// WrappingPaletteContainer.cs
//  
// Author:
//       Don McComb <don.mccomb@gmail.com>
// 
// Copyright (c) 2014 Don McComb
//
// This class is loosely based on the FlowLayoutContainer found in the Ribbons
// Mono project created during the Google Summer of Code, 2007.
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xwt;
using Xwt.Drawing;
//using Gtk;

namespace Pinta.Gui.Widgets
{
    public class WrappingPaletteContainer : Xwt.Canvas
    {
        int iconSize = 16;

		/// <summary>Default constructor.</summary>
        public WrappingPaletteContainer(int iconSize)
		{
            this.iconSize = iconSize;
		}

		/// <summary>Adds a widget after all existing widgets.</summary>
		/// <param name="w">The widget to add.</param>
		public void Append (Xwt.Widget w)
		{
			Insert (w);
		}

		/// <summary>Inserts a widget at the specified location.</summary>
		/// <param name="w">The widget to add.</param>
		/// <param name="WidgetIndex">The index (starting at 0) at which the widget must be inserted, or -1 to insert the widget after all existing widgets.</param>
		public void Insert (Xwt.Widget w)//, int WidgetIndex)
		{
			w.Visible = true;
			AddChild (w);
		}

		protected override void OnBoundsChanged ()
		{
			base.OnBoundsChanged ();

			Xwt.Rectangle childAlloc = new Rectangle ();// = allocation;
			int lineHeight = 0;
			//this.
			foreach(Widget w in Children)
			{
				if(w.Visible)
				{
					childAlloc.Width = w.Size.Width;
					childAlloc.Height = w.Size.Height;

					if(childAlloc.X != 0 && childAlloc.Right > this.Bounds.Width)
					{
						childAlloc.X = 0;
						childAlloc.Y += lineHeight;
						//preferredSize.Height += lineHeight;
						lineHeight = 0;
					}
					this.SetChildBounds (w, new Rectangle(childAlloc.X, childAlloc.Y, w.Size.Width, w.Size.Height));
					//children[i] (childAlloc);
					childAlloc.X += childAlloc.Width;
					//preferredSize.Width = Math.Max (childAlloc.X, preferredSize.Width);
					lineHeight = Math.Max ((int)childAlloc.Height, lineHeight);
				}
			}
			//preferredSize.Height += lineHeight;
		}

		protected override Size OnGetPreferredSize (SizeConstraint widthConstraint, SizeConstraint heightConstraint)
		{
			return base.OnGetPreferredSize (widthConstraint, heightConstraint);

			Size preferredSize = new Size ();

			Xwt.Rectangle childAlloc = new Rectangle ();// = allocation;
			int lineHeight = 0;
			//this.
			foreach(Widget w in Children)
			{
				if(w.Visible)
				{
					childAlloc.Width = w.WidthRequest;
					childAlloc.Height = w.HeightRequest;

					if(childAlloc.X != 0 && childAlloc.Right > widthConstraint.AvailableSize)
					{
						childAlloc.X = 0;
						childAlloc.Y += lineHeight;
						preferredSize.Height += lineHeight;
						lineHeight = 0;
					}
					this.SetChildBounds (w, new Rectangle(childAlloc.X, childAlloc.Y, w.WidthRequest, w.HeightRequest));
					//children[i] (childAlloc);
					childAlloc.X += childAlloc.Width;
					preferredSize.Width = Math.Max (childAlloc.X, preferredSize.Width);
					lineHeight = Math.Max ((int)childAlloc.Height, lineHeight);
				}
			}
			preferredSize.Height += lineHeight;

			this.MinWidth = this.MinHeight = iconSize;

			if (preferredSize.Width == 0)
				preferredSize.Width = preferredSize.Height = 50;

			Console.WriteLine("toolbox: " +preferredSize.ToString ());

			return preferredSize;
		}
    }
}
