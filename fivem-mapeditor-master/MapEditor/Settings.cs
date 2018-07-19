using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.UI;
using CitizenFX.Core.Native;
namespace MapEditor
{
    public static class Settings
    {

        // UI Toggles
        public static bool Show3DCursor = true;
        public static bool ShowXYZAxis = true;
        public static bool ShowInstructionalButtons = true;
        public static bool ShowSelectionMarker = true;
        public static bool ShowBoundingBox = true;
        public static bool ShowInfoBars = true;
        public static bool RelativeTranslation = true;

        public static bool MoveCamToMap = true;
        public static float CameraSpeed = 1f;

        public static bool ShowRadar = true;
        public static bool NetworkObjects = false; // EXPERIMENTAL


        // UI Controls
        public static readonly Control ToggleMenuKey = Control.DropAmmo;
        
        // Map Controls
        public static readonly Control ObjectCreateKey = Control.SelectCharacterMichael;
        public static readonly Control PedCreateKey = Control.SelectCharacterFranklin;
        public static readonly Control VehicleCreateKey = Control.SelectCharacterTrevor;
        public static readonly Control SelectEntityKey = Control.Attack;
        public static readonly Control AlignItemKey = Control.VehicleDuck;
        public static readonly Control PlaceItemKey = Control.PhoneSelect;
        public static readonly Control DeselectEntityKey = Control.FrontendPauseAlternate;
        public static readonly Control FreemoveEntity = Control.Aim;
        public static readonly Control DeleteEntityKey = Control.CreatorDelete;
        public static readonly Control CloneEntityKey = Control.LookBehind;

        public static readonly Control ChangeTranslationModeKey = Control.SelectWeaponUnarmed;
        public static readonly Control ChangeRotationAxisKey = Control.SelectWeaponMelee;
        public static readonly Control RotateOverAngleKey = Control.SelectWeaponShotgun;
        public static readonly Control RotateCWKey = Control.VehicleRadioWheel;
        public static readonly Control RotateACWKey = Control.Context;
        public static readonly Control TranslateZUpKey = Control.Reload;
        public static readonly Control TranslateZDownKey = Control.VehicleExit;
        public static readonly Control TranslateXUpKey = Control.PhoneRight;
        public static readonly Control TranslateXDownKey = Control.PhoneLeft;
        public static readonly Control TranslateYUpKey = Control.PhoneUp;
        public static readonly Control TranslateYDownKey = Control.PhoneDown;

        // Camera Controls
        public static readonly Control CamForwardKey = Control.VehicleAccelerate;
        public static readonly Control CamBackwardKey = Control.VehicleBrake;
        public static readonly Control CamLeftKey = Control.VehicleMoveLeftOnly;
        public static readonly Control CamRightKey = Control.VehicleMoveRightOnly;
        public static readonly Control CamUpKey = Control.Jump;
        public static readonly Control CamDownKey = Control.VehicleSubDescend;

        // Speed Controls
        public static readonly Control GottaGoFastKey = Control.Sprint;
        public static readonly Control GottaGoSlowKey = Control.CharacterWheel;
    }
}
