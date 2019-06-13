using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using static Unity.Mathematics.math;
using System.Collections.Generic;
using Unity.Rendering;
using System.Linq;

namespace EGW
{
    [UpdateAfter(typeof(VisibleRectUpdateSystem))]
    public class ProcessQuadTreeLevelInfoSystem : ComponentSystem
    {
        QuadTreeLevelInfo m_quadTreeLevelInfo;
        GameSettingsSingletonComponent m_gameSettings;
        protected override void OnStartRunning()
        {
            m_gameSettings = GetSingleton<GameSettingsSingletonComponent>();
            m_quadTreeLevelInfo = new QuadTreeLevelInfo(m_gameSettings.planetSpawnProbability,
                    20,
                    10,
                    UnityEngine.Random.Range(0, 100),
                    m_gameSettings.specialModeVisiblePlanetsCount,
                    m_gameSettings.minRating,
                    m_gameSettings.maxRating);
        }

        void CollectPlanetsInfo(QuadTreeLevelInfo.QuadTreeInfo info, List<PlanetsInfo> planetsInfo, int maxDepth)
        {
            if (info.nodes !=  null)
            {
                foreach (var item in info.nodes)
                {
                    CollectPlanetsInfo(item, planetsInfo, maxDepth);
                }
            }
            else
            {
                if (info.depth == maxDepth)
                {
                    for (int i = 0; i < info.planetsValuesInRange.Count; i++)
                    {
                        var position = info.planetsPositionsInRange[i];
                        planetsInfo.Add(new PlanetsInfo
                        {
                            rating = info.planetsValuesInRange[i],
                            position = new int2(position.x, position.y),
                            depth = info.depth
                        });
                    }
                }
            }
        }


        struct PlanetsInfo
        {
            public int2 position;
            public int rating;
            public int depth;
        }

        protected override void OnUpdate()
        {
            var proxyData = GetSingleton<ProxyInfo>();
            int maxDepth = 10;
            int fibIndex = 1;

            while (true)
            {
                if (proxyData.zoom > Fib(fibIndex))
                {
                    maxDepth--;
                    fibIndex++;
                }
                else
                {
                    break;
                }
            }

            var info = m_quadTreeLevelInfo.GetQuadTreeInfoForRectInt(proxyData.visibleRectInt, maxDepth, proxyData.shipValue);
            var planetsInfo = new List<PlanetsInfo>();
            int currentMaxDepth = m_quadTreeLevelInfo.currentMaxDepth;
            CollectPlanetsInfo(info, planetsInfo, currentMaxDepth);
            planetsInfo.Shuffle(info.random);
            planetsInfo.Shuffle(info.random);
            planetsInfo.Shuffle(info.random);
            var planetsInfoSorted = planetsInfo.OrderBy(x => x.rating).ToList();
            int i = 0;
            Entities.ForEach((Entity planet, ref PlanetInfoComponent planetInfoComponent) =>
            {
                if (i < m_gameSettings.specialModeVisiblePlanetsCount)
                {
                    planetInfoComponent.isVisible = true;
                    planetInfoComponent.position = planetsInfoSorted[i].position;
                    planetInfoComponent.rating = planetsInfoSorted[i].rating;
                    PostUpdateCommands.RemoveComponent(planet, ComponentType.ReadOnly<Disabled>());
                    i++;
                }
                else
                {
                    planetInfoComponent.isVisible = false;
                    PostUpdateCommands.AddComponent<Disabled>(planet, new Disabled());
                }
            });
        }

        static int Fib(int n)
        {
            if (n <= 0)
                return 0;

            if (n == 1)
                return 150;

            return Fib(n - 1) + Fib(n - 2);
        }
    }
}