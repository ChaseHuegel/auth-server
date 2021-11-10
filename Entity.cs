using System;

namespace mmorpg_server
{
    public class Entity
    {
        public Guid GUID;

        public float x, y, z;
        public float rotX, rotY;

        public Entity(Guid guid)
        {
            GUID = guid;
        }
    }
}
