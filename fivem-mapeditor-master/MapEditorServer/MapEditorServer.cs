using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using System.IO;
using System.Xml.Linq;
using System.Text.RegularExpressions;

namespace MapEditorServer
{
    public class MapEditorServer : BaseScript
    {
        List<dynamic> sent_filenames = new List<dynamic>();
        public MapEditorServer()
        {
            EventHandlers["mapeditor:save"] += new Action<Player, string>(SaveMap);
            EventHandlers["mapeditor:load"] += new Action<Player, string>(LoadMap);
            EventHandlers["mapeditor:getmaplist"] += new Action<Player>(SendMapListToPlayer);
            Tick += OnTick;
        }
        public void SendMapListToPlayer([FromSource] Player p)
        {
            Debug.WriteLine("Received getmaplist from client " + p.Name);
            p.TriggerEvent("mapeditor:maplist", sent_filenames);
        }

        public void SaveMap([FromSource] Player p, string data)
        {
            try
            {
                var xml = XDocument.Load(new StringReader(data));
                var name = xml.Descendants("Name").First().Value;
                char[] BAD_CHARS = new char[] { '\\', '/', ':', '*', '?', '"', '<', '>', '|' };
                string filename = string.Concat(name.Split(BAD_CHARS, StringSplitOptions.RemoveEmptyEntries));
                filename = filename.ToLower().Replace(' ', '-');
                Regex rgx = new Regex("[-]{2,}");
                filename = rgx.Replace(filename, "-");
                Debug.WriteLine($"[mapeditor] Saving map {filename} for player {p.Name}");
                API.SaveResourceFile("mapeditor", $@"maps/{filename}.xml", data, -1);
                p.TriggerEvent("mapeditor:save:ok");
            }
            catch (Exception)
            {
                TriggerClientEvent("mapeditor:save:error", p);
            }
        }
        public void LoadMap([FromSource] Player p, string mapname)
        {
            try
            {
                string data = API.LoadResourceFile("mapeditor",$"/maps/{mapname}");
                TriggerClientEvent("mapeditor:load:ok", data);
                //p.TriggerEvent("mapeditor:load:ok", data);
                Debug.WriteLine($"Player {p.Name} requested the load of map {mapname}. Map size: { (data.Length/1000).ToString()} kB");
            }
            catch (Exception)
            {
                Debug.WriteLine($"ERROR: Could not send map {mapname} to player {p.Name}");
                p.TriggerEvent("mapeditor:load:error");
            }
        }
        public async Task OnTick()
        {
            await BaseScript.Delay(5000);
            MapListUpdate();
        }

        public void MapListUpdate()
        {
            List<dynamic> filenames = new List<dynamic>();
            DirectoryInfo d = new DirectoryInfo("resources/mapeditor/maps");
            foreach (var file in d.GetFiles("*.xml"))
            {
                filenames.Add(file.Name);
            }
            if (!filenames.All(sent_filenames.Contains) || sent_filenames.Count != filenames.Count) // Only send updates when needed
            {
                Debug.WriteLine("Updated maplist");
                TriggerClientEvent("mapeditor:maplist", filenames); // To ALL players
                sent_filenames = filenames;
            }
        }
    }
}
