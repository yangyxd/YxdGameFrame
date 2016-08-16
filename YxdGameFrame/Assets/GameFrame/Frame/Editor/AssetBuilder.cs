using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

/// <summary>
/// 游戏架构 - 编辑器工具
/// </summary>
namespace GameFrame.Editor {
	
	/// <summary>
	/// 资源编译打包器
	/// </summary>
	public class AssetBuilder: UnityEditor.Editor {
	    public static string sourcePath = Application.dataPath;
	    const string AssetBundlesOutputPath = "Assets/StreamingAssets/";
	    const string assetTail = ".unity3d";

		[MenuItem("Plugin/资源编译助手/Build CSV Table (*.csv)")]
	    public static void BuildCSV() {
	        BuildCSV(EditorUserBuildSettings.activeBuildTarget);
	    }
		[MenuItem("Plugin/资源编译助手/Build CSV Table To Windowns")]
		public static void BuildCSV_Win() {
			BuildCSV(BuildTarget.StandaloneWindows);
		}
		[MenuItem("Plugin/资源编译助手/Build CSV Table To Android")]
		public static void BuildCSV_Android() {
			BuildCSV(BuildTarget.Android);
		}
		[MenuItem("Plugin/资源编译助手/Build CSV Table To iOS")]
		static void BuildCSV_iOS() {
			BuildCSV (BuildTarget.iOS);
		}
		[MenuItem("Plugin/资源编译助手/Build Textfile (*.txt)")]
		public static void BuildTXT() {
			BuildAssetBundle(".txt", EditorUserBuildSettings.activeBuildTarget);
		}
		[MenuItem("Plugin/资源编译助手/Build Textfile To Windowns")]
		public static void BuildTXT_Win() {
			BuildAssetBundle(".txt", BuildTarget.StandaloneWindows);
		}
		[MenuItem("Plugin/资源编译助手/Build Textfile To Android")]
		public static void BuildTXT_Android() {
			BuildAssetBundle(".txt", BuildTarget.Android);
		}
		[MenuItem("Plugin/资源编译助手/Build Textfile To iOS")]
		public static void BuildTXT_IOS() {
			BuildAssetBundle(".txt", BuildTarget.iOS);
		}

	    public static void BuildCSV(BuildTarget bt) {
	        BuildAssetBundle(".csv", bt);
	    }

	    /// <summary>
	    /// 编译当前选择文件夹AssetBundle
	    /// </summary>
	    /// <param name="ftype">文件扩展名 (可以带"."，也可以不带, 建议自带)</param>
	    public static void BuildAssetBundle(string ftype) {
	        BuildAssetBundle(ftype, null, EditorUserBuildSettings.activeBuildTarget);
	    }
	    
	    /// <summary>
	    /// 编译当前选择文件夹AssetBundle
	    /// </summary>
	    /// <param name="ftype">文件扩展名 (可以带"."，也可以不带, 建议自带)</param>
	    public static void BuildAssetBundle(string ftype, BuildTarget bt) {
	        BuildAssetBundle(ftype, null, bt);
	    }

	    /// <summary>
	    /// 编译AssetBundle
	    /// </summary>
	    /// <param name="ftype">文件扩展名 (可以带"."，也可以不带, 建议自带)</param>
	    /// <param name="path">要编译的文件目录，为空时处理当前选中的目录</param>
	    public static void BuildAssetBundle(string ftype, string path) {
	        BuildAssetBundle(ftype, path, EditorUserBuildSettings.activeBuildTarget);
	    }

	    /// <summary>
	    /// 编译AssetBundle
	    /// </summary>
	    /// <param name="ftype">文件扩展名 (可以带"."，也可以不带, 建议自带)</param>
	    /// <param name="path">要编译的文件目录，为空时处理当前选中的目录</param>
	    public static void BuildAssetBundle(string ftype, string path, BuildTarget bt) {
	        //为扩展名加上"."
	        //string typeName = "";
	        if (ftype != null && ftype.Length > 0) {
	            if (!ftype.StartsWith(".")) {
	                //typeName = "/" + ftype;
	                ftype = "." + ftype;
	            }
				// else
	            //typeName = "/" + ftype.Substring(1);
	        }
	        ClearAssetBundlesName();

	        string outputPath = Path.Combine(AssetBundlesOutputPath, GetPlatformFolder(EditorUserBuildSettings.activeBuildTarget)); // + typeName);
	        if (!Directory.Exists(outputPath)) {
	            Directory.CreateDirectory(outputPath);
	        }

	        if (path == null || path.Length == 0) {
	            Object[] selections = Selection.GetFiltered(typeof(object), SelectionMode.DeepAssets);
	            for (int i = 0, max = selections.Length; i < max; i++) {
	                Object obj = selections[i];
	                string fname = AssetDatabase.GetAssetPath(obj);
	                if (!CheckFileType(ftype, fname))
	                    continue;
	                fileAsset(fname);
	            }
	        } else {
	            Pack(ftype, sourcePath);
	        }

	        //根据BuildSetting里面所激活的平台进行打包 
	        BuildPipeline.BuildAssetBundles (outputPath, 0, bt);
	        AssetDatabase.Refresh ();
	        EditorUtility.DisplayDialog("OK", "打包完成", "OK");
	    }   

	    /// <summary> 
	    /// 清除之前设置过的AssetBundleName，避免产生不必要的资源也打包 
	    /// 之前说过，只要设置了AssetBundleName的，都会进行打包，不论在什么目录下
	    /// </summary> 
	    static void ClearAssetBundlesName() {
	        int length = AssetDatabase.GetAllAssetBundleNames ().Length;
	        Debug.Log (length); string[] oldAssetBundleNames = new string[length];
	        for (int i = 0; i < length; i++) {
	            oldAssetBundleNames[i] = AssetDatabase.GetAllAssetBundleNames()[i];
	        }
	        for (int j = 0; j < oldAssetBundleNames.Length; j++) {
	            AssetDatabase.RemoveAssetBundleName(oldAssetBundleNames[j],true);
	        }
	        length = AssetDatabase.GetAllAssetBundleNames ().Length; Debug.Log (length);
	    }

	    // ftype 为空时， 处理所有文件
	    static void Pack(string ftype, string source) {
	        DirectoryInfo folder = new DirectoryInfo (source);
	        FileSystemInfo[] files = folder.GetFileSystemInfos ();
	        int length = files.Length;
	        for (int i = 0; i < length; i++) {
	            if (files[i] is DirectoryInfo) {
	                Pack(ftype, files[i].FullName);
	            } else {
	                if (CheckFileType(ftype, files[i].Name)) {
	                    file(files[i].FullName);
	                }
	            }
	        }
	    }

	    static bool CheckFileType(string ftype, string fname) {
	        if (ftype == null || ftype.Length == 0) {
	            return !fname.EndsWith(".meta");
	        } else {
	            return fname.EndsWith(ftype);
	        }
	    }

	    static void file(string source) {
	        string _source = Replace (source);
	        fileAsset("Assets" + _source.Substring (Application.dataPath.Length));
	    }

	    static void fileAsset(string assetPath) {
	        string _assetPath2 = assetPath;
	        //Debug.Log (_assetPath);  
	        //在代码中给资源设置       AssetBundleName 
	        AssetImporter assetImporter = AssetImporter.GetAtPath(assetPath);
	        string assetName = _assetPath2.Substring(_assetPath2.IndexOf("/") + 1);
	        assetName = assetName.Replace(Path.GetExtension(assetName), assetTail);
	        //Debug.Log (assetName);
	        assetImporter.assetBundleName = assetName;
	    }

	    static string Replace(string s) {
	        return s.Replace("\\","/");
	    }

	    /// <summary>
	    /// 获取平台文件夹
	    /// </summary>
	    /// <param name="target"></param>
	    /// <returns></returns>
	    public static string GetPlatformFolder(BuildTarget target) {
	        switch (target) {
	            case BuildTarget.Android:
	                return "Android";
	            case BuildTarget.iOS:
	                return "IOS";
	            case BuildTarget.WebPlayer:
	                return "WebPlayer";
	            case BuildTarget.StandaloneWindows:
	            case BuildTarget.StandaloneWindows64:
	                return "Windows";
	            case BuildTarget.StandaloneOSXIntel:
	            case BuildTarget.StandaloneOSXIntel64:
	            case BuildTarget.StandaloneOSXUniversal:
	                return "OSX";
	            default: return null;
	        }
	    }
	}
}
