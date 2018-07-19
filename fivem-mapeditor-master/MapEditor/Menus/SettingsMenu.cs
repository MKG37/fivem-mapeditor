using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NativeUI;
using CitizenFX.Core.UI;

namespace MapEditor.Menus
{
    class SettingsMenu : UIMenu
    {
        public SettingsMenu() : base("~w~Five~g~MapEditor", "~b~SETTINGS")
        {
            UIMenuCheckboxItem SM_ShowRadar = new UIMenuCheckboxItem("Show Radar in editor", Settings.ShowRadar);
            SM_ShowRadar.CheckboxEvent += (_, checkd) =>
            {
                Settings.ShowRadar = checkd;
            };
            AddItem(SM_ShowRadar);

            UIMenuCheckboxItem SM_ShowAxis = new UIMenuCheckboxItem("Show XYZ Axes", Settings.ShowXYZAxis);
            SM_ShowAxis.CheckboxEvent += (_, checkd) =>
            {
                Settings.ShowXYZAxis = checkd;
            };
            AddItem(SM_ShowAxis);

            UIMenuCheckboxItem SM_ShowCursor = new UIMenuCheckboxItem("Show 3D Cursor", true);
            SM_ShowCursor.CheckboxEvent += (_, checkd) =>
            {
                Settings.Show3DCursor = checkd;
            };
            AddItem(SM_ShowCursor);

            List<dynamic> numberList = new List<dynamic> { "Very Slow", "Slow", "Normal", "Fast", "Very Fast"};
            UIMenuListItem CameraSpeedItem = new UIMenuListItem("Camera Speed", numberList, 2);
            AddItem(CameraSpeedItem);
            CameraSpeedItem.OnListChanged += (_, index) =>
            {
                float camSpeedSetting = (2f * index / (numberList.Count - 1)) + 0.05f;
                Settings.CameraSpeed = camSpeedSetting;
            };

            //UIMenuCheckboxItem SM_MoveCamToMap = new UIMenuCheckboxItem("Move Cam to Map on load", Settings.MoveCamToMap);
            //SM_MoveCamToMap.CheckboxEvent += (_, checkd) =>
            //{
            //    Settings.MoveCamToMap = checkd;
            //};
            //AddItem(SM_MoveCamToMap);

            UIMenuCheckboxItem SM_ShowInstructionalButtons = new UIMenuCheckboxItem("Show Instructional Buttons", true);
            SM_ShowInstructionalButtons.CheckboxEvent += (_, checkd) =>
            {
                Settings.ShowInstructionalButtons = checkd;
            };
            AddItem(SM_ShowInstructionalButtons);

            UIMenuCheckboxItem SM_ShowEntityCounts = new UIMenuCheckboxItem("Show Entity Counts", true);
            SM_ShowEntityCounts.CheckboxEvent += (_, checkd) =>
            {
                Settings.ShowInfoBars = checkd;
            };
            AddItem(SM_ShowEntityCounts);

            UIMenuCheckboxItem SM_ShowSelectedMarker = new UIMenuCheckboxItem("Show Rotation Axis Markers", true);
            SM_ShowSelectedMarker.CheckboxEvent += (_, checkd) =>
            {
                Settings.ShowSelectionMarker = checkd;
            };
            AddItem(SM_ShowSelectedMarker);

            UIMenuCheckboxItem SM_ShowBoundingBox = new UIMenuCheckboxItem("Show Bounding Box", true);
            SM_ShowBoundingBox.CheckboxEvent += (_, checkd) =>
            {
                Settings.ShowBoundingBox = checkd;
            };
            AddItem(SM_ShowBoundingBox);

            UIMenuCheckboxItem SM_NetwObj = new UIMenuCheckboxItem("Network Loaded Objects", Settings.NetworkObjects, "EXPERIMENTAL: Set if the created map entities should be networked.");
            SM_NetwObj.SetLeftBadge(UIMenuItem.BadgeStyle.Alert);
            SM_NetwObj.CheckboxEvent += (_, checkd) =>
            {
                Settings.NetworkObjects = checkd;
            };
            AddItem(SM_NetwObj);

            DisableInstructionalButtons(true);

        }

    }
}
