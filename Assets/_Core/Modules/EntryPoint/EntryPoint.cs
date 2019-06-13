using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;
using TMPro;

namespace EGW
{

    [DisallowMultipleComponent]
    public class EntryPoint : MonoBehaviour
    {
        [SerializeField] UpdateTextValue labelPrefab;
        [SerializeField] float planetSpawnProbability = 30.0f;

        [SerializeField] int maxCellsVisibleSize = 10000;
        [SerializeField] int minCellsVisibleSize = 5;

        [SerializeField] int minRating = 0;
        [SerializeField] int maxRating = 10000;

        [SerializeField] int specialModeVisiblePlanetsCount = 20;

        [SerializeField] float  minVisiblePlanetScale = 0.2f;
        [SerializeField] float minShipScale = 0.1f;

        [SerializeField] Material planetMaterial;
        [SerializeField] Material shipMaterial;

        private EntityManager entityManager;

        void Awake()
        {
            entityManager = World.Active.EntityManager;
            Entity entity = entityManager.CreateEntity(
                                ComponentType.ReadOnly<GameSettingsSingletonComponent>(),
                                ComponentType.ReadWrite<ProxyInfo>()
                            );
            var gameSettings = new GameSettingsSingletonComponent
            {
                maxCellsVisibleSize = maxCellsVisibleSize,
                minCellsVisibleSize = minCellsVisibleSize,

                minRating = minRating,
                maxRating = maxRating,

                specialModeVisiblePlanetsCount = specialModeVisiblePlanetsCount,

                minVisiblePlanetScale = minVisiblePlanetScale,
                minShipScale = minShipScale,
                planetSpawnProbability = planetSpawnProbability
            };
            var proxyInfo = new ProxyInfo
            {
                shipPosition = new int2(2, 2),
                visibleRectInt = new RectInt(2, 2, 5, 5),
                zoom = 5,
                shipValue = UnityEngine.Random.Range(minRating, maxRating)
            };
            var spawnSystem = World.Active.GetOrCreateSystem<SpawnPlanetsSystem>();
            spawnSystem.shipMaterial = this.shipMaterial;
            spawnSystem.planetMaterial = this.planetMaterial;
            spawnSystem.shipMesh = CreateMesh(2, 2);
            spawnSystem.planetMesh = CreateMesh(2, 1);
            var quadSystem = World.Active.GetOrCreateSystem<ProcessQuadTreeLevelInfoSystem>();
            quadSystem.SetSingleton(gameSettings);
            quadSystem.SetSingleton(proxyInfo);
            var updatePlanetPositionsByZoomSystem = World.Active.GetOrCreateSystem<UpdatePlanetPositionsByZoomSystem>();
            updatePlanetPositionsByZoomSystem.labelPrefab = labelPrefab;
        }

        private Mesh CreateMesh(float width, float height)
        {
            Vector3[] vertices = new Vector3[4];
            Vector2[] uv = new Vector2[4];
            int[] triangles = new int[6];
            /* 0, 0
             * 0, 1
             * 1, 1
             * 1, 0
             * */
            float halfWidth = width / 2f;
            float halfHeight = height / 2f;
            vertices[0] = new Vector3(-halfWidth, -halfHeight);
            vertices[1] = new Vector3(-halfWidth, +halfHeight);
            vertices[2] = new Vector3(+halfWidth, +halfHeight);
            vertices[3] = new Vector3(+halfWidth, -halfHeight);
            uv[0] = new Vector2(0, 0);
            uv[1] = new Vector2(0, 1);
            uv[2] = new Vector2(1, 1);
            uv[3] = new Vector2(1, 0);
            triangles[0] = 0;
            triangles[1] = 1;
            triangles[2] = 3;
            triangles[3] = 1;
            triangles[4] = 2;
            triangles[5] = 3;
            Mesh mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.uv = uv;
            mesh.triangles = triangles;
            return mesh;
        }

    }
}