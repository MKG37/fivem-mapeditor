using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;

namespace MapEditor
{
    public class Freecam : BaseScript
    {
        private static Camera _cam;
        private static Vector3 _forwardVector;
        private float MouseSensitivity = 5f;
        private float speedModifier;

        public static bool Active = false;
        public static bool isControllable = true;
        public static Vector3 ForwardVector { get => _forwardVector;}
        public static Vector3 Position { get => _cam.Position; }
        public static float Heading { get => _cam.Rotation.Z; }

        public Freecam()
        {
            Tick += Update;
        }
        public static void Enable()
        {
            World.DestroyAllCameras();
            _cam = World.CreateCamera(Game.Player.Character.Position, Game.Player.Character.Rotation * new Vector3(0f,0f,1f), 60f);
            World.RenderingCamera = _cam;
            Game.Player.Character.IsPositionFrozen = true;
            Game.Player.Character.IsVisible = false;
            //Function.Call(Hash.SET_ENTITY_COLLISION, Game.PlayerPed.Handle, false, false);
            Game.Player.Character.IsCollisionEnabled = false;
            Game.Player.Character.IsInvincible = true;
            Freecam.Active = true;
        }
        public static void Disable()
        {
            //API.UnlockMinimapAngle();
            //API.UnlockMinimapPosition();
            World.RenderingCamera = null;
            Game.Player.Character.IsPositionFrozen = false;
            Game.Player.Character.IsVisible = true;
            Game.Player.Character.IsCollisionEnabled = true;
            Game.Player.Character.IsInvincible = false;
            Freecam.Active = false;
            Game.Player.Character.Position -= new Vector3(0f, 0f, Game.Player.Character.HeightAboveGround - 1f);
        }
        public async Task Update()
        {
            if (Freecam.Active)
            {
                Game.DisableAllControlsThisFrame(1);
                Screen.Hud.HideComponentThisFrame(HudComponent.WeaponIcon);
                Screen.Hud.HideComponentThisFrame(HudComponent.WeaponWheel);
                Screen.Hud.HideComponentThisFrame(HudComponent.WeaponWheelStats);
                Screen.Hud.HideComponentThisFrame(HudComponent.StreetName);
                Screen.Hud.HideComponentThisFrame(HudComponent.AreaName);
                //API.LockMinimapAngle(180 - (int) Math.Round(_cam.Rotation.Z));
                //API.LockMinimapPosition(_cam.Position.X, _cam.Position.Y);

                if (!Settings.ShowRadar)
                {
                    API.HideHudAndRadarThisFrame();
                }

                Game.Player.Character.Position = _cam.Position;
                Game.Player.Character.Rotation = _cam.Rotation;
                Game.Player.Character.IsCollisionEnabled = false;

                Vector3 rotation = _cam.Rotation;
                _forwardVector = Utils.RotationToDirection(rotation);

                if (isControllable)
                {
                    // Speed Control
                    if (Game.IsControlPressed(0, Settings.GottaGoFastKey))
                    {
                        speedModifier = 3f;
                    }
                    else if (Game.IsControlPressed(0, Settings.GottaGoSlowKey))
                    {
                        speedModifier = 0.2f;
                    }
                    else
                    {
                        speedModifier = 0.6f;
                    }

                    //Camera Position Control
                    Vector3 positionUpdate = Vector3.Zero;
                    Vector3 rightVector = Utils.RightVector(_forwardVector);
                    if (Game.IsControlPressed(0, Settings.CamForwardKey))
                    {

                        positionUpdate += _forwardVector;
                    }
                    if (Game.IsControlPressed(0, Settings.CamBackwardKey))
                    {

                        positionUpdate -= _forwardVector;
                    }
                    if (Game.IsControlPressed(0, Settings.CamLeftKey))
                    {

                        positionUpdate -= rightVector;
                    }
                    if (Game.IsControlPressed(0, Settings.CamRightKey))
                    {

                        positionUpdate += rightVector;
                    }
                    if (Game.IsControlPressed(0, Settings.CamUpKey))
                    {

                        positionUpdate += new Vector3(0f, 0f, 1f);
                    }
                    if (Game.IsControlPressed(0, Settings.CamDownKey))
                    {

                        positionUpdate += new Vector3(0f, 0f, -1f);
                    }

                    _cam.Position += positionUpdate * speedModifier * Settings.CameraSpeed;

                    // Camera Rotation Control
                    var rightAxisX = Game.GetDisabledControlNormal(0, Control.ScriptRightAxisX);
                    var rightAxisY = Game.GetDisabledControlNormal(0, Control.ScriptRightAxisY);
                    if (rightAxisX != 0 || rightAxisY != 0)
                    {
                        var newZ = rotation.Z - rightAxisX * MouseSensitivity;
                        var newX = MathUtil.Clamp(rotation.X - rightAxisY * MouseSensitivity, -89f, 89f);
                        _cam.Rotation = new Vector3(newX, 0f, newZ);
                    }
                } // end if isControllable

                await Task.FromResult(0);
            }

        }
    }
}
