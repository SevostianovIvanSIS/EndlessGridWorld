using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Collections;
using TMPro;

namespace EGW
{
    [UpdateAfter(typeof(VisibleRectUpdateSystem))]
    public class UpdatePlanetPositionsByZoomSystem : ComponentSystem
    {
        public UpdateTextValue labelPrefab;
        List<UpdateTextValue> m_labelBuffer = new List<UpdateTextValue>();
        UpdateTextValue m_shipText;
        GameSettingsSingletonComponent m_gameSettings;
        protected override void OnStartRunning()
        {
            m_gameSettings = GetSingleton<GameSettingsSingletonComponent>();

            for (int i = 0; i < 120; i++)
            {
                var tm = GameObject.Instantiate(labelPrefab, Vector3.zero, Quaternion.identity);
                tm.gameObject.SetActive(false);
                m_labelBuffer.Add(tm);
            }

            m_shipText = GameObject.Instantiate(labelPrefab, Vector3.zero, Quaternion.identity);
        }

        protected override void OnUpdate()
        {
            var proxy = GetSingleton<ProxyInfo>();
            int2 shipPos = new int2();
            int shipValue = 0;
            Entities.ForEach((ref ShipComponent ship, ref Translation translation) =>
            {
                shipPos.x = (int)translation.Value.x;
                shipPos.y = (int)translation.Value.y;
                shipValue = ship.rating;
            });
            m_shipText.Text = shipValue.ToString();
            m_shipText.transform.position = new Vector2(shipPos.x, shipPos.y + 1);
            int normalZoom = m_gameSettings.minCellsVisibleSize;
            float scale = (float)proxy.zoom / normalZoom;
            int i = 0;
            Entities.ForEach((ref Rotation rotation, ref PlanetInfoComponent planet, ref Translation translation) =>
            {
                UpdateTextValue text = m_labelBuffer[i];

                if (planet.isVisible)
                {
                    text.gameObject.SetActive(true);
                    text.Text = planet.rating.ToString();
                    rotation.Value = quaternion.Euler(0, 0, math.PI * Time.realtimeSinceStartup);

                    if (scale > 3)
                    {
                        var scale_ = scale - 2;
                        var vect = planet.position - shipPos;
                        var len = math.length(vect);
                        var normolizedVect = math.normalize(vect);
                        len = len / scale_;
                        normolizedVect *= len;
                        normolizedVect += shipPos;
                        translation.Value.x = normolizedVect.x;
                        translation.Value.y = normolizedVect.y;
                    }
                    else
                    {
                        translation.Value.x = planet.position.x;
                        translation.Value.y = planet.position.y;
                    }

                    text.gameObject.transform.position = new Vector2(translation.Value.x, translation.Value.y + 1);
                }
                else
                {
                    text.gameObject.SetActive(false);
                }

                i++;
            });
        }
    }
}