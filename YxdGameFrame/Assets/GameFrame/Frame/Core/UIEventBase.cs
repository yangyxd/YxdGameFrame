using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

/// <summary>
/// 游戏架构
/// </summary>
namespace GameFrame {


	/// <summary>
	/// UI 基类, 支持基本的事件
	/// </summary>
	public abstract class UIEventBase : UIBase, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler {
		private bool m_isDown = false;
		private bool m_isPointerInside = false;

		/// <summary>
		/// 是否按下了手指或鼠标
		/// </summary>
		public bool isDown {
			get { return m_isDown; }
		}

		/// <summary>
		/// 指针是否进入
		/// </summary>
		public bool isPointerInside {
			get { return m_isPointerInside; }
		}

		public virtual void OnPointerClick(PointerEventData eventData) {
			if (eventData.button != PointerEventData.InputButton.Left)
				return;
			DoClick(eventData);
		}

		public virtual void OnPointerDown(PointerEventData eventData) {
			if (eventData.button != PointerEventData.InputButton.Left)
				return;
			m_isDown = true;
			DoPointerDown (eventData);
		}

		public virtual void OnPointerUp(PointerEventData eventData) {
			if (eventData.button != PointerEventData.InputButton.Left)
				return;
			m_isDown = false;
			DoPointerUp (eventData);
		}

		public virtual void OnPointerEnter(PointerEventData eventData) {
			m_isPointerInside = true;
			DoPointerEnter (eventData);
		}

		public virtual void OnPointerExit(PointerEventData eventData) {
			m_isPointerInside = false;
			DoPointerExit (eventData);
		}

		/// <summary>
		/// 点击事件
		/// </summary>
		protected virtual void DoClick(PointerEventData eventData) {
		}

		/// <summary>
		/// 指针按下
		/// </summary>
		protected virtual void DoPointerDown(PointerEventData eventData) {
		}

		/// <summary>
		/// 指针松开
		/// </summary>
		protected virtual void DoPointerUp(PointerEventData eventData) {
		}

		/// <summary>
		/// 指针进入
		/// </summary>
		protected virtual void DoPointerEnter(PointerEventData eventData) {
		}

		/// <summary>
		/// 指针离开
		/// </summary>
		protected virtual void DoPointerExit(PointerEventData eventData) {
		}
	}

}