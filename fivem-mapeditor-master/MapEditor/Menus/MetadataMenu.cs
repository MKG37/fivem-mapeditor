using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NativeUI;
using CitizenFX.Core.UI;
using CitizenFX.Core;
using MapLoader;

namespace MapEditor.Menus
{
    class MetadataMenu : UIMenu
    {
        UIMenuItem NameItem, CreatorItem, DescriptionItem;
        public MetadataMenu(Map m) : base("~w~Five~g~MapEditor", "~b~Map Metadata")
        {
            Rebuild(m);
        }

        public void Rebuild(Map m)
        {
            Clear();
            NameItem = new UIMenuItem("Edit Name", $"Currently: {m.Name}");
            NameItem.Activated += async (_, __) =>
            {
                string name = await GetCustomUserInput("Set Map Name", m.Name, 50);
                if (name != String.Empty)
                {
                    m.Name = name;
                    NameItem.Description = $"Currently: {m.Name}";
                }
            };
            AddItem(NameItem);
            CreatorItem = new UIMenuItem("Edit Creator", $"Currently: {m.Creator}");
            CreatorItem.Activated += async (_, __) =>
            {
                string name = await GetCustomUserInput("Set Map Creator", m.Creator, 50);
                if (name != String.Empty)
                {
                    m.Creator = name;
                    CreatorItem.Description = $"Currently: {m.Creator}";
                }
            };
            AddItem(CreatorItem);
            DescriptionItem = new UIMenuItem("Edit Description", $"Currently: {m.Description}");
            DescriptionItem.Activated += async (_, __) =>
            {
                string name = await GetCustomUserInput("Set Map Description", m.Description, 200);
                if (name != String.Empty)
                {
                    m.Description = name;
                    DescriptionItem.Description = $"Currently: {m.Description}";
                }
            };
            AddItem(DescriptionItem);
            RefreshIndex();
            DisableInstructionalButtons(true);
        }

        private async Task<string> GetCustomUserInput(string windowTitle, string defaultText, int maxLength)
        {
            Visible = false;
            Freecam.Active = false;
            string name = await Utils.GetCustomUserInput(windowTitle, defaultText, maxLength);
            Visible = true;
            Freecam.Active = true;
            return name;
        }

    }
}
