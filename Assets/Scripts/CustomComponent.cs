using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;

namespace com.editor.customuicreator
{
    public class CustomComponent : MonoBehaviour
    {
        private void Start()
        {
            GameObject gameObject = new GameObject("Text");
            gameObject.AddComponent<TextMeshProUGUI>();
            gameObject.GetComponent<TextMeshProUGUI>().text = "Default Text";
            gameObject.GetComponent<TextMeshProUGUI>().color = Color.white; // Set a default color
            gameObject.GetComponent<TextMeshProUGUI>().fontSize = 12;
        }
    }
}
