using System;

namespace TankGame
{
    [Serializable]
    public struct TankShell
    {
        public enum Category
        {
            AP,
            HE
        }

        //public string name;
        public Category shellCategory;
        public float muzzleVelocity;
    }
}
