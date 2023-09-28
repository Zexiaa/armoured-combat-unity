using System;

namespace TankGame.Vehicles
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
