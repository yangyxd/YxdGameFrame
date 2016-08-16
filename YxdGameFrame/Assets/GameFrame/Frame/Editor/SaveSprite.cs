using UnityEngine;
using UnityEditor;

/// <summary>
/// 游戏架构 - 编辑器工具
/// </summary>
namespace GameFrame.Editor {
	
	public class SaveSprite : UnityEditor.EditorWindow {

	    [MenuItem("Plugin/导出精灵...")]
	    public static void SaveSpriteExec() {
	        //创建窗口
	        Rect wr = new Rect(0, 0, 400, 600);
	        SaveSprite wnd = (SaveSprite) EditorWindow.GetWindowWithRect(typeof(SaveSprite),
	            wr, true, "导出精灵");
	        wnd.Show();
	    }

	    private string[] fname = new string[6];
	    private string[] fnewname = new string[6];
	    private Sprite[] sprite = new Sprite[6];

	    void OnGUI() {
	        GUILayout.Label("选择要导出的精灵（至少选一个）： ");
	        for (int i = 0, max = fname.Length; i < max; i++) {
	            fnewname[i] = EditorGUILayout.TextField("文件名称:", fnewname[i]);
	            Sprite sp = EditorGUILayout.ObjectField("精灵对象:", sprite[i], typeof(Sprite), true) as Sprite;
	            sprite[i] = sp;
	            if (sp && sp.name != fname[i]) {
	                fname[i] = sp.name;
	                fnewname[i] = sp.name;
	            }
	        }
	        if (GUILayout.Button("清空", GUILayout.Width(200))) {
	            doClear();
	        }
	        if (GUILayout.Button("导出", GUILayout.Width(200))) {
	            doExport();
	            doClear();
	        }
	    }

	    void doClear() {
	        for (int i = 0, max = fname.Length; i < max; i++) {
	            fname[i] = "";
	            fnewname[i] = "";
	            sprite[i] = null;
	        }
	    }

	    void doExport() {
	        int m = 0;

	        for (int i = 0, max = fname.Length; i < max; i++) {
	            Sprite sp = sprite[i];
	            if (string.IsNullOrEmpty(fnewname[i]) || sp == null)
	                continue;

	            if (sp.texture == null)
	                continue;

	            // 得到输出文件位置
	            string path = Common.getFilePath(AssetDatabase.GetAssetPath(sp.texture)) + "/" + fnewname[i] + ".png";

	            // 创建单独的纹理
	            try {
	                Texture2D tex = new Texture2D((int) sp.rect.width, (int) sp.rect.height, sp.texture.format, false);
	                Color[] pixels = sp.texture.GetPixels((int) sp.rect.xMin, (int) sp.rect.yMin, (int) sp.rect.width, (int) sp.rect.height);
	                if (sp.texture.format == TextureFormat.ARGB32) {
	                    tex.SetPixels(pixels);
	                    tex.Apply();

	                    // 写入成PNG文件
	                    System.IO.File.WriteAllBytes(path, tex.EncodeToPNG());

	                    m++;
	                } else {
	                    EditorUtility.DisplayDialog("Error", "请将纹理属性中的 Format 设为 ARGB 32bit", "OK");
	                }
	            } catch (UnityException e) {
	                EditorUtility.DisplayDialog("Error", 
	                    string.Format("导出 {2} 时发生错误: \n\n{0}\n\n请检查纹理 {1} 的Texture Type属性是否为 Advanced， "+
	                        "并且选中 Read/Write Enabled ，点击 Apply 按钮。", 
	                    e.Message, sp.texture.name, fnewname[i]), "OK");
	            }
	        }

	        if (m > 0) {
	            AssetDatabase.Refresh();
	            EditorUtility.DisplayDialog("OK", "已经导出" + m + "个文件", "OK");
	        }
	    }

	}
}
