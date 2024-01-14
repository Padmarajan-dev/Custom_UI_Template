using Codice.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;
namespace com.editor.customuicreator
{
    [System.Serializable]
    public class SaveTemplate
    {

        private string m_JsonDataPath = "Assets/Resources/Data";
        public void SaveTemplateData(List<Template> templatedata)
        {
            string path = m_JsonDataPath + "/TemplatesData.json";
            string jsondata = JsonConvert.SerializeObject(templatedata);

            File.WriteAllText(path, jsondata);

        }


        public List<Template> LoadTemplateData()
        {
            string path = m_JsonDataPath + "/TemplatesData.json";
            try
            {
                string jsondata = System.IO.File.ReadAllText(path);
                List<Template> templates = JsonConvert.DeserializeObject<List<Template>>(jsondata);
                return templates;
            }
            catch (Exception e)
            {
                return new List<Template>(); // Return an empty list or handle the error accordingly
            }

        }

        //to save UI Objects Data
        public void SaveUIObjectsData(List<UIObject> UIObjects)
        {
            string path = m_JsonDataPath + "/UIObjectsData.json";
            string jsondata = JsonConvert.SerializeObject(UIObjects);

            File.WriteAllText(path, jsondata);
        }

        public List<UIObject> LoadUIObjectsData()
        {
            string path = m_JsonDataPath + "/TemplatesData.json";
            try
            {
                string jsondata = System.IO.File.ReadAllText(path);
                List<UIObject> templates = JsonConvert.DeserializeObject<List<UIObject>>(jsondata);
                return templates;
            }
            catch (Exception e)
            {
                return new List<UIObject>(); // Return an empty list or handle the error accordingly
            }

        }

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
        public GameObject _Object;
        public string _ObjectName;
        public Vector3 _ObjectPosition;
        public Vector3 _ObjectScale;
        public Vector3 _ObjectRotation;
        public Texture2D _Image;
        public Color _ObjectColor;
        public Color _TextColor;
        public GameObject _Parent;
        public List<UnityEngine.Component> _Components;
    }
}

