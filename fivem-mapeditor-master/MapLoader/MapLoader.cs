using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;

namespace MapLoader
{
    public class MapLoader : BaseScript
    {
        private List<Map> LoadedMaps = new List<Map>(); // This should probably be a Dict<string uuid, Map>
        public MapLoader()
        {
            EventHandlers["maploader:load"] += new Action<string>(LoadMap);
            EventHandlers["maploader:unload"] += new Action<string>(UnloadMap);
            EventHandlers["maploader:unload:all"] += new Action(UnloadAllMaps);
        }
        public void LoadMap(string json_data)
        {
            Debug.WriteLine("[maploader] LOAD MAP!");
            //PARSE JSON
            // Map m = Map(json)
            // LoadedMaps.Add(m)

        }
        public void UnloadMap(string uuid)
        {
            Debug.WriteLine("[maploader] UNLOAD MAP!");
            //get map corresponding to this uuid
            // map.Unload()
            // LoadedMaps.Remove(map)

        }
        public void UnloadAllMaps()
        {
            Debug.WriteLine("[maploader] UNLOAD ALL MAPS!");
            foreach (Map m in LoadedMaps)
            {
                m.Unload();
            }
            LoadedMaps.Clear();
        }
    }
}
