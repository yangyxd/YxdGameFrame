using UnityEngine;
using System.Collections;

/// <summary>
/// 游戏架构
/// </summary>
namespace GameFrame {

	/// <summary>
	/// 消息包接口
	/// </summary>
	public interface IMsgPack {
		/// <summary>
		/// 消息ID
		/// </summary>
		int MsgID { get; set; }
		/// <summary>
		/// 消息发送者
		/// </summary>
		Object Sender { get; set; }
	}

	/// <summary>
	/// 基本的消息类
	/// </summary>
	public class MsgBase : IMsgPack {
		private int m_MsgID;
		private Object m_Sender;

		public MsgBase (int msgID) {
			m_MsgID = msgID;
		}

		/// <summary>
		/// 消息ID
		/// </summary>
		public int MsgID {
			get { return m_MsgID; }
			set { m_MsgID = value; }
		}

		/// <summary>
		/// 消息发送者
		/// </summary>
		public Object Sender {
			get { return m_Sender; }
			set { m_Sender = value; }
		}
	}

	/// <summary>
	/// 文本消息基础类
	/// </summary>
	public class MsgTextBase : MsgBase {
		public string Text;
		public MsgTextBase (int msgID, string text) : base(msgID) {
			Text = text;
		}
	}

}
