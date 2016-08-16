using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 游戏架构
/// </summary>
namespace GameFrame {

	/// <summary>
	/// 对话框管理器 Dialog manager.
	/// </summary>
	[AddComponentMenu("GameFrame/Dialog Manager", 180)]
	public class DialogManager : MonoBase, IDebugMessage {

		/// <summary>
		/// 对话框预制体 The dilog prebs.
		/// </summary>
		public GameObject[] dilogPrebs;
		/// <summary>
		/// 所有对话框的父对象
		/// </summary>
		public GameObject dialogParent;

		private Dictionary<string, GameObject> dialogPrebsMap = new Dictionary<string, GameObject>();
		private Dictionary<string, GameObject> dialogShowed = new Dictionary<string, GameObject>();
		private Dictionary<string, System.Action<GameObject>> dialogCloseEvent = new Dictionary<string, System.Action<GameObject>>();

		public class DialogMsg : MsgBase {
			// 对话框名称
			public string name;

			public DialogMsg(string name, int msgID) : base(msgID) {
				this.name = name;
			}
		}

		/// <summary>
		/// 对话框消息 - 显示
		/// </summary>
		public class DialogShowMsg: DialogMsg {
			public System.Action<GameObject> onCloseEvent;
			public DialogShowMsg(string name) : base(name, (int) DialogMsgID.show) {
				this.onCloseEvent = null;
			}
			public DialogShowMsg(string name, System.Action<GameObject> onCloseEvent) : base(name, (int) DialogMsgID.show) {
				this.onCloseEvent = onCloseEvent;
			}
		}

		/// <summary>
		/// 对话框消息 - 隐藏
		/// </summary>
		public class DialogHideMsg: DialogMsg {
			public DialogHideMsg(string name) : base(name, (int) DialogMsgID.hide) {}
		}

		/// <summary>
		/// 对话框消息 - 关闭
		/// </summary>
		public class DialogCloseMsg: DialogMsg {
			public DialogCloseMsg(string name) : base(name, (int) DialogMsgID.close) {}
		}

		/// <summary>
		/// 对话框消息 - 关闭OK
		/// </summary>
		public class DialogClosedMsg: MsgBase {
			public GameObject dlg;
			public DialogClosedMsg(GameObject dlgObj) : base((int) DialogMsgID.closed) {
				dlg = dlgObj;
			}
		}

		/// <summary>
		/// 对话框消息 - 添加对话框预制到管理器
		/// </summary>
		public class DialogAddMsg: DialogMsg {
			// 对话框预制体
			public GameObject prebs;

			public DialogAddMsg(string name, GameObject prebs) : base(name, (int) DialogMsgID.add) {
				this.prebs = prebs;
			}
		}

		/// <summary>
		/// 对话框消息
		/// </summary>
		public enum DialogMsgID {
			/// <summary>
			/// 显示
			/// </summary>
			show = MsgConst.UI_Dialog,
			/// <summary>
			/// 隐藏
			/// </summary>
			hide,
			/// <summary>
			/// 开始关闭
			/// </summary>
			close,
			/// <summary>
			/// 添加对话框
			/// </summary>
			add,
			/// <summary>
			/// 关闭完成
			/// </summary>
			closed,
		}

		// 消息处理
		public override void ProcessMsg (IMsgPack msg) {
			if (msg.MsgID == (int)DialogMsgID.show) {
				DialogMsg v = (DialogMsg)msg;
				if (v is DialogShowMsg) {
					// 如果设置了事件，将事件加入事件表中
					System.Action<GameObject> onEvent = ((DialogShowMsg)v).onCloseEvent;
					if (onEvent != null && !dialogCloseEvent.ContainsKey(v.name)) {
						dialogCloseEvent.Add (v.name, onEvent);
					}						
				}
				ShowDialog (v.name);
			} else if (msg.MsgID == (int)DialogMsgID.hide) {
				DialogMsg v = (DialogMsg)msg;
				HideDialog (v.name);
			} else if (msg.MsgID == (int)DialogMsgID.close) {
				DialogMsg v = (DialogMsg)msg;
				CloseDialog (v.name);
			} else if (msg.MsgID == (int)DialogMsgID.add) {
				DialogAddMsg v = (DialogAddMsg)msg;
				AddDialog (v.name, v.prebs);
			} else if (msg.MsgID == (int)DialogMsgID.closed) {
				ClosedDialog (((DialogClosedMsg)msg).dlg);
			}
		}

		void Awake() {			
			if (dialogParent == null) 
				dialogParent = GameObject.Find ("Canvas");

			for (int i = 0; i < dilogPrebs.Length; i++) {
				GameObject obj = dilogPrebs [i];
				if (obj == null)
					continue;
				dialogPrebsMap.Add (obj.name, obj);
			}

			RegMsg ((int)DialogMsgID.show);
			RegMsg ((int)DialogMsgID.hide);
			RegMsg ((int)DialogMsgID.close);
			RegMsg ((int)DialogMsgID.add);
			RegMsg ((int)DialogMsgID.closed);
		}

		/// <summary>
		/// 对话框关闭后
		/// </summary>
		/// <param name="dlgObj">Dlg object.</param>
		void ClosedDialog(GameObject dlgObj) {
			if (dlgObj == null)
				return;
			string name = GetDialogName (dlgObj);
			if (string.IsNullOrEmpty (name))
				return;
			dialogShowed.Remove (name);
			if (dialogCloseEvent.ContainsKey (name)) {
				System.Action<GameObject> onEvent = dialogCloseEvent [name];
				dialogCloseEvent.Remove (name);
				try {
					onEvent (dlgObj);
				} catch (System.Exception e) {
					this.LOGERROR ("ClosedDialog: " + e.Message);
				}
			}
		}

		/// <summary>
		/// 获取一个已经显示的对话框名字
		/// </summary>
		/// <returns>The dialog name.</returns>
		/// <param name="dialogObj">Dialog object.</param>
		public string GetDialogName(GameObject dialogObj) {
			if (dialogObj != null) {
				foreach (KeyValuePair<string, GameObject> item in dialogShowed) {
					if (item.Value == dialogObj)
						return item.Key;
				}
			}
			return null;
		}

		/// <summary>
		/// 添加对话框（对话框预制体）
		/// </summary>
		/// <param name="prebs">Prebs.</param>
		public void AddDialog(string name, GameObject prebs) {
			if (prebs == null || name == null || name.Length == 0)
				return;
			dialogPrebsMap.Add (name, prebs);
		}

		/// <summary>
		/// 显示对话框
		/// </summary>
		/// <param name="name">Name.</param>
		public void ShowDialog(string name) {
			// 判断是否已经显示出来
			if (dialogShowed.ContainsKey (name)) {
				GameObject obj = dialogShowed [name];
				if (obj != null) {
					obj.SetActive (true);
					if (obj.transform != null)
						obj.transform.SetAsLastSibling ();
					return;
				} else
					dialogShowed.Remove (name);
			}

			if (!dialogPrebsMap.ContainsKey (name)) {
				this.LOG ("Dialog " + name + " does not exist.");
				return;
			}
			GameObject prebs = dialogPrebsMap [name];
			if (prebs == null) {
				this.LOG ("Dialog " + name + " does invalid.");
				return;
			}

			// 实例化对话框
			GameObject dialog = GameObject.Instantiate(prebs, new Vector3(1, 1, 1), Quaternion.identity) as GameObject;
			if (dialogParent != null)
				dialog.transform.SetParent (dialogParent.transform);
			dialog.transform.localScale = new Vector3 (1, 1, 1);
			RectTransform rt = dialog.GetComponent<RectTransform> ();
			rt.sizeDelta = new Vector3 (rt.sizeDelta.x, rt.sizeDelta.y);
			float h = rt.sizeDelta.y;
			float w = rt.sizeDelta.x;
			Vector3 newPos = new Vector3 (-(w / 2f), h / 2f, 0);
			rt.localPosition = newPos;
			dialogShowed.Add (name, dialog);
		}

		/// <summary>
		/// 关闭对话框
		/// </summary>
		/// <param name="name">Name.</param>
		public void CloseDialog(string name) {
			if (dialogShowed.ContainsKey (name)) {
				GameObject obj = dialogShowed [name];
				if (obj != null) {					
					DestroyObject (obj);
				} else
					dialogShowed.Remove (name);
			}
		}

		/// <summary>
		/// 隐藏对话框
		/// </summary>
		/// <param name="name">Name.</param>
		public void HideDialog(string name) {
			if (dialogShowed.ContainsKey (name)) {
				GameObject obj = dialogShowed [name];
				if (obj != null) {
					obj.SetActive (false);
				} else
					dialogShowed.Remove (name);
			}
		}
	}
}