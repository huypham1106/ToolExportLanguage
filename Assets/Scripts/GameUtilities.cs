//using Sfs2X.Entities.Data;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
public class GameUtilities : MonoBehaviour {


    public static Dictionary<string, object> ParseStringToDictionary(string source)
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

    public static int ParseStringToInt(string str)
    {
        int result = 0;
        int.TryParse(str, out result);
        return result;
    }

    public static bool ParseStringToBool(string str)
    {
        bool result = false;
        bool.TryParse(str, out result);
        return result;
    }
    public static bool ParseIntToBool(int str)
    {
        bool result = str != 0;
        return result;
    }
    public static float ParseStringToFloat(string str)
    {
        float result = 0;
        float.TryParse(str, out result);
        return result;
    }
    public static double ParseStringToDouble(string str)
    {
        double result = 0;
        double.TryParse(str, out result);
        return result;
    }
    public static List<object> ParseStringToListObject(string source)
    {
        List<object> result = MiniJSON.Json.Deserialize(source) as List<object>;
        return result as List<object>;
    }
    public static Dictionary<string, object> GetDictionaryFromDictionary(Dictionary<string, object> source, string key)
    {
        return source[key] as Dictionary<string, object>;
    }


    public static long ParseStringToLong(string str)
    {
        long result = 0;
        long.TryParse(str, out result);
        return result;
    }

  

    public static string ParseIntArrayToString(int[] source)
    {
        return null;
        //System.Text.StringBuilder sb = new System.Text.StringBuilder();
        //for (int i = 0; i < source.Length; i++)
        //{
        //    sb.Append(source[i]);
        //    if (i < source.Length - 1)
        //    {
        //        sb.Append(GameConstants.GACH_DUOI);
        //    }
        //}
        //return sb.ToString();
    }
    static public object GetValProObject(object obj, string propertyName)
    {
        return obj.GetType().GetProperty(propertyName).GetValue(obj, null);
    }

}

