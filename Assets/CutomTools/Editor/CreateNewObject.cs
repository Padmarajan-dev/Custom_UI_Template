using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Codice.Utils;
using UnityEngine.UI;
using System.ComponentModel;
using System.IO;
using System;

namespace com.editor.customuicreator
{
    public class CreateNewObject : EditorWindow
    {

        //elemnent props
        private string m_ElementName;
        private Vector3 m_ElementPosition;
        private Vector3 m_ElementScale;
        private Vector3 m_ElementRotation;
        private GameObject m_Parent;
        private List<UIObject> m_Elements;
        private string m_ParentObjectName;
        private string m_ObjectType;

        private bool showEmptyGameObjectPropsCalled = false;
        CreateTemplateWindow templateWindow;

        private Texture2D selectedImage;
        private string selectedImagePath;

        private GameObject m_CreatedGameobject;
        UIObject newobject;
        public static void ShowCreateNeobjectWindow()
        {
            GetWindow<CreateNewObject>("Create Object");
        }
        private List<string> options;

        private int OptionId = -1;


        private void OnGUI()
        {
            //Align the first two buttons
                GUILayout.BeginHorizontal();
            if (GUILayout.Button(options[0]))
            {
                OptionId = 0;
                if (m_CreatedGameobject != null)
                {
                    ResetObject();
                }
                else
                {

                    m_CreatedGameobject = new GameObject(m_ElementName);
                }
            }
            if (GUILayout.Button(options[1]))
            {
                OptionId = 1;
                if (m_CreatedGameobject != null)
                {
                    ResetObject();
                }
                else
                {

                    m_CreatedGameobject = new GameObject(m_ElementName);
                }
            }
            GUILayout.EndHorizontal();

            // Align the last two buttons
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(options[2]))
            {
                // Handle button click actions for options[2]
            }
            if (GUILayout.Button(options[3]))
            {
                // Handle button click actions for options[3]
            }
            GUILayout.EndHorizontal();


            if (OptionId >= 0)
            {
                GUILayout.BeginScrollView(new Vector2(0, 0), GUILayout.Width(700));
                EditGameObjects(OptionId);
                GUILayout.EndScrollView();
            }


            //}
        }

        private void ResetObject()
        {
            m_CreatedGameobject = null;
            m_ElementName = string.Empty;
            m_ElementPosition = Vector3.zero;
            m_ElementScale = Vector3.one;
            m_Parent = null;
            newobject = null;
        }

        //to show and edit gameobjects props
        public void EditGameObjects(int i)
        {
            switch (i)
            {
                case 0:
                    ShowEmptyGameObjectProps();
                    break;
               case 1:
                    ShowImageGameObjectProps(); 
                    break;
               case 2:
                    ShowButtonGameObjectProps();
                    break;
               case 3:
                    ShowTextFieldGameObjectProps();
                    break;
               default: 
                    break;
            }
        }
        //empty gameobject
        private void ShowEmptyGameObjectProps()
        {
            if (m_CreatedGameobject != null)
            {
                m_ObjectType = "Empty";
                CommonGameObjectProps();
                Buttons();
            }
            EditCreatedObjet();

        }
        #region Object methods
        //image object
        private void ShowImageGameObjectProps()
        {
            if (m_CreatedGameobject != null)
            {
                CommonGameObjectProps();
                if (m_CreatedGameobject.GetComponent<Image>() == null)
                {
                    m_CreatedGameobject.AddComponent<Image>();
                }
                
                GUILayout.Label("Element Image", EditorStyles.boldLabel);

                if (selectedImage != null)
                {
                    GUILayout.Label(selectedImage, GUILayout.Width(60), GUILayout.Height(60));

                        // Create a sprite from the loaded texture
                        Sprite sprite = Sprite.Create(selectedImage, new Rect(0, 0, selectedImage.width, selectedImage.height), Vector2.one * 0.5f);
                            // Assign the sprite to the Image component
                        m_CreatedGameobject.GetComponent<Image>().sprite = sprite;
                        m_ObjectType = "Image";
                }

                if (GUILayout.Button("Select Image"))
                {
                    OpenImagePicker();
                }

                Buttons();
            }
            EditCreatedObjet();

        }
        //Button Object
        private void ShowButtonGameObjectProps()
        {
            if (m_CreatedGameobject != null)
            {
                CommonGameObjectProps();
                Buttons();
            }
            EditCreatedObjet();

        }

        //textfield Object
        private void ShowTextFieldGameObjectProps()
        {
            if (m_CreatedGameobject != null)
            {
                CommonGameObjectProps();
                Buttons();
            }
            EditCreatedObjet();

        }
        #endregion
        #region Common Methods
        private void EditCreatedObjet()
        {
            if (m_CreatedGameobject != null)
            {
                if (IsGameObjectInHierarchy(m_CreatedGameobject))
                {
                    m_CreatedGameobject.transform.name = m_ElementName;
                    m_CreatedGameobject.transform.localPosition = m_ElementPosition;
                    m_CreatedGameobject.transform.localEulerAngles = m_ElementRotation;
                    m_CreatedGameobject.transform.localScale = m_ElementScale;
                    if (m_Parent != null)
                    {
                        m_CreatedGameobject.transform.parent = m_Parent.transform;

                    }
                    if (newobject == null)
                    {
                        newobject = new UIObject();
                    }
                    newobject._Object = m_CreatedGameobject;
                    newobject._ObjectName = m_ElementName;
                    newobject._ObjectPosition = m_ElementPosition;
                    newobject._ObjectRotation = m_ElementRotation;
                    newobject._ObjectScale = m_ElementScale;
                    newobject._Parent = m_Parent;
                    newobject._Image = selectedImage;
                    newobject._ParentObjectName = m_Parent.transform.name;
                    newobject._ObjectType = m_ObjectType;
                    newobject._ImagePath = selectedImagePath;

                    if (m_Elements.Contains(newobject) == false)
                    {
                        m_Elements.Add(newobject);
                    }
                    else
                    {
                        m_Elements.Remove(newobject);
                        m_Elements.Add(newobject);
                    }
                }
            }
        }

        private void Buttons()
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Add In Template"))
            {

                CreateTemplateWindow.newTemplate._UIObjects = m_Elements;
                if (m_CreatedGameobject != null)
                {
                    //DestroyImmediate(m_CreatedGameobject);
                    m_CreatedGameobject = null;
                }

            }
            if (GUILayout.Button("Remove"))
            {
                if (m_CreatedGameobject != null)
                {
                    DestroyImmediate(m_CreatedGameobject);
                    m_CreatedGameobject = null;
                }
            }
            GUILayout.EndHorizontal();
        }



        private void CommonGameObjectProps()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("GameObjectName", EditorStyles.boldLabel);
            m_ElementName = EditorGUILayout.TextField("", m_ElementName);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("GameObjectPosition", EditorStyles.boldLabel);
            m_ElementPosition = EditorGUILayout.Vector3Field("", m_ElementPosition);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("GameObjectRoatation", EditorStyles.boldLabel);
            m_ElementRotation = EditorGUILayout.Vector3Field("", m_ElementRotation);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("GameObjectScale", EditorStyles.boldLabel);
            m_ElementScale = EditorGUILayout.Vector3Field("", m_ElementScale);
            GUILayout.EndHorizontal();
            GUILayout.Label("Parent Object", EditorStyles.boldLabel);
            m_Parent = EditorGUILayout.ObjectField("My GameObject Field", m_Parent, typeof(GameObject), true) as GameObject;
        }

        private void OpenImagePicker()
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
                    selectedImage = LoadTexture(selectedImagePath);
                    Repaint();
                }
                catch (Exception e)
                {
                    Debug.LogError("Error copying image: " + e.Message);
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
        #endregion

        #region event methods

        [System.Obsolete]
        private void OnEnable()
        {
            EditorApplication.hierarchyWindowChanged += OnHierarchyChanged;
            options = new List<string>() { "empty", "Image", "Button", "TextField"};
            m_Elements = new List<UIObject>();
            templateWindow = new CreateTemplateWindow();
            Canvas canvas = FindObjectOfType<Canvas>();
            // If Canvas is found, use it as the default parent
            GameObject defaultParent = canvas != null ? canvas.gameObject : null;
            // Display a dropdown to select the parent
            m_Parent = EditorGUILayout.ObjectField("My GameObject Field", defaultParent, typeof(GameObject), true) as GameObject;
        }

        [System.Obsolete]
        private void OnDisable()
        {
            EditorApplication.hierarchyWindowChanged -= OnHierarchyChanged;
            
        }

        private void OnHierarchyChanged()
        {
            // Check if m_CreatedGameobject is null, 
            // or if it's not null but the corresponding GameObject is not in the hierarchy
            if (m_CreatedGameobject == null || !IsGameObjectInHierarchy(m_CreatedGameobject))
            {
                ResetObject();
            }
        }

        private bool IsGameObjectInHierarchy(GameObject gameObject)
        {
            // Check if the GameObject is in the hierarchy
            return gameObject != null && gameObject.scene.IsValid();
        }
        #endregion
    }
}