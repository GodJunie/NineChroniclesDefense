using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UnityExtensions {
    public static List<T> Splice<T> (this List<T> source, int index, int count) {
        var items = source.GetRange(index, count);
        source.RemoveRange(index, count);
        return items;
    }

    public static string GetHexString(this Color color) {
        string r = ((int)(color.r * 255)).ToString("X2");
        string g = ((int)(color.g * 255)).ToString("X2");
        string b = ((int)(color.b * 255)).ToString("X2");
        string a = ((int)(color.a * 255)).ToString("X2");

        string result = string.Format("{0}{1}{2}{3}", r, g, b, a);

        return result;
    }
}

namespace DG.Tweening {

}