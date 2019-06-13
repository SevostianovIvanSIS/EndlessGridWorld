using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace EGW
{

    [Serializable]
    public struct InputInfoComponent : IComponentData
    {
        public int zoomValue;
        public MoveCommand moveCommand;
    }

    public enum MoveCommand
    {
        NONE = 0,
        UP = 1,
        DOWN = 2,
        LEFT = 3,
        RIGHT = 4
    }
}
