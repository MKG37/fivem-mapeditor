using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace MapLoader
{
    public class MapProp : MapEntity
    {
        private int texturevariant = 1;
        public int TextureVariant
        {
            get
            {
                return texturevariant;
            }
            set
            {
                texturevariant = value;
                API.SetObjectTextureVariant(Entity.Handle, value);
            }
        }
        public bool Dynamic { get; set; }
        public MapProp(Prop p) : base(p)
        {

        }
    }
}
