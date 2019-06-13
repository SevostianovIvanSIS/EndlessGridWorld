using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using static Unity.Mathematics.math;

namespace EGW
{
    public class SpawnPlanetsSystem : ComponentSystem
    {
        public Mesh shipMesh;
        public Material shipMaterial;
        public Mesh planetMesh;
        public Material planetMaterial;


        protected override void OnUpdate()
        {
            EntityManager manager = Unity.Entities.World.Active.EntityManager;
            var proxy = GetSingleton<ProxyInfo>();
            var gameSettings = GetSingleton<GameSettingsSingletonComponent>();

            for (int i = 0; i < 120; i++)
            {
                var planet = manager.CreateEntity(
                                 ComponentType.ReadOnly<RenderMesh>(),
                                 ComponentType.ReadWrite<PlanetInfoComponent>(),
                                 ComponentType.ReadOnly<LocalToWorld>(),
                                 ComponentType.ReadOnly<Translation>(),
                                 ComponentType.ReadOnly<Rotation>(),
                                 ComponentType.ReadWrite<Scale>()
                             );
                manager.SetComponentData(planet, new Scale
                {
                    Value = 1
                });
                manager.SetSharedComponentData(planet, new RenderMesh
                {
                    mesh = planetMesh,
                    material = planetMaterial,
                });
                manager.SetComponentData(planet, new PlanetInfoComponent
                {
                    isVisible = false
                });
            }

            var ship = manager.CreateEntity(
                           ComponentType.ReadOnly<RenderMesh>(),
                           ComponentType.ReadWrite<ShipComponent>(),
                           ComponentType.ReadOnly<LocalToWorld>(),
                           ComponentType.ReadWrite<Translation>(),
                           ComponentType.ReadWrite<Rotation>(),
                           ComponentType.ReadWrite<InputInfoComponent>(),
                           ComponentType.ReadWrite<Scale>()
                       );
            manager.SetComponentData(ship, new InputInfoComponent
            {
                zoomValue = 5,
                moveCommand = MoveCommand.NONE
            });
            manager.SetComponentData(ship, new ShipComponent
            {
                rating = proxy.shipValue,
                position = new int2(0, 0)
            });
            manager.SetComponentData(ship, new Scale
            {
                Value = 1
            });
            manager.SetSharedComponentData(ship, new RenderMesh
            {
                mesh = shipMesh,
                material = shipMaterial,
            });
            Enabled = false;
        }
    }
}