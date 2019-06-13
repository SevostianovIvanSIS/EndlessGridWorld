using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace EGW
{
    public class UpdateTextValue : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI m_text;
        public string Text
        {
            set
            {
                m_text.text = value;
            }
        }
    }
}