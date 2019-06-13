using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace EGW
{
    [Serializable]
    public struct GameSettingsSingletonComponent : IComponentData
    {
        public int maxCellsVisibleSize;
        public int minCellsVisibleSize;

        public int minRating;
        public int maxRating;

        public int specialModeVisiblePlanetsCount;
        public float planetSpawnProbability;

        public float minVisiblePlanetScale;
        public float minShipScale;

        // public int maxWorldOffset;
    }
}