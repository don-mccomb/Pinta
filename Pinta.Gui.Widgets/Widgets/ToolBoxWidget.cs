using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gtk;
using Pinta.Core;
using Xwt;

namespace Pinta.Gui.Widgets
{
	[System.ComponentModel.ToolboxItem (true)]
	public class ToolBoxWidget : WrappingPaletteContainer
	{
		public ToolBoxWidget () : base(16)
		{
			PintaCore.Tools.ToolAdded += HandleToolAdded;
			PintaCore.Tools.ToolRemoved += HandleToolRemoved;

			//ShowAll ();
		}
		
		// TODO: This should handle sorting the items
		public void AddItem (Xwt.ToggleButton item)
		{
            Append(item);
			item.Show ();
		}

		public void RemoveItem (Xwt.ToggleButton item)
		{
			//Run a remove on both tables since it might be in either-

            RemoveChild(item);
		}

		private void HandleToolAdded (object sender, ToolEventArgs e)
		{
			Append (e.Tool.ToolItem);
			e.Tool.ToolItem.Show ();
		}

		private void HandleToolRemoved (object sender, ToolEventArgs e)
		{
			RemoveChild (e.Tool.ToolItem);
		}
	}
}
