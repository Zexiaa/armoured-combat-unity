using System;

namespace TankGame
{
    [Serializable]
    public struct Shell
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
