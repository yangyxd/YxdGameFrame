using UnityEngine;
using System.Collections;

/// <summary>
/// 游戏架构
/// </summary>
namespace GameFrame {

	/// <summary>
	/// 场景单例模板，只在当前场景有效，切换场景时会被消毁
	/// </summary>
	/// <typeparam name="T">必须是Component</typeparam>
	public class Singleton<T> : MonoBase, IDebugMessage where T : Component {
		private static bool m_IsDestroying = false;  
		protected static T _Instance;

		public override void ProcessMsg (IMsgPack msg) {
			throw new System.NotImplementedException ();
		}

		public static T Instance {
			get {
				if (_Instance == null) {
					// 如果不存在实例, 则查找所有这个类型的对象
					_Instance = FindObjectOfType(typeof(T)) as T;
					if (_Instance == null) {
						// 如果没有找到， 则新建一个
						GameObject obj = new GameObject(typeof(T).Name);
						// 对象不可见，不会被保存
						obj.hideFlags = HideFlags.HideAndDontSave;
						// 强制转换为 T 
						_Instance = obj.AddComponent(typeof(T)) as T;
					}
				}
				return _Instance;
			}
		}

		public static bool IsDestroying  {  
			get { return m_IsDestroying; }  
		}

		protected virtual void Awake() {
			this.LOG (this.GetComponent<T> ().GetType ().Name + " Awake.");
			if (_Instance == null)
				_Instance = this as T;
			else {
				GameObject.Destroy (_Instance);
				_Instance = this as T;
			}
		}

		public override void DoDestroy () {
			_Instance = null;
		}

		void OnApplicationQuit() {
			if (_Instance != null) {
				this.LOG (_Instance.GetType ().Name + " Destroy.");
				GameObject.Destroy (_Instance);
				_Instance = null;
				m_IsDestroying = true;
			}
		}


	}
}
