using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
namespace MapEditor
{
    static class ItemDatabase
    {
        public static SortedDictionary<string, int> PedDB, VehicleDB;
        public static List<String> PropDB;
        public static Dictionary<string, string> PropGxtDB;
        public static Dictionary<string, List<string>> PropDB2;

        public static void Reload()
        {
            // TODO: also load extra files declared in resource manifest
            // Vehicles
            PedDB = new SortedDictionary<string, int>();
            PropDB = new List<string>();
            PropGxtDB = new Dictionary<string, string>();
            VehicleDB = new SortedDictionary<string, int>();
            foreach (string veh in Enum.GetNames(typeof(VehicleHash)))
            {
                VehicleHash hash;
                Enum.TryParse(veh, out hash);
                if (VehicleDB.ContainsKey(veh)) continue;
                VehicleDB.Add(veh, (int)hash);
            }
            // Peds
            foreach (string ped in Enum.GetNames(typeof(PedHash)))
            {
                PedHash hash;
                Enum.TryParse(ped, out hash);
                if (PedDB.ContainsKey(ped)) continue;
                PedDB.Add(ped, (int)hash);
            }
            // Props
            string propfile = Function.Call<string>(Hash.LOAD_RESOURCE_FILE, "mapeditor", "proplist.txt");
            if (propfile != null && propfile != String.Empty)
            {
                PropDB2 = new Dictionary<string, List<string>>();
                List<string> propList = new List<string>();
                string currentCategoryName = "";
                foreach (var line in propfile.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (line.StartsWith(";"))
                    {
                        continue;
                    }
                    if (line.StartsWith("[category:"))
                    {
                        var categoryName = line.Substring(10);
                        categoryName = categoryName.Remove(categoryName.Length-1);
                        if (propList.Count != 0)
                        {
                            PropDB2.Add(currentCategoryName, propList);
                        }
                        currentCategoryName = categoryName;
                        propList = new List<string>();
                    }
                    else
                    {
                        var propEntry = line.Replace(" ", String.Empty);
                        string[] propFields = propEntry.Split(':');
                        var propName = propFields[0];
                        propList.Add(propName);
                        PropDB.Add(propName);

                        if (propFields.Length > 1)
                        {
                            var gxtLabel = propFields[1];
                            PropGxtDB[propName] = gxtLabel;
                        }
                    }

                }
                if (propList.Count != 0) // for the end of the file
                {
                     PropDB2.Add(currentCategoryName, propList);
                }
                //foreach (var item in PropDB2["whatever"])
                //{
                //    int hash;
                //    if (Int32.TryParse(item, out hash))
                //    {
                //        Debug.WriteLine($"{item}:{Utils.func_3999(hash)}");
                //    }
                //    else
                //    {
                //        Debug.WriteLine($"{item}:{Utils.func_3999(Game.GenerateHash(item))}");
                //    }

                //}
            }
            else
            {
                Debug.WriteLine("[mapeditor] failed to load proplist");
            }
        }
    }
}
