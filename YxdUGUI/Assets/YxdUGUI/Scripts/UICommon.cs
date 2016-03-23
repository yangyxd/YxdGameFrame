using System;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;


namespace UnityEngine.UI {
	
	/// <summary>
	/// 自定义UI公用库, 部分代码来自 Unity 开源代码 （MenuOptions, DefaultControls)
	/// <remarks>作者: YangYxd</remarks>
	/// </summary>
	public class UICommon {
		private const string kUILayerName = "UI";
		private static Color   s_PanelColor             = new Color(1f, 1f, 1f, 0.392f);
		private static Color   s_TextColor              = new Color(50f / 255f, 50f / 255f, 50f / 255f, 1f);

		/// <summary>
		/// 设置父级对齐方式
		/// </summary>
		public static void SetParentAndAlign(GameObject child, GameObject parent) {
			if (parent == null)
				return;

			child.transform.SetParent(parent.transform, false);
			SetLayerRecursively(child, parent.layer);
		}

		/// <summary>
		/// 设置子组件层属性
		/// </summary>
		public static void SetLayerRecursively(GameObject go, int layer) {
			go.layer = layer;
			Transform t = go.transform;
			for (int i = 0; i < t.childCount; i++)
				SetLayerRecursively(t.GetChild(i).gameObject, layer);
		}

		/// <summary>
		/// 创建UI对象
		/// </summary>
		/// <param name="name">对象名称</param>
		/// <param name="parent">父级</param>
		public static GameObject CreateUIObject(string name, GameObject parent) {
			GameObject go = new GameObject(name);
			go.AddComponent<RectTransform>();
			SetParentAndAlign(go, parent);
			return go;
		}

		/// <summary>
		/// 创建根结点UI对象
		/// </summary>
		/// <param name="name">对象名称</param>
		/// <param name="size">对象大小</param>
		public static GameObject CreateUIElementRoot(string name, Vector2 size) {
			GameObject go = new GameObject(name);
			RectTransform rect = go.AddComponent<RectTransform> ();
			rect.sizeDelta = size; 
			return go;
		}

		/// <summary>
		/// 创建根结点UI对象
		/// </summary>
		/// <param name="name">对象名称</param>
		/// <param name="w">宽度</param>
		/// <param name="h">高度</param>
		public static GameObject CreateUIElementRoot(string name, float w, float h) {
			return CreateUIElementRoot (name, new Vector2 (w, h));
		}

		/// <summary>
		/// 创建UI Text 控件
		/// </summary>
		/// <param name="name">名称</param>
		/// <param name="text">文本内容</param>
		/// <param name="parent">父亲</param>
		public static GameObject CreateUIText(string name, string text, GameObject parent) {
			GameObject childText = UICommon.CreateUIObject(name, parent);
			Text v = childText.AddComponent<Text>();
			v.text = text;
			v.alignment = TextAnchor.MiddleCenter;
			UICommon.SetDefaultTextValues(v);

			RectTransform r = childText.GetComponent<RectTransform> ();
			r.anchorMin = Vector2.zero;
			r.anchorMax = Vector2.one;
			r.sizeDelta = Vector2.zero;

			return childText;
		}

		/// <summary>
		/// 设置默认Text颜色
		/// </summary>
		public static void SetDefaultTextValues(Text lbl) {
			lbl.color = s_TextColor;
		}

		/// <summary>
		/// 设置默认Transition（状态过渡）颜色
		/// </summary>
		public static void SetDefaultColorTransitionValues(Selectable slider) {
			ColorBlock colors = slider.colors;
			colors.highlightedColor = new Color(0.882f, 0.882f, 0.882f);
			colors.pressedColor     = new Color(0.698f, 0.698f, 0.698f);
			colors.disabledColor    = new Color(0.521f, 0.521f, 0.521f);
		}

		#if UNITY_EDITOR
		#else
		public sealed class MenuCommand {
			public Object context;
			public int userData;

		}
		#endif

		/// <summary>
		/// 放置UI对象到UI层Canvas中
		/// </summary>
		public static void PlaceUIElementRoot(GameObject element, MenuCommand menuCommand) {
			#if UNITY_EDITOR
			GameObject parent = menuCommand.context as GameObject;
			if (parent == null || parent.GetComponentInParent<Canvas>() == null) {
				parent = GetOrCreateCanvasGameObject();
			}

			string uniqueName = GameObjectUtility.GetUniqueNameForSibling(parent.transform, element.name);
			element.name = uniqueName;
			Undo.RegisterCreatedObjectUndo(element, "Create " + element.name);
			Undo.SetTransformParent(element.transform, parent.transform, "Parent " + element.name);
			GameObjectUtility.SetParentAndAlign(element, parent);
			if (parent != menuCommand.context) // not a context click, so center in sceneview
				SetPositionVisibleinSceneView(parent.GetComponent<RectTransform>(), element.GetComponent<RectTransform>());

			Selection.activeGameObject = element;
			#endif
		}

		/// <summary>
		/// 创建新的UI层
		/// </summary>
		public static GameObject CreateNewUI() {
			// Root for the UI
			var root = new GameObject("Canvas");
			root.layer = LayerMask.NameToLayer(kUILayerName);
			Canvas canvas = root.AddComponent<Canvas>();
			canvas.renderMode = RenderMode.ScreenSpaceOverlay;
			root.AddComponent<CanvasScaler>();
			root.AddComponent<GraphicRaycaster>();
			#if UNITY_EDITOR
			Undo.RegisterCreatedObjectUndo(root, "Create " + root.name);
			#endif
			// if there is no event system add one...
			CreateEventSystem(false);
			return root;
		}

		/// <summary>
		/// 创建UI事件系统
		/// </summary>
		/// <param name="select">是否选中</param>
		public static void CreateEventSystem(bool select) {
			CreateEventSystem(select, null);
		}

		/// <summary>
		/// 创建UI事件系统 (在非 UNITY_EDITOR 时不执行任何操作)
		/// </summary>
		/// <param name="select">是否选中</param>
		public static void CreateEventSystem(bool select, GameObject parent) {
			#if UNITY_EDITOR
			var esys = Object.FindObjectOfType<EventSystem>();
			if (esys == null) {
				var eventSystem = new GameObject("EventSystem");
				GameObjectUtility.SetParentAndAlign(eventSystem, parent);
				esys = eventSystem.AddComponent<EventSystem>();
				eventSystem.AddComponent<StandaloneInputModule>();

				Undo.RegisterCreatedObjectUndo(eventSystem, "Create " + eventSystem.name);
			}

			if (select && esys != null)	{
				Selection.activeGameObject = esys.gameObject;
			}
			#endif
		}

		/// <summary>
		/// 创建UI事件系统
		/// </summary>
		public static void CreateEventSystem(MenuCommand menuCommand) {
			GameObject parent = menuCommand.context as GameObject;
			CreateEventSystem(true, parent);
		}

		/// <summary>
		/// 创建或返回一个现存的UI Canvas对象 (在非 UNITY_EDITOR 时返回null)
		/// </summary>
		public static GameObject GetOrCreateCanvasGameObject() {
			#if UNITY_EDITOR
			GameObject selectedGo = Selection.activeGameObject;

			// Try to find a gameobject that is the selected GO or one if its parents.
			Canvas canvas = (selectedGo != null) ? selectedGo.GetComponentInParent<Canvas>() : null;
			if (canvas != null && canvas.gameObject.activeInHierarchy)
				return canvas.gameObject;

			// No canvas in selection or its parents? Then use just any canvas..
			canvas = Object.FindObjectOfType(typeof(Canvas)) as Canvas;
			if (canvas != null && canvas.gameObject.activeInHierarchy)
				return canvas.gameObject;

			// No canvas in the scene at all? Then create a new one.
			return CreateNewUI();
			#else
			return null;
			#endif
		}

		#if UNITY_EDITOR
		private static void SetPositionVisibleinSceneView(RectTransform canvasRTransform, RectTransform itemTransform) {
			// Find the best scene view
			SceneView sceneView = SceneView.lastActiveSceneView;
			if (sceneView == null && SceneView.sceneViews.Count > 0)
				sceneView = SceneView.sceneViews[0] as SceneView;

			// Couldn't find a SceneView. Don't set position.
			if (sceneView == null || sceneView.camera == null)
				return;

			// Create world space Plane from canvas position.
			Vector2 localPlanePosition;
			Camera camera = sceneView.camera;
			Vector3 position = Vector3.zero;
			if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRTransform, new Vector2(camera.pixelWidth / 2, camera.pixelHeight / 2), camera, out localPlanePosition))
			{
				// Adjust for canvas pivot
				localPlanePosition.x = localPlanePosition.x + canvasRTransform.sizeDelta.x * canvasRTransform.pivot.x;
				localPlanePosition.y = localPlanePosition.y + canvasRTransform.sizeDelta.y * canvasRTransform.pivot.y;

				localPlanePosition.x = Mathf.Clamp(localPlanePosition.x, 0, canvasRTransform.sizeDelta.x);
				localPlanePosition.y = Mathf.Clamp(localPlanePosition.y, 0, canvasRTransform.sizeDelta.y);

				// Adjust for anchoring
				position.x = localPlanePosition.x - canvasRTransform.sizeDelta.x * itemTransform.anchorMin.x;
				position.y = localPlanePosition.y - canvasRTransform.sizeDelta.y * itemTransform.anchorMin.y;

				Vector3 minLocalPosition;
				minLocalPosition.x = canvasRTransform.sizeDelta.x * (0 - canvasRTransform.pivot.x) + itemTransform.sizeDelta.x * itemTransform.pivot.x;
				minLocalPosition.y = canvasRTransform.sizeDelta.y * (0 - canvasRTransform.pivot.y) + itemTransform.sizeDelta.y * itemTransform.pivot.y;

				Vector3 maxLocalPosition;
				maxLocalPosition.x = canvasRTransform.sizeDelta.x * (1 - canvasRTransform.pivot.x) - itemTransform.sizeDelta.x * itemTransform.pivot.x;
				maxLocalPosition.y = canvasRTransform.sizeDelta.y * (1 - canvasRTransform.pivot.y) - itemTransform.sizeDelta.y * itemTransform.pivot.y;

				position.x = Mathf.Clamp(position.x, minLocalPosition.x, maxLocalPosition.x);
				position.y = Mathf.Clamp(position.y, minLocalPosition.y, maxLocalPosition.y);
			}

			itemTransform.anchoredPosition = position;
			itemTransform.localRotation = Quaternion.identity;
			itemTransform.localScale = Vector3.one;
		}
		#endif

	}

}
