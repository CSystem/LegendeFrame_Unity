using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEditor;
using LitJson;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public delegate void ResCallBack(string path,string name,object obj,object param);

namespace com.LengdeFrame.commom.Tool
{
    public class UnityTools
    {
        public static void Destroy(params object[] obj)
        {
            _Destroy(obj);
        }

        private static void _Destroy(IEnumerable eles)
        {
            foreach (object ele in eles)
            {
                Object obj = ele as Object;
                if (null != obj)
                {
                    Object.DestroyImmediate(obj);
                }
            }
        }

        /// <summary>
        /// To load the resource from the resource folder
        /// U can choose to instantiate the obj Yes or No
        /// </summary>
        /// <returns>The object from resources.</returns>
        /// <param name="path">Path.</param>
        /// <param name="bInstance">If set to <c>true</c> b instance.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T GetResources<T>(string path, bool bInstance = true)
        {
            return GetResources<T>(path, default(T), bInstance);
        }

        public static T GetResources<T>(string path, T dftObj, bool bInstance = true)
        {
            object ret = dftObj;
            Type type = typeof(T);
            Object obj = Resources.Load(path);
            if (null != obj)
            {
                if (typeof(String) == type)
                {
                    TextAsset textAsset = obj as TextAsset;
                    ret = textAsset;
                } else
                {
                    if (typeof(GameObject) == type)
                    {
                        if (bInstance)
                        {
                            ret = GameObject.Instantiate(obj) as GameObject;
                        } else
                        {
                            ret = obj as GameObject;
                        }
                    } else
                    {
                        ret = obj;
                    }

                }
            }
            return (T)ret;
        }

        public static void GetRes<T>(string path, string name, ResCallBack resCall, Object param, bool bInstace = true)
        {
            if (string.IsNullOrEmpty(path))
            {
                if (null != resCall)
                {
                    resCall(path, name, null, param);
                }
                return;
            }

            try
            {
                T ret = GetResources<T>(path);

                if (null != ret)
                {
                    resCall(path, name, ret, param);
                    return;
                } else
                {
                    //TODO load from streamassets, if not exist ,download from WWW
                }
            } catch (System.Exception e)
            {
                Debug.LogError("GetRes " + e.ToString());           
            }
        }

        public static void TextureCallBack(string path, string name, Object obj, Object param)
        {
            if (param is UITexture)
            {
                ((UITexture)param).mainTexture = obj as Texture;      
            }
        }

        public static string GetPlayerPrefsString(string key)
        {
            return PlayerPrefs.GetString(key);
        }

        public static int GetPlayerPrefsInt(string key)
        {
            return PlayerPrefs.GetInt(key);
        }

        public static float GetPlayerPrefsFloat(string key)
        {
            return PlayerPrefs.GetFloat(key);
        }

        public static void SetPlayerPrefsString(string key, string val)
        {
            PlayerPrefs.SetString(key, val);
            PlayerPrefs.Save();
        }

        public static void SetPlayerPrefsInt(string key, int val)
        {
            PlayerPrefs.SetInt(key, val);
            PlayerPrefs.Save();
        }

        public static void SetPlayerPrefsFloat(string key, float val)
        {
            PlayerPrefs.SetFloat(key, val);
            PlayerPrefs.Save();
        }

        public static void SetActive(bool bIsActive, params GameObject[] objs)
        {
            if (null == objs || 0 >= objs.Length)
                return;

            for (int i = 0; i < objs.Length; i++)
            {
                if (null == objs [i])
                    continue;

                SetActive(objs [i], bIsActive);
            }
        }

        public static void SetActive(bool bIsActive, params Transform[] trans)
        {
            if (null == trans || 0 >= trans.Length)
                return;

            for (int i = 0; i < trans.Length; i++)
            {
                if (null == trans [i])
                    continue;

                SetActive(trans [i], bIsActive);
            }

        }

        public static void SetActive(GameObject obj, bool bIsActive)
        {
            NGUITools.SetActive(obj, bIsActive);
        }

        public static void SetActive(Transform tran, bool bIsActive)
        {
            if (null == tran)
                return;
            
            SetActive(tran.gameObject, bIsActive);
        }

        public static GameObject Instantiate(GameObject go)
        {
            return Instantiate(go, null);
        }

        public static GameObject Instantiate(GameObject go, Component parent)
        {
            GameObject clone = Object.Instantiate(go) as GameObject;
            clone.name = go.name;

            if (null != parent)
            {
                clone.transform.parent = parent.transform;
            }
            if (null != clone.renderer && null == clone.renderer.sharedMaterials)
            {
                clone.renderer.sharedMaterials = go.renderer.sharedMaterials;
            }

            clone.transform.localScale = go.transform.localScale;
            clone.transform.localPosition = go.transform.localPosition;
            clone.transform.localEulerAngles = go.transform.localEulerAngles;
            clone.transform.localRotation = go.transform.localRotation;

            return clone;
        }

        public static GameObject Instantiate(GameObject go, Vector3 pos, Component parent)
        {
            GameObject clone = Instantiate(go, parent);
            clone.transform.localPosition = pos;
            return clone;
        }

        public static GameObject Instantiate(GameObject go, Quaternion rote, Component parent)
        {
            GameObject clone = Instantiate(go, parent);
            clone.transform.localRotation = rote;
            return clone;
        }

        public static GameObject Instantiate(GameObject go, Vector3 pos, Quaternion rote, Component parent)
        {
            GameObject clone = Instantiate(go, parent);
            clone.transform.localRotation = rote;
            clone.transform.localPosition = pos;
            return clone;
        }

        public static List<T> FindComponentInChildren<T>(Transform root) where T : UnityEngine.Component
        {
            if (null == root)
                return null;

            List<T> ret = new List<T>();

            T com = root.GetComponent<T>();

            if (null != com)
                ret.Add(com);

            for (int i = 0; i < root.childCount; i++)
            {
                Transform tran = root.GetChild(i);

                if (null == tran)
                    continue;

                List<T> tmpComp = FindComponentInChildren<T>(tran);

                if (null == tmpComp)
                {
                    ret.AddRange(tmpComp);
                }
            }

            return ret;
        }

        public static List<T> FindComponentInChildren<T>(GameObject go) where T : UnityEngine.Component
        {
            if (null == go)
                return null;
            
            List<T> ret = new List<T>();

            Transform root = go.transform;

            T com = root.GetComponent<T>();
            
            if (null != com)
                ret.Add(com);
            
            for (int i = 0; i < root.childCount; i++)
            {
                Transform tran = root.GetChild(i);
                
                if (null == tran)
                    continue;
                
                List<T> tmpComp = FindComponentInChildren<T>(tran);
                
                if (null == tmpComp)
                {
                    ret.AddRange(tmpComp);
                }
            }
            
            return ret;
        }

        public static void SetActive<T>(Transform tran, bool bIsActive) where T : UnityEngine.Component
        {
            List<T> comp = FindComponentInChildren<T>(tran);
            
            if (null != comp)
            {
                for (int i = 0; i< comp.Count; i++)
                {
                    SetActive(comp [i], bIsActive);
                }
            }
        }

        public static void SetActive<T>(GameObject go, bool bIsActive) where T : UnityEngine.Component
        {
            List<T> comp = FindComponentInChildren<T>(go);

            if (null != comp)
            {
                for (int i = 0; i< comp.Count; i++)
                {
                    SetActive(comp [i], bIsActive);
                }
            }
        }

        public static void SetActive<T>(bool bIsActive, params GameObject[] objs) where T : UnityEngine.Component
        {
            if (null != objs && 0 < objs.Length)
            {
                for (int i = 0; i < objs.Length; i++)
                {
                    
                    if (null != objs [i])
                    {
                        SetActive<T>(objs [i], bIsActive);
                    }
                }
            }
        }

        public static void SetActive<T>(bool bIsActive, params Transform[] trans) where T : UnityEngine.Component
        {
            if (null != trans && 0 < trans.Length)
            {
                for (int i = 0; i < trans.Length; i++)
                {
                    if (null != trans [i])
                    {
                        SetActive<T>(trans [i], bIsActive);
                    }
                }
            }
        }
        /// <summary>
        /// setActive the component
        /// </summary>
        /// <param name="comp">Comp.</param>
        /// <param name="bIsActive">If set to <c>true</c> b is active.</param>
        public static void SetActive(Component comp, bool bIsActive)
        {
            if (null != comp)
            {
                SetActive(comp.gameObject, bIsActive);
            }
        }
    }
}
