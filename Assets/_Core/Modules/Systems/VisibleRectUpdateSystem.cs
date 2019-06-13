using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using static Unity.Mathematics.math;

namespace EGW
{
    [UpdateAfter(typeof(InputSystem))]
    public class VisibleRectUpdateSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            var proxyData = GetSingleton<ProxyInfo>();
            var gameSettings = GetSingleton<GameSettingsSingletonComponent>();
            int2 shipPosition = new int2();
            int zoomValue = 0;
            int shipValue = proxyData.shipValue;
            float3 shipRealPos = new float3();
            Entities.ForEach((ref ShipComponent ship, ref Translation translation, ref InputInfoComponent inputInfoComponent) =>
            {
                shipPosition = ship.position;
                zoomValue = inputInfoComponent.zoomValue;
                shipRealPos = translation.Value;
            });
            int normalZoom = gameSettings.minCellsVisibleSize;
            float scale = (float)proxyData.zoom / normalZoom;
            var visibleRectInt = proxyData.visibleRectInt;
            var visibleZonePosition = new Vector2Int((int)(shipRealPos.x - zoomValue / 2.0f), (int)(shipRealPos.y - zoomValue / 2.0f));
            visibleRectInt = new RectInt(visibleZonePosition, new Vector2Int(zoomValue, zoomValue));
            proxyData.visibleRectInt = visibleRectInt;
            SetSingleton(new ProxyInfo
            {
                shipPosition = new int2((int)shipPosition.x, (int)shipPosition.y),
                visibleRectInt = visibleRectInt,
                shipValue = shipValue,
                zoom = zoomValue
            });
        }
    }
}