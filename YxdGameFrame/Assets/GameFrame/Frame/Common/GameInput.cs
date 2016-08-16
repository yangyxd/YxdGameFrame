using UnityEngine;
using System.Collections;

/// <summary>
/// 游戏架构
/// </summary>
namespace GameFrame {

	/// <summary>
	/// 游戏输入接口
	/// </summary>
	public interface IGameInput {
		/// <summary>
		/// 是否按下鼠标或手指
		/// </summary>	
		bool isDown {get;}
		/// <summary>
		/// 是否松开鼠标或手指
		/// </summary>	
		bool isUp {get;}
		/// <summary>
		/// 是否长按
		/// </summary>	
		bool isLongDown {get;}
		/// <summary>
		/// 是否触碰
		/// </summary>
		bool hasTouch {get;}
		/// <summary>
		/// 是否正在移动
		/// </summary>
		bool isMove {get;}
		/// <summary>
		/// 触碰数量（有几个手指在屏幕上）
		/// </summary>
		int touchCount {get;}
		/// <summary>
		/// 鼠标或手势点位置
		/// </summary>
		Vector3 mousePosition {get;}
	}

	/// <summary>
	/// Windows 平台游戏输入
	/// </summary>
	public class WinGameInput : SingletonEntire<WinGameInput>, IGameInput {	

		public bool isDown {
			get { return Input.GetMouseButtonDown (0); }
		}

		public bool isUp {
			get { return Input.GetMouseButtonUp (0); }
		}

		public bool isMove {
			get { return isLongDown; }
		}

		public bool isLongDown {
			get { return Input.GetMouseButton (0); }
		}

		public Vector3 mousePosition {
			get { return Input.mousePosition; }
		}

		// 在 Windows 是永远被触摸
		public bool hasTouch {
			get { return true; }
		}

		// 在 Windows 上只有一个点
		public int touchCount { get { return 1; } }
	}

	/// <summary>
	/// 单指触控类游戏输入
	/// </summary>
	public class SingleTouchGameInput : SingletonEntire<SingleTouchGameInput>, IGameInput {
		public bool isDown {
			get { return Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began; }
		}

		public bool isUp {
			get { return Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Ended; }
		}

		public bool isMove {
			get { return Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved; }
		}

		public bool isLongDown {
			get { return Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Stationary; }
		}

		public Vector3 mousePosition {
			get { 
				if (Input.touchCount == 1)
					return Input.GetTouch (0).position;
				else
					return Input.mousePosition;
			}
		}

		public bool hasTouch {
			get { return Input.touchCount > 0; }
		}

		public int touchCount { get { return Input.touchCount; } }
	}

	/// <summary>
	/// 游戏输入
	/// </summary>
	#if UNITY_EDITOR || UNITY_STANDALONE
	public class GameInput : WinGameInput {}
	#else
	public class GameInput : SingleTouchGameInput {}
	#endif

}
