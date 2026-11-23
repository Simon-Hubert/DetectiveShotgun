using UnityEngine;

namespace DShotgun
{
    public static class MathsUtils 
    {
        public static float ExpDecay(float a,float b, float decay, float dt){
            return b + (a-b) * Mathf.Exp(-decay*dt); 
        }
        
        public static float HalfLifeDecay(float a,float b, float halfLife, float dt) {
            return b + (a - b) * Mathf.Pow(2,-dt / halfLife);
        }

        public static float RemainderDecay(float a, float b, float remainder, float dt) {
            return b + (a - b) * Mathf.Pow(remainder, dt);
        }
    }
}
