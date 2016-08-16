#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace UnityEngine.UI
{
	/// <summary>
	/// 列表视图布局
    /// 作者: YangYxd
	/// </summary>
	[AddComponentMenu("GameFrame/Layout/ListView Layout Group", 180)]
	public class ListViewLayoutGroup : LayoutGroup 
	{
		public enum ListDirection {
			horizontal,
			vertical
		}

		protected ListViewLayoutGroup()
		{}

        public override void CalculateLayoutInputHorizontal() {
            base.CalculateLayoutInputHorizontal();
			CalcAlongAxis(0, m_Direction == ListDirection.vertical);
        }

        public override void CalculateLayoutInputVertical() { 
            CalcAlongAxis(1, m_Direction == ListDirection.vertical);
        }

        public override void SetLayoutHorizontal() {
			SetChildrenAlongAxis(0, m_Direction == ListDirection.vertical);
        }

        public override void SetLayoutVertical() {
			SetChildrenAlongAxis(1, m_Direction == ListDirection.vertical);
        }

		[SerializeField] protected float m_Spacing = 0;
		public float spacing { get { return m_Spacing; } set { SetProperty(ref m_Spacing, value); } }

		[SerializeField] protected bool m_ItemForceWidth = true;
		public bool childForceExpandWidth { get { return m_ItemForceWidth; } set { SetProperty(ref m_ItemForceWidth, value); } }

		[SerializeField] protected bool m_ItemForceHeight = true;
		public bool childForceExpandHeight { get { return m_ItemForceHeight; } set { SetProperty(ref m_ItemForceHeight, value); } }

		[SerializeField] 
		protected float m_ItemWidth = 60;
		public float itemWidth { 
			get { return m_ItemWidth; } 
			set { SetProperty (ref m_ItemWidth, value); }
		}

		[SerializeField] 
		protected float m_ItemHegiht = 60;
		public float itemHeight { 
			get { return m_ItemHegiht; } 
			set { SetProperty (ref m_ItemHegiht, value); }
		}

		[SerializeField] 
		protected ListDirection m_Direction = ListDirection.vertical;
		public ListDirection direction { 
			get { return m_Direction; } 
			set {
                SetProperty (ref m_Direction, value);
            }
		}

		protected void CalcAlongAxis(int axis, bool isVertical)
		{
			float combinedPadding = (axis == 0 ? padding.horizontal : padding.vertical);


			float totalMin = combinedPadding;
			float totalPreferred = combinedPadding;
			float totalFlexible = 0;

			bool alongOtherAxis = (isVertical ^ (axis == 1));
			for (int i = 0; i < rectChildren.Count; i++)
			{
				RectTransform child = rectChildren[i];
				float min = LayoutUtility.GetMinSize(child, axis);
				float preferred = LayoutUtility.GetPreferredSize(child, axis);
				float flexible = LayoutUtility.GetFlexibleSize(child, axis);
				if ((axis == 0 ? childForceExpandWidth : childForceExpandHeight))
					flexible = Mathf.Max(flexible, 1);

				if (alongOtherAxis)
				{
					totalMin = Mathf.Max(min + combinedPadding, totalMin);
					totalPreferred = Mathf.Max(preferred + combinedPadding, totalPreferred);
					totalFlexible = Mathf.Max(flexible, totalFlexible);
				}
				else
				{
					totalMin += min + spacing;
					totalPreferred += preferred + spacing;

					// Increment flexible size with element's flexible size.
					totalFlexible += flexible;
				}
            }

			if (!alongOtherAxis && rectChildren.Count > 0)
			{
				totalMin -= spacing;
				totalPreferred -= spacing;
			}
			totalPreferred = Mathf.Max(totalMin, totalPreferred);
			SetLayoutInputForAxis(totalMin, totalPreferred, totalFlexible, axis);
        }

        protected void SetChildrenAlongAxis(int axis, bool isVertical)
		{
            RectTransform rect = gameObject.GetComponent<RectTransform>();
            rect.anchoredPosition = Vector2.zero;

            bool isForceExpand = (isVertical ? childForceExpandHeight : childForceExpandWidth);
            if (isForceExpand) {
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.one;
                rect.pivot = new Vector2(0.5f, 0.5f);
                rect.sizeDelta = Vector2.zero;
            }

            float size = rectTransform.rect.size[axis];

			bool alongOtherAxis = (isVertical ^ (axis == 1));
            float wh = (axis == 0 ? padding.horizontal : padding.vertical);

            float itemwh;

            if (alongOtherAxis)
			{
                float innerSize = size - (axis == 0 ? padding.horizontal : padding.vertical);
                itemwh = (axis == 1 ? itemWidth : itemHeight);
                float itemsize = (axis == 0 ? itemWidth : itemHeight);
                for (int i = 0; i < rectChildren.Count; i++)
				{
					RectTransform child = rectChildren[i];
                    float requiredSpace = 0;
  
                    float min = LayoutUtility.GetMinSize(child, axis);
                    float preferred = LayoutUtility.GetPreferredSize(child, axis);
                    float flexible = LayoutUtility.GetFlexibleSize(child, axis);

                    if ((axis == 0 ? childForceExpandWidth : childForceExpandHeight))
                        flexible = Mathf.Max(flexible, 1);

                    requiredSpace = Mathf.Clamp(innerSize, min, flexible > 0 ? size : preferred);
                    wh += itemwh + spacing;
                
                    float startOffset = GetStartOffset(axis, requiredSpace);
                    if ((axis == 1 ? childForceExpandHeight : childForceExpandWidth)) 
                        SetChildAlongAxis(child, axis, startOffset, innerSize);
                    else
                        SetChildAlongAxis(child, axis, startOffset, itemsize);
                }
			}
			else
			{
                itemwh = (axis == 0 ? itemWidth : itemHeight);

                float pos = (axis == 0 ? padding.left : padding.top);
				if (GetTotalFlexibleSize(axis) == 0 && GetTotalPreferredSize(axis) < size)
					pos = GetStartOffset(axis, GetTotalPreferredSize(axis) - (axis == 0 ? padding.horizontal : padding.vertical));

				float minMaxLerp = 0;
				if (GetTotalMinSize(axis) != GetTotalPreferredSize(axis))
					minMaxLerp = Mathf.Clamp01((size - GetTotalMinSize(axis)) / (GetTotalPreferredSize(axis) - GetTotalMinSize(axis)));

				float itemFlexibleMultiplier = 0;
				if (size > GetTotalPreferredSize(axis))
				{
					if (GetTotalFlexibleSize(axis) > 0)
						itemFlexibleMultiplier = (size - GetTotalPreferredSize(axis)) / GetTotalFlexibleSize(axis);
				}

                for (int i = 0; i < rectChildren.Count; i++)
				{
					RectTransform child = rectChildren[i];
                    float childSize = 0;
                    if ((axis == 1 ? childForceExpandHeight : childForceExpandWidth)) {
                        float min = LayoutUtility.GetMinSize(child, axis);
                        float preferred = LayoutUtility.GetPreferredSize(child, axis);
                        float flexible = LayoutUtility.GetFlexibleSize(child, axis);
                        flexible = Mathf.Max(flexible, 1);                        
                        childSize = Mathf.Lerp(min, preferred, minMaxLerp);
                        childSize += flexible * itemFlexibleMultiplier;
                    } else {
                        childSize += itemwh;
                        wh += childSize + spacing;
                    }
					
					SetChildAlongAxis(child, axis, pos, childSize);
					pos += childSize + spacing;   
                }

            }

            if (!isForceExpand) {
                if (!isVertical) {                    
                    rect.pivot = new Vector2(0f, 0.5f);
                    rect.sizeDelta = new Vector2(wh, 0);
                    rect.anchorMin = new Vector2(0f, 0f);
                    rect.anchorMax = new Vector2(0f, 1.0f);
                } else {                    
                    rect.pivot = new Vector2(0.5f, 1.0f);
                    rect.sizeDelta = new Vector2(0, wh);
                    rect.anchorMin = new Vector2(0, 1.0f);
                    rect.anchorMax = new Vector2(1.0f, 1.0f);
                }
            }
        }
	}
}
