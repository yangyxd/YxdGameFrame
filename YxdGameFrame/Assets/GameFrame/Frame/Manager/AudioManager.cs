using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 游戏架构
/// </summary>
namespace GameFrame {

	/// <summary>
	/// 音效管理器
	/// </summary>
	[AddComponentMenu("GameFrame/MsgEngine 音效播放器", 50)]
	public class AudioManager : Singleton<AudioManager> {
		/// <summary>
		/// 音效播放器
		/// </summary>
		public AudioSource efxSource;
		/// <summary>
		/// 音效播放器
		/// </summary>
		public AudioSource audioSource;
		/// <summary>
		/// 背景音乐播放器
		/// </summary>
		public AudioSource bgSource;
	    /// <summary>
	    /// 随机播放音乐时的最小速度
	    /// </summary>
	    public float minPitch = 0.9f;
	    /// <summary>
	    /// 随机播放音乐时的最大速度
	    /// </summary>
	    public float maxPitch = 1.1f;
		/// <summary>
		/// 尝试从 StreamingAssets 中加载资源
		/// </summary>
		public bool tryLoadAssetBundle = true;
		/// <summary>
		/// 音效资源存放位置 (StreamingAssets目录下的子文件夹名称，不区分大小写）
		/// </summary>
		public string soundsPath = "Sounds/";

	    /// <summary>
	    /// 随机播放声音
	    /// </summary>
	    /// <param name="clips"></param>
	    public void RandomPlay(params AudioClip[] clips) {
			if (efxSource == null || clips == null || clips.Length == 0)
				return;
	        float pitch = Random.Range(minPitch, maxPitch);
	        int index = Random.Range(0, clips.Length); 
	        AudioClip clip = clips[index];
	        efxSource.clip = clip;
	        efxSource.pitch = pitch;
	        efxSource.Play();
	    }

		/// <summary>
		/// 播放音效
		/// </summary>
		public void Play(AudioClip clip) {
			if (clip == null || audioSource == null)
				return;
			audioSource.clip = clip;
			audioSource.Play();
		}

		private Dictionary<string, AudioClip> mClipMap = new Dictionary<string, AudioClip> ();
		// 将尝试加载过的音效文件名称加入此表中，不再进行加载尝试
		private Dictionary<string, bool> mClipName = new Dictionary<string, bool> ();

		/// <summary>
		/// 播放音效
		/// </summary>
		public void Play(string clipName) {
			if (audioSource == null || clipName == null || clipName.Length == 0)
				return;
			AudioClip clip = null;
			if (mClipMap.ContainsKey(clipName)) 
				clip = mClipMap [clipName];
			else {
				clip = Common.findRes<AudioClip> (clipName);
				if (clip != null)
					mClipMap.Add (clipName, clip);
			}

			// 尝试从 
			if (clip == null && tryLoadAssetBundle && !mClipName.ContainsKey(clipName)) {
				mClipName.Add (clipName, true);
				AssetBundleLoader.Instance.LoadResReturnWWW(soundsPath + clipName + ".wav", (www) => {
					if (www != null && www.audioClip != null) {
						mClipMap.Add (clipName, www.audioClip);
						Play(clipName);
					}
				});
				return;
			}

			if (clip == null) 
				return;
			audioSource.clip = clip;
			audioSource.Play();
		}

	    /// <summary>
	    /// 停止背景音乐
	    /// </summary>
	    public void StopBgMusic() {
			if (bgSource != null)
	        	bgSource.Stop();
	    }

	    /// <summary>
	    /// 开始背景音乐
	    /// </summary>
	    public void StartBgMusic() {
			if (bgSource != null)
				bgSource.Play();
	    }
	}
}
