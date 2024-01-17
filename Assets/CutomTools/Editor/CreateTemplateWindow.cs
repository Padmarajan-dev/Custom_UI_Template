using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.ComponentModel;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;
using TMPro;

namespace com.editor.customuicreator
{
    public class CreateTemplateWindow : EditorWindow
    {
        private Texture2D selectedImage;
        private string selectedImagePath;

        private string m_TemplateName = string.Empty;
        private List<Template> m_Templates;
        private string m_ElemntType = string.Empty;

        private int selectedOptionIndex = -1;

        SaveTemplate saveTemplate = new SaveTemplate();
        public static Template newTemplate;

        Vector2 scrollPosition = Vector2.zero;
        private void OnEnable()
        {
            if (m_Templates == null)
            {
                m_Templates = new List<Template>();
            }
            m_Templates = saveTemplate.LoadTemplateData();
            if (newTemplate == null)
            {
                newTemplate = new Template();
            }

            if(newTemplate._UIObjects == null)
            {
                newTemplate._UIObjects = new List<UIObject>();
            }
        }

        private void OnDisable()
        {
            if (newTemplate != null)
            {
                newTemplate = null;
            }
        }
        public static void ShowNewTemplateWindow()
        {
            GetWindow<CreateTemplateWindow>("CreateTemplate");
        }
        #region DrawGUI Elements
        private void OnGUI()
        {
            GUILayout.Label("Template Image",EditorStyles.boldLabel);

            if (selectedImage != null)
            {
                GUILayout.Label(selectedImage, GUILayout.Width(60), GUILayout.Height(60));

            }

            if (GUILayout.Button("Select Image"))
            {
                OpenImagePicker(ref selectedImage);
            }

            //template name field
            GUILayout.BeginHorizontal();
            GUILayout.Label("Template Name", EditorStyles.boldLabel);
            m_TemplateName = EditorGUILayout.TextField("", m_TemplateName);
            GUILayout.EndHorizontal();

            if(GUILayout.Button("ADD Element"))
            {
                CreateNewObject.ShowCreateNeobjectWindow();
            }

            //to save template
            if (GUILayout.Button("Save Template"))
            {
                // Check if a template with the same name already exists
                Template existingTemplate = null;
                if (m_Templates != null && m_Templates.Count > 0)
                {
                    existingTemplate = m_Templates.Find(t => t._TemplateName == m_TemplateName);
                }
                if (existingTemplate != null)
                {
                    existingTemplate._TemplateName = m_TemplateName;
                    existingTemplate._TemplateImage = selectedImagePath;
                }
                else
                {
                    newTemplate._TemplateName = m_TemplateName;
                    newTemplate._TemplateImage = selectedImagePath;
                    if (m_Templates != null)
                    {
                        m_Templates.Add(newTemplate);
                    }
                    else
                    {
                        m_Templates = new List<Template>();
                    }

                }



                if (m_Templates.Count > 0)
                {
                    saveTemplate = new SaveTemplate();
                    saveTemplate.SaveTemplateData(m_Templates);
                    m_TemplateName = string.Empty;
                    selectedImagePath = string.Empty;
                    selectedImage = null;
                    newTemplate = null;
                    DestroyAllCanvasObjects();
                    Repaint();
                }

            }

            // List of added elements in template

            if (newTemplate != null)
            {

                scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(600), GUILayout.Height(400));
                if (newTemplate._UIObjects != null)
                {
                    if(newTemplate._UIObjects.Count > 0)
                    {
                        foreach (UIObject uIObject in newTemplate._UIObjects)
                        {
                            UIObject modifiedObject = uIObject;
                            ShowUIElements(ref modifiedObject,ref newTemplate);
                        }
                    }
                    else
                    {
                        GUILayout.Label("No new elements added", EditorStyles.boldLabel);
                    }
                    
                }
                else
                {
                    newTemplate._UIObjects = new List<UIObject>();
                }

                GUILayout.EndScrollView();
            }
            else
            {
                GUILayout.Label("No new elements added", EditorStyles.boldLabel);
            }

            
        }
        #endregion

        public static GameObject FindObject(string objName)
        {
            Transform[] roots = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects().Select(go => go.transform).ToArray();

            foreach (Transform root in roots)
            {
                Transform result = FindObjectInHierarchy(root, objName);
                if (result != null)
                {

                    return result.gameObject;
                }
            }

            return FindObjectOfType<Canvas>().gameObject;
        }

        private static Transform FindObjectInHierarchy(Transform parent, string objName)
        {
            Transform result = parent.Find(objName);

            if (result != null)
            {
                return result;
            }

            foreach (Transform child in parent)
            {
                result = FindObjectInHierarchy(child, objName);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        #region Common Methods

        //to select image from files
        public void OpenImagePicker(ref Texture2D image)
        {
            string imagePath = EditorUtility.OpenFilePanel("Select Image", "", "png,jpg,jpeg,gif");
            if (!string.IsNullOrEmpty(imagePath))
            {
                // Load the texture

                string fileName = Path.GetFileName(imagePath);
                string destinationPath = "Assets/Sprites/" + fileName;
                try
                {
                    // Check if the file already exists
                    if (!File.Exists(destinationPath))
                    {
                        FileUtil.CopyFileOrDirectory(imagePath, destinationPath);
                    }
                    AssetDatabase.Refresh();
                    selectedImagePath = destinationPath;
                    image = LoadTexture(selectedImagePath);
                    Repaint();
                }
                catch (Exception e)
                {
                    Debug.LogError("Error copying image: " + e.Message);
                }

            }
        }

        //to show different UI elements Added In Template
        public void ShowUIElements(ref UIObject uiobject,ref Template template)
        {
            if (uiobject != null)
            {

                GUILayout.BeginHorizontal();
                GUILayout.Label("GameObjectName", EditorStyles.boldLabel);
                uiobject._ObjectName = EditorGUILayout.TextField("", uiobject._ObjectName);

                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label("GameObjectPosition", EditorStyles.boldLabel);
                uiobject._ObjectPosition = EditorGUILayout.Vector3Field("", uiobject._ObjectPosition);

                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label("GameObjectRoatation", EditorStyles.boldLabel);
                uiobject._ObjectRotation = EditorGUILayout.Vector3Field("", uiobject._ObjectRotation);

                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label("GameObjectScale", EditorStyles.boldLabel);
                uiobject._ObjectScale = EditorGUILayout.Vector3Field("", uiobject._ObjectScale);

                GUILayout.EndHorizontal();
                GUILayout.Label("Parent Object", EditorStyles.boldLabel);

                if (uiobject._Object)
                {
                    GameObject parent = null;
                    uiobject._Object.name = uiobject._ObjectName;
                    uiobject._Object.transform.localPosition = uiobject._ObjectPosition;
                    uiobject._Object.transform.localEulerAngles = uiobject._ObjectRotation;
                    uiobject._Object.transform.localScale = uiobject._ObjectScale;

                    if (uiobject._ParentObjectName != null)
                    {
                        parent = CreateTemplateWindow.FindObject(uiobject._ParentObjectName);

                        if (parent != null)
                        {
                            uiobject._Object.transform.parent = parent.transform;
                        }
                    }
                    parent = EditorGUILayout.ObjectField("My GameObject Field", uiobject._Object.transform.parent, typeof(GameObject), true) as GameObject;
                }

                if (uiobject._Object)
                {
                    if (uiobject._Object.GetComponent<Image>())
                    {
                        GUILayout.Label("Element Image", EditorStyles.boldLabel);
                        if (uiobject._Image != null)
                        {
                            GUILayout.Label(uiobject._Image, GUILayout.Width(60), GUILayout.Height(60));
                            // Create a sprite from the loaded texture
                            Sprite sprite = Sprite.Create(uiobject._Image, new Rect(0, 0, uiobject._Image.width, uiobject._Image.height), Vector2.one * 0.5f);
                            if (uiobject._Object)
                            {
                                // Assign the sprite to the Image component
                                uiobject._Object.GetComponent<Image>().sprite = sprite;
                            }
                        }

                        if (GUILayout.Button("Select Image"))
                        {
                            OpenImagePicker(ref uiobject._Image);
                        }
                    }
                    if (uiobject._Object.GetComponent<TextMeshProUGUI>())
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Text Color", EditorStyles.boldLabel);
                        uiobject._TextColor = EditorGUILayout.ColorField(uiobject._TextColor);
                        uiobject._Object.GetComponent<TextMeshProUGUI>().color = uiobject._TextColor;


                        GUILayout.EndHorizontal();

                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Text Size", EditorStyles.boldLabel);
                        uiobject._TextSize = EditorGUILayout.FloatField(uiobject._TextSize);
                        uiobject._Object.GetComponent<TextMeshProUGUI>().fontSize = uiobject._TextSize;
                        GUILayout.EndHorizontal();

                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Text", EditorStyles.boldLabel);
                        uiobject._Text = EditorGUILayout.TextField(uiobject._Text);
                        uiobject._Object.GetComponent<TextMeshProUGUI>().text = uiobject._Text;
                        GUILayout.EndHorizontal();
                    }
                }

                   
                if (GUILayout.Button("Remove"))
                {
                        GameObject obj = uiobject._Object;
                        template._UIObjects.Remove(uiobject);
                        DestroyImmediate(obj);
                }
            }
            
        }

        //to load Texture
        public Texture2D LoadTexture(string path)
        {
            byte[] fileData = System.IO.File.ReadAllBytes(path);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(fileData);
            return texture;
        }

        public static Texture2D SpriteToTexture2D(Sprite sprite)
        {
            if (sprite == null)
            {
                Debug.LogError("Sprite is null.");
                return null;
            }

            // Create a temporary RenderTexture
            RenderTexture renderTexture = RenderTexture.GetTemporary(sprite.texture.width, sprite.texture.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);

            // Set the active RenderTexture
            RenderTexture.active = renderTexture;

            // Draw the sprite onto the RenderTexture
            Graphics.Blit(sprite.texture, renderTexture);

            // Create a new Texture2D and read the RenderTexture data into it
            Texture2D texture = new Texture2D(sprite.texture.width, sprite.texture.height);
            texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            texture.Apply();

            // Release the temporary RenderTexture
            RenderTexture.ReleaseTemporary(renderTexture);

            return texture;
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

        public void DestroyAllCanvasChilds()
        {
            Canvas[] canvasObjects = UnityEngine.Object.FindObjectsOfType<Canvas>();
            foreach (Canvas canvas in canvasObjects)
            {
                // Destroy each Canvas GameObject
                foreach (Transform childTransform in canvas.transform)
                {
                    // Access the child GameObject
                    GameObject child = childTransform.gameObject;
                    DestroyImmediate(child.gameObject);
                }
            }
        }
        #endregion



        private void OnDestroy()
        {
            saveTemplate.SaveTemplateData(m_Templates);
            DestroyAllCanvasObjects();
        }
    }



}

