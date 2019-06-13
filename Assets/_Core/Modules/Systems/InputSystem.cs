using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using static Unity.Mathematics.math;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

namespace EGW
{
    [UpdateBefore(typeof(ShipMoveSystem))]
    public class InputSystem : ComponentSystem
    {
        protected override void OnStartRunning()
        {
            m_slider =  GameObject.FindGameObjectWithTag("ZoomSlider").GetComponent<Slider>();
        }

        private Slider m_slider;
        protected override void OnUpdate()
        {
            MoveCommand command = MoveCommand.NONE;
            int zoom = 0;

            if (Input.GetKey(KeyCode.W) || CrossPlatformInputManager.GetButton("W"))
            {
                command = MoveCommand.UP;
            }

            if (Input.GetKey(KeyCode.S) || CrossPlatformInputManager.GetButton("S"))
            {
                command = MoveCommand.DOWN;
            }

            if (Input.GetKey(KeyCode.A) || CrossPlatformInputManager.GetButton("A"))
            {
                command = MoveCommand.LEFT;
            }

            if (Input.GetKey(KeyCode.D) || CrossPlatformInputManager.GetButton("D"))
            {
                command = MoveCommand.RIGHT;
            }

            var gameSettings = GetSingleton<GameSettingsSingletonComponent>();
            zoom = (int)(m_slider.value * gameSettings.maxCellsVisibleSize) + 10;
            Entities.ForEach((ref InputInfoComponent input) =>
            {
                var newZoom = zoom; //input.zoomValue +
                newZoom = Mathf.Clamp(newZoom, gameSettings.minCellsVisibleSize, gameSettings.maxCellsVisibleSize);
                input.zoomValue = newZoom;
                input.moveCommand = command;
            });
        }
    }
}