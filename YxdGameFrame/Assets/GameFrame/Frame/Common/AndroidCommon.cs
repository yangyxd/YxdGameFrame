using UnityEngine;
using System.Collections;

/// <summary>
/// 游戏架构
/// </summary>
namespace GameFrame {

	/// <summary>
	/// Android common.
	/// </summary>
	public static class AndroidCommon {
		#if UNITY_ANDROID || UNITY_ANDROID_API
		#if UNITY_EDITOR
		#else
		private static AndroidJavaClass jc = null;
		#endif
		private static AndroidJavaObject jo = null;

		static AndroidCommon() {
			#if UNITY_EDITOR
			#else
			jc = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
			jo = jc.GetStatic<AndroidJavaObject> ("currentActivity");
			#endif
		}
		#endif

		public static void Call(string func, params object[] args) {
			#if UNITY_ANDROID || UNITY_ANDROID_API
			if (jo != null)
				jo.Call (func, args);
			#endif
		}

		/// <summary>
		/// 显示一个提示
		/// </summary>
		public static void Hint(string msg) {
			#if UNITY_ANDROID || UNITY_ANDROID_API
			if (jo != null)
				jo.Call ("showToast", msg);
			#endif
		}

		public static void Log(string msg) {
			#if UNITY_ANDROID || UNITY_ANDROID_API
			if (jo != null)
				jo.Call ("LogD", "UnityLogD", msg);
			#endif
		}

		public static void LogE(string msg) {
			#if UNITY_ANDROID || UNITY_ANDROID_API
			if (jo != null)
				jo.Call ("LogE", "UnityLogE", msg);
			#endif
		}

		public static void LogW(string msg) {
			#if UNITY_ANDROID || UNITY_ANDROID_API
			if (jo != null)
				jo.Call ("LogW", "UnityLogW", msg);
			#endif
		}
	}
}
