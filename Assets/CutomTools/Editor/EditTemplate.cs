using Codice.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
namespace com.editor.customuicreator
{
    public class EditTemplate : EditorWindow
    {
        private UITemplateWindow UITemplateWindow;

        private CreateTemplateWindow m_CreateTemplateWindow;

        Vector2 scrollPosition = Vector2.zero;
        public static void ShowMenu()
        {
            GetWindow<EditTemplate>("EditTemplate");
        }

        private void OnEnable()
        {
            UITemplateWindow = new UITemplateWindow();
            m_CreateTemplateWindow = new CreateTemplateWindow();
        }

        private void OnGUI()
        {
            Debug.Log(UITemplateWindow._EditTemplate);
            if (UITemplateWindow._EditTemplate != null)
            {
                GUILayout.Label("Template Image", EditorStyles.boldLabel);

                if (UITemplateWindow._EditTemplate._TemplateImage != null)
                {
                    Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(UITemplateWindow._EditTemplate._TemplateImage);
                    GUILayout.Label(texture, GUILayout.Width(60), GUILayout.Height(60));

                }

                if (GUILayout.Button("Select Image"))
                {
                    Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(UITemplateWindow._EditTemplate._TemplateImage);
                    m_CreateTemplateWindow.OpenImagePicker(ref texture);
                }

                //template name field
                GUILayout.BeginHorizontal();
                GUILayout.Label("Template Name", EditorStyles.boldLabel);
                UITemplateWindow._EditTemplate._TemplateName = EditorGUILayout.TextField("", UITemplateWindow._EditTemplate._TemplateName);
                GUILayout.EndHorizontal();
                GUILayout.Space(30);
                scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(600), GUILayout.Height(400));
                for (int i = 0;i< UITemplateWindow._EditTemplate._UIObjects.Count; i++)
                {
                    UIObject uiobj = UITemplateWindow._EditTemplate._UIObjects[i];
                    m_CreateTemplateWindow.ShowUIElements(ref uiobj,ref UITemplateWindow._EditTemplate);
                }
                GUILayout.EndScrollView();

            }
        }

    }
}
