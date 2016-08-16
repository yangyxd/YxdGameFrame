using UnityEngine;
using System.Collections;

/// <summary>
/// 游戏架构 - 核心层
/// </summary>
namespace GameFrame.Core {

	/// <summary>
	/// 事件系统
	/// </summary>
	public partial class EventSystem {
		/// <summary>
		/// 无参数代理
		/// </summary>
		public delegate void EventDelegate();
		/// <summary>
		/// 一个参数代理
		/// </summary>
		public delegate void EventDelegate<T>(T t);
	}

}
