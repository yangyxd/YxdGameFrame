using UnityEngine;
using System.Collections;
using UnityEngine.Serialization;


/// <summary>
/// 游戏架构 - 核心层
/// </summary>
namespace GameFrame.Core {
	
	/// <summary>
	/// 引擎状态, 显示游戏基本状态（FPS、项点数、面数、引擎状态）
	/// </summary>
	[AddComponentMenu("GameFrame/MsgEngine 引擎状态显示器", 51)]
	public class MsgEngineState : Singleton<MsgEngineState> {
	    // 上一帧的刷新帧
	    private float LastInterval;
	    // 总帧数
	    private int Frames = 0;
	    // 平均帧数，每秒帧率
	    private float m_FPS;

	    public float FPS {
	        get { return m_FPS;  }
	    }

	    /// <summary>
	    /// 刷新频率
	    /// </summary>
	    public float UpdateInterval = 0.5f;
		/// <summary>
		/// 是否显示FPS
		/// </summary>
		public bool ShowFPS = true;
		/// <summary>
		/// 是否显示引擎状态
		/// </summary>
		public bool ShowEngine = true;
		/// <summary>
		/// 是否显示顶点数和面数
		/// </summary>
		public bool ShowVT = false;
		/// <summary>
		/// 是否显示主像机位置
		/// </summary>
		public bool ShowMainCamera = false;
		/// <summary>
		/// 是否显示触控状态
		/// </summary>
		public bool ShowTouch = false;

	    /// <summary>
	    /// 当前的顶点数
	    /// </summary>
	    public static int verts;
	    /// <summary>
	    /// 当前的面数
	    /// </summary>
	    public static int tris;

		protected override void Awake() {
	        base.Awake();
	        Application.targetFrameRate = 45;  // 设定基本帧率
	    }

	    void Start() {
	        // 从游戏开始到现在经过的时间
	        LastInterval = Time.realtimeSinceStartup;
	        Frames = 0;
	    }

		/// <summary>
	    /// 统计游戏当前的所有状态
	    /// </summary>
	    void GetObjectStats() {
	        verts = 0;
	        tris = 0;
	        GameObject[] ob = FindObjectsOfType(typeof(GameObject)) as GameObject[];
	        foreach (GameObject obj in ob) {
	            GetObjectStats(obj);
	        }
	    }

	    /// <summary>
	    /// 统计单个对象的状态
	    /// </summary>
	    void GetObjectStats(GameObject obj) {
	        // 过滤器
	        Component[] filters = obj.GetComponentsInChildren<MeshFilter>();

	        foreach (MeshFilter f in filters) {
				if (f == null || f.sharedMesh == null)
					continue;
	            // 将所有的顶点统计起来
	            // 统计共享的面数
	            tris += f.sharedMesh.triangles.Length / 3; // 一个三角形有三个顶点，除以3表示一个面。
	            // 统计共享的顶点数
	            verts += f.sharedMesh.vertexCount;
	        }
	    }

	    /// <summary>
	    /// 将统计结果显示出来
	    /// </summary>
	    void OnGUI() {
	        GUI.skin.label.normal.textColor = Color.white;
			if (ShowFPS)
	        	GUILayout.Label("FPS: " + m_FPS.ToString("f2"));
			if (ShowVT) {
				GUILayout.Label (verts.ToString ("顶点数: #,##0"));
				GUILayout.Label (tris.ToString ("面数: #,##0"));
			}
			if (ShowEngine) {
				MsgEngine engine = MsgEngine.Instance;
				GUILayout.Label (string.Format("消息引擎 MsgCount: {0}, MsgHandler: {1}", engine.MsgCount, engine.MsgHandlerCount));
			}
			if (ShowMainCamera && Camera.main != null)
	        	GUILayout.Label(string.Format("摄像机 x: {0:0.00}, y: {1:0.00}, z: {2:0.00}", 
					Camera.main.transform.localPosition.x, 
					Camera.main.transform.localPosition.y, 
					Camera.main.transform.localPosition.z));
			if (ShowTouch && Input.touchCount > 0) {
				for (int i=0, max = Input.touchCount; i < max; i++) {
					Touch tc = Input.GetTouch (i);
					GUILayout.Label(string.Format("Touch {2} x: {0:0.00}, y: {1:0.00}", tc.deltaPosition.x, tc.deltaPosition.y, i));
				}
			}
	    }

	    void Update() {
	        // 平均帧率法，在每一帧进行计数
	        ++Frames;
	        if (Time.realtimeSinceStartup > LastInterval + UpdateInterval) {
	            m_FPS = Frames / (Time.realtimeSinceStartup - LastInterval);
	            // 重置状态
	            Frames = 0;
	            LastInterval = Time.realtimeSinceStartup;
				if (ShowVT)
	            	GetObjectStats();
	        }
	    }

	}

}
