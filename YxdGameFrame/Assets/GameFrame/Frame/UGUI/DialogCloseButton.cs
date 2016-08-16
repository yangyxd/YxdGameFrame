#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using GameFrame;

namespace UnityEngine.UI {

	/// <summary>
	/// 对话框关闭按钮 Dialog close button.
	/// </summary>
	public class DialogCloseButton : UnityEngine.UI.Selectable, IPointerClickHandler {
		// 所属对话框
		public Dialog dialog;

		public virtual void OnPointerClick(PointerEventData eventData) {
			if (eventData.button != PointerEventData.InputButton.Left)
				return;
			if (dialog != null)
				dialog.DoClose ();
		}
	}

}
