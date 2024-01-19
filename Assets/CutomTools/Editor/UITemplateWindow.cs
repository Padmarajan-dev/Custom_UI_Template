using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using com.editor.customuicreator;
using System.Collections.Generic;
using Codice.Utils;
using Unity.Plastic.Newtonsoft.Json;

namespace com.editor.customuicreator
{
    public class UITemplateWindow : EditorWindow
    {
        List<Template> _templates;
        public GameObject canvasObject;

        public static Template _EditTemplate;




        [MenuItem("Window/Custom-UI/CustomUITemplate-Generator")]
        public static void ShowMenu()
        {
            GetWindow<UITemplateWindow>("CustomUITemplate");
        }

        private void OnEnable()
        {
            _templates = new List<Template>();
            if(_EditTemplate == null)
            {
                _EditTemplate = new Template();
            }


        }

        private void OnGUI()
        {

            //to show List of Templates already been created
            SaveTemplate saveTemplate = new SaveTemplate();

            _templates = saveTemplate.LoadTemplateData();


            if (_templates !=null && _templates.Count > 0)
            {
                GUILayout.BeginScrollView(new Vector2(0, 0), GUILayout.Width(200));
                List<Template> templatesToRemove = new List<Template>();
                foreach (Template template in _templates)
                {
                    GUILayout.Label(template._TemplateName, EditorStyles.boldLabel);

                    GUILayout.Space(5);
                    Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(template._TemplateImage);
                    if (texture != null)
                    {
                        GUILayout.Label(texture, GUILayout.Width(60), GUILayout.Height(60));
                    }

                    GUILayout.Space(8);
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("Load"))
                    {
                        InitializeCanvas();
                        saveTemplate.LoadHierarchyFromJson(template._UIObjects);
                    }
                    if (GUILayout.Button("Edit"))
                    {
                        InitializeCanvas();
                        saveTemplate.LoadHierarchyFromJson(template._UIObjects);
                        _EditTemplate = template;
                        EditTemplate.ShowMenu();
                    }

                    if (GUILayout.Button("Delete"))
                    {
                        templatesToRemove.Add(template);

                    }
                    GUILayout.EndHorizontal();
                }
                foreach (Template template in templatesToRemove)
                {
                    _templates.Remove(template);
                }
                saveTemplate.SaveTemplateData(_templates);
                GUILayout.EndScrollView();
            }
            else
            {
                GUILayout.Label("Templates are not added yet", EditorStyles.boldLabel);
            }
            GUILayout.Space(5);
            if (GUILayout.Button("New Template"))
            {
                //to open new template creation window
                InitializeCanvas();
                CreateTemplateWindow.ShowNewTemplateWindow();
            }
        }

        private void InitializeCanvas()
        {
            DestroyAllCanvasObjects();
            // Create a new GameObject with a Canvas component
            if (canvasObject == null)
            {
                canvasObject = new GameObject("Canvas");
                CanvasScaler canvasScaler = canvasObject.AddComponent<CanvasScaler>();
                canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                canvasScaler.referenceResolution = new Vector2(1920, 1080);

                // Create an EventSystem if it doesn't exist
                if (!Object.FindObjectOfType<EventSystem>())
                {
                    GameObject eventSystem = new GameObject("EventSystem");
                    eventSystem.AddComponent<EventSystem>();
                    eventSystem.AddComponent<StandaloneInputModule>();
                }

                Debug.Log("Canvas instantiated in the scene!");
            }
            else
            {
                DestroyImmediate(canvasObject);
                canvasObject = null;
            }
        }

        private void DestroyAllCanvasObjects()
        {
            // Find all Canvas objects in the scene
            Canvas[] canvasObjects = Object.FindObjectsOfType<Canvas>();

            // Destroy each Canvas GameObject
            foreach (Canvas canvas in canvasObjects)
            {
                DestroyImmediate(canvas.gameObject);
            }

            Debug.Log("All Canvas objects in the hierarchy destroyed!");
        }
    }
}

