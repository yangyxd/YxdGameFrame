using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameFrame.Core;

namespace GameFrame {
	#if UNITY_EDITOR
	using UnityEditor;
	#endif
	using UnityEngine.UI;
	using UnityEngine.Events;
	using UnityEngine.EventSystems;

	/// <summary>
	/// 对话框
	/// </summary>
	public class Dialog : UIEventBase {

		private Vector3 last;
		private GameObject Panel;
		private Image img;
		private bool sceneStarting = true;
		private float alpha = 0.2f;

		/// <summary>
		/// 是否允许拖动
		/// </summary>
		public bool allowMove = true;
		/// <summary>
		/// 内容区域
		/// </summary>
		public Transform content;

		private bool isContent = true;

		public override void ProcessMsg (IMsgPack msg) {}

		void Start() {
			Panel = UICommon.CreateUIElementRoot ("DialogFog", new Vector2 (Screen.width, Screen.height));
			Panel.AddComponent<CanvasRenderer> ();
			Panel.AddComponent<DialogCloseButton> ().dialog = this;
			img = Panel.AddComponent<Image> ();
			img.color = new Color(0,0,0,alpha);
			img.fillCenter = true;
			img.raycastTarget = true;
			img.enabled = true;
			RectTransform R = Panel.GetComponent<RectTransform> ();
			R.anchorMin = new Vector2 (0, 0);
			R.anchorMax = new Vector2 (1, 1);
			R.pivot = new Vector2 (0.5f, 0.5f);
			Panel.transform.SetParent (FindComponentInParent<Canvas>().transform);
			Panel.transform.localScale = new Vector3 (1, 1, 1);
			R.offsetMax = new Vector2 (0, 0);
			R.offsetMin = new Vector2 (0, 0);
			transform.SetAsLastSibling ();
		}

		void Update () {
			if (Input.GetKeyDown (KeyCode.Escape)) { // 按返回或ESC关闭对话框
				DoClose ();
				return;
			}
			if (sceneStarting)
				StartScene();
			if (isDown && allowMove && !isContent && GameInput.Instance.isMove && GameInput.Instance.isLongDown) {
				Vector3 pos = Camera.main.ScreenToWorldPoint(GameInput.Instance.mousePosition);
				transform.position += new Vector3(pos.x - last.x, pos.y - last.y, 0);
				last = pos;
			}
		}

		void StartScene() {
			FadeToClear();
			if(Mathf.Abs(alpha) >= 0.75f) {
				SetColor(0.75f);
				sceneStarting  = false;
			}
		}

		void FadeToClear() {
			SetColor(Mathf.Lerp(alpha, 1.0f, 4 * Time.deltaTime));
		}

		void SetColor(float v) {
			alpha = v;
			img.color = new Color (0, 0, 0, alpha);
		}

		void OnDestroy() {
			if (Panel != null)
				DestroyObject (Panel);
			SendMsgSync (new DialogManager.DialogClosedMsg (gameObject));
		}

		protected override void DoClick(PointerEventData eventData) {
			transform.SetAsLastSibling ();
		}

		protected override void DoPointerDown(PointerEventData eventData) {
			if (eventData.button == PointerEventData.InputButton.Left) {
				isContent = false;
				if (content != null) {
					List<RaycastResult> hits = new List<RaycastResult>();  
					EventSystem.current.RaycastAll (eventData, hits);
					foreach (RaycastResult h in hits) {
						if (h.gameObject == content.gameObject) {
							isContent = true;
							return;
						}
					}
					if (!isContent) {
						last = Camera.main.ScreenToWorldPoint (GameInput.Instance.mousePosition);
					}
				}
			}
		}

		public void DoClose() {
			this.LOG ("Dialog DoClose: " + gameObject.name);
			DestroyObject (gameObject);
		}

		public void DoHide() {
			gameObject.SetActive (false);
		}

		public void DoShow() {		
			gameObject.SetActive (true);
		}

		#if UNITY_EDITOR
		[MenuItem("GameObject/UI/Dialog")]
		static void CreateDialog(MenuCommand menuCmd) {
			// 创建游戏对象
			float w = 300f;
			float h = 200f;
			GameObject root = UICommon.CreateUIElementRoot("Dialog", w, h);
			root.AddComponent<CanvasRenderer> ();
			Image img = root.AddComponent<Image> ();
			img.color = new Color(1,1,1,0.75f);
			img.fillCenter = true;
			img.raycastTarget = true;
			img.sprite = Common.findRes<Sprite>("Background");
			if (img.sprite != null)
				img.type = Image.Type.Sliced;
			RectTransform R = root.GetComponent<RectTransform> ();
			R.anchorMin = new Vector2 (0, 1);
			R.anchorMax = new Vector2 (0, 1);
			R.pivot = new Vector2 (0, 1);
			Dialog dlg = root.AddComponent<Dialog> ();

			// 创建Text对象
			GameObject text = UICommon.CreateUIText("Text", "对话框", root);
			R = text.GetComponent<RectTransform> ();
			R.anchorMin = new Vector2 (0, 1);
			R.anchorMax = new Vector2 (1, 1);
			R.pivot = new Vector2 (1, 1);
			//R.= new Rect (8, -5, 8, 32);
			R.anchoredPosition = new Vector2(0, -5);
			R.sizeDelta = new Vector2 (0, 32);
			Text textPro = text.GetComponent<Text> ();
			textPro.fontSize = 20;
			textPro.supportRichText = true;
			textPro.resizeTextForBestFit = true;
			textPro.resizeTextMinSize = 1;
			textPro.resizeTextMaxSize = 20;
			textPro.color = Color.white;
			text.AddComponent<Outline> ();
			//text.AddComponent<Shadow> ();

			// 内容区
			GameObject content = UICommon.CreateUIObject ("Content", root);
			R = content.GetComponent<RectTransform> ();
			R.anchorMin = new Vector2 (0, 0);
			R.anchorMax = new Vector2 (1, 1);
			R.pivot = new Vector2 (0, 1);
			R.anchoredPosition = new Vector2(12, -40);
			R.sizeDelta = new Vector2 (-24, -52);
			img = content.AddComponent<Image> ();
			img.color = new Color(1, 1, 1, 0.5f);
			img.fillCenter = true;
			img.raycastTarget = true;
			content.AddComponent<Mask> ();
			ScrollRect scroll = content.AddComponent<ScrollRect>();
			scroll.horizontal = true;
			scroll.vertical = true;
			scroll.movementType = ScrollRect.MovementType.Clamped;
			scroll.elasticity = 0.1f;
			scroll.inertia = true;
			scroll.decelerationRate = 0.135f;
			scroll.scrollSensitivity = 1;
			dlg.content = content.transform;

			GameObject body = new GameObject("Body");
			body.transform.SetParent(content.transform, false);
			RectTransform ItemsRect = body.AddComponent<RectTransform>();
			ItemsRect.anchorMin = Vector2.zero;
			ItemsRect.anchorMax = Vector2.one;
			ItemsRect.anchoredPosition = Vector2.zero;
			ItemsRect.sizeDelta = Vector2.zero;
			body.AddComponent<CanvasRenderer>();
			scroll.content = body.GetComponent<RectTransform>();


			// 关闭按钮
			GameObject close = UICommon.CreateUIObject ("BtnClose", root);
			R = close.GetComponent<RectTransform> ();
			R.anchorMin = new Vector2 (1, 1);
			R.anchorMax = new Vector2 (1, 1);
			R.pivot = new Vector2 (0.5f, 0.5f);
			R.anchoredPosition = new Vector2(-24, -21);
			R.sizeDelta = new Vector2 (24, 24);
			close.AddComponent<CanvasRenderer> ();
			img = close.AddComponent<Image> ();
			img.color = new Color(1, 1, 1, 0.5f);
			img.fillCenter = true;
			img.raycastTarget = true;
			img.sprite = Common.findRes<Sprite>("UISprite");
			if (img.sprite != null)
				img.type = Image.Type.Sliced;
			close.AddComponent<DialogCloseButton> ().dialog = dlg;

			// 放入到UI Canvas中
			UICommon.PlaceUIElementRoot(root, menuCmd);
		}
		#endif

	}
}