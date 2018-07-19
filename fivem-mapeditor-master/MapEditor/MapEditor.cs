using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using NativeUI;
using System.IO;
using System.Xml;
using MapLoader;

namespace MapEditor
{
    enum EDITOR_STATE
    {
        INACTIVE,
        FLYING,
        ADDING,
        SELECTED
    }
    enum RotationMode
    {
        Pitch,
        Roll,
        Yaw
    }
    public class MapEditor : BaseScript
    {
        private readonly MenuPool _menuPool = new MenuPool();
        private readonly UIMenu _mainMenu;
        private readonly Menus.SettingsMenu _settingsMenu;
        private readonly Menus.SpawnListMenu _pedListMenu;
        private readonly Menus.SpawnListMenu _propListMenu;
        private readonly Menus.SpawnListMenu _vehicleListMenu;

        private readonly UIMenu _mapListMenu;

        private Menus.MetadataMenu _metadataMenu;

        private readonly TimerBarPool _timerbarPool = new TimerBarPool();
        private TextTimerBar _barObjectCount = new TextTimerBar("Objects", "0");
        private TextTimerBar _barPedCount = new TextTimerBar("Peds", "0");
        private TextTimerBar _barVehicleCount = new TextTimerBar("Vehicles", "0");
#if DEBUG
        private TextTimerBar _barDebugState = new TextTimerBar("EDITOR-STATE", "");
#endif
        private EDITOR_STATE _state = EDITOR_STATE.INACTIVE;
        private Vector3 _3dcursorPos;
        private Scaleform _btnScaleform = new Scaleform("instructional_buttons");

        private RotationMode _rotationMode = RotationMode.Yaw;
        private MapEntity _selectedMapEntity;

        private Map _map;


        Vector3 startPos = new Vector3();
        Vector3 endPos = new Vector3();
        bool _snapping = false;
        Prop _cursorProp;

        private Menus.ObjectPlacingMenu _objPlacingMenu;

        public MapEditor()
        {
            #region Eventhandlers

            EventHandlers["onClientMapStart"] += new Action(delegate ()
            {
                TriggerServerEvent("mapeditor:getmaplist");
                _btnScaleform = new Scaleform("instructional_buttons"); // Needed because resource inits before gfx can be used
                SetupObjectPlacingMenu();
            });
            EventHandlers["mapeditor:load:ok"] += new Action<string>(LoadMap);

            _mapListMenu = new UIMenu("~w~Five~g~MapEditor", "~b~Load map");
            _mapListMenu.OnItemSelect += (_, item, index) =>
            {
                BaseScript.TriggerServerEvent("mapeditor:load", item.Text);
            };
            _menuPool.Add(_mapListMenu);

            EventHandlers["mapeditor:maplist"] += new Action<List<dynamic>>(delegate (List<dynamic> mapnames)
            {
                Debug.WriteLine("[mapeditor] Received maplist update from server.");
                _mapListMenu.MenuItems.Clear();
                foreach (var mapname in mapnames)
                {
                    UIMenuItem i = new UIMenuItem(mapname);
                    _mapListMenu.AddItem(i);
                }
                _mapListMenu.RefreshIndex();
            });

            EventHandlers["mapeditor:save:ok"] += new Action<dynamic>(delegate (dynamic _)
            {
                API.SetNotificationBackgroundColor(20);
                API.SetNotificationTextEntry("STRING");
                API.AddTextComponentString($"Map saved!");
                API.DrawNotification(false, false);
            });

            EventHandlers["mapeditor:save:error"] += new Action<dynamic>(delegate (dynamic _)
            {
                API.SetNotificationBackgroundColor(6);
                API.SetNotificationTextEntry("STRING");
                API.AddTextComponentString($"Could not save map!");
                API.DrawNotification(false, false);
            });

            EventHandlers["mapeditor:load:error"] += new Action<dynamic>(delegate (dynamic _)
            {
                API.SetNotificationBackgroundColor(6);
                API.SetNotificationTextEntry("STRING");
                API.AddTextComponentString($"Could not load map!");
                API.DrawNotification(false, false);
            });

            #endregion

            #region Menus
            // Main Menu
            _mainMenu = new UIMenu("~w~Five~g~MapEditor", "~b~MAIN MENU");
            _mainMenu.AddItem(new UIMenuItem("Enter/Exit the Editor"));
            _mainMenu.AddItem(new UIMenuItem("New Map", "Start a new map."));
            _mainMenu.AddItem(new UIMenuItem("Save Map", "Save the current map to the server."));
            _mainMenu.AddItem(new UIMenuItem("Load Map", "Load a map from the server."));
            _mainMenu.DisableInstructionalButtons(true);
            _mainMenu.OnItemSelect += (menu, item, index) =>
            {
                switch (index)
                {
                    case 0: // ENTER/EXIT EDITOR
                        if (Freecam.Active)
                        {
                            _cursorProp.Delete();
                            Freecam.Disable();
                            _state = EDITOR_STATE.INACTIVE;
                        }
                        else
                        {
                            Freecam.Enable();
                            _state = EDITOR_STATE.FLYING;
                        }
                        break;
                    case 1: // NEW MAP
                        // TODO: Maybe provide screen to handle: "you have unchanged changes, do you really want to start a new map?"
                        _map.Unload();
                        _map = new Map();
                        UpdateCountBars();
                        break;
                    case 2: // SAVE MAP
                        //BaseScript.TriggerServerEvent("mapeditor:save", _map.Serialize());
                        break;
                    case 3: // LOAD MAP
                        _mainMenu.Visible = false;
                        _mapListMenu.Visible = true;
                        break;
                    default:
                        break;
                }
            };
            _menuPool.Add(_mainMenu);

            //Metadata Menu
            _metadataMenu = new Menus.MetadataMenu(_map);
            _menuPool.Add(_metadataMenu);

            // Settings Menu
            _settingsMenu = new Menus.SettingsMenu();
            _menuPool.Add(_settingsMenu);

            // Adding submenus to main menu
            UIMenuItem MetadataItem = new UIMenuItem("Map Metadata", "Set the metadata for the current map");
            _mainMenu.AddItem(MetadataItem);
            MetadataItem.Activated += (wew_, lad) => {
                _metadataMenu.Rebuild(_map);
                _mainMenu.Visible = false;
                _metadataMenu.Visible = true;
            };
            UIMenuItem MainSettingsItem = new UIMenuItem("Settings");
            _mainMenu.AddItem(MainSettingsItem);
            _mainMenu.BindMenuToItem(_settingsMenu, MainSettingsItem);

            // Load props from resource files.
            ItemDatabase.Reload();

            SetupObjectPlacingMenu();

            // Vehicles
            _vehicleListMenu = new Menus.SpawnListMenu("Select Vehicle Model", ItemDatabase.VehicleDB.Keys.ToList());
            _vehicleListMenu.OnItemSelect += async (_, item, __) =>
            {
                Vehicle v = await World.CreateVehicle(new Model(ItemDatabase.VehicleDB[item.Text]), _3dcursorPos);
                if (v != null)
                {
                    _selectedMapEntity = new MapVehicle(v);
                    _selectedMapEntity.Entity.IsPositionFrozen = true;
                    _selectedMapEntity.Entity.IsInvincible = true;
                    _map.Add((MapVehicle)_selectedMapEntity);
                    UpdateCountBars();
                    _state = EDITOR_STATE.SELECTED;
                    _vehicleListMenu.Visible = false;
                }
                else
                {
                    Screen.ShowNotification(item.Text + " could not be created");
                }
            };
            _pedListMenu = new Menus.SpawnListMenu("Select Ped Model", ItemDatabase.PedDB.Keys.ToList());
            _pedListMenu.OnItemSelect += async (_, item, __) =>
            {
                Ped p = await World.CreatePed(new Model(ItemDatabase.PedDB[item.Text]), _3dcursorPos);
                if (p != null)
                {
                    _selectedMapEntity = new MapPed(p);
                    _selectedMapEntity.Entity.IsPositionFrozen = true;
                    _selectedMapEntity.Entity.IsInvincible = true;
                    _map.Add((MapPed)_selectedMapEntity);
                    Function.Call(Hash.TASK_SET_BLOCKING_OF_NON_TEMPORARY_EVENTS, _selectedMapEntity.Entity.Handle, true);
                    UpdateCountBars();
                    _state = EDITOR_STATE.SELECTED;
                    _pedListMenu.Visible = false;
                }
                else
                {
                    Screen.ShowNotification(item.Text + " could not be created");
                }
            };

            _menuPool.Add(_vehicleListMenu);
            _menuPool.Add(_pedListMenu);

            //After the entire menupool is constructed
            foreach (UIMenu menu in _menuPool.ToList())
            {
                menu.MouseControlsEnabled = false;
                menu.MouseEdgeEnabled = false;
            }
            _menuPool.RefreshIndex();

            #endregion

            // Info bars
            _timerbarPool.Add(new TextTimerBar("", ""));
            _timerbarPool.Add(_barObjectCount);
            _timerbarPool.Add(_barPedCount);
            _timerbarPool.Add(_barVehicleCount);
#if DEBUG
            _timerbarPool.Add(_barDebugState);
#endif
            // Subscribe to the tick.
            Tick += OnTick;
        }

        private void UpdateCountBars()
        {
            _barObjectCount.Text = _map.PropCount.ToString();
            _barPedCount.Text = _map.PedCount.ToString();
            _barVehicleCount.Text = _map.VehicleCount.ToString();
        }


        public async Task OnTick()
        {
            _menuPool.ProcessMenus();

            if (Game.IsControlJustPressed(0, Settings.ToggleMenuKey))
            {
                _mainMenu.Visible = !_mainMenu.Visible;
            }

            if (_state != EDITOR_STATE.INACTIVE)
            {
#if DEBUG
                _barDebugState.Text = "~r~" + _state.ToString();
                if (_snapping)
                {
                    _barDebugState.Text = "~y~SNAPPING";
                }
#endif
                if (Settings.ShowInfoBars)
                {
                    _timerbarPool.Draw();
                }

                //World.DrawLine(startPos, endPos, Color.FromArgb(123, 50, 186)); // debug show pivot line for snapping

                if (Settings.ShowBoundingBox)
                {
                    if (_selectedMapEntity != null && _selectedMapEntity.Entity.Exists())
                    {
                        Utils.DrawEntityBox(_selectedMapEntity.Entity, Color.FromArgb(255, 0, 0));
                    }
                }

                // Instructional Buttons
                if (Settings.ShowInstructionalButtons)
                {
                    if (_btnScaleform.IsLoaded)
                    {
                        _btnScaleform.CallFunction("SET_DATA_SLOT_EMPTY");
                        switch (_state)
                        {
                            case EDITOR_STATE.FLYING:
                                _btnScaleform.CallFunction("SET_DATA_SLOT", 0, Utils.InstructionalBtnFromControl(Settings.VehicleCreateKey), "Vehicle");
                                _btnScaleform.CallFunction("SET_DATA_SLOT", 1, Utils.InstructionalBtnFromControl(Settings.PedCreateKey), "Ped");
                                _btnScaleform.CallFunction("SET_DATA_SLOT", 2, Utils.InstructionalBtnFromControl(Settings.ObjectCreateKey), "Object");
                                _btnScaleform.CallFunction("SET_DATA_SLOT", 3, Utils.InstructionalBtnFromControl(Settings.SelectEntityKey), "Select Entity");
                                break;
                            case EDITOR_STATE.ADDING:
                                _btnScaleform.CallFunction("SET_DATA_SLOT", 1, Utils.InstructionalBtnFromControl(Settings.RotateCWKey), "");
                                _btnScaleform.CallFunction("SET_DATA_SLOT", 2, Utils.InstructionalBtnFromControl(Settings.RotateACWKey), "Rotate");
                                _btnScaleform.CallFunction("SET_DATA_SLOT", 3, Utils.InstructionalBtnFromControl(Settings.AlignItemKey), "Align");
                                _btnScaleform.CallFunction("SET_DATA_SLOT", 4, Utils.InstructionalBtnFromControl(Settings.PlaceItemKey), "Place");
                                break;
                            case EDITOR_STATE.SELECTED:
                                Screen.DisplayHelpTextThisFrame($"~INPUT_SELECT_WEAPON_UNARMED~ toggle translation mode \n~INPUT_SELECT_WEAPON_MELEE~ toggle rotation axis\n~INPUT_SELECT_WEAPON_SHOTGUN~ rotate over 15°\n~INPUT_FRONTEND_PAUSE_ALTERNATE~ de-select~");
                                _btnScaleform.CallFunction("SET_DATA_SLOT", 0, Utils.InstructionalBtnFromControl(Settings.TranslateZDownKey), "");
                                _btnScaleform.CallFunction("SET_DATA_SLOT", 1, Utils.InstructionalBtnFromControl(Settings.TranslateZUpKey), "Translate");
                                _btnScaleform.CallFunction("SET_DATA_SLOT", 2, Utils.InstructionalBtnFromControl(Settings.RotateCWKey), "");
                                _btnScaleform.CallFunction("SET_DATA_SLOT", 3, Utils.InstructionalBtnFromControl(Settings.RotateACWKey), $"Rotate({Enum.GetNames(typeof(RotationMode))[(int)_rotationMode]})");
                                _btnScaleform.CallFunction("SET_DATA_SLOT", 4, Utils.InstructionalBtnFromControl(Settings.CloneEntityKey), $"Clone");
                                _btnScaleform.CallFunction("SET_DATA_SLOT", 5, Utils.InstructionalBtnFromControl(Settings.DeleteEntityKey), "Delete");
                                if (Game.IsControlPressed(0, Settings.FreemoveEntity))
                                { _btnScaleform.CallFunction("SET_DATA_SLOT", 6, Utils.InstructionalBtnFromControl(Settings.AlignItemKey), "Align"); }
                                _btnScaleform.CallFunction("SET_DATA_SLOT", 7, Utils.InstructionalBtnFromControl(Settings.FreemoveEntity), "Drag Entity");
                                break;
                            default:
                                break;
                        }
                        _btnScaleform.CallFunction("DRAW_INSTRUCTIONAL_BUTTONS", -1);
                        _btnScaleform.Render2D();
                    }
                }

                // RAYCASTS BOIIIIIIIIIII
                Vector3 CastToPosition = Freecam.Position + (Freecam.ForwardVector * 150f);
                RaycastResult ray = World.Raycast(Freecam.Position, CastToPosition, IntersectOptions.Everything, _selectedMapEntity.Entity);
                _3dcursorPos = ray.DitHit ? ray.HitPosition : CastToPosition;

                if (Settings.Show3DCursor)
                {
                    Vector3 minDim, maxDim = Vector3.Zero;
                    if (_selectedMapEntity != null)
                    {
                        _selectedMapEntity.Entity.Model.GetDimensions(out minDim, out maxDim);
                    }
                    if (_cursorProp != null && _cursorProp.Exists())
                    {
                        _cursorProp.Position = _3dcursorPos;
                        _cursorProp.Heading = Freecam.Heading;
                    }
                    else
                    {
                        _cursorProp = await World.CreateProp("prop_mp_placement", _3dcursorPos, false, false);
                        _cursorProp.IsVisible = false;
                        _cursorProp.IsCollisionEnabled = false;
                    }
                    switch (_state)
                    {
                        case EDITOR_STATE.FLYING:
                            API.SetEntityLocallyVisible(_cursorProp.Handle);
                            if (ray.HitEntity != null)
                            {
                                World.DrawMarker(MarkerType.HorizontalCircleSkinny, _3dcursorPos + new Vector3(0f, 0f, 0.09f), Vector3.Zero, Vector3.Zero, Vector3.One * 4f, Color.FromArgb(163, 255, 183));
                                Utils.DrawEntityBox(ray.HitEntity, Color.FromArgb(0, 0, 255));
                            }
                            else
                            {
                                World.DrawMarker(MarkerType.HorizontalCircleSkinny, _3dcursorPos + new Vector3(0f, 0f, 0.09f), Vector3.Zero, Vector3.Zero, Vector3.One * 4f, Color.FromArgb(255, 255, 255));
                            }
                            break;
                        case EDITOR_STATE.ADDING:
                            API.SetEntityLocallyVisible(_cursorProp.Handle);
                            _cursorProp.AttachTo(_selectedMapEntity.Entity, new Vector3(0, 0, maxDim.Z));
                            //World.DrawMarker(MarkerType.HorizontalCircleSkinnyArrow, _cursorProp.Position, Vector3.Zero, new Vector3(0f, 0f, _selectedEntity.Heading), Vector3.One * 4f, Color.FromArgb(255, 255, 255));
                            break;
                        case EDITOR_STATE.SELECTED:
                            API.SetEntityLocallyVisible(_cursorProp.Handle);
                            _cursorProp.AttachTo(_selectedMapEntity.Entity, new Vector3(0, 0, maxDim.Z));
                            break;
                        default:
                            break;
                    }
                }

                if (Settings.ShowXYZAxis && _state == EDITOR_STATE.SELECTED)
                {
                    Vector3 dims = _selectedMapEntity.Entity.Model.GetDimensions() / 2;
                    if (Settings.RelativeTranslation)
                    {
                        var x = _selectedMapEntity.Position + _selectedMapEntity.Entity.RightVector * (2 + dims.X);
                        World.DrawLine(_selectedMapEntity.Position, x, Color.FromArgb(255, 0, 0));
                        var y = _selectedMapEntity.Position + _selectedMapEntity.Entity.ForwardVector * (2 + dims.Y);
                        World.DrawLine(_selectedMapEntity.Position, y, Color.FromArgb(0, 255, 0));
                        var z = _selectedMapEntity.Position + _selectedMapEntity.Entity.UpVector * (2 + dims.Z);
                        World.DrawLine(_selectedMapEntity.Position, z, Color.FromArgb(0, 0, 255));
                    }
                    else
                    {
                        World.DrawLine(_selectedMapEntity.Position, _selectedMapEntity.Position + Vector3.UnitX * dims.X, Color.FromArgb(255, 0, 0));
                        World.DrawLine(_selectedMapEntity.Position, _selectedMapEntity.Position + Vector3.UnitY * dims.Y, Color.FromArgb(0, 255, 0));
                        World.DrawLine(_selectedMapEntity.Position, _selectedMapEntity.Position + Vector3.UnitZ * dims.Z, Color.FromArgb(0, 0, 255));
                    }
                }

                // Selection marker (actually now rotation axis)
                if (Settings.ShowSelectionMarker)
                {
                    switch (_state)
                    {
                           case EDITOR_STATE.SELECTED:
                            // Code courtesy of Rockstar North
                            Vector3 vVar4 = new Vector3(-90f, 0f, 0f);
                            Vector3 vVar5 = new Vector3(1f, 1f, 1f);
                            float fVar6 = 1.25f;
                            Vector3 vVar0, vVar1;
                            _selectedMapEntity.Entity.Model.GetDimensions(out vVar0, out vVar1);
                            Vector3 vVar2 = vVar1 - vVar0;
                            Vector3 vVar9 = new Vector3(API.Absf(vVar2.X), API.Absf(vVar2.Y), API.Absf(vVar2.Z));
                            float fVar8 = 1f;
                            float fVar10 = 1f;
                            if (vVar9.X > vVar9.Y && vVar9.X > vVar9.Z)
                            {
                                fVar8 = vVar9.X;
                            }
                            else if (vVar9.Y > vVar9.X && vVar9.Y > vVar9.Z)
                            {
                                fVar8 = vVar9.Y;
                            }
                            else if (vVar9.Z > vVar9.X && vVar9.Z > vVar9.Y)
                            {
                                fVar8 = vVar9.Z;
                            }
                            if (fVar8 > 10f)
                            {
                                fVar10 = (fVar8 / 10f);
                                vVar5 = vVar5 * new Vector3(fVar10, fVar10, fVar10);
                                fVar6 = (fVar6 * fVar10);
                            }
                            Vector3 pos1, pos2, vVar3;
                    
                            
                            switch (_rotationMode)
                            {
                                case RotationMode.Pitch:
                                    vVar3 = _selectedMapEntity.Entity.GetOffsetPosition(new Vector3(1f,0f,0f)) - _selectedMapEntity.Position;
                                    pos1 = _selectedMapEntity.Entity.GetOffsetPosition(new Vector3(vVar1.X + fVar6, 0f, 0f));
                                    World.DrawMarker(MarkerType.UpsideDownCone, pos1, vVar3, vVar4, vVar5, Color.FromArgb(0, 255, 0));
                                    pos2 = _selectedMapEntity.Entity.GetOffsetPosition(new Vector3(vVar0.X - fVar6, 0f, 0f));
                                    World.DrawMarker(MarkerType.UpsideDownCone, pos2, -vVar3, vVar4, vVar5, Color.FromArgb(0, 0, 255));
                                    break;
                                case RotationMode.Roll:
                                    vVar3 = _selectedMapEntity.Entity.GetOffsetPosition(new Vector3(0f, 1f, 0f)) - _selectedMapEntity.Position;
                                    pos1 = _selectedMapEntity.Entity.GetOffsetPosition(new Vector3(0f, vVar1.Y + fVar6, 0f));
                                    World.DrawMarker(MarkerType.UpsideDownCone, pos1, vVar3, vVar4, vVar5, Color.FromArgb(0, 255, 0));
                                    pos2 = _selectedMapEntity.Entity.GetOffsetPosition(new Vector3(0f, vVar0.Y - fVar6, 0f));
                                    World.DrawMarker(MarkerType.UpsideDownCone, pos2, -vVar3, vVar4, vVar5, Color.FromArgb(0, 0, 255));
                                    break;
                                case RotationMode.Yaw:
                                    vVar3 = _selectedMapEntity.Entity.GetOffsetPosition(new Vector3(0f, 0f, 1f)) - _selectedMapEntity.Position;
                                    pos1 = _selectedMapEntity.Entity.GetOffsetPosition(new Vector3(0f, 0f, vVar1.Z + fVar6));
                                    World.DrawMarker(MarkerType.UpsideDownCone, pos1, vVar3, vVar4, vVar5, Color.FromArgb(0, 255, 0));
                                    pos2 = _selectedMapEntity.Entity.GetOffsetPosition(new Vector3(0f, 0f, vVar0.Z - fVar6));
                                    World.DrawMarker(MarkerType.UpsideDownCone, pos2, -vVar3, vVar4, vVar5, Color.FromArgb(0, 0, 255));
                                    break;
                            }
                            break;
                        default:
                            break;
                    }
                }

                // Speed Control
                float speedModifier = 0.8f;
                if (Game.IsControlPressed(0, Settings.GottaGoFastKey))
                {
                    speedModifier = 2f;
                }
                else if (Game.IsControlPressed(0, Settings.GottaGoSlowKey))
                {
                    speedModifier = 0.03f;
                }

                //Controls
                if (!_mainMenu.Visible && ! _settingsMenu.Visible)
                {
                    switch (_state)
                    {
                        case EDITOR_STATE.FLYING:
                            if (Game.IsControlJustPressed(0, Settings.SelectEntityKey))
                            {
                                if (ray.HitEntity != null)
                                {
                                    //_selectedMapEntity = ray.HitEntity;
                                    // find if this hitentity corresponds to a MapEntity, then set that as selected
                                    _state = EDITOR_STATE.SELECTED;
                                }
                            }
                            if (Game.IsControlJustPressed(0, Settings.ObjectCreateKey))
                            {
                                _objPlacingMenu.Visible = true;
                                MapProp p = new MapProp(await World.CreatePropNoOffset(new Model(_objPlacingMenu.GetSelectedPropModel()), _3dcursorPos, false));
                                p.TextureVariant = _objPlacingMenu.SelectedColorIndex;
                                _state = EDITOR_STATE.ADDING;
                            }
                            if (Game.IsControlJustPressed(0, Settings.PedCreateKey))
                            {
                                _pedListMenu.Visible = true;
                            }
                            if (Game.IsControlJustPressed(0, Settings.VehicleCreateKey))
                            {
                                _vehicleListMenu.Visible = true;
                                //Freecam.isControllable = false;
                                //String modelName = await Utils.GetCustomUserInput("Vehicle Model Name:", "", 50);
                                //Freecam.isControllable = true;
                                //if (modelName != String.Empty)
                                //{
                                //    Vehicle v = await World.CreateVehicle(new Model(modelName), _3dcursorPos);
                                //    if (v != null)
                                //    {
                                //        _selectedEntity = v;
                                //        _selectedEntity.IsPositionFrozen = true;
                                //        _selectedEntity.IsInvincible = true;
                                //        _vehiclePool.Add((Vehicle)_selectedEntity);
                                //        _map.Vehicles.Add(new MapItems.Vehicle((Vehicle)_selectedEntity));
                                //        UpdateCountBars();
                                //        _state = EDITOR_STATE.SELECTED;
                                //    }
                                //    else
                                //    {
                                //        Screen.ShowNotification(modelName + " could not be created");
                                //    }
                                //}

                            }
                            break;
                        case EDITOR_STATE.ADDING:
                            _selectedMapEntity.Entity.PositionNoOffset = _3dcursorPos;

                            if (Game.IsControlJustPressed(0, Settings.AlignItemKey))
                            {
                                if (ray.HitEntity != null)
                                {
                                    _selectedMapEntity.Entity.Quaternion = ray.HitEntity.Quaternion;
                                    //Experimental snapping stuff
                                    //Vector3 dimSnapTo = ray.HitEntity.Model.GetDimensions();
                                    //Vector3 dimSnapping = _selectedEntity.Model.GetDimensions();
                                    //float snapoffset = dimSnapTo.X / 2 + dimSnapping.X / 2; 
                                    //_selectedEntity.Position = ray.HitEntity.GetOffsetPosition(new Vector3(snapoffset,0,0));
                                    //_map.Add((Prop)_selectedEntity);
                                    //UpdateCountBars();
                                    //_objPlacingMenu.Visible = false;
                                    //_state = EDITOR_STATE.SELECTED;
                                }
                                else
                                {
                                    Vector3 rot = GameMath.DirectionToRotation(ray.SurfaceNormal,0f)+new Vector3(-90f, 0, 0);
                                    _selectedMapEntity.Rotation = rot;
                                }
                            }
                            if (Game.IsControlJustPressed(0, Settings.PlaceItemKey))
                            {
                                if (_selectedMapEntity is MapProp prop)
                                {
                                    _map.Add(prop);
                                }
                                else if (_selectedMapEntity is MapPed ped)
                                {
                                    _map.Add(ped);
                                }
                                else if (_selectedMapEntity is MapVehicle vehicle)
                                {
                                    _map.Add(vehicle);
                                }
                                UpdateCountBars();
                                _objPlacingMenu.Visible = false;
                                //_selectedEntity = await CloneEntity(_selectedEntity);
                                _state = EDITOR_STATE.SELECTED;
                            }
                            //Rotation
                            Quaternion qr = Quaternion.RotationYawPitchRoll(0f, 0f, 0.03f * speedModifier);
                            if (Game.IsControlPressed(0, Settings.RotateCWKey))
                            {
                                _selectedMapEntity.Entity.Quaternion *= qr;
                            }
                            if (Game.IsControlPressed(0, Settings.RotateACWKey))
                            {
                                qr.Invert();
                                _selectedMapEntity.Entity.Quaternion *= qr;
                            }

                            //if (Game.IsControlPressed(0, Settings.AlignToWorld))
                            //{
                            //    var normal = ray.SurfaceNormal;
                            //    var rot = GameMath.DirectionToRotation(normal, 0f);
                            //    _selectedEntity.Rotation = new Vector3(rot.X-90f, rot.Y, _selectedEntity.Rotation.Z);
                            //}
                            break;
                        case EDITOR_STATE.SELECTED:
                            if (_selectedMapEntity.Entity.Exists() && _selectedMapEntity != null)
                            {
                                //Deselecting
                                if (Game.IsControlJustPressed(0, Settings.DeselectEntityKey))
                                {
                                    _cursorProp.Delete();
                                    _snapping = false;
                                    if (Utils.IsEntityAVehicle(_selectedMapEntity.Entity) || Utils.IsEntityAPed(_selectedMapEntity.Entity))
                                    {
                                        _selectedMapEntity.Entity.IsPositionFrozen = false;
                                    }
                                    _selectedMapEntity = null;
                                    _state = EDITOR_STATE.FLYING;
                                    break;
                                }

                                //Change translation mode (relative - absolute)
                                if (Game.IsControlJustPressed(0, Settings.ChangeTranslationModeKey))
                                {
                                    Settings.RelativeTranslation = !Settings.RelativeTranslation;
                                }

                                //Clone direction
                                //if (Game.IsControlJustPressed(0, Settings.ChangeCloneDirectionKey))
                                //{
                                //    _cloneDirection++;
                                //    if ((int)_cloneDirection == Enum.GetNames(typeof(CloneDirection)).Length)
                                //    {
                                //        _cloneDirection = 0;
                                //    }
                                //}

                                //Rotation Axis
                                if (Game.IsControlJustPressed(0, Settings.ChangeRotationAxisKey))
                                {
                                    _rotationMode++;
                                    if ((int)_rotationMode == Enum.GetNames(typeof(RotationMode)).Length)
                                    {
                                        _rotationMode = 0;
                                    }
                                }

                                //Cloning
                                if (Game.IsControlPressed(0, Settings.CloneEntityKey))
                                {
                                    Vector3 dims = _selectedMapEntity.Entity.Model.GetDimensions();
                                    Vector3 movement = Vector3.Zero;
                                    if (Game.IsControlJustPressed(0, Settings.TranslateYUpKey) || Game.IsControlJustPressed(0, Settings.TranslateYDownKey))
                                    {
                                        switch (_rotationMode)
                                        {
                                            case RotationMode.Pitch: //X
                                                movement = _selectedMapEntity.Entity.RightVector * dims.X;
                                                break;
                                            case RotationMode.Roll: //Y
                                                movement = _selectedMapEntity.Entity.ForwardVector * dims.Y;
                                                break;
                                            case RotationMode.Yaw://Z
                                                movement = _selectedMapEntity.Entity.UpVector * dims.Z;
                                                break;
                                        }
                                        if (Game.IsControlJustPressed(0, Settings.TranslateYDownKey))
                                        {
                                            movement *= -1f;
                                        }
                                        if (Utils.IsEntityAPed(_selectedMapEntity.Entity))
                                        {
                                            MapPed p = (MapPed) _selectedMapEntity;
                                            MapPed clone = new MapPed(await World.CreatePed(p.Entity.Model, p.Position + movement, p.Entity.Heading));
                                            _map.Add(clone);
                                            _selectedMapEntity = clone;
                                        }
                                        else if (Utils.IsEntityAnObject(_selectedMapEntity.Entity))
                                        {
                                            MapProp p = (MapProp)_selectedMapEntity;
                                            MapProp clone = new MapProp(await World.CreatePropNoOffset(p.Entity.Model, p.Position + movement, p.Rotation, false));
                                            clone.TextureVariant = _objPlacingMenu.SelectedColorIndex;
                                            _map.Add(clone);
                                            _selectedMapEntity = clone;
                                        }
                                        else if (Utils.IsEntityAVehicle(_selectedMapEntity.Entity))
                                        {
                                            MapVehicle v = (MapVehicle)_selectedMapEntity;
                                            MapVehicle clone = new MapVehicle (await World.CreateVehicle(v.Entity.Model, v.Position + movement, v.Entity.Heading));
                                            _map.Add(clone);
                                            _selectedMapEntity = clone;
                                        }
                                        UpdateCountBars();
                                    }
                                    break; // When clone key is pressed, no other translation/rotation should be allowed
                                }

                                //Free Draggin/Moving 
                                if (Game.IsControlPressed(0, Settings.FreemoveEntity))
                                {
                                    _snapping = false;
                                    _selectedMapEntity.Position = _3dcursorPos;
                                    if (Game.IsControlJustPressed(0, Settings.AlignItemKey))
                                    {
                                        if (ray.HitEntity != null)
                                        {
                                            _selectedMapEntity.Entity.Quaternion = ray.HitEntity.Quaternion;
                                        }
                                        else
                                        {
                                            Vector3 rot = GameMath.DirectionToRotation(ray.SurfaceNormal, 0f) + new Vector3(-90f, 0, 0);
                                            _selectedMapEntity.Rotation = rot;
                                        }
                                    }
                                }
                                // Rotation copy // SNAPPING FOR NOW
                                //if (Game.IsControlJustPressed(0, Settings.SnapKey))
                                //{
                                //    if (ray.HitEntity != null)
                                //    {
                                //        var e = ray.HitEntity;
                                //        float width = (e.Model.GetDimensions().X / 2) + (_selectedEntity.Model.GetDimensions().X / 2);
                                //        var movement = -e.RightVector * width;
                                //        _selectedEntity.Position = e.Position + movement;
                                //        _selectedEntity.Rotation = e.Rotation + Vector3.UnitZ * 180f;

                                //        Vector3 pos = e.Position;
                                //        Vector3 dim = e.Model.GetDimensions();
                                //        dim = new Vector3(dim.X / 2f, dim.Y / 2f, dim.Z / 2f);
                                //        Vector3 entityOffset = e.GetOffsetPosition(new Vector3(-dim.X, -dim.Y, -dim.Z * 0f));
                                //        Vector3 entityOffset2 = e.GetOffsetPosition(new Vector3(-dim.X, dim.Y, -dim.Z * 0f));
                                //        startPos = entityOffset;
                                //        endPos = entityOffset2;
                                //        _snapping = true;

                                //    }
                                //}

                                // Rotate over 15°
                                if (Game.IsControlJustPressed(0, Settings.RotateOverAngleKey))
                                {
                                    Quaternion qa = new Quaternion();
                                    switch (_rotationMode)
                                    {
                                        case RotationMode.Pitch:
                                            qa = Quaternion.RotationYawPitchRoll(0f, MathUtil.DegreesToRadians(15f), 0f);
                                            break;
                                        case RotationMode.Roll:
                                            qa = Quaternion.RotationYawPitchRoll(MathUtil.DegreesToRadians(15f), 0f, 0f);
                                            break;
                                        case RotationMode.Yaw:
                                            qa = Quaternion.RotationYawPitchRoll(0f, 0f, MathUtil.DegreesToRadians(15f));
                                            break;
                                    }
                                    _selectedMapEntity.Entity.Quaternion *= qa;
                                }

                                //Rotation
                                Quaternion q = Quaternion.Zero;
                                switch (_rotationMode)
                                {
                                    case RotationMode.Roll:
                                        q = Quaternion.RotationYawPitchRoll(0.03f * speedModifier, 0f, 0f);
                                        break;
                                    case RotationMode.Pitch:
                                        q = Quaternion.RotationYawPitchRoll(0f, 0.03f * speedModifier, 0f);
                                        break;
                                    case RotationMode.Yaw:
                                        q = Quaternion.RotationYawPitchRoll(0f, 0f, 0.03f * speedModifier);
                                        break;
                                }
                                if (Game.IsControlPressed(0, Settings.RotateCWKey))
                                {
                                    if (_snapping)
                                    {
                                        var posOriginal = _selectedMapEntity.Position;
                                        Vector3 axis = startPos - endPos;
                                        axis.Normalize();
                                        Matrix3x3 rotMat = Matrix3x3.RotationAxis(axis, MathUtil.DegreesToRadians(speedModifier));
                                        _selectedMapEntity.Position = Utils.MultMatrixVec(rotMat, posOriginal - startPos) + startPos; // rot about 90 deg
                                        _selectedMapEntity.Rotation = new Vector3(_selectedMapEntity.Rotation.X, _selectedMapEntity.Rotation.Y - speedModifier, _selectedMapEntity.Rotation.Z);
                                    }
                                    else
                                    {
                                        _selectedMapEntity.Entity.Quaternion *= q;
                                    }
                                }
                                if (Game.IsControlPressed(0, Settings.RotateACWKey))
                                {
                                    if (_snapping)
                                    {
                                        var posOriginal = _selectedMapEntity.Position;
                                        Vector3 axis = startPos - endPos;
                                        axis.Normalize();
                                        Matrix3x3 rotMat = Matrix3x3.RotationAxis(axis, MathUtil.DegreesToRadians(-speedModifier));
                                        _selectedMapEntity.Position = Utils.MultMatrixVec(rotMat, posOriginal - startPos) + startPos; // rot about 90 deg
                                        _selectedMapEntity.Rotation = new Vector3(_selectedMapEntity.Rotation.X, _selectedMapEntity.Rotation.Y + speedModifier, _selectedMapEntity.Rotation.Z);
                                    }
                                    else
                                    {
                                        q.Invert();
                                        _selectedMapEntity.Entity.Quaternion *= q;
                                    }
                                }

                                //Translation
                                Vector3 translationVector = Vector3.Zero;

                                if (Settings.RelativeTranslation)
                                {
                                    // X
                                    if (Game.IsControlPressed(0, Settings.TranslateXUpKey))
                                    {
                                        translationVector += _selectedMapEntity.Entity.RightVector;
                                    }
                                    if (Game.IsControlPressed(0, Settings.TranslateXDownKey))
                                    {
                                        translationVector -= _selectedMapEntity.Entity.RightVector;
                                    }
                                    // Y
                                    if (Game.IsControlPressed(0, Settings.TranslateYUpKey))
                                    {
                                        translationVector += _selectedMapEntity.Entity.ForwardVector;
                                    }
                                    if (Game.IsControlPressed(0, Settings.TranslateYDownKey))
                                    {
                                        translationVector -= _selectedMapEntity.Entity.ForwardVector;
                                    }
                                    // Z
                                    if (Game.IsControlPressed(0, Settings.TranslateZUpKey))
                                    {
                                        translationVector += _selectedMapEntity.Entity.UpVector;
                                    }
                                    if (Game.IsControlPressed(0, Settings.TranslateZDownKey))
                                    {
                                        translationVector -= _selectedMapEntity.Entity.UpVector;
                                    }
                                }
                                else
                                {
                                    // X
                                    if (Game.IsControlPressed(0, Settings.TranslateXUpKey))
                                    {
                                        translationVector += Vector3.UnitX;
                                    }
                                    if (Game.IsControlPressed(0, Settings.TranslateXDownKey))
                                    {
                                        translationVector -= Vector3.UnitX;
                                    }
                                    // Y
                                    if (Game.IsControlPressed(0, Settings.TranslateYUpKey))
                                    {
                                        translationVector += Vector3.UnitY;
                                    }
                                    if (Game.IsControlPressed(0, Settings.TranslateYDownKey))
                                    {
                                        translationVector -= Vector3.UnitY;
                                    }
                                    // Z
                                    if (Game.IsControlPressed(0, Settings.TranslateZUpKey))
                                    {
                                        translationVector += Vector3.UnitZ;
                                    }
                                    if (Game.IsControlPressed(0, Settings.TranslateZDownKey))
                                    {
                                        translationVector -= Vector3.UnitZ;
                                    }
                                }

                                _selectedMapEntity.Entity.PositionNoOffset = _selectedMapEntity.Position + translationVector * speedModifier;

                                //Deletion
                                if (Game.IsControlJustPressed(0, Settings.DeleteEntityKey))
                                {
                                    if (_map.ContainsMapEntity(_selectedMapEntity))
                                    {
                                        _map.Remove(_selectedMapEntity);
                                    }
                                    _selectedMapEntity.Delete();
                                    _selectedMapEntity = null;
                                    UpdateCountBars();
                                    _state = EDITOR_STATE.FLYING;
                                }
                            }
                            else // If, for some reason, the selectedEntity doesn't exist anymore.
                            {
                                if (_map.ContainsMapEntity(_selectedMapEntity))
                                {
                                    _map.Remove(_selectedMapEntity);
                                }
                                UpdateCountBars();
                                _selectedMapEntity = null;
                                _state = EDITOR_STATE.FLYING;
                            }
                            break;
                        default:
                            break;
                    }
                }
            }

            await Task.FromResult(0);
        }

        private async Task<Entity> CloneEntity(Entity e)
        {
            Entity clone;
            switch (Function.Call<int>(Hash.GET_ENTITY_TYPE, e.Handle))
            {
                case 1:
                    clone = await World.CreatePed(e.Model, e.Position, e.Heading);
                    clone.Rotation = e.Rotation;
                    return clone;
                case 2:
                    clone = await World.CreateVehicle(e.Model, e.Position, e.Heading);
                    clone.Rotation = e.Rotation;
                    return clone;
                case 3:
                    clone = await World.CreateProp(e.Model, e.Position, false, false);
                    clone.Rotation = e.Rotation;
                    return clone;
                default:
                    throw new InvalidOperationException("Wtf this is no entity I've ever seen!");
            }
        }

        public void LoadMap(string data)
        {
        }

        private void SetupObjectPlacingMenu()
        {
            while (!API.HasThisAdditionalTextLoaded("FMMC", 7))
            {
                Function.Call(Hash.REQUEST_ADDITIONAL_TEXT, "FMMC", 7); // To request text entries used in the Creator
                Delay(0);
            }
            _objPlacingMenu = new Menus.ObjectPlacingMenu();
            _menuPool.Add(_objPlacingMenu);
            _objPlacingMenu.NewObjectSelected += async (_, objhash) =>
            {
                Vector3 rot = Vector3.Zero;
                if (_selectedMapEntity != null)
                {
                    rot = _selectedMapEntity.Rotation;
                    _selectedMapEntity.Delete();
                }
                Prop p = await World.CreatePropNoOffset(new Model(objhash), _3dcursorPos, false);
                p.Rotation = rot;
                Function.Call(Hash._SET_OBJECT_TEXTURE_VARIANT, p, _objPlacingMenu.SelectedColorIndex);
                if (objhash == Game.GenerateHash("stt_prop_hoop_constraction_01a"))
                {
                    string dict = "scr_stunts";
                    Function.Call((Hash)0x4B10CEA9187AFFE6, dict); // Probably load PTFX ASSET
                    if (Function.Call<bool>((Hash)0xC7225BF8901834B2,dict)) // probably DOES PTFX ASSET EXIST
                    {
                        Function.Call((Hash)0xFF62C471DC947844, dict); // probably SET PTFX ASSET FOR NEXT CALL
                        Function.Call((Hash)0x2FBC377D2B29B60F, "scr_stunts_fire_ring", p.Handle, new Vector3(0f, 0f, 25f), new Vector3(-12.5f, 0f, 0f), 1f, 0, 0, 0); // Sets the actual PTFX with all arguments in one call instead of seperate ones for rotation, size, etc.
                    }
                }
                else if (objhash == Game.GenerateHash("stt_prop_hoop_small_01"))
                {
                    string dict = "core";
                    Function.Call((Hash)0x4B10CEA9187AFFE6, dict);
                    if (Function.Call<bool>((Hash)0xC7225BF8901834B2, dict))
                    {
                        Function.Call((Hash)0xFF62C471DC947844, dict);
                        Function.Call((Hash)0x2FBC377D2B29B60F, "ent_amb_fire_ring", p.Handle, new Vector3(0f, 0f, 4.5f), new Vector3(0f, 0f, 90f), 3.5f, 0, 0, 0);
                    }
                }

                _selectedMapEntity = new MapProp(p);
            };
            _objPlacingMenu.NewObjectTextureVariantSelected += (_, index) =>
            {
                MapProp p = (MapProp)_selectedMapEntity;
                p.TextureVariant = index;
            };
            _objPlacingMenu.OnMenuClose += (_) =>
            {
                _selectedMapEntity.Delete();
                _selectedMapEntity = null;
                _state = EDITOR_STATE.FLYING;
            };
        }
    }
}