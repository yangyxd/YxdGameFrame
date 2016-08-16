using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;

/// <summary>
/// 游戏架构 - 编辑器工具
/// </summary>
namespace GameFrame.Editor {
	
	/// <summary>
	/// 创建工程常用目录
	/// </summary>
	public class CreateProjectDirWnd : EditorWindow {

		[MenuItem("Plugin/创建项目常用目录...")]
		static void Create() {
			//创建窗口
			Rect wr = new Rect (0, 0, 650, 500);
			CreateProjectDirWnd wnd = (CreateProjectDirWnd)EditorWindow.GetWindowWithRect (typeof (CreateProjectDirWnd), 
				wr, true, "创建项目常用目录");	
			wnd.Show();
		}

		bool isEditer, isEditor_NGUI, isGizmos, isStreamingAssets, isModels = true, isOthers, isPlugins = true;
		bool isPrefabs = true, isResources = true, isScenes = true, isScripts = true, isSounds = true;
		bool isTextures = true, isZ_Test, isMaterials = true, isFonts = true, isDataTable, isAnimations = true;
		bool isEffects, isShaders, isStandard_Assets, isControllers;

		void initArray(out bool[] vs, out string[] vn) {
			vs = new bool[] {
				isEditer, isEditor_NGUI, isGizmos, isStreamingAssets, isModels, isOthers, isPlugins,
				isPrefabs, isResources, isScenes, isScripts, isSounds,
				isTextures, isZ_Test, isMaterials, isFonts, isDataTable, isAnimations,
				isEffects, isShaders, isStandard_Assets, isControllers
			};

			vn = new string[] {
				"Editer", "Editor_NGUI", "Gizmos", "StreamingAssets", "Models", "Others", "Plugins",
				"Prefabs", "Resources", "Scenes", "Scripts", "Sounds",
				"Textures", "Z_Test", "Materials", "Fonts", "DataTable", "Animations",
				"Effects", "Shaders", "Standard Assets", "Animations/Controllers"
			};
		}

		void OnGUI() {
			GUILayout.Label ("常用目录： ");

			isControllers = isAnimations;
			isAnimations = GUILayout.Toggle (isAnimations, "Animations (动画文件)");
			isPrefabs = GUILayout.Toggle (isPrefabs, "Prefabs (预制品文件)");
			isResources = GUILayout.Toggle (isResources, "Resources (动态加载的资源文件)");
			isScenes = GUILayout.Toggle (isScenes, "Scenes (场景文件)");
			isScripts = GUILayout.Toggle (isScripts, "Scripts (脚本代码文件)");
			isSounds = GUILayout.Toggle (isSounds, "Sounds (音效文件)");
			isTextures = GUILayout.Toggle (isTextures, "Textures (所有的贴图)");
			isMaterials = GUILayout.Toggle (isMaterials, "Materials (材质文件)");
			isFonts = GUILayout.Toggle (isFonts, "Fonts (字体文件)");


			isModels = GUILayout.Toggle (isModels, "Models (模型文件，其中会包括自动生成的材质球文件)");
			isPlugins = GUILayout.Toggle (isPlugins, "Plugins (外部插件文件夹，如一些外部DLL插件)");
			isEffects = GUILayout.Toggle (isEffects, "Effects (特效文件)");
			isShaders = GUILayout.Toggle (isShaders, "Shaders (着色器文件)");
			isStandard_Assets = GUILayout.Toggle (isStandard_Assets, "Standard Assets (标准的需要打包的文件)");

			GUILayout.Label ("其它目录： ");

			isEditer = GUILayout.Toggle (isEditer, "Editer (自写的灵活方便插件)");
			isEditor_NGUI = GUILayout.Toggle (isEditor_NGUI, "Editor_NGUI (较大型三方的插件)");
			isGizmos = GUILayout.Toggle (isGizmos, "Gizmos (使用GIZMOS所需要的标志等临时文件)");
			isStreamingAssets = GUILayout.Toggle (isStreamingAssets, "StreamingAssets (流式数据，包括JsonData、Assetbundle、各种只读文件预储存)");
			isDataTable = GUILayout.Toggle (isDataTable, "DataTable (策划数据表文件)");
			isOthers = GUILayout.Toggle (isOthers, "Others (其他类型的文件。例如添加的不常用的Shader、物理材质、动画文件。)");

			isZ_Test = GUILayout.Toggle (isZ_Test, "Z_Test (临时测试文件)");

			if (GUILayout.Button("全选", GUILayout.Width(200))) {
				doSelectAll (true);
			}
			if (GUILayout.Button("取消选择", GUILayout.Width(200))) {
				doSelectAll (false);
			}
			if (GUILayout.Button("立即创建", GUILayout.Width(200))) {
				doCreate ();
				this.Close ();
			}
		}

		void doSelectAll(bool v) {
			isEditer = v;
			isEditor_NGUI = v;
			isGizmos = v;
			isStreamingAssets = v;
			isModels = v;
			isOthers = v;
			isPlugins = v;
			isPrefabs = v;
			isResources = v;
			isScenes = v;
			isScripts = v;
			isSounds = v;
			isTextures = v;
			isZ_Test = v;
			isMaterials = v;
			isFonts = v;
			isDataTable = v;
			isAnimations = v;
			isEffects = v;
			isShaders = v;
			isStandard_Assets = v;
			isControllers = v;
		}

		void doCreate() {	
			bool[] vs; 
			string[] vn;
			initArray (out vs, out vn);
			int m = 0;
			for (int i = 0, max = vs.Length; i < max; i++) {
				m += CreateFolder (vs [i], vn [i]);
			}
			if (m > 0)
				AssetDatabase.Refresh ();
		}

		int CreateFolder(bool v, string name) {
			if (v) {
				string path = Path.Combine ("Assets", name);
				return CeateFolder (path);
			} else
				return 0;
		}

		int CeateFolder(string path) {
			if (!Directory.Exists(path)) {
				Directory.CreateDirectory(path);
				return 1;
			} return 0;
		}
	}
}
