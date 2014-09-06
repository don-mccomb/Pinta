// 
// ColorPaletteWidget.cs
//  
// Author:
//       Jonathan Pobst <monkey@jpobst.com>
// 
// Copyright (c) 2010 Jonathan Pobst
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
//using Cairo;
using Pinta.Core;
using Mono.Unix;
using Xwt;
using Xwt.Drawing;

namespace Pinta.Gui.Widgets
{
	[System.ComponentModel.ToolboxItem (true)]
	public class ColorPaletteWidget : Canvas
	{
		private Rectangle primary_rect = new Rectangle (7, 7, 30, 30);
		private Rectangle secondary_rect = new Rectangle (22, 22, 30, 30);
		private Rectangle swap_rect = new Rectangle (37, 6, 15, 15);
		private Rectangle reset_rect = new Rectangle (7, 37, 15, 15);

        const int primarySecondaryAreaSize = 60;
        const int swatchSize = 15;
        const int swatchAreaMargin = 7;

        OrientationEnum orientation;

        const int defaultNumOfJRows = 3;
        int jRows;
        int iRows;

		Xwt.Drawing.Image swapIcon;
		Xwt.Drawing.Image resetIcon;
		//private Gdk.Pixbuf swap_icon;
		//private Gdk.Pixbuf reset_icon;

		private Palette palette;

        enum OrientationEnum
        {
            Horizontal,
            Vertical
        }

		public ColorPaletteWidget ()
		{
			// Insert initialization code here.
			//this.AddEvents ((int)Gdk.EventMask.ButtonPressMask);



			swapIcon = PintaCore.Resources.GetXwtIcon ("ColorPalette.SwapIcon.png");
			resetIcon = PintaCore.Resources.GetXwtIcon ("ColorPalette.ResetIcon.png");
			palette = PintaCore.Palette.CurrentPalette;

			MinWidth = MinHeight = primarySecondaryAreaSize;

			//HasTooltip = true;
			//QueryTooltip += HandleQueryTooltip;
		}

		public void Initialize ()
		{
			PintaCore.Palette.PrimaryColorChanged += new EventHandler (Palette_ColorChanged);
			PintaCore.Palette.SecondaryColorChanged += new EventHandler (Palette_ColorChanged);
			PintaCore.Palette.CurrentPalette.PaletteChanged += new EventHandler (Palette_ColorChanged);
		}
		
		private void Palette_ColorChanged (object sender, EventArgs e)
		{
			// Color change events may be received while the widget is minimized,
			// so we only call Invalidate() if the widget is shown.
			if (this.Visible)
				QueueDraw ();

		}

		protected override void OnButtonPressed (ButtonEventArgs args)
		{
			if (swap_rect.ContainsPoint (args.X, args.Y)) {
				Cairo.Color temp = PintaCore.Palette.PrimaryColor;
				PintaCore.Palette.PrimaryColor = PintaCore.Palette.SecondaryColor;
				PintaCore.Palette.SecondaryColor = temp;
				QueueDraw ();
			} else if (reset_rect.ContainsPoint (args.X, args.Y)) {
				PintaCore.Palette.PrimaryColor = new Cairo.Color (0, 0, 0);
				PintaCore.Palette.SecondaryColor = new Cairo.Color (1, 1, 1);
				QueueDraw ();
			}

			if (primary_rect.ContainsPoint (args.X, args.Y))
			{
				SelectColorDialog csd = new SelectColorDialog ("Choose Primary Color");
				csd.SupportsAlpha = true;
				csd.Color = PintaCore.Palette.PrimaryColor.ToXwtColor ();

				bool colorSelected = csd.Run ();

				if (colorSelected)
					PintaCore.Palette.PrimaryColor = csd.Color.ToCairoColor();
			}
			else if (secondary_rect.ContainsPoint (args.X, args.Y))
			{
				SelectColorDialog csd = new SelectColorDialog ("Choose Secondary Color");
				csd.SupportsAlpha = true;
				csd.Color = PintaCore.Palette.SecondaryColor.ToXwtColor ();

				bool colorSelected = csd.Run ();

				if (colorSelected)
					PintaCore.Palette.SecondaryColor = csd.Color.ToCairoColor ();
			}

			int pal = PointToPalette ((int)args.X, (int)args.Y);

			if (pal >= 0) {
				if (args.Button == PointerButton.Right)
					PintaCore.Palette.SecondaryColor = palette[pal];
				else if (args.Button == PointerButton.Left)
					PintaCore.Palette.PrimaryColor = palette[pal];
				else {
					/*
					Gtk.ColorSelectionDialog csd = new Gtk.ColorSelectionDialog (Catalog.GetString ("Choose Palette Color"));
					csd.TransientFor = PintaCore.Chrome.MainWindow;
					csd.ColorSelection.PreviousColor = palette[pal].ToGdkColor ();
					csd.ColorSelection.CurrentColor = palette[pal].ToGdkColor ();
					csd.ColorSelection.CurrentAlpha = palette[pal].GdkColorAlpha ();
					csd.ColorSelection.HasOpacityControl = true;

					int response = csd.Run ();

					if (response == (int)Gtk.ResponseType.Ok) {
						palette[pal] = csd.ColorSelection.GetCairoColor ();
					}

					csd.Destroy ();
					*/

					SelectColorDialog csd = new SelectColorDialog ("Choose Palette Color");
					csd.SupportsAlpha = true;
					csd.Color = palette[pal].ToXwtColor ();

					bool colorSelected = csd.Run ();

					if (colorSelected)
						palette[pal] = csd.Color.ToCairoColor ();
				}

				QueueDraw ();	
			}

			base.OnButtonPressed (args);
		}

		protected override void OnDraw (Context ctx, Rectangle dirtyRect)
		{
			base.OnDraw (ctx, dirtyRect);

			if (Bounds.IsEmpty)
				return;

			// Draw Primary / Secondary Area

			ctx.FillRectangle (secondary_rect, PintaCore.Palette.SecondaryColor.ToXwtColor ());

			ctx.DrawRectangle (new Rectangle (secondary_rect.X + 1, secondary_rect.Y + 1, secondary_rect.Width - 2, secondary_rect.Height - 2), new Color (1, 1, 1), 1);
			ctx.DrawRectangle (secondary_rect, new Color (0, 0, 0), 1);

			ctx.FillRectangle (primary_rect, PintaCore.Palette.PrimaryColor.ToXwtColor ());
			ctx.DrawRectangle (new Rectangle (primary_rect.X + 1, primary_rect.Y + 1, primary_rect.Width - 2, primary_rect.Height - 2), new Color (1, 1, 1), 1);
			ctx.DrawRectangle (primary_rect, new Color (0, 0, 0), 1);

			ctx.DrawImage (swapIcon, swap_rect.Location);
			ctx.DrawImage (resetIcon, reset_rect.Location);

			// Draw color swatches

			int startI = primarySecondaryAreaSize;
			int startJ = swatchAreaMargin;

			int paletteIndex = 0;
			for (int jRow = 0; jRow < jRows; jRow++)
			{
				for (int iRow = 0; iRow < iRows; iRow++)
				{
					if (paletteIndex >= palette.Count)
						break;

					int x = (orientation == OrientationEnum.Horizontal) ? startI + iRow * swatchSize : startJ + jRow * swatchSize;
					int y = (orientation == OrientationEnum.Horizontal) ? startJ + jRow * swatchSize : startI + iRow * swatchSize;

					ctx.FillRectangle(new Rectangle(x, y, swatchSize, swatchSize), palette[paletteIndex].ToXwtColor ());

					paletteIndex++;
				}
			}
		}

		/*
		protected override bool OnExposeEvent (Gdk.EventExpose ev)
		{
			base.OnExposeEvent (ev);
			
			using (Context g = Gdk.CairoHelper.Create (GdkWindow)) {
				
                // Draw Primary / Secondary Area

				g.FillRectangle (;
			
				g.DrawRectangle (new Rectangle (secondary_rect.X + 1, secondary_rect.Y + 1, secondary_rect.Width - 2, secondary_rect.Height - 2), new Color (1, 1, 1), 1);
				g.DrawRectangle (secondary_rect, new Color (0, 0, 0), 1);
	
				g.FillRectangle (primary_rect, PintaCore.Palette.PrimaryColor);
				g.DrawRectangle (new Rectangle (primary_rect.X + 1, primary_rect.Y + 1, primary_rect.Width - 2, primary_rect.Height - 2), new Color (1, 1, 1), 1);
				g.DrawRectangle (primary_rect, new Color (0, 0, 0), 1);
	
				g.DrawPixbuf (swap_icon, swap_rect.Location ());
				g.DrawPixbuf (reset_icon, reset_rect.Location ());

                // Draw color swatches

                int startI = primarySecondaryAreaSize;
                int startJ = swatchAreaMargin;

                int paletteIndex = 0;
                for (int jRow = 0; jRow < jRows; jRow++)
                {
                    for (int iRow = 0; iRow < iRows; iRow++)
                    {
                        if (paletteIndex >= palette.Count)
                            break;

                        int x = (orientation == OrientationEnum.Horizontal) ? startI + iRow * swatchSize : startJ + jRow * swatchSize;
                        int y = (orientation == OrientationEnum.Horizontal) ? startJ + jRow * swatchSize : startI + iRow * swatchSize;

                        g.FillRectangle(new Rectangle(x, y, swatchSize, swatchSize), palette[paletteIndex]);

                        paletteIndex++;
                    }
                }
			}
			
			return true;
		}
		*/

		/*
		protected override void OnSizeRequested (ref Gtk.Requisition requisition)
		{
			// Calculate desired size here.
            requisition.Height = requisition.Width = primarySecondaryAreaSize;
		}
		*/

		protected override void OnBoundsChanged ()
		{
			base.OnBoundsChanged ();

			int iSpaceAvailable, jSpaceAvailable;

			// The orientation is horizontal when the widget is wider than it is tall
			// The direction of 'i' is the horizontal (left-to-right) when
			// widget orientation is horizontal,
			// and vertical (top-to-bottom) when widget orientation is vertical.
			// 'j' is in the other direction.
			if (Bounds.Width > Bounds.Height)
			{
				orientation = OrientationEnum.Horizontal;
				iSpaceAvailable = (int) Bounds.Width;
				jSpaceAvailable = (int) Bounds.Height;
			}
			else
			{
				orientation = OrientationEnum.Vertical;
				iSpaceAvailable = (int) Bounds.Height;
				jSpaceAvailable = (int) Bounds.Width;
			}

			iSpaceAvailable -= primarySecondaryAreaSize + swatchAreaMargin;
			jSpaceAvailable -= 2 * swatchAreaMargin;

			// Determine max number of rows that can be displayed in available area
			int maxPossibleIRows = Math.Max(1, iSpaceAvailable / swatchSize);
			int maxPossibleJRows = Math.Max(1, jSpaceAvailable / swatchSize);

			int iRowsWithDefaultNumOfJRows = (int)Math.Ceiling((double)palette.Count / defaultNumOfJRows);

			// Display palette in default configuration if space is available
			// (it looks better)
			if ((maxPossibleJRows >= defaultNumOfJRows) && (maxPossibleIRows >= iRowsWithDefaultNumOfJRows))
			{
				iRows = iRowsWithDefaultNumOfJRows;
				jRows = defaultNumOfJRows;
			}
			else // Compress palette to fit within available space
			{
				iRows = (int)Math.Ceiling((double)palette.Count / maxPossibleJRows);
				jRows = (int)Math.Ceiling((double)palette.Count / iRows);
			}
		}

		/*
        protected override void OnSizeAllocated(Gdk.Rectangle allocation)
        {
            base.OnSizeAllocated(allocation);

            int iSpaceAvailable, jSpaceAvailable;

            // The orientation is horizontal when the widget is wider than it is tall
            // The direction of 'i' is the horizontal (left-to-right) when
            // widget orientation is horizontal,
            // and vertical (top-to-bottom) when widget orientation is vertical.
            // 'j' is in the other direction.
            if (Allocation.Width > Allocation.Height)
            {
                orientation = OrientationEnum.Horizontal;
                iSpaceAvailable = Allocation.Width;
                jSpaceAvailable = Allocation.Height;
            }
            else
            {
                orientation = OrientationEnum.Vertical;
                iSpaceAvailable = Allocation.Height;
                jSpaceAvailable = Allocation.Width;
            }

            iSpaceAvailable -= primarySecondaryAreaSize + swatchAreaMargin;
            jSpaceAvailable -= 2 * swatchAreaMargin;

            // Determine max number of rows that can be displayed in available area
            int maxPossibleIRows = Math.Max(1, iSpaceAvailable / swatchSize);
            int maxPossibleJRows = Math.Max(1, jSpaceAvailable / swatchSize);

            int iRowsWithDefaultNumOfJRows = (int)Math.Ceiling((double)palette.Count / defaultNumOfJRows);

            // Display palette in default configuration if space is available
            // (it looks better)
            if ((maxPossibleJRows >= defaultNumOfJRows) && (maxPossibleIRows >= iRowsWithDefaultNumOfJRows))
            {
                iRows = iRowsWithDefaultNumOfJRows;
                jRows = defaultNumOfJRows;
            }
            else // Compress palette to fit within available space
            {
                iRows = (int)Math.Ceiling((double)palette.Count / maxPossibleJRows);
                jRows = (int)Math.Ceiling((double)palette.Count / iRows);
            }
        }
        */
		
		private int PointToPalette (int x, int y)
		{
            int i, j;

            if (orientation == OrientationEnum.Horizontal)
            {
                i = x;
                j = y;
            }
            else
            {
                i = y;
                j = x;
            }

            i -= primarySecondaryAreaSize;
            j -= swatchAreaMargin;

            // Determine the swatch position under the mouse pointer.
            int iRow = i / swatchSize;
            int jRow = j / swatchSize;

            if ((i < 0) || (iRow >= iRows) || (j < 0) || (jRow >= jRows))
            { // Mouse pointer is outside of swatch area
                return -1;
            }
            else if ((jRow * iRows + iRow) >= palette.Count)
            { // Mouse pointer is within swatch area, but not on valid color
                return -1;
            }
            else
            { // Return the palette color number under the mouse pointer
                return jRow * iRows + iRow;
            }
		}

		/*
		/// <summary>
		/// Provide a custom tooltip based on the cursor location.
		/// </summary>
		private void HandleQueryTooltip (object o, Gtk.QueryTooltipArgs args)
		{
			int x = args.X;
			int y = args.Y;
			string text = null;

			if (swap_rect.ContainsPoint (x, y)) {
				text = Catalog.GetString ("Click to switch between primary and secondary color.");
			} else if (reset_rect.ContainsPoint (x, y)) {
				text = Catalog.GetString ("Click to reset primary and secondary color.");
			} else if (primary_rect.ContainsPoint (x, y)) {
				text = Catalog.GetString ("Click to select primary color.");
			} else if (secondary_rect.ContainsPoint (x, y)) {
				text = Catalog.GetString ("Click to select secondary color.");
			} else if (PointToPalette (x, y) >= 0) {
				text = Catalog.GetString ("Left click to set primary color. Right click to set secondary color. Middle click to choose palette color.");
			}

			args.Tooltip.Text = text;
			args.RetVal = (text != null);
		}
		*/
	}
}
