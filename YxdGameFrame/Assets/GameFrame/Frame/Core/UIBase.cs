using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

/// <summary>
/// 游戏架构
/// </summary>
namespace GameFrame {	

	/// <summary>
	/// UI 基类
	/// </summary>
	public abstract class UIBase : MonoBase, IDebugMessage {
		
		private Dictionary<string, GameObject> objs = new Dictionary<string, GameObject> ();

		/// <summary>
		/// 当前是否触摸在UI上
		/// </summary>
		public bool IsPointerUI {
			get {
				return EventSystem.current.IsPointerOverGameObject ();
			}
		}

		/// <summary>
		/// 查找对象
		/// </summary>
		public GameObject Find(string name) {
			if (objs.ContainsKey (name))
				return objs [name];
			GameObject obj = GameObject.Find (name);
			if (obj != null) {
				objs.Add (name, obj);
				return obj;
			} else
				return null;
		}

		/// <summary>
		/// 查找对象
		/// </summary>
		public T Find<T>(string name) {
			GameObject obj = Find (name);
			if (obj == null)
				return default(T);
			return obj.GetComponent<T> ();
		}

		/// <summary>
		/// 查找子对象 (不支持带路径)
		/// </summary>
		public GameObject FindChild(string name) {
			return FindChild (gameObject, name);
		}

		/// <summary>
		/// 查找子对象 (不支持带路径)
		/// </summary>
		public GameObject FindChild(GameObject parent, string name) {
			GameObject dest = null;
			List<GameObject> childs = new List<GameObject> ();
			childs.Add (parent);

			int i = 0;
			while (i < childs.Count) {
				GameObject obj = childs [i];
				for (int j = 0, max = obj.transform.childCount; j < max; j++) {
					GameObject c = obj.transform.GetChild (j).gameObject;
					if (c.name == name) {
						dest = c;
						break;
					} else						
						childs.Add (obj.transform.GetChild (j).gameObject);
				}
				if (dest == null)
					i++;
				else
					break;
			}

			return dest;
		}

		/// <summary>
		/// 查找父级对象 (父级可能有多个层次，查找到名称为 name 的层时返回)
		/// </summary>
		/// <returns>如果在所有父级中都没有找到与name相同的对象，则返回null.</returns>
		/// <param name="obj">Object.</param>
		/// <param name="name">Name.</param>
		public GameObject FindParent(string name) {
			return FindParent (gameObject, name);
		}

		/// <summary>
		/// 查找父级对象 (父级可能有多个层次，查找到名称为 name 的层时返回)
		/// </summary>
		/// <returns>如果在所有父级中都没有找到与name相同的对象，则返回null.</returns>
		/// <param name="obj">Object.</param>
		/// <param name="name">Name.</param>
		public GameObject FindParent(GameObject obj, string name) {
			while (obj.transform.parent != null) {
				obj = obj.transform.parent.gameObject;
				if (obj.name == name) {
					return obj;
				}
			}
			return null;
		}

		/// <summary>
		/// 在父级中查找包含指定组件的对象，返回找到的组件
		/// </summary>
		/// <returns>返回找到的组件.</returns>
		/// <param name="obj">Object.</param>
		public T FindComponentInParent<T>() {
			return FindComponentInParent<T> (gameObject);
		}

		/// <summary>
		/// 在指定的对象的父级中查找包含指定组件的对象，返回找到的组件
		/// </summary>
		/// <returns>返回找到的组件.</returns>
		/// <param name="obj">Object.</param>
		public T FindComponentInParent<T>(GameObject obj) {
			while (obj.transform.parent != null) {
				obj = obj.transform.parent.gameObject;
				T v = obj.GetComponent<T> ();
				if (v != null) 
					return v;
			}
			return default(T);
		}

		/// <summary>
		/// 隐藏指定的类
		/// </summary>
		/// <param name="name">Name.</param>
		/// <param name="isVisible">If set to <c>true</c> is visible.</param>
		public void SetVisible(string name, bool isVisible) {
			GameObject obj = Find (name);
			if (obj == null)
				return;
			obj.SetActive (isVisible);
		}

		/// <summary>
		/// 设置控件的Text属性
		/// </summary>
		public void SetText(string name, string text) {
			Text v = Find<Text> (name);
			if (v != null)
				v.text = text;
		}

		/// <summary>
		/// 设置控件的子控件的Text属性 (应用条件： 一个GameObject中有一个Text组件)
		/// </summary>
		public void SetChildText(string name, string text) {
			GameObject obj = Find (name);
			if (obj != null) {
				Text v = null;
				string key = "childText_" + name;
				if (objs.ContainsKey (key)) {
					v = objs [key].GetComponent<Text> ();
					if (v != null) {
						v.text = text;
						return;
					}
				}
				if (obj.transform.childCount == 1) {
					v = obj.transform.GetChild (0).GetComponent<Text>();
				} else {
					for (int i = 0, max = obj.transform.childCount; i < max; i++) {
						v = obj.transform.GetChild (i).GetComponent<Text> ();
						if (v != null)
							break;
					}
				}
				if (v != null) {
					v.text = text;
					objs.Add (key, v.gameObject);
				}
			}
		}

		/// <summary>
		/// 进入场景 (没有过场动画)
		/// </summary>
		/// <param name="name">场景名称.</param>
		public void LoadScene(string name) {
			if (name == null || name.Length == 0)
				return;
			this.LOG("LoadScene: " + name);
			try {
				SceneManager.LoadScene(name);
			} catch (System.Exception e) {
				this.LOGERROR ("LoadScene Error: " + e.Message);
			}
		}

	}


}