using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 游戏架构 - 核心引擎
/// </summary>
namespace GameFrame.Core {

	/// <summary>
	/// 日志状态
	/// </summary>
//	public static class LogState {
//		public static bool RemoteLog = false;
//		public static bool WriteLog = false;
//		public static UDPClient UDP = null;
//
//		/// <summary>
//		/// 设置远程日志输出地址
//		/// </summary>
//		public static void SetRemoteLog(bool v, string host, int port) {
//			if (v) {
//				if (LogState.UDP == null) {
//					LogState.RemoteLog = v;
//					LogState.UDP = new UDPClient (host, port);
//				} else
//					LogState.UDP.SetRemoteAddr (host, port);
//			} else {
//				if (LogState.UDP != null)
//					LogState.UDP.Close ();
//				LogState.UDP = null;
//			}
//		}
//
//		/// <summary>
//		/// 是否开启日志输出
//		/// </summary>
//		/// <param name="v">If set to <c>true</c> v.</param>
//		public static void SetWriteLog(bool v) {
//			LogState.WriteLog = v;
//		}
//
//		public static void SendLog(string message) {
//			if (LogState.UDP != null)
//				LogState.UDP.Send (message);
//		}
//	}

	/// <summary>
	/// 消息引擎核心
	/// </summary>
	[DisallowMultipleComponent]
	public class MsgEngine : SingletonMonoEntire<MsgEngine>, IDebugMessage, IMsgProcessHandler {
		private Hashtable MsgHandlerMap = new Hashtable();
		private Queue<IMsgPack> MsgQueue = new Queue<IMsgPack> ();
		private bool m_IsPause = false;

		/// <summary>
		/// 是否暂停游戏
		/// </summary>
		public bool IsPause {
			get { return m_IsPause; }
			set { PauseGame (value); }
		}

		/// <summary>
		/// 在 MessageQueue 中，每一帧最多处理多少条消息. 此值越大, FPS掉的越多, 消息分发速度越快
		/// </summary>
		public int SpeedRatio = 250;

//		private static bool isInitRemote = false;
//		/// <summary>
//		/// 是否输出日志
//		/// </summary>
//		public bool WriteLog = false;
//		/// <summary>
//		/// 是否远程日志输出（运行时设置无效）
//		/// </summary>
//		public bool RemoteLog = false;
//		/// <summary>
//		/// 远程日志输出地址
//		/// </summary>
//		public string RemoteAddr = "";
//		/// <summary>
//		/// 远程日志端口
//		/// </summary>
//		public int RemotePort = 6699;

		/// <summary>
		/// 消息处理器
		/// </summary>
		protected MsgHandlerBase MsgHandler;

		/// <summary>
		/// 消息处理过程
		/// </summary>
		public virtual void ProcessMsg (IMsgPack msg) {
			if (msg.MsgID == MsgConst.GameDestory) {
				this.LOG ("MsgEngine: Msg GameDestory.");
				//If we are running in the editor
				#if UNITY_EDITOR
				//Stop playing the scene
				UnityEditor.EditorApplication.isPlaying = false;
				#else
				Application.Quit();
				#endif
			} else if (msg.MsgID == MsgConst.GameEnd) {
				this.LOG ("MsgEngine: Msg GameEnd.");
				SendMsg (new MsgBase (MsgConst.GameDestory));
			}
		}

		/// <summary>
		/// 处理器链表
		/// </summary>
		protected class HandlerLink {
			public IMsgHandler Handler;
			public HandlerLink Next = null;
		}

		void Start() {
			if (messageQueueRuning == false || Common.currentTimeMillis () - lastStartMsgQueue > 5000) {
				messageQueueRuning = false;
				StartMessageQueue ();
			}
		}

		protected override void Awake() {	
//			if (!isInitRemote) {
//				isInitRemote = true;
//				LogState.SetWriteLog (WriteLog);
//				if (RemoteLog && RemotePort > 0 && !string.IsNullOrEmpty (RemoteAddr)) {
//					LogState.SetRemoteLog (true, RemoteAddr, RemotePort);
//				}
//			}
			base.Awake ();
			messageQueueRuning = false;
			StartMessageQueue ();
		}

		protected virtual void OnApplicationPause(bool isPause) {
			if (isPause) {
				this.LOG ("Game Pause.");
				m_IsPause = true;
				SendMsg (new MsgBase (MsgConst.GamePause));
			} else {
				m_IsPause = false;
				this.LOG ("Game Resume.");
				SendMsg (new MsgBase (MsgConst.GameResume));
			}
		}

		public int MsgHandlerCount {
			get { return MsgHandlerMap.Count; }
		}

		public int MsgCount {
			get { return MsgQueue.Count; }
		}

		/// <summary>
		/// 注册消息
		/// </summary>
		protected void RegMsg(int msgId) {
			if (MsgHandler == null)
				MsgHandler = new MsgHandlerBase (this, this);
			MsgHandler.AddMsgId (msgId);
		}

		/// <summary>
		/// 取消注册的消息ID
		/// </summary>
		protected void UnRegMsg(int msgId) {
			if (MsgHandler == null)
				return;
			MsgHandler.RemoveMsgId (msgId);
		}

		/// <summary>
		/// 中止游戏
		/// </summary>
		public void EndGame() {
			SendMsg (new MsgBase (MsgConst.GameEnd));
		}

		/// <summary>
		/// 暂停游戏
		/// </summary>
		public void PauseGame(bool v) {
			if (v != IsPause) {
				IsPause = v;
				if (IsPause) {
					SendMsg (new MsgBase (MsgConst.GamePause));
					Time.timeScale = 0;
				} else {
					Time.timeScale = 1;
					SendMsg (new MsgBase (MsgConst.GameResume));
				}	
			}
		}

		/// <summary>
		/// Hint the specified msg.
		/// </summary>
		/// <param name="msg">Message.</param>
		public void Hint(string msg) {
			Common.Hint (msg);
		}

		/// <summary>
		/// Writes the log.
		/// </summary>
		/// <param name="msg">Message.</param>
		public void WriteLog(string msg) {
			#if UNITY_ANDROID || UNITY_ANDROID_API
			AndroidCommon.Log (msg);
			#else
			Debug.Log(msg);
			#endif
		}

		/// <summary>
		/// 获取指定MsgID的消息处理器链表
		/// </summary>
		protected HandlerLink GetHandlerLink(int msgID) {
			if (MsgHandlerMap.ContainsKey (msgID)) {
				return MsgHandlerMap [msgID] as HandlerLink;
			} else
				return null;
		}

		/// <summary>
		/// 检测处理器链表中是否已经存在相同的处理器
		/// </summary>
		protected bool ExistHandler(HandlerLink link, IMsgHandler msgHandler) {
			while (link != null) {
				if (link.Handler == msgHandler)
					return true;
				link = link.Next;
			}
			return false;
		}

		/// <summary>
		/// 注册消息处理服务
		/// </summary>
		/// <param name="msgHandler">消息处理器对象</param>
		public void Register(IMsgHandler msgHandler, int msgID) {
			if (msgHandler == null || msgID == 0)
				return;
			HandlerLink link = GetHandlerLink (msgID);
			if (link == null) {
				link = new HandlerLink ();
				link.Handler = msgHandler;
				MsgHandlerMap.Add (msgID, link);
			} else if (!ExistHandler (link, msgHandler)) {
				while (link.Next != null)
					link = link.Next;
				link.Next = new HandlerLink();
				link.Next.Handler = msgHandler;
			}
		}

		/// <summary>
		/// 取消指定消息ID注册的消息处理器
		/// </summary>
		public void UnRegister(int msgID) {
			MsgHandlerMap.Remove (msgID);
		}

		/// <summary>
		/// 取消指定消息处理器的注册
		/// </summary>
		/// <param name="msgHandler">要取消注册的消息处理器</param>
		public void UnRegister(IMsgHandler msgHandler) {
			if (msgHandler == null)
				return;
			int[] keys = msgHandler.MsgIds;
			if (keys != null && keys.Length > 0) {
				foreach (int key in keys) {
					if (MsgHandlerMap.ContainsKey (key)) {
						HandlerLink link = MsgHandlerMap [key] as HandlerLink;
						HandlerLink up = null;
						while (link != null) {							
							if (link.Handler == msgHandler) {
								link.Handler = null;
								if (up != null) {
									up.Next = link.Next;
								} else if (link.Next == null) {
									MsgHandlerMap.Remove (key);
								} else {
									MsgHandlerMap [key] = link.Next;
								}
								break;
							} else {
								up = link;
								link = link.Next;
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// 解除所有已经注册的消息处理器
		/// </summary>
		public void ClearRegister() {
			MsgHandlerMap.Clear ();
		}

		/// <summary>
		/// 发送消息 (此处不在做参数正确性检查，请在消息处理器中确保msg的合法性)
		/// </summary>
		public void SendMsg (IMsgPack msg) {
			MsgQueue.Enqueue (msg);
		}

		/// <summary>
		/// 同步发送消息 (此处不在做参数正确性检查，请在消息处理器中确保msg的合法性)
		/// </summary>
		public void SendMsgSync (IMsgPack msg) {
			SendMsg (GetHandlerLink (msg.MsgID), msg);
		}

		protected void SendMsg(HandlerLink link, IMsgPack msg) {
			while (link != null) {
				if (link.Handler != null) {
					try {
						link.Handler.ProcessMsg (msg);
					} catch (System.Exception e) {
						this.LOG ("SendMsg: " + e.Message);
					}
				}
				link = link.Next;
			}
		}

		private bool messageQueueRuning = false;
		private long lastStartMsgQueue = 0;
		private void StartMessageQueue() {
			if (messageQueueRuning || IsDestroying)
				return;
			messageQueueRuning = true;
			lastStartMsgQueue = Common.currentTimeMillis ();
			StartCoroutine (MessageQueue());
			RegMsg (MsgConst.GameEnd);
			RegMsg (MsgConst.GameDestory);
		}

		IEnumerator MessageQueue() {			
			WaitUntil waitUnitl = new WaitUntil (ExistNewMsg);	
			int wref = 0;

			while (true) {

				if (IsPause || MsgQueue.Count == 0) {
					wref = 0;
					if (waitUnitl == null)
						waitUnitl = new WaitUntil (ExistNewMsg);
					yield return waitUnitl;
				} else {
					wref++;
					if (wref > SpeedRatio) {
						if (IsDestroying)
							yield break;
						else
							yield return null;
						wref = 0;
					}
				}

				IMsgPack msg = MsgQueue.Dequeue();
				object o = MsgHandlerMap [msg.MsgID];
				if (o != null) 
					SendMsg (o as HandlerLink, msg);

				//StartCoroutine(SendMessage(MsgQueue.Dequeue()));
			}

		}

		/**
		IEnumerator SendMessage(IMsgPack msg) {
			object o = MsgHandlerMap [msg.MsgID];
			if (o != null) 
				(o as IMsgHandler).ProcessMsg (msg);
			yield return null;
		}
		*/

		private bool ExistNewMsg() {
			return MsgQueue.Count > 0;
		}
	}
}