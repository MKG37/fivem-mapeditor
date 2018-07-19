using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using System.Xml;
using Newtonsoft.Json;

namespace MapLoader
{
    public class Map
    {
        public string Name;
        public string Creator;
        public string Description;

        private List<MapPed> _peds;
        private List<MapVehicle> _vehicles;
        private List<MapProp> _props;

        public Map()
        {
            Name = "Nameless Map";
            Description = "";
            Creator = Function.Call<string>(Hash.GET_PLAYER_NAME, Game.Player.Handle);
            _peds = new List<MapPed>();
            _vehicles = new List<MapVehicle>();
            _props = new List<MapProp>();
        }

        public int PedCount { get => _peds.Count; }
        public int VehicleCount { get => _vehicles.Count; }
        public int PropCount { get => _props.Count; }
        //public void AddEntityFromHandle(int handle)
        //{
        //    switch (Function.Call<int>(Hash.GET_ENTITY_TYPE, handle))
        //    {
        //        case 1:
        //            Add((Ped)Entity.FromHandle(handle));
        //            break;
        //        case 2:
        //            Add((Vehicle)Entity.FromHandle(handle));
        //            break;
        //        case 3:
        //            Add((Prop)Entity.FromHandle(handle));
        //            break;
        //        default:
        //            break;
        //    }
        //}
        public void Add(MapPed p)
        {
            _peds.Add(p);
        }
        public void Add(MapVehicle v)
        {
            _vehicles.Add(v);
        }
        public void Add(MapProp p)
        {
            p.Entity.AttachBlip();
            p.Entity.AttachedBlip.Color = BlipColor.Blue;
            _props.Add(p);
        }
        public bool ContainsMapEntity(MapEntity e)
        {
            return _peds.Contains(e) || _vehicles.Contains(e) || _props.Contains(e);
        }
        public void Remove(MapEntity e)
        {
            if (_props.Contains(e))
            {
                _props.Remove((MapProp) e);
            }
            else if (_vehicles.Contains(e))
            {
                _vehicles.Remove((MapVehicle)e);
            }
            else if (_peds.Contains(e))
            {
                _peds.Remove((MapPed) e);
            }
        }

        public void Unload()
        {
            foreach (var item in _peds)
            {
                item.Delete();
            }
            _peds.Clear();
            foreach (var item in _vehicles)
            {
                item.Delete();
            }
            _vehicles.Clear();
            foreach (var item in _props)
            {
                item.Delete();
            }
            _props.Clear();
        }
        [Obsolete]
        public string To5MEXml()
        {
            using (var sw = new System.IO.StringWriter())
            {
                //TODO: ASYNC EVERYTHING
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                using (var xw = XmlWriter.Create(sw,settings))
                {
                    xw.WriteStartElement("Map");
                    xw.WriteAttributeString("version", "5me-v1");
                    xw.WriteElementString("Name", Name);
                    xw.WriteElementString("Creator", Creator);
                    xw.WriteElementString("Description", Description);

                    xw.WriteStartElement("Peds");
                    foreach (var ped in _peds)
                    {
                        xw.WriteStartElement("Ped");
                        xw.WriteElementString("Hash", ped.Entity.Model.Hash.ToString());
                        xw.WriteElementString("Type", API.GetPedType(ped.Entity.Handle).ToString());

                        xw.WriteStartElement("Pos");
                        xw.WriteElementString("X", ped.Position.X.ToString());
                        xw.WriteElementString("Y", ped.Position.Y.ToString());
                        xw.WriteElementString("Z", ped.Position.Z.ToString());
                        xw.WriteEndElement();

                        xw.WriteStartElement("Rot");
                        xw.WriteElementString("X", ped.Rotation.X.ToString());
                        xw.WriteElementString("Y", ped.Rotation.Y.ToString());
                        xw.WriteElementString("Z", ped.Rotation.Z.ToString());
                        xw.WriteEndElement();

                        xw.WriteEndElement();
                    }
                    xw.WriteEndElement();
                    xw.WriteStartElement("Vehicles");
                    foreach (var veh in _vehicles)
                    {
                        xw.WriteStartElement("Vehicle");
                        xw.WriteElementString("Hash", veh.Entity.Model.Hash.ToString());

                        xw.WriteStartElement("Pos");
                        xw.WriteElementString("X", veh.Position.X.ToString());
                        xw.WriteElementString("Y", veh.Position.Y.ToString());
                        xw.WriteElementString("Z", veh.Position.Z.ToString());
                        xw.WriteEndElement();

                        xw.WriteStartElement("Rot");
                        xw.WriteElementString("X", veh.Rotation.X.ToString());
                        xw.WriteElementString("Y", veh.Rotation.Y.ToString());
                        xw.WriteElementString("Z", veh.Rotation.Z.ToString());
                        xw.WriteEndElement();

                        xw.WriteEndElement();
                    }
                    xw.WriteEndElement();

                    xw.WriteStartElement("Props");
                    foreach (var prop in _props)
                    {
                        xw.WriteStartElement("Prop");
                        xw.WriteElementString("Hash", prop.Entity.Model.Hash.ToString());

                        xw.WriteStartElement("Pos");
                        xw.WriteElementString("X", prop.Position.X.ToString());
                        xw.WriteElementString("Y", prop.Position.Y.ToString());
                        xw.WriteElementString("Z", prop.Position.Z.ToString());
                        xw.WriteEndElement();

                        xw.WriteStartElement("Rot");
                        xw.WriteElementString("X", prop.Rotation.X.ToString());
                        xw.WriteElementString("Y", prop.Rotation.Y.ToString());
                        xw.WriteElementString("Z", prop.Rotation.Z.ToString());
                        xw.WriteEndElement();

                        xw.WriteEndElement();
                    }
                    xw.WriteEndElement();

                    xw.WriteEndDocument();
                }
                return sw.ToString();
            }
        }

        public string ToJSON()
        {
            return "{}";
        }
    }
}

