using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace client
{
    /// <summary>
    /// Rozwiązuje równania ruchu dla wirtualnego punktu.
    /// </summary>
    class VirtualPoint
    {
        public class Vec
        {
            public float x, y;

            public Vec(float x = 0, float y = 0){ this.x = x; this.y = y; }
            static public Vec operator +(Vec v, Vec u){ return new Vec(v.x + u.x, v.y + u.y); }
            static public Vec operator *(Vec v, float k) { return new Vec(v.x * k, v.y * k); }
        }


        public Vec pos; // pozycja
        public Vec vel; // prędkość
        public Vec acc; // przyspieszenie

        public VirtualPoint()
        {
            pos = new Vec();
            vel = new Vec();
            acc = new Vec();
        }

        public void setAcceleration(float x, float y) { acc.x = x; acc.y = y; }
        public void setVelocity(float x, float y) { vel.x = x; vel.y = y; }

        // rozwiązuje równanie ruchu
        public void solve(float time)
        {
            pos = pos + vel * time;
            vel = vel + acc * time;
        }

    }
}
