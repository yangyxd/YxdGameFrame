using UnityEngine;
using System.Collections;
using GameFrame.Core;
using UnityEngine.UI;

/// <summary>
/// 游戏架构
/// </summary>
namespace GameFrame {
	using UnityEngine.EventSystems;
	using System.Collections.Generic;
	
	/// <summary>
	/// 为对象添加拖放支持
	/// </summary>
	[AddComponentMenu("GameFrame/Component/DropDrag", 180)]
	public class DropDrag : UIEventBase {
		private Vector3 last;
		private Transform parent;

		/// <summary>
		/// 当前对象是否是一个拖放落下目标
		/// </summary>
		public bool DropTarget = false;
		/// <summary>
		/// 是否允许拖放自己
		/// </summary>
		public bool AllowDrag = true;

		/// <summary>
		/// 拖放消息
		/// </summary>
		public enum DropDragMsgID {
			/// <summary>
			/// 开始拖放
			/// </summary>
			drg_Start = MsgConst.UI_DropDrag,
			/// <summary>
			/// 拖放中
			/// </summary>
			drg_Draging,
			/// <summary>
			/// 拖放结束
			/// </summary>
			drg_End,
			/// <summary>
			/// 拖放被接受
			/// </summary>
			drg_AccetDrop,
		}

		/// <summary>
		/// 拖放消息
		/// </summary>
		public class DropDragMsg : MsgBase {
			/// <summary>
			/// 被拖放的对象
			/// </summary>
			public GameObject gameObject;
			/// <summary>
			/// 源Parent
			/// </summary>
			public Transform srcParent;
			/// <summary>
			/// 探测到的鼠标位置下的UI组件
			/// </summary>
			public GameObject rayTarget;

			public DropDragMsg(int msgID) : base(msgID) {}
		}

		private DropDragMsg dragMsg = null;
		private Canvas canvas;
		private Text state;

		/// <summary>
		/// 游戏场景里的对象的世界坐标转化到ui界面上的坐标
		/// 赋值操作  对象.transform.position =返回值
		/// </summary>
		/// <param name="canvas">需要转化到的uicanvas</param>
		/// <param name="worldGo">对象在游戏场景中的世界坐标</param>
		/// <returns>转化到ui视图下的坐标</returns>
		public static Vector3 WorldToUIPoint(Canvas canvas, Vector3 transformPos)
		{
			Vector3 v_v3 = Camera.main.WorldToScreenPoint(transformPos);
			Vector3 v_ui = canvas.worldCamera.ScreenToWorldPoint(v_v3);
			Vector3 v_new = new Vector3(v_ui.x, v_ui.y, canvas.GetComponent<RectTransform>().anchoredPosition3D.z);
			return v_new;
		}


		/// <summary>
		/// Cast a ray to test if screenPosition is over any UI object in canvas. This is a replacement
		/// for IsPointerOverGameObject() which does not work on Android in 4.6.0f3
		/// </summary>
		private bool IsPointerOverUIObject(Canvas canvas, Vector2 screenPosition) {
			// Referencing this code for GraphicRaycaster https://gist.github.com/stramit/ead7ca1f432f3c0f181f
			// the ray cast appears to require only eventData.position.
			PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
			eventDataCurrentPosition.position = screenPosition;

			GraphicRaycaster uiRaycaster = canvas.gameObject.GetComponent<GraphicRaycaster>();
			List<RaycastResult> results = new List<RaycastResult>();
			uiRaycaster.Raycast(eventDataCurrentPosition, results);
			return results.Count > 0;
		}

		public override void ProcessMsg (IMsgPack msg) {
			DropDragMsg dragMsg = null;
			switch (msg.MsgID) {
			case (int) DropDragMsgID.drg_Start:
				break;
			case (int) DropDragMsgID.drg_Draging:
				break;
			case (int) DropDragMsgID.drg_End:
				dragMsg = msg as DropDragMsg;
				if (dragMsg.rayTarget == gameObject) {
					dragMsg.gameObject.transform.SetParent(this.transform);
					// 发送一个接受消息
					msg.MsgID = (int)DropDragMsgID.drg_AccetDrop;
					SendMsg (msg);
				}
				break;
			}
		}

		void Start() {
			state = GameObject.Find ("State").GetComponent<Text> ();
			canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
			if (DropTarget) {				
				RegMsg ((int)DropDragMsgID.drg_Draging);
				RegMsg ((int)DropDragMsgID.drg_End);
			}
		}

		void Update () {
			Vector3 lt = canvas.worldCamera.WorldToScreenPoint (transform.position);
			state.text = string.Format ("LT: x={0}, y={1}", lt.x, lt.y);
			if (AllowDrag) {
				if (isDown && GameInput.Instance.isMove && GameInput.Instance.isLongDown) {
					Vector3 pos = Camera.main.ScreenToWorldPoint (GameInput.Instance.mousePosition);
					float x = pos.x - last.x;
					float y = pos.y - last.y;
					transform.position += new Vector3 (x, y, 0);
					last = pos;
				}
			}
		}

		protected override void DoPointerDown(PointerEventData eventData) {
			if (!AllowDrag)
				return;
			
			last = Camera.main.ScreenToWorldPoint(GameInput.Instance.mousePosition);
			parent = transform.parent;

			Transform first = parent;
			if (parent != null) {
				while (true) {
					if (first.parent != null)
						first = first.parent;
					else
						break;
				}
			}

			transform.SetParent (first);
			startDown = Common.currentTimeMillis ();

			// 发送消息，表示已经开始拖放对象
			dragMsg = new DropDragMsg((int)DropDragMsgID.drg_Start);
			dragMsg.srcParent = parent;
			dragMsg.gameObject = gameObject;
			SendMsg (dragMsg);
			StartCoroutine (ToTop ());
		}

		protected override void DoPointerUp(PointerEventData eventData) {
			if (AllowDrag) {
				Transform p = transform.parent;

				dragMsg.rayTarget = null;
				dragMsg.MsgID = (int)DropDragMsgID.drg_End;
				PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
				eventDataCurrentPosition.position = GameInput.Instance.mousePosition;
				// 使和射线检测鼠标下的UI对象
				GraphicRaycaster uiRaycaster = canvas.gameObject.GetComponent<GraphicRaycaster>();
				List<RaycastResult> results = new List<RaycastResult>();
				uiRaycaster.Raycast(eventDataCurrentPosition, results);
				if (results.Count > 0) {
					List<GameObject> srcChilds = new List<GameObject>();
					if (dragMsg.gameObject != null) {
						Transform[] allChildren = dragMsg.gameObject.GetComponentsInChildren<Transform>();
						foreach (Transform child in allChildren){
							srcChilds.Add (child.gameObject);
						}
					}
					int index = -1;
					for (int i = 0; i < results.Count; i++) {
						RaycastResult item = results [i];
						bool isContinue = false;
						for (int j = 0, max = srcChilds.Count; j < max; j++) {
							if (srcChilds [j] == item.gameObject) {
								isContinue = true;
								break;
							}
						}
						if (isContinue)
							continue;
						if (index == -1 || results [index].depth < item.depth)
							index = i;
					}
					if (index != -1)						
						dragMsg.rayTarget = results [index].gameObject;
				}
				//  发送消息
				SendMsgSync (dragMsg);

				if (transform.parent == p)
					transform.SetParent (parent);
			}
		}

		IEnumerator ToTop() {
			yield return new WaitUntil (WaitTime);
			transform.SetAsLastSibling ();
			yield return null;
		}

		private long startDown = 0;
		private bool WaitTime() {
			return Common.currentTimeMillis() - startDown > 50;
		}

	}
}