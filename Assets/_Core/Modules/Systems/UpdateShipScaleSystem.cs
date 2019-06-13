using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using static Unity.Mathematics.math;

namespace EGW
{
    [UpdateAfter(typeof(VisibleRectUpdateSystem))]
    public class UpdateShipScaleSystem : ComponentSystem
    {

        protected override void OnUpdate()
        {
            var gameSettings = GetSingleton<GameSettingsSingletonComponent>();
            int zoomValue = 0;
            Entities.ForEach((ref ProxyInfo proxyInfo) =>
            {
                zoomValue =  proxyInfo.zoom;
            });
            int normalZoom = gameSettings.minCellsVisibleSize;
            float scale = (float)zoomValue / normalZoom;
            scale = math.clamp(1.0f / scale, gameSettings.minShipScale, 2);
            Entities.ForEach((ref ShipComponent ship, ref Scale scaleComponent) =>
            {
                scaleComponent.Value = scale;
            });
        }
    }
}
