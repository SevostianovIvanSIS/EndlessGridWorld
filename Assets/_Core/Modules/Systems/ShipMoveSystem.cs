using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using static Unity.Mathematics.math;

namespace EGW
{

    [UpdateAfter(typeof(VisibleRectUpdateSystem))]
    public class ShipMoveSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            List<int2> planetsPositions = new List<int2>();
            Entities.ForEach((ref Translation translation, ref PlanetInfoComponent planetInfo) =>
            {
                if (planetInfo.isVisible)
                {
                    planetsPositions.Add(planetInfo.position);
                }
            });
            var gameSettings = GetSingleton<GameSettingsSingletonComponent>();
            var planetsGroup = GetEntityQuery(typeof(Rotation), typeof(PlanetInfoComponent), typeof(Translation));
            Entities.ForEach((ref Translation translation, ref Rotation rotation, ref ShipComponent ship, ref InputInfoComponent input) =>
            {
                var pos = ship.position;

                if (input.zoomValue < 20)
                {
                    switch (input.moveCommand)
                    {
                        case MoveCommand.UP:
                            {
                                pos.y += 1;

                                while (true)
                                {
                                    if (!IsCellFree(pos, planetsPositions))
                                    {
                                        pos.y += 1;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }

                                break;
                            }

                        case MoveCommand.DOWN:
                            {
                                pos.y -= 1;

                                while (true)
                                {
                                    if (!IsCellFree(pos, planetsPositions))
                                    {
                                        pos.y -= 1;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }

                                break;
                            }

                        case MoveCommand.LEFT:
                            {
                                pos.x -= 1;

                                while (true)
                                {
                                    if (!IsCellFree(pos, planetsPositions))
                                    {
                                        pos.x -= 1;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }

                                break;
                            }

                        case MoveCommand.RIGHT:
                            {
                                pos.x += 1;

                                while (true)
                                {
                                    if (!IsCellFree(pos, planetsPositions))
                                    {
                                        pos.x += 1;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }

                                break;
                            }

                        default: break;
                    }

                    ship.position = pos;
                    float scale = (float)input.zoomValue / gameSettings.minCellsVisibleSize;
                    translation.Value.x = pos.x;
                    translation.Value.y = pos.y;
                }
            });
        }

        bool IsCellFree(int2 position, List<int2> planetsPositions)
        {
            foreach (var planetPos in planetsPositions)
            {
                if (planetPos.x == position.x && planetPos.y == position.y)
                {
                    return false;
                }
            }

            return true;
        }
    }

    public class CameraMoveSystem : ComponentSystem
    {
        GameObject m_cameraTargetObj;
        protected override void OnStartRunning()
        {
            m_cameraTargetObj = GameObject.FindGameObjectWithTag("CameraTarget");
        }
        protected override void OnUpdate()
        {
            float3 pos = new float3();
            Entities.ForEach((ref Translation translation, ref ShipComponent ship) =>
            {
                pos = translation.Value;
            });
            m_cameraTargetObj.transform.position = new Vector3(pos.x, pos.y, pos.z);
        }

    }
}