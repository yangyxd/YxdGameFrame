using UnityEngine;
using System.Collections;

/// <summary>
/// 游戏架构
/// </summary>
namespace GameFrame {
	using Core;

	/// <summary>
	/// Mono基类
	/// </summary>
	public abstract class MonoBase : MonoBehaviour, IDebugMessage, IMsgProcessHandler {
		protected MsgHandlerBase MsgHandler;

		/// <summary>
		/// 消息处理过程
		/// </summary>
		public abstract void ProcessMsg (IMsgPack msg);

		public virtual void DoDestroy () {
		}

		void OnDestroy() {			
			UnRegAllMsg ();
			DoDestroy ();
		}

		/// <summary>
		/// 是否暂停游戏
		/// </summary>
		public bool IsPause {
			get { return MsgEngine.Instance.IsPause; }
			set { MsgEngine.Instance.PauseGame (value); }
		}

		/// <summary>
		/// 中止游戏
		/// </summary>
		public void EndGame() {
			MsgEngine.Instance.EndGame ();
		}

		/// <summary>
		/// 暂停游戏
		/// </summary>
		public void PauseGame(bool v) {
			MsgEngine.Instance.PauseGame (v);
		}

		/// <summary>
		/// Hint the specified msg.
		/// </summary>
		/// <param name="msg">Message.</param>
		public void Hint(string msg) {
			MsgEngine.Instance.Hint (msg);
		}

		/// <summary>
		/// Writes the log.
		/// </summary>
		/// <param name="msg">Message.</param>
		public void WriteLog(string msg) {
			MsgEngine.Instance.WriteLog (msg);
		}

		/// <summary>
		/// 发送消息
		/// </summary>
		public void SendMsg (IMsgPack msg) {
			if (MsgHandler == null) 
				MsgHandler = new MsgHandlerBase (this, this);
			MsgHandler.SendMsg (msg);
		}

		/// <summary>
		/// 同步发送消息
		/// </summary>
		public void SendMsgSync (IMsgPack msg) {
			if (MsgHandler == null)
				MsgHandler = new MsgHandlerBase (this, this);
			MsgHandler.SendMsgSync (msg);
		}

		/// <summary>
		/// 注册消息
		/// </summary>
		public void RegMsg(int msgId) {
			if (MsgHandler == null)
				MsgHandler = new MsgHandlerBase (this, this);
			MsgHandler.AddMsgId (msgId);
		}

		/// <summary>
		/// 注册消息
		/// </summary>
		public void RegMsg(int[] msgIds) {
			if (MsgHandler == null)
				MsgHandler = new MsgHandlerBase (this, this);
			MsgHandler.AddMsgId (msgIds);
		}

		/// <summary>
		/// 取消注册的消息ID
		/// </summary>
		public void UnRegMsg(int msgId) {
			if (MsgHandler == null)
				return;
			MsgHandler.RemoveMsgId (msgId);
		}

		/// <summary>
		/// 取消注册的所有消息
		/// </summary>
		public void UnRegAllMsg() {
			if (MsgHandler == null)
				return;
			MsgHandler.ClearMsgId ();
		}

	}
}