using System;
using System.Collections.Generic;

namespace mmorpg_server
{
    public static class Demo
    {
        private static Random Random = new Random();
        private static List<Entity> npcs = new List<Entity>();

        public static void Initialize()
        {
            for (int i = 0; i < 20; i++)
            {
                Entity entity = new Entity(Guid.NewGuid());

                entity.y = 11;
                entity.x = (float) (Random.NextDouble() * 100d - 50d);
                entity.z = (float) (Random.NextDouble() * 100d - 50d);

                entity.rotY = (float) (Random.NextDouble() * 360d - 180d);

                npcs.Add(entity);
            }

            Server.GetEntities().AddRange(npcs);
        }

        public static void Tick(float deltaTime)
        {
            foreach (Entity entity in npcs)
            {
                entity.rotY += deltaTime * 10f;
                if (entity.rotY >= 180f)
                    entity.rotY -= 360f;

                float[] direction = DirectionFromDegrees(entity.rotY);
                
                entity.x += direction[0] * deltaTime * 1f;
                entity.z += direction[1] * deltaTime * 1f;
            }
        }

        private const double DegToRad = Math.PI/180d;

        private static float[] DirectionFromDegrees(float degrees)
        {
            float[] coords = new float[2];
            double radians = degrees * DegToRad;

            coords[0] = (float) Math.Cos(radians);
            coords[1] = (float) Math.Sin(radians);

            return coords;
        }

        private static float[] RotatePosition(float x, float y, double degrees)
        {
            float[] coords = new float[2];
            double radians = degrees * DegToRad;

            double ca = Math.Cos(radians);
            double sa = Math.Sin(radians);

            coords[0] = (float)(ca *x - sa*y);
            coords[1] = (float)(sa *x + ca*y);

            return coords;
        }
    }
}
