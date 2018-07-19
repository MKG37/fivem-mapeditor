using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;

namespace MapLoader
{
    public class MapEntity
    {
        //This first Entity field is not to be serialized and is only relevant when loaded in game
        public Entity Entity { get;}
        public int Hash { get { return Entity.Model.Hash; } }
        public string Name { get; set; }
        public Vector3 Position
        {
            get
            {
                return Entity.Position;
            }
            set { Entity.Position = value; }
        }
        public Vector3 Rotation
        {
            get
            {
                return Entity.Rotation;
            }
            set { Entity.Rotation = value; }
        }
        public string Guid { get; set; } = System.Guid.NewGuid().ToString();
        public bool Scripted { get; set; }
        public void Delete()
        {
            Entity.Delete();
        }

        public MapEntity(Entity e)
        {
            Entity = e;
        }
    }
}
