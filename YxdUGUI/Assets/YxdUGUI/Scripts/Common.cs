using UnityEngine;
using System.Collections;

/// <summary>
/// 公用基础函数库
/// <remarks>作者: YangYxd</remarks>
/// </summary>
public class Common {
	private const long Jan1st1970Ms = 621355968000000000L; //DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks;
	private const long Jan1st1970Ms_U8 = Jan1st1970Ms + 288000000000L;

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
	/// 返回当前时间的毫秒数, 这个毫秒其实就是自1970年1月1日0时起的毫秒数
	/// </summary>
	public static long currentTimeMillis() {		
		return (System.DateTime.UtcNow.Ticks - Jan1st1970Ms) / 10000;
	}

	/// <summary>
	/// 返回指定时间的毫秒数, 这个毫秒是自1970年1月1日0时起的毫秒数
	/// </summary>
	public static long getTimeInMillis(System.DateTime value) {		
		return (value.Ticks - Jan1st1970Ms) / 10000;
	}

	/// <summary>
	/// 从一个代表自1970年1月1日0时起的毫秒数，转换为DateTime (北京时间)
	/// </summary>
	public static System.DateTime getDateTime(long timeMillis) {
		return new System.DateTime (timeMillis * 10000 + Jan1st1970Ms_U8);
	}

	/// <summary>
	/// 从一个代表自1970年1月1日0时起的毫秒数，转换为DateTime (UTC时间)
	/// </summary>
	public static System.DateTime getDateTimeUTC(long timeMillis) {
		return new System.DateTime (timeMillis * 10000 + Jan1st1970Ms);
	}

	/// <summary>
	/// 返回一个以毫秒为单位的时间 （0001-01-01 00:00:00.000 算起）
	/// </summary>
	public static long getTicks() {
		return (long) (System.DateTime.UtcNow.Ticks * 0.0001f)	;
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
