using UnityEngine;
using System.Collections;
using System.IO;
using System.Security;  
using System.Security.Cryptography;  
using System.Text;  

/// <summary>
/// 游戏架构
/// </summary>
namespace GameFrame {

	public static class DES {
		//默认密钥向量  
		private static byte[] Keys = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };         
		/// DES加密字符串          
		/// 待加密的字符串  
		/// 加密密钥,要求为8位  
		/// 加密成功返回加密后的字符串，失败返回源串   
		public static string EncryptDES(string encryptString, string encryptKey)  
		{  
			try
			{  
				byte[] rgbKey = Encoding.UTF8.GetBytes(encryptKey.Substring(0, 8));  
				byte[] rgbIV = Keys;  
				byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);  
				DESCryptoServiceProvider dCSP = new DESCryptoServiceProvider();  
				MemoryStream mStream = new MemoryStream();  
				CryptoStream cStream = new CryptoStream(mStream, dCSP.CreateEncryptor(rgbKey, rgbIV), CryptoStreamMode.Write);  
				cStream.Write(inputByteArray, 0, inputByteArray.Length);  
				cStream.FlushFinalBlock();  
				return System.Convert.ToBase64String(mStream.ToArray());  
			}  
			catch  
			{  
				return encryptString;  
			}  
		}  
		///   
		/// DES解密字符串          
		/// 待解密的字符串  
		/// 解密密钥,要求为8位,和加密密钥相同  
		/// 解密成功返回解密后的字符串，失败返源串  
		public static string DecryptDES(string decryptString, string decryptKey)  
		{  
			try  
			{  
				byte[] rgbKey = Encoding.UTF8.GetBytes(decryptKey);  
				byte[] rgbIV = Keys;  
				byte[] inputByteArray = System.Convert.FromBase64String(decryptString);  
				DESCryptoServiceProvider DCSP = new DESCryptoServiceProvider();  
				MemoryStream mStream = new MemoryStream();  
				CryptoStream cStream = new CryptoStream(mStream, DCSP.CreateDecryptor(rgbKey, rgbIV), CryptoStreamMode.Write);  
				cStream.Write(inputByteArray, 0, inputByteArray.Length);  
				cStream.FlushFinalBlock();  
				return Encoding.UTF8.GetString(mStream.ToArray());  
			}  
			catch  
			{  
				return decryptString;  
			}  
		}  
	}
	
	/// <summary>
	/// 公用基础函数库
	/// <remarks>作者: YangYxd</remarks>
	/// </summary>
	public static class Common {
		private const long Jan1st1970Ms = 621355968000000000L; //DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).Ticks;
		private const long Jan1st1970Ms_U8 = Jan1st1970Ms + 288000000000L;


		public delegate bool CondDelegate(); 

		/// <summary>
		/// 等待指定的毫秒
		/// </summary>
		public static IEnumerator WaitForMillis(long millis, CondDelegate cond = null) { 
			long s = Common.Ticks;
			while(true) { 
				if (Common.Ticks - s < millis && (cond == null || !cond())) { 
					yield return null; 
				} else { 
					break; 
				} 
			} 
		} 

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
		public static long Ticks {
			get { 
				return (long) (System.DateTime.UtcNow.Ticks * 0.0001f)	;
			}
		}

		/// <summary>
		/// 是否是触摸设备
		/// </summary>
		public static bool isTouchDevice {
			get {
				#if UNITY_EDITOR
				return false;
				#else
				#if UNITY_IPHONE || UNITY_ANDROID
				return true;
				#else
				return false;
				#endif
				#endif
			}
		}


	    /// <summary>
	    /// 平台对应文件夹
	    /// </summary>
	    /// <returns></returns>
	    public static string getRuntimePlatform() {
	        string platform = "";
	        if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor) {
	            platform = "Windows";
	        } else if (Application.platform == RuntimePlatform.Android) {
	            platform = "Android";
	        } else if (Application.platform == RuntimePlatform.IPhonePlayer) {
	            platform = "IOS";
	        } else if (Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor) {
	            platform = "OSX";
	        }
	        return platform;
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
			objs = AssetBundle.FindObjectsOfType<T>();
			if (objs != null && objs.Length > 0) {
				foreach (Object obj in objs) {
					if (obj.name == name)
						return obj as T;
				}
			}
			return default(T);
		}
	    
	    /// <summary>
	    /// 获取文件扩展名
	    /// </summary>
	    /// <param name="file">文件名称（可以带路径）.</param>
	    public static string getFileExt(string file) {
	        if (file == null || file.Length == 0)
	            return "";
	        int i = file.LastIndexOf('.');
	        if (i < 0) return "";
	        return file.Substring(i + 1);
	    }

	    /// <summary>
	    /// 获取文件名称（包含扩展名）
	    /// </summary>
	    /// <param name="file">文件名称或完整路径</param>
	    public static string getFileName(string file) {
			return getFileName (file, true);
	    }

		/// <summary>
		/// 获取文件名称
		/// </summary>
		/// <param name="file">文件名称或完整路径</param>
		/// <param name="isHaveExt">是否包含有文件扩展名</param>
		public static string getFileName(string file, bool isHaveExt) {
			if (file == null || file.Length == 0)
				return "";
			int i = file.LastIndexOf('/');
			if (i < 0) {
				if (!isHaveExt) {
					int j = file.LastIndexOf ('.');
					if (j >= 0)
						return file.Substring (0, j);
				}
				return file;
			} else {
				if (isHaveExt)
					return file.Substring (i + 1);
				else {
					int j = file.LastIndexOf ('.');
					if (j > i)
						return file.Substring (i + 1, j - i - 1);
					else
						return file.Substring (i + 1);
				}
			}
		}

		/// <summary>
		/// 删除文件的扩展名
		/// </summary>
		public static string removeFileExt(string file) {
			if (file == null || file.Length == 0)
				return "";
			int i = file.LastIndexOf ('.');
			if (i < 0)
				return file;
			return file.Substring (0, i);
		}

	    /// <summary>
	    /// 获取文件路径
	    /// </summary>
	    /// <param name="file">文件完整路径</param>
	    public static string getFilePath(string file) {
	        if (file == null || file.Length == 0)
	            return "";
	        return Path.GetDirectoryName(file);
	    }

	    /// <summary>
	    /// 创建文件夹（如果文件夹已经存在，则不再创建）
	    /// </summary>
	    public static bool createFolder(string path) {
	        if (path == null || path.Length == 0 || Directory.Exists(path))
	            return false;
	        Directory.CreateDirectory(path);
	        return true;
	    }

	    /// <summary>
	    /// 检测文件夹是否存在
	    /// </summary>
	    public static bool existsPath(string path) {
	        if (path == null || path.Length == 0)
	            return false;
	        else
	            return Directory.Exists(path);
	    }

	    /// <summary>
	    /// 检测文件是否存在
	    /// </summary>
	    public static bool existsFile(string file) {
	        if (file == null || file.Length == 0)
	            return false;
	        else
	            return File.Exists(file);
	    }

	    /// <summary>
	    /// 删除文件
	    /// </summary>
	    public static bool deleteFile(string file) {
	        if (!existsFile(file))
	            return true;
	        try {
	            File.Delete(file);
	            return true;
	        } catch (System.Exception e) {
	            Debug.LogError(e.Message);
	            return false;
	        }
	    }

	    /// <summary>
	    /// 删除文件夹
	    /// <param name="recursive">是否删除所有子文件夹和文件</param>
	    /// </summary>
	    public static bool deleteFolder(string path, bool recursive) {
	        if (path == null || path.Length == 0)
	            return true;
	        try {
	            DirectoryInfo dir = new DirectoryInfo(path);
	            dir.Delete(recursive);
	            return true;
	        } catch (System.Exception e) {
	            Debug.LogError(e.Message);
	            return false;
	        }
	    }

	    /// <summary>
	    /// 返回URL形式的streamingAssets地址（自动兼容平台）
	    /// </summary>
	    public static string streamingAssetsURL {
	        get {
				#if UNITY_ANDROID && !UNITY_EDITOR
				return "jar:file://" + Application.dataPath + "!/assets/";
				#elif UNITY_IPHONE
				return Application.dataPath + "/Raw/";
				#elif UNITY_STANDALONE_WIN || UNITY_EDITOR
	            return "file://" + Application.dataPath + "/StreamingAssets/";
				#endif
	        }
	    }
	    
		/// <summary>
		/// 获取当前透视摄像机的视口区域
		/// </summary>
		public static Vector3[] getCameraCorners (float distance) {		
			Camera theCamera = Camera.main;
			Transform tx = theCamera.transform;
			Vector3[] corners = new Vector3[ 4 ];

			float halfFOV = ( theCamera.fieldOfView * 0.5f ) * Mathf.Deg2Rad;
			float aspect = theCamera.aspect;

			float height = distance * Mathf.Tan( halfFOV );
			float width = height * aspect;

			// UpperLeft
			corners[ 0 ] = tx.position - ( tx.right * width );
			corners[ 0 ] += tx.up * height;
			corners[ 0 ] += tx.forward * distance;

			// UpperRight
			corners[ 1 ] = tx.position + ( tx.right * width );
			corners[ 1 ] += tx.up * height;
			corners[ 1 ] += tx.forward * distance;

			// LowerLeft
			corners[ 2 ] = tx.position - ( tx.right * width );
			corners[ 2 ] -= tx.up * height;
			corners[ 2 ] += tx.forward * distance;

			// LowerRight
			corners[ 3 ] = tx.position + ( tx.right * width );
			corners[ 3 ] -= tx.up * height;
			corners[ 3 ] += tx.forward * distance;

			return corners;
		}

		/// <summary>
		/// 获取使用正交相机时，主摄像机在地图上的移动区域。
		/// 使用正交相机时，可以通过旋转摄像机X轴来显示地图，此时通过Y轴控制摄像机高度，X控制水平位置，Z轴控制上下位置。
		/// </summary>
		/// <returns>返回摄像机移动区域.</returns>
		/// <param name="RotationX">摄像机旋转角度(X轴)</param>
		/// <param name="CameraSize">相机大小</param>
		/// <param name="CamersY">摄像机高度(Y轴坐标)</param>
		/// <param name="MapW">地图宽度</param>
		/// <param name="MapH">地图高度</param>
		public static Rect OrthographicCameraEdge(float RotationX, float CameraSize, float CamersY, float MapW, float MapH) {
			float CXSize = CameraSize * 2 * ((float) Screen.width / (float) Screen.height);
			float CYSize = CameraSize * 2 / Mathf.Cos ( RotationX * Mathf.Deg2Rad );
			return new Rect (
				CXSize / 2.0f, 
				CYSize / 2.0f - CamersY, 
				MapW - CXSize, 
				MapH - CYSize);;
		}

		/// <summary>
		/// 显示一个提示信息 Toast
		/// </summary>
		public static void Hint(string msg) {
			#if UNITY_ANDROID || UNITY_ANDROID_API
			AndroidCommon.Hint(msg);
			#else
			#endif
		}
	}
}
