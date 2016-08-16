using UnityEngine;
using System.Collections;

namespace UnityEngine.UI
{
	/// <summary>
	/// 场景淡入淡出 (将此脚本挂在UGUI Canvas 上) Scene fade in out.
	/// </summary>
	[AddComponentMenu("GameFrame/Scene/淡入淡出 SceneFadeInOut", 180)]
	public class SceneFadeInOut : MonoBehaviour	{
		public float fadeSpeed = 1.5f;

		private bool sceneStarting = true;
		private bool sceneEnding = false;

		private string nextSceneName;
		private float alpha;

		private GameObject Panel;
		private Image img;

		void Start() {
			Init ();
			SetColor (1);
		}

		void Update() {
			if (sceneStarting)
				StartScene();
			if (sceneEnding)
				EndScene();
		}

		// 创建一个Panel
		void Init() {
			if (Panel != null)
				return;
			Panel = UICommon.CreateUIElementRoot ("SceneFadeInOut", new Vector2 (Screen.width, Screen.height));
			Panel.AddComponent<CanvasRenderer> ();
			img = Panel.AddComponent<Image> ();
			img.color = new Color(0,0,0,1);
			img.fillCenter = true;
			img.raycastTarget = false;
			img.enabled = true;
			RectTransform R = Panel.GetComponent<RectTransform> ();
			R.anchorMin = new Vector2 (0, 0);
			R.anchorMax = new Vector2 (1, 1);
			R.pivot = new Vector2 (0.5f, 0.5f);
			Panel.transform.SetParent (transform);
			Panel.transform.localScale = new Vector3 (1, 1, 1);
			R.offsetMax = new Vector2 (0, 0);
			R.offsetMin = new Vector2 (0, 0);
		}

		void SetColor(float v) {
			alpha = v;
			img.color = new Color (0, 0, 0, alpha);
		}

		void FadeToClear() {
			SetColor(Mathf.Lerp(alpha, 0, fadeSpeed * Time.deltaTime));
		}

		void FadeToBlack() {
			SetColor(Mathf.Lerp(alpha, 1, fadeSpeed * Time.deltaTime));
		}

		void StartScene() {
			FadeToClear();
			if(Mathf.Abs(alpha) < 0.05f) {
				SetColor(0);
				sceneStarting  = false;
				img.enabled = false;
			}
		}

		void EndScene()	{
			FadeToBlack();
			if(Mathf.Abs(alpha) >= 0.95f) {
				Camera.main.enabled = false;
				img.enabled = false;
				sceneEnding = false;
				SceneManagement.SceneManager.LoadScene (nextSceneName);
			}
		}

		/// <summary>
		/// 进入场景
		/// </summary>
		/// <param name="name">场景名称.</param>
		public void LoadScene(string name) {
			if (name == null || name.Length == 0)
				return;
			SetColor (0);
			img.enabled = true;
			nextSceneName = name;
			sceneEnding = true;
			EndScene ();
		}
	}
}