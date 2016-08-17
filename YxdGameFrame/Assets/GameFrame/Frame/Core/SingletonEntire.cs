using UnityEngine;
using System.Collections;

/// <summary>
/// 游戏架构
/// </summary>
namespace GameFrame {
	
	/// <summary>
	/// 所有场景单例模板，使其不会在场景切换时消失
	/// </summary>
	/// <typeparam name="T">必须是Component</typeparam>
	public class SingletonEntire<T> : Singleton<T> where T : Component {
		
		protected override void Awake() {
			this.LOG (this.GetComponent<T> ().GetType ().Name + " Awake.");
	        // 如果单例为空，将当前对象赋值给它
			if (_Instance == null) {
				_Instance = this as T;
				// 指明切换场景时不会被删除
				DontDestroyOnLoad(this.gameObject);
			} else {
				Destroy(gameObject.GetComponent<T>()); // 如果已经有实例了，则直接删除自己 (单例不允许重复存在)
			}
	    }

	}
}
