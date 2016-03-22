using UnityEngine;
using System.Collections;

/// <summary>
/// 公用基础函数库
/// <remarks>作者: YangYxd</remarks>
/// </summary>
public class Common {
	

    /// <summary>
    /// 整型转为字符串
    /// </summary>
	public static string intToStr(int v) {
        return v.ToString();
	}

    /// <summary>
    /// 字符串转为整型
    /// </summary>
    public static int strToInt(string v, int defaultValue = 0) {
        int i = defaultValue;
        int.TryParse(v, out i);
        return i;
    }

	/// <summary>
	/// 根据指定的资源类型，查找游戏资源
	/// </summary>
	/// <returns>返回资源对象</returns>
	/// <param name="name">资源名称</param>
	public static T findRes<T>(string name) where T : Object {
		T[] objs = Resources.FindObjectsOfTypeAll<T>();
		if (objs != null && objs.Length > 0) {
			foreach (Object obj in objs) {
				if (obj.name == name)
					return obj as T;
			}
		}
		return default(T);
	}

}
