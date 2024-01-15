using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.ComponentModel;
using System.Collections.Generic;
using System.IO;
using System;

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

            // List of added elements in template

            if (newTemplate != null)
            {
                GUILayout.BeginScrollView(new Vector2(0, 0), GUILayout.Width(600));
                if (newTemplate._UIObjects != null)
                {
                    if(newTemplate._UIObjects.Count > 0)
                    {
                        foreach (UIObject uIObject in newTemplate._UIObjects)
                        {
                            ShowUIElements(uIObject);
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

            //to save template
            if (GUILayout.Button("Save Template"))
            {
                // Check if a template with the same name already exists
                Template existingTemplate = null;
                if (m_Templates!=null && m_Templates.Count > 0)
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
                    if(m_Templates != null)
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
                    Debug.Log(newTemplate._UIObjects.Count);
                    m_TemplateName = string.Empty;
                    selectedImagePath = string.Empty;
                    selectedImage = null;
                    newTemplate = null;
                    Repaint();
                }
                
            }
        }
        #endregion

        #region Common Methods

        //to select image from files
        private void OpenImagePicker(ref Texture2D image)
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
        public void ShowUIElements(UIObject uiobject)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("GameObjectName", EditorStyles.boldLabel);
            uiobject._ObjectName = EditorGUILayout.TextField("", uiobject._ObjectName);
            uiobject._Object.name = uiobject._ObjectName;
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("GameObjectPosition", EditorStyles.boldLabel);
            uiobject._ObjectPosition = EditorGUILayout.Vector3Field("", uiobject._ObjectPosition);
            uiobject._Object.transform.localPosition = uiobject._ObjectPosition;
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("GameObjectRoatation", EditorStyles.boldLabel);
            uiobject._ObjectRotation= EditorGUILayout.Vector3Field("", uiobject._ObjectRotation);
            uiobject._Object.transform.localEulerAngles = uiobject._ObjectRotation;
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("GameObjectScale", EditorStyles.boldLabel);
            uiobject._ObjectScale = EditorGUILayout.Vector3Field("", uiobject._ObjectScale);
            uiobject._Object.transform.localScale = uiobject._ObjectScale;
            GUILayout.EndHorizontal();
            GUILayout.Label("Parent Object", EditorStyles.boldLabel);
            uiobject._Parent = EditorGUILayout.ObjectField("My GameObject Field", uiobject._Parent, typeof(GameObject), true) as GameObject;
            uiobject._Object.transform.parent = uiobject._Parent.transform;
            if (uiobject._Object.GetComponent<Image>())
            {
                GUILayout.Label("Element Image", EditorStyles.boldLabel);
                if (uiobject._Image != null)
                {
                    GUILayout.Label(uiobject._Image, GUILayout.Width(60), GUILayout.Height(60));
                    // Create a sprite from the loaded texture
                    Sprite sprite = Sprite.Create(uiobject._Image, new Rect(0, 0, uiobject._Image.width, uiobject._Image.height), Vector2.one * 0.5f);
                    // Assign the sprite to the Image component
                    uiobject._Object.GetComponent<Image>().sprite = sprite;
                }

                if (GUILayout.Button("Select Image"))
                {
                    OpenImagePicker(ref uiobject._Image);
                }
            }
        }

        //to load Texture
        private Texture2D LoadTexture(string path)
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
        #endregion



        private void OnDestroy()
        {
            saveTemplate.SaveTemplateData(m_Templates);
        }
    }



}

