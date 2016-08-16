#if UNITY_EDITOR && !REMOTELOG
#define DEBUGLOG
#endif

using UnityEngine;
using System.Collections;

/// <summary>
/// 游戏架构
/// </summary>
namespace GameFrame {

	/// <summary>
	/// 调试消息接口
	/// </summary>
	public interface IDebugMessage {};

	/// <summary>
	/// 调试消息共用模块
	/// <remarks>如果需要启用，请设置Unity的Player Setting， 在 Scripting Define Symbols 中添加 NEEDLOG</remarks>
	/// </summary>
	public static class DebugMessage {		
		/// <summary>
		/// 远程日志输出地址
		/// </summary>
		public static string RemoteHost = 
			#if UNITY_IPHONE || UNITY_ANDROID
			"192.168.1.16";
			#else
			"127.0.0.1";
			#endif
		/// <summary>
		/// 远程日志输出端口
		/// </summary>
		public static int RemotePort = 6699;


		#if REMOTELOG
		private static UDPClient UDP = null;
		#endif

		private static void Print(string message) {
			#if UNITY_ANDROID || UNITY_ANDROID_API	
			AndroidCommon.Log(message);
			#else
			Debug.Log(message);
			#endif
		}

		private static void PrintE(string message) {
			#if UNITY_ANDROID || UNITY_ANDROID_API	
			AndroidCommon.LogE(message);
			#else
			Debug.LogError(message);
			#endif
		}

		private static void PrintW(string message) {
			#if UNITY_ANDROID || UNITY_ANDROID_API	
			AndroidCommon.LogW(message);
			#else
			Debug.LogWarning(message);
			#endif
		}

		private static void SendLog(string message) {
			#if REMOTELOG
			if (UDP == null)
				UDP = new UDPClient(RemoteHost, RemotePort);
			#if UNITY_EDITOR
			UDP.Send (message);
			#else
			try {
				UDP.Send (message);
			} catch (System.Exception e) {
				Print(message);
				PrintE(e.Message);
			}
			#endif
			#endif
		}

		/// <summary>
		/// 输出普通日志
		/// </summary>
		public static void Log(string message) {
			#if NEEDLOG 
			#if DEBUGLOG
			Debug.Log(message);
			#else
			SendLog(message);
			#endif
			#endif
		}

		/// <summary>
		/// 输出错误日志
		/// </summary>
		public static void LogE(string message) {
			#if NEEDLOG
			Print(message);
			#if REMOTELOG
			SendLog("Error: " + message);
			#endif
			#endif
		}

		/// <summary>
		/// 输出警告日志
		/// </summary>
		public static void LogW(string message) {
			#if NEEDLOG
			PrintW(message);
			#if REMOTELOG
			SendLog("Warning: " + message);
			#endif
			#endif
		}

		/// <summary>
		/// 方法开始时调用
		/// </summary>
		public static void STARTMETHOD<T>(this T t, string methodName) where T: IDebugMessage {
			#if NEEDLOG
			Log(string.Format("===== Method Start: {0}.{1} =====", t.GetType().Name, methodName));
			#endif
		}

		/// <summary>
		/// 方法结束时调用
		/// </summary>
		public static void ENDMETHOD<T>(this T t, string methodName) where T: IDebugMessage {
			#if NEEDLOG
			Log(string.Format("===== Method End:   {0}.{1} =====", t.GetType().Name, methodName));
			#endif
		}

		/// <summary>
		/// 打印消息
		/// </summary>
		public static void LOG<T>(this T t, string message) where T: IDebugMessage {
			#if NEEDLOG
			Log(string.Format("[{0}] {1}", t.GetType().Name, message));
			#endif
		}

		/// <summary>
		/// 打印消息
		/// </summary>
		public static void LOG<T>(this T t, int message) where T: IDebugMessage {
			#if NEEDLOG
			Log(string.Format("[{0}] {1}", t.GetType().Name, message.ToString()));
			#endif
		}

		/// <summary>
		/// 打印消息
		/// </summary>
		public static void LOG<T>(this T t, float message) where T: IDebugMessage {
			#if NEEDLOG
			Log(string.Format("[{0}] {1}", t.GetType().Name, message.ToString()));
			#endif
		}

		/// <summary>
		/// 打印消息
		/// </summary>
		public static void LOG<T>(this T t, long message) where T: IDebugMessage {
			#if NEEDLOG
			Log(string.Format("[{0}] {1}", t.GetType().Name, message.ToString()));
			#endif
		}

		/// <summary>
		/// 打印错误消息
		/// </summary>
		public static void LOGERROR<T>(this T t, string message) where T: IDebugMessage {
			#if NEEDLOG
			LogE(string.Format("[{0}] {1}", t.GetType().Name, message));
			#endif
		}

		/// <summary>
		/// 打印警告消息
		/// </summary>
		public static void LOGWARNING<T>(this T t, string message) where T: IDebugMessage {
			#if NEEDLOG
			LogW(string.Format("[{0}] {1}", t.GetType().Name, message));
			#endif
		}
	}
}
