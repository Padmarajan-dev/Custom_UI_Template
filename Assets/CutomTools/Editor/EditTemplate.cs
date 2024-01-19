using Codice.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
namespace com.editor.customuicreator
{
    public class EditTemplate : EditorWindow
    {
        private UITemplateWindow UITemplateWindow;

        private CreateTemplateWindow m_CreateTemplateWindow;

        Texture2D selectedImage;

        Vector2 scrollPosition = Vector2.zero;
        public static GameObject _DefaultParent;

        public SaveTemplate SaveTemplate;
        public static void ShowMenu()
        {
            GetWindow<EditTemplate>("EditTemplate");

        }

        private void OnEnable()
        {
            UITemplateWindow = new UITemplateWindow();
            m_CreateTemplateWindow = new CreateTemplateWindow();
            SaveTemplate = new SaveTemplate();

        }

        private void OnGUI()
        {
            if (UITemplateWindow._EditTemplate != null)
            {
                GUILayout.Label("Template Image", EditorStyles.boldLabel);

                if (UITemplateWindow._EditTemplate._TemplateImage != null && selectedImage == null)
                {
                    selectedImage = AssetDatabase.LoadAssetAtPath<Texture2D>(UITemplateWindow._EditTemplate._TemplateImage);
                    GUILayout.Label(selectedImage, GUILayout.Width(60), GUILayout.Height(60));
                }

                if (GUILayout.Button("Select Image"))
                {
                    if (m_CreateTemplateWindow != null)
                    {
                        m_CreateTemplateWindow.OpenImagePicker(ref selectedImage, ref UITemplateWindow._EditTemplate._TemplateImage);
                        Repaint(); // Ensure the window is repainted to reflect changes
                    }
                }

                if (selectedImage != null)
                {
                    GUILayout.Label(selectedImage, GUILayout.Width(60), GUILayout.Height(60));
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

                if (GUILayout.Button("SaveChanges"))
                {
                    SaveTemplate.UpdateTemplateData(UITemplateWindow._EditTemplate);
                    DestroyAllCanvasObjects();
                    this.Close();
                }

            }
        }
        static void DebugObjectProperties(object obj)
        {
            Debug.Log(UITemplateWindow._EditTemplate._UIObjects[0]._ImagePath);
        }

        public void DestroyAllCanvasObjects()
        {
            // Find all Canvas objects in the scene
            Canvas[] canvasObjects = UnityEngine.Object.FindObjectsOfType<Canvas>();

            // Destroy each Canvas GameObject
            foreach (Canvas canvas in canvasObjects)
            {
                DestroyImmediate(canvas.gameObject);
            }

            Debug.Log("All Canvas objects in the hierarchy destroyed!");
        }

    }
}
