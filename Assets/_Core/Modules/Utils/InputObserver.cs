using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace EGW
{
    public class InputObserver : MonoBehaviour
    {
        InputSystem m_inputSystem;

        CrossPlatformInputManager.VirtualButton m_WButton;
        CrossPlatformInputManager.VirtualButton m_SButton;
        CrossPlatformInputManager.VirtualButton m_AButton;
        CrossPlatformInputManager.VirtualButton m_DButton;

        private void Start()
        {
            m_WButton = new CrossPlatformInputManager.VirtualButton("W");
            CrossPlatformInputManager.RegisterVirtualButton(m_WButton);
            m_SButton = new CrossPlatformInputManager.VirtualButton("S");
            CrossPlatformInputManager.RegisterVirtualButton(m_SButton);
            m_AButton = new CrossPlatformInputManager.VirtualButton("A");
            CrossPlatformInputManager.RegisterVirtualButton(m_AButton);
            m_DButton = new CrossPlatformInputManager.VirtualButton("D");
            CrossPlatformInputManager.RegisterVirtualButton(m_DButton);
        }
        public void OnClickMoveButtonUP(bool isDown)
        {
            if (isDown)
            {
                m_WButton.Pressed();
            }
            else
            {
                m_WButton.Released();
            }
        }
        public void OnClickMoveButtonDown(bool isDown)
        {
            if (isDown)
            {
                m_SButton.Pressed();
            }
            else
            {
                m_SButton.Released();
            }
        }
        public void OnClickMoveButtonLeft(bool isDown)
        {
            if (isDown)
            {
                m_AButton.Pressed();
            }
            else
            {
                m_AButton.Released();
            }
        }

        public void OnClickMoveButtonRight(bool isDown)
        {
            if (isDown)
            {
                m_DButton.Pressed();
            }
            else
            {
                m_DButton.Released();
            }
        }
    }
}