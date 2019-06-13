using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace EGW
{
    [Serializable]
    public struct PlanetInfoComponent : IComponentData
    {
        public int rating;
        public int2 position;
        public bool isVisible;
    }
}
