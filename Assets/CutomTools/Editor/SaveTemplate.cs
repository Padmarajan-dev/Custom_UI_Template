using Codice.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Plastic.Newtonsoft.Json;
using Unity.Plastic.Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace com.editor.customuicreator
{
    [System.Serializable]
    public class SaveTemplate
    {

        private string m_JsonDataPath = "Assets/Resources/Data";
        private GameObject rootGameObject;

        JsonSerializerSettings settings = new JsonSerializerSettings
        {
            Converters = new List<JsonConverter> { new Vector3Converter(), new ColorConverter(),new Vector2Converter() }
        };
        private GameObject canvasObject;

        public void SaveTemplateData(List<Template> templatedata)
        {
            string path = m_JsonDataPath + "/TemplatesData.json";
            string jsondata = JsonConvert.SerializeObject(templatedata,settings);

            File.WriteAllText(path, jsondata);
            

        }


        public List<Template> LoadTemplateData()
        {
            string path = m_JsonDataPath + "/TemplatesData.json";
            try
            {
                string jsondata = System.IO.File.ReadAllText(path);
                List<Template> templates = JsonConvert.DeserializeObject<List<Template>>(jsondata,settings);
                return templates;
            }
            catch (Exception e)
            {
                Debug.Log("Couldn't get data from json");
                return new List<Template>(); // Return an empty list or handle the error accordingly
            }

        }


        #region Custom Converters
        public class Vector3Converter : JsonConverter
        {
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                Vector3 vector = (Vector3)value;
                writer.WriteStartObject();
                writer.WritePropertyName("x");
                writer.WriteValue(vector.x);
                writer.WritePropertyName("y");
                writer.WriteValue(vector.y);
                writer.WritePropertyName("z");
                writer.WriteValue(vector.z);
                writer.WriteEndObject();
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                JObject obj = JObject.Load(reader);
                float x = obj.GetValue("x").Value<float>();
                float y = obj.GetValue("y").Value<float>();
                float z = obj.GetValue("z").Value<float>();
                return new Vector3(x, y, z);
            }

            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(Vector3);
            }
        }

        //public class GameObjectConverter : JsonConverter
        //{
        //    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        //    {
        //        GameObject gameObject = (GameObject)value;
        //        writer.WriteValue(gameObject.name);
        //    }

        //    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        //    {
        //        string gameObjectName = (string)reader.Value;
        //        return GameObject.Find(gameObjectName);
        //    }

        //    public override bool CanConvert(Type objectType)
        //    {
        //        return objectType == typeof(GameObject);
        //    }
        //}

        public class ColorConverter : JsonConverter
        {
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                Color color = (Color)value;
                writer.WriteStartObject();
                writer.WritePropertyName("r");
                writer.WriteValue(color.r);
                writer.WritePropertyName("g");
                writer.WriteValue(color.g);
                writer.WritePropertyName("b");
                writer.WriteValue(color.b);
                writer.WritePropertyName("a");
                writer.WriteValue(color.a);
                writer.WriteEndObject();
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                JObject obj = JObject.Load(reader);
                float r = obj.GetValue("r").Value<float>();
                float g = obj.GetValue("g").Value<float>();
                float b = obj.GetValue("b").Value<float>();
                float a = obj.GetValue("a").Value<float>();
                return new Color(r, g, b, a);
            }

            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(Color);
            }
        }


        public class Texture2DConverter : JsonConverter
        {
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                Texture2D texture = (Texture2D)value;
                byte[] textureData = texture.EncodeToPNG();
                string base64Texture = System.Convert.ToBase64String(textureData);
                writer.WriteValue(base64Texture);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                string base64Texture = (string)reader.Value;
                byte[] textureData = System.Convert.FromBase64String(base64Texture);

                Texture2D texture = new Texture2D(2, 2); // Adjust the dimensions as needed
                texture.LoadImage(textureData);
                return texture;
            }

            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(Texture2D);
            }
        }
        public class Vector2Converter : JsonConverter
        {
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                Vector2 vector2 = (Vector2)value;
                writer.WriteStartObject();
                writer.WritePropertyName("x");
                writer.WriteValue(vector2.x);
                writer.WritePropertyName("y");
                writer.WriteValue(vector2.y);
                writer.WriteEndObject();
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                JObject obj = JObject.Load(reader);
                float x = obj.GetValue("x").Value<float>();
                float y = obj.GetValue("y").Value<float>();
                return new Vector2(x, y);
            }

            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(Vector2);
            }
        }
        #endregion

        #region to load gameobjects in hierarchy through json data
        public void LoadHierarchyFromJson(List<UIObject> uiobject)
        {
            if(uiobject.Count > 0)
            {
                foreach(UIObject obj in uiobject)
                {
                    CreateGameObject(obj);
                }
            }
        }



  
        private GameObject CreateGameObject(UIObject data)
        {
            GameObject newObject = data._Object != null ? data._Object : new GameObject(data._ObjectName);

            //if (data._Parent != null)
            //{
            //    newObject.transform.parent = data._Parent.transform;
            //}



            newObject.transform.localPosition = data._ObjectPosition;
            newObject.transform.localEulerAngles = data._ObjectRotation;
            newObject.transform.localScale = data._ObjectScale;

            if (data._ObjectType == "Image")
            {

                if (!string.IsNullOrEmpty(data._ImagePath))
                {

                    // Load the sprite from Resources based on the ImagePath
                    Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(data._ImagePath);

                    // Check if the sprite is not null before assigning
                    if (sprite != null)
                    {
                        // Get the Image component and assign the sprite
                        Image imageComponent = newObject.GetComponent<Image>();
                        if (imageComponent != null)
                        {
                            imageComponent.sprite = sprite;
                        }
                        else
                        {
                            newObject.AddComponent<Image>();
                            newObject.GetComponent<Image>().sprite = sprite;
                        }
                    }
                    else
                    {
                        Debug.LogWarning("Sprite not found at path: " + data._ImagePath);
                    }
                }
            }
            if (data._ParentObjectName != null)
            {
                GameObject parent = CreateTemplateWindow.FindObject(data._ParentObjectName);

                if (parent != null)
                {
                    newObject.transform.parent = parent.transform;
                }
            }

            return newObject;
        }
        #endregion

    }

    [System.Serializable]
    public class Template
    {
        public string _TemplateName;
        public string _TemplateImage;
        public List<UIObject> _UIObjects;
    }

    [System.Serializable]
    public class UIObject
    {

        [JsonIgnore] public GameObject _Object;
        public string _ObjectName;
        public Vector3 _ObjectPosition;
        public Vector3 _ObjectScale;
        public Vector3 _ObjectRotation;
        [JsonIgnore] public Texture2D _Image;
        public string _ImagePath;
        //public Color _ObjectColor;
        //public Color _TextColor;
        [JsonIgnore] public GameObject _Parent;
        public string _ParentObjectName;
        public string _ObjectType;
    }
}

