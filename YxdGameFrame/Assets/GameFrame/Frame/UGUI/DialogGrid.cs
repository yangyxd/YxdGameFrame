using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace GameFrame {
	#if UNITY_EDITOR
	using UnityEditor;
	#endif
	using UnityEngine.UI;
	using UnityEngine.Events;
	using UnityEngine.EventSystems;

	/// <summary>
	/// 格子对话框
	/// </summary>
	public class DialogGrid : UIBase {
		// 空格子预制体
		public GameObject gridPrebs;
		// 格子总数
		public int GridCount = 18;
		// 每行格子数
		public int Columns = 6;
		// 最多显示几行
		public float RowMax = 3;

		// 内容区域父节点
		private GameObject body;

		// 格子布局中设定的格子大小
		private float gw, gh;

		private int Rows;

		public override void ProcessMsg (IMsgPack msg) {
			throw new System.NotImplementedException ();
		}

		void Awake() {	
			// 计算行数	
			Rows = (int) (GridCount / Columns);
			if (GridCount % Columns > 0)
				Rows++;

			// 获取对话框中的内容和主体区域
			Transform content = transform.FindChild ("Content");
			body = content.FindChild("Body").gameObject;

			// 根据格子数量，计算对话框宽度
			// 宽度需要加上父节点的左右边距
			RectTransform rt = content.GetComponent<RectTransform> ();
			float ow = rt.offsetMin.x - rt.offsetMax.x;
			float oy = rt.offsetMin.y - rt.offsetMax.y;

			rt = body.GetComponent<RectTransform> ();
			ow += rt.offsetMin.x - rt.offsetMax.x;

			GridLayoutGroup grids = body.GetComponent<GridLayoutGroup> ();
			gw = grids.cellSize.x;
			gh = grids.cellSize.y;

			ow += gw * Columns + grids.spacing.x * (Columns - 1);
			oy += gh * RowMax + grids.spacing.y * (RowMax - 1);
			

			// 更新对话框宽度
			rt = transform.GetComponent<RectTransform> ();
			if (RowMax <= 0)
				oy = rt.sizeDelta.y;
			rt.sizeDelta = new Vector2 (ow, oy);

			// 更新内容区域高度
			rt = body.GetComponent<RectTransform> ();
			rt.sizeDelta = new Vector2 (rt.sizeDelta.x, Rows * gh - rt.offsetMax.y + grids.spacing.y * (Rows - 1)) ;

			// 实例化格子
			for (int i = 0; i < GridCount; i++) {
				GameObject item = GameObject.Instantiate (gridPrebs, Vector3.one, Quaternion.identity) as GameObject;
				item.transform.SetParent (body.transform);
				item.transform.localScale = new Vector3 (1, 1, 1);
			}
		}

		#if UNITY_EDITOR
		[MenuItem("GameObject/UI/Dialog Grid")]
		static void CreateDialogGrid(MenuCommand menuCmd) {
			// 创建游戏对象
			float w = 300f;
			float h = 200f;
			GameObject root = UICommon.CreateUIElementRoot("DialogGrid", w, h);
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
			//DialogGrid dlgGrid = root.AddComponent<DialogGrid> ();
			root.AddComponent<DialogGrid> ();

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
			GridLayoutGroup grid = body.AddComponent<GridLayoutGroup> ();
			grid.cellSize = new Vector2 (72, 72);
			//body.AddComponent<Mask> ().showMaskGraphic = true;


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
			DialogCloseButton closeSript = close.AddComponent<DialogCloseButton> ();
			closeSript.dialog = dlg;

			// 放入到UI Canvas中
			UICommon.PlaceUIElementRoot(root, menuCmd);
		}
		#endif

	}
}
