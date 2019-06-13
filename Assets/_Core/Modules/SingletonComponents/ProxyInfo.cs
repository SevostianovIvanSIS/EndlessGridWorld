using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace EGW
{
    [Serializable]
    public struct ProxyInfo : IComponentData
    {
        public int2 shipPosition;
        public int shipValue;
        public RectInt visibleRectInt;
        public int zoom;
    }
}
