using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameFrame.Core;

/// <summary>
/// 游戏架构
/// </summary>
namespace GameFrame {

	/// <summary>
	/// 消息处理器接口
	/// </summary>
	public interface IMsgProcessHandler {
		/// <summary>
		/// 消息处理过程
		/// </summary>
		void ProcessMsg (IMsgPack msg);
	}

	/// <summary>
	/// 消息处理器接口
	/// </summary>
	public interface IMsgHandler : IMsgProcessHandler {
		/// <summary>
		/// 已经注册过的消息ID
		/// </summary>
		int[] MsgIds { get; }
		/// <summary>
		/// 发送消息
		/// </summary>
		void SendMsg (IMsgPack msg);
	}

	/// <summary>
	/// 消息处理器基类
	/// </summary>
	public class MsgHandlerBase : IMsgHandler, System.IDisposable {
		private List<int> m_MsgIds;
		private Object m_Owner;
		private IMsgProcessHandler onProcessMsg;

		public MsgHandlerBase(Object Owner, IMsgProcessHandler msgProcessHandler) {
			m_Owner = Owner;
			onProcessMsg = msgProcessHandler;
		}

		//供GC调用的析构函数
		~MsgHandlerBase() {
			Dispose(false); //释放非托管资源
		}

		public int[] MsgIds {
			get {
				if (m_MsgIds == null)
					return null;
				return m_MsgIds.ToArray ();
			}
		}

		/// <summary>
		/// 所有者
		/// </summary>
		public Object Owner {
			get { return m_Owner; }
		}

		/// <summary>
		/// 添加消息ID
		/// </summary>
		public void AddMsgId(int msgID) {
			if (m_MsgIds == null)
				m_MsgIds = new List<int> ();
			if (m_MsgIds.Contains (msgID))
				return;
			m_MsgIds.Add (msgID);
			MsgEngine.Instance.Register (this, msgID);
		}

		/// <summary>
		/// 添加消息ID （默认msgIds不包含重复的消息ID）
		/// </summary>
		public void AddMsgId(int[] msgIds) {
			if (msgIds == null || msgIds.Length == 0)
				return;
			if (m_MsgIds == null) {
				m_MsgIds = new List<int> ();
				foreach (int id in msgIds) {
					m_MsgIds.Add (id);
					MsgEngine.Instance.Register (this, id);
				}
			} else {
				foreach (int id in msgIds) {
					if (!m_MsgIds.Contains (id)) {
						m_MsgIds.Add (id);
						MsgEngine.Instance.Register (this, id);
					}
				}
			}
		}

		/// <summary>
		/// 移除一个消息ID
		/// </summary>
		public void RemoveMsgId(int msgId) {
			if (m_MsgIds == null)
				return;
			if (m_MsgIds.Remove (msgId))
				MsgEngine.Instance.UnRegister (msgId);
		}

		/// <summary>
		/// 清空消息ID
		/// </summary>
		public void ClearMsgId() {
			if (m_MsgIds == null)
				return;
			MsgEngine.Instance.UnRegister (this);
			m_MsgIds.Clear ();
		}

		/// <summary>
		/// 消息处理过程
		/// </summary>
		public void ProcessMsg (IMsgPack msg) {
			if (onProcessMsg != null)
				onProcessMsg.ProcessMsg (msg);
		}

		/// <summary>
		/// 发送消息
		/// </summary>
		public void SendMsg (IMsgPack msg) {
			if (msg == null)
				return;
			if (msg.Sender == null)
				msg.Sender = Owner;
			MsgEngine.Instance.SendMsg (msg);
		}

		/// <summary>
		/// 同步发送消息
		/// </summary>
		public void SendMsgSync (IMsgPack msg) {
			if (msg == null)
				return;
			if (msg.Sender == null)
				msg.Sender = Owner;
			MsgEngine.Instance.SendMsgSync (msg);
		}

		public void Dispose() {
			//调用带参数的Dispose方法，释放托管和非托管资源
			Dispose(true);
			//手动调用了Dispose释放资源，那么析构函数就是不必要的了，这里阻止GC调用析构函数
			System.GC.SuppressFinalize(this);
		}

		//protected的Dispose方法，保证不会被外部调用。
		//传入bool值disposing以确定是否释放托管资源
		protected void Dispose(bool disposing) {
			m_Owner = null;
			onProcessMsg = null;
			if (m_MsgIds != null && !MsgEngine.IsDestroying) {
				MsgEngine.Instance.UnRegister (this);
				m_MsgIds.Clear ();
			}
			if (disposing) {
				///TODO: 在这里加入清理"托管资源"的代码，应该是xxx.Dispose();
			}
			///TODO:在这里加入清理"非托管资源"的代码
		}
	}

}
