using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 游戏架构 - 核心引擎
/// </summary>
namespace GameFrame.Core {
	
	/// <summary>
	/// 引擎代理 (游戏对象可通过添加此脚本来操作消息引擎)
	/// </summary>
	[AddComponentMenu("GameFrame/MsgEngine 消息引擎", 50)]
	public class MsgEngineAgent : GameFrame.MonoBase, IDebugMessage {
		public override void ProcessMsg (IMsgPack msg) {}

		/// <summary>
		/// 注册消息处理服务
		/// </summary>
		/// <param name="msgHandler">消息处理器对象</param>
		public void Register(IMsgHandler msgHandler, int msgID) {
			MsgEngine.Instance.Register (msgHandler, msgID);
		}

		/// <summary>
		/// 取消指定消息ID注册的消息处理器
		/// </summary>
		public void UnRegister(int msgID) {
			MsgEngine.Instance.UnRegister (msgID);
		}

		/// <summary>
		/// 解除所有已经注册的消息处理器
		/// </summary>
		public void ClearRegister() {
			MsgEngine.Instance.ClearRegister ();
		}

	}

}