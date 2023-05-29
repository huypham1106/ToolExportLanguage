using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using DG.Tweening;
using System.Text.RegularExpressions;
using System.Linq;
using System.Text;
//using Spine.Unity;
//using Spine;
//using Spine.Unity.AttachmentTools;
//using CodeStage.AntiCheat.ObscuredTypes;

public static class GameExtension
{
   
    public static void SetSize(this RawImage image, float w, float h)
    {
        image.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(w, h);
    }

    public static void SetHeight(this Image image, float h)
    {
        Vector2 sizeDelta = image.transform.GetComponent<RectTransform>().sizeDelta;
        image.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(sizeDelta.x, h);
    }
    public static void SetWidth(this Image image, float w)
    {
        Vector2 sizeDelta = image.transform.GetComponent<RectTransform>().sizeDelta;
        image.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(w, sizeDelta.y);
    }
    public static void SetHeight(this Transform transform, float h)
    {
        Vector2 sizeDelta = transform.GetComponent<RectTransform>().sizeDelta;
        transform.GetComponent<RectTransform>().sizeDelta = new Vector2(sizeDelta.x, h);
    }
    public static void AddHeight(this Transform transform, float delta)
    {
        Vector2 sizeDelta = transform.GetComponent<RectTransform>().sizeDelta;
        transform.GetComponent<RectTransform>().sizeDelta = new Vector2(sizeDelta.x, sizeDelta.y + delta);
    }

    public static List<T> ToList<T>(this T[] array)
    {
        List<T> result = new List<T>(array);
        return result;
    }

    private static Shader shaderGray;
    private static Material materialGray;
    public static void GrayColor2(this MaskableGraphic maskableGraphic) // có thể xài alpha
    {
        if (shaderGray == null)
        {
            shaderGray = Shader.Find("Unlit/Grayscale");
            materialGray = new Material(shaderGray);
        }
        maskableGraphic.material = materialGray;
        Outline outline = maskableGraphic.GetComponent<Outline>();
        if (outline != null)
        {
            outline.enabled = false;
        }
    }
    public static void NormalColor(this MaskableGraphic maskableGraphic, bool autoEnableOutline = true, bool checkOtherMaterial = false)
    {
        if (!checkOtherMaterial)
        {
            maskableGraphic.material = null;
        }
        else
        {
            if (maskableGraphic.material == materialGray) maskableGraphic.material = null;
            //if (maskableGraphic.material == materialHSBC) maskableGraphic.material = null;
        }
        if (autoEnableOutline)
        {
            Outline outline = maskableGraphic.GetComponent<Outline>();
            if (outline != null)
            {
                outline.enabled = true;
            }
        }
    }

    public static V Get<K, V>(this Dictionary<K, V> keyValuePairs, K key)
    {
        if (keyValuePairs.ContainsKey(key))
        {
            return keyValuePairs[key];
        }
        return default(V);
    }

    public static void Put<K, V>(this Dictionary<K, V> keyValuePairs, K key, V value)
    {
        if (keyValuePairs.ContainsKey(key))
        {
            keyValuePairs.Remove(key);
        }
        keyValuePairs.Add(key, value);
    }

    public static string ToJson(this Dictionary<string, object> dictionary)
    {
        if (dictionary == null)
        {
            dictionary = new Dictionary<string, object>();
        }
        return MiniJSON.Json.Serialize(dictionary);
    }
    public static string ToJson(this List<object> list)
    {
        if (list == null)
        {
            list = new List<object>();
        }
        return MiniJSON.Json.Serialize(list);
    }

    public static bool IsNullOrEmpty(this ICollection collection)
    {
        return collection == null || collection.Count == 0;
    }

    public static bool IsNullOrEmpty(this string _string)
    {
        return _string == null || _string == string.Empty;
    }

    public static bool IsNullOrEmpty<T>(this T[] array)
    {
        return array == null || array.Length == 0;
    }

    public static void SetLeft(this RectTransform rt, float left)
    {
        rt.offsetMin = new Vector2(left, rt.offsetMin.y);
    }

    public static void SetRight(this RectTransform rt, float right)
    {
        rt.offsetMax = new Vector2(-right, rt.offsetMax.y);
    }

    public static List<object> GetList(this Dictionary<string, object> keyValuePairs, string key)
    {
        if (keyValuePairs.ContainsKey(key))
            return keyValuePairs[key] as List<object>;
        return null;
    }
    public static Dictionary<string, object> GetDictionary(this Dictionary<string, object> keyValuePairs, string key)
    {
        if (keyValuePairs.ContainsKey(key))
        {
            return keyValuePairs[key] as Dictionary<string, object>;
        }
        return null;
    }

    public static Dictionary<string, object> ToDictionary(this string source)
    {
        return GameUtilities.ParseStringToDictionary(source);
    }
    public static List<object> ToListDictionary(this string source)
    {
        return GameUtilities.ParseStringToListObject(source);
    }
    public static Dictionary<string, object> TryToDictionary(this string source)
    {
        object result = null;
        try
        {
            result = MiniJSON.Json.Deserialize(source);
        }
        catch (System.Exception e)
        {
        }
        return result as Dictionary<string, object>;
    }

    public static int ToInt(this object obj)
    {
        if (obj == null) return 0;
        return GameUtilities.ParseStringToInt(obj.ToString());
    }
    public static long ToLong(this object obj)
    {
        if (obj == null) return 0;
        return GameUtilities.ParseStringToLong(obj.ToString());
    }
    public static float ToFloat(this object obj)
    {
        return GameUtilities.ParseStringToFloat(obj.ToString());
    }
    public static double ToDouble(this object obj)
    {
        return GameUtilities.ParseStringToDouble(obj.ToString());
    }


    public static int GetInt(this Dictionary<string, object> keyValuePairs, string key)
    {
        int result = 0;
        try
        {
            if (keyValuePairs.ContainsKey(key))
            {
                int.TryParse(keyValuePairs[key].ToString(), out result);
            }
        }
        catch (System.Exception)
        {
            Debug.Log("key ne " + key);
        }
        
        return result;
    }

    public static long GetLong(this Dictionary<string, object> keyValuePairs, string key)
    {
        long result = 0;
        try
        {
            if (keyValuePairs.ContainsKey(key))
            {
                long.TryParse(keyValuePairs[key].ToString(), out result);
            }
        }
        catch (System.Exception)
        {
            Debug.Log("key ne " + key);
        }

        return result;
    }

    public static uint GetUInt(this Dictionary<string, object> keyValuePairs, string key)
    {
        uint result = 0;
        if (keyValuePairs.ContainsKey(key))
        {
            uint.TryParse(keyValuePairs[key].ToString(), out result);
        }
        return result;
    }
    public static float GetFloat(this Dictionary<string, object> keyValuePairs, string key)
    {
        float result = 0;
        if (keyValuePairs.ContainsKey(key))
        {
            float.TryParse(keyValuePairs[key].ToString(), out result);
        }
        return result;
    }
    public static string GetString(this Dictionary<string, object> keyValuePairs, string key)
    {
        if (keyValuePairs.ContainsKey(key))
        {
            if (keyValuePairs[key] != null)
            {
                return keyValuePairs[key].ToString();
            }
            return null;
        }
        return null;
    }

    public static bool GetBool(this Dictionary<string, object> keyValuePairs, string key)
    {
        bool result = false;
        if (keyValuePairs.ContainsKey(key))
        {
            bool.TryParse(keyValuePairs[key].ToString(), out result);
        }
        return result;
    }

    public static Transform[] GetAllChilds(this Transform trans)
    {
        int childCount = trans.childCount;
        Transform[] result = new Transform[childCount];
        for (int i = 0; i < childCount; i++)
        {
            result[i] = trans.GetChild(i);
        }
        return result;
    }

    public static void ActionFlip(this Transform _tranform, System.Action callbackHalf, System.Action callbackComplete)
    {
        float scaleX = _tranform.localScale.x;
        //_tranform.DOScaleX(0, 0.2f).OnComplete(() =>
        //{
        //    callbackHalf?.Invoke();
        //    _tranform.DOScaleX(scaleX, 0.2f).OnComplete(() =>
        //    {
        //        callbackComplete?.Invoke();
        //    });
        //});
    }

    public static string[] RemoveDuplicate(this string[] source)
    {
        List<string> result = new List<string>();
        foreach (var item in source)
        {
            if (!result.Contains(item))
            {
                result.Add(item);
            }
        }
        source = result.ToArray();
        return source;
    }
    public static void SetAlpha(this Transform transItem, float alpha = 1)
    {
        //Image[] images = transItem.GetComponentsInChildren<Image>();
        //if (images != null)
        //{
        //    for (int i = 0; i < images.Length; i++)
        //    {
        //        images[i].SetAlpha(alpha);
        //    }
        //}
        //RawImage[] rawImages = transItem.GetComponentsInChildren<RawImage>();
        //if (rawImages != null)
        //{
        //    for (int i = 0; i < rawImages.Length; i++)
        //    {
        //        rawImages[i].SetAlpha(alpha);
        //    }
        //}
        //Text[] texts = transItem.GetComponentsInChildren<Text>();
        //if (rawImages != null)
        //{
        //    for (int i = 0; i < texts.Length; i++)
        //    {
        //        texts[i].SetAlpha(alpha);
        //    }
        //}
    }

    public static void SetButtonInteractable(this Button button, bool allow)
    {
        //button.enabled = allow;
        //EffectScaleClick effectScaleClick = button.GetComponent<EffectScaleClick>();
        //if (effectScaleClick != null)
        //{
        //    effectScaleClick.enabled = allow;
        //}
    }
  
    public static void Shuffle<T>(this IList<T> ts)
    {
        var count = ts.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i)
        {
            var r = UnityEngine.Random.Range(i, count);
            var tmp = ts[i];
            ts[i] = ts[r];
            ts[r] = tmp;
        }
    }
}
