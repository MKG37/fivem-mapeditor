using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using NativeUI;
using System.Drawing;

namespace MapEditor.Menus
{
    class SpawnListMenu : UIMenu
    {
        public SpawnListMenu(string title, List<string> modelNameList) : base("", title, new Point(0, -107))
        {
            var rectangle = new UIResRectangle();
            rectangle.Color = Color.FromArgb(0, 0, 0, 0);
            SetBannerType(rectangle);
            DisableInstructionalButtons(true);
            ResetKey(UIMenu.MenuControls.Back);
            SetKey(UIMenu.MenuControls.Back, Control.FrontendPauseAlternate);

            foreach (var item in modelNameList)
            {
                string gxtLabel;
                bool ok = ItemDatabase.PropGxtDB.TryGetValue(item, out gxtLabel);
                if (ok)
                {
                    AddItem(new UIMenuItem(Game.GetGXTEntry(gxtLabel)));
                }
                else
                {
                    AddItem(new UIMenuItem(item));
                }
                
            }
        }
    }
}
