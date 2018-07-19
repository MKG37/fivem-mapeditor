using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;

namespace MapLoader
{
    public class MapVehicle : MapEntity
    {
        public int Color { get; set; }
        public int Color2 { get; set; }
        public int Color3 { get; set; }
        public MapVehicle(Vehicle v) : base(v)
        {
        }
    }
}
