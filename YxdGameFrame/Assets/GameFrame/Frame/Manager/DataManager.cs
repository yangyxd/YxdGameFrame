using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System;

/// <summary>
/// 游戏架构
/// </summary>
namespace GameFrame {
	
	/// <summary>
	/// 数据管理器 (会自动加载 CSVDataFile)
	/// </summary>
	public class DataManager<T> : SingletonEntire<T>, IDebugMessage where T : Component {
	    /// <summary>
	    /// 已经加载的资源队列
	    /// </summary>
		protected Dictionary<string, UnityEngine.Object> ObjectPool = new Dictionary<string, UnityEngine.Object>();

		/// <summary>
		/// 是否加载完成
		/// </summary>
		[HideInInspector]
		public bool Inited = false;
		/// <summary>
		/// 是否正在加载
		/// </summary>
		protected bool Initing = false;

	    void Start() {
			DoLoadData ();
		}

	    /// <summary>
	    /// 当前对像池大小
	    /// </summary>
	    public int PoolSize {
	        get { return ObjectPool.Count; }
	    }

	    /// <summary>
	    /// 清空资源池
	    /// </summary>
	    public void clearPool() {
	        ObjectPool.Clear();
	    }

	    /// <summary>
	    /// 根据名称获取对象
	    /// </summary>
		public UnityEngine.Object getObject(string name) {
	        return ObjectPool[name];
	    }

		/// <summary>
		/// 加载数据
		/// </summary>
		protected virtual void DoLoadData() {}

	    /// <summary>
		/// 加载单个资源 (注意： 资源路径和名称都要区分大小写)
	    /// </summary>
	    /// <param name="needLoadAssetName">要加载的资源名称</param>
	    public void Load(string needLoadAssetName, System.Action<UnityEngine.Object> callback) {
	        Load(needLoadAssetName, true, callback);
	    }

	    /// <summary>
		/// 加载单个资源 (注意： 资源路径和名称都要区分大小写)
	    /// </summary>
	    /// <param name="needLoadAssetName">要加载的资源名称</param>
	    /// <param name="isReLoad">是否强制重新加载</param>
	    public void Load(string needLoadAssetName, bool isReLoad = true, System.Action<UnityEngine.Object> callback = null) {
	        if (!isReLoad) {
	            if (ObjectPool.ContainsKey(needLoadAssetName)) return;
	        }
	        AssetBundleLoader.Instance.LoadAssetBundle(needLoadAssetName, (obj) => {
				UnityEngine.Object go = null;
				if (obj != null) {
					go = UnityEngine.Object.Instantiate(obj) as UnityEngine.Object;
		            int index = needLoadAssetName.LastIndexOf("/");
		            string assetName = needLoadAssetName.Substring(index + 1);

		            //加载出来的GameObject放到GameObjectPool存储
		            ObjectPool.Add(assetName, go);
				}
	            if (callback != null)
	                callback(go);
	        });
	    }

	    /// <summary>
		/// 加载指定队列的资源 (注意： 资源路径和名称都要区分大小写)
	    /// </summary>
	    /// <param name="needLoadQueue">要加载的队列</param>
	    public void Load(Queue<string> needLoadQueue, System.Action<UnityEngine.Object> callback) {
	        Load(needLoadQueue, true, callback);
	    }

	    /// <summary>
		/// 加载指定队列的资源 (注意： 资源路径和名称都要区分大小写)
	    /// </summary>
	    /// <param name="needLoadQueue">要加载的队列</param>
	    public void Load(Queue<string> needLoadQueue, bool isReLoad = true, System.Action<UnityEngine.Object> callback = null) {
	        if (needLoadQueue.Count > 0) {
	            string needLoadAssetName = needLoadQueue.Dequeue();
	            if (!isReLoad) {
	                if (ObjectPool.ContainsKey(needLoadAssetName)) {
	                    Load(needLoadQueue, isReLoad);
	                    return;
	                }
	            }
	            AssetBundleLoader.Instance.LoadAssetBundle(needLoadAssetName, (obj) => {
	                GameObject go = GameObject.Instantiate(obj) as GameObject;
	                int index = needLoadAssetName.LastIndexOf("/");
	                string assetName = needLoadAssetName.Substring(index + 1);

	                //加载出来的GameObject放到GameObjectPool存储
	                ObjectPool.Add(assetName, go);

	                if (callback != null)
	                    callback(go);

	                // 加载下一个
	                Load(needLoadQueue, isReLoad, callback);
	            });
	        } else {
	            Debug.Log("all finished"); 
	        }
	    }

		/// <summary>
		/// 加载资源，先从Resources中加载，加载失败时，尝试从Assets中加载
		/// </summary>
		public void LoadRes<TT>(string resName, Action<TT> callback) where TT: UnityEngine.Object {
			LoadRes<TT> (resName, default(TT), callback);
		}

		/// <summary>
		/// 加载资源，先从Resources中加载，加载失败时，尝试从Assets中加载
		/// </summary>
		public void LoadRes<TT>(string resName, TT defaultValue, Action<TT> callback, bool tryAsset = true) where TT: UnityEngine.Object {
			if (callback == null || string.IsNullOrEmpty(resName))
				return;
			if (ObjectPool.ContainsKey (resName)) {
				callback (ObjectPool [resName] as TT);
				return;
			}
			try {
				TT v = Resources.Load<TT> (Common.removeFileExt(resName));
				// 尝试从Assets中加载资源
				if (v == null && tryAsset) {
					if (Common.getFileExt(resName).Length == 0) // 如果不存在扩展名，则默认是.png格式
						resName = resName + ".png";
					AssetBundleLoader.Instance.LoadResReturnWWW(resName, (www)=> {
						string assetName = Common.getFileName(resName, false);
						if (www.isDone) {
							TT obj = defaultValue;
							if (typeof(TT) == (typeof(Texture)) || (www.texture != null && typeof(TT).IsInstanceOfType(www.texture)))
								obj = www.texture as TT;
							else if (www.size > 0) {
								AssetBundle asset = AssetBundle.LoadFromMemory(www.bytes);
								if (asset != null) {
									obj = asset.LoadAsset<TT>(assetName);
								}
							}
							if (obj != null) {
								
								//加载出来的GameObject放到GameObjectPool存储
								ObjectPool.Add(resName, obj);

								callback(obj);
								return;
							}
						} else {
							this.LOG("LoadRes Failed. " + www.error);
						}
						callback(defaultValue);
					});
					return;
				} else 
					callback(v);
			} catch (Exception e) {
				this.LOG ("LoadRes Error. " + e.Message);
				callback(defaultValue);
			}
		}

		/// <summary>
		/// 从CSVReader中初始化数据
		/// </summary>
		public void ReadCsv(object t, CsvReader reader) {
			foreach (System.Reflection.FieldInfo p in t.GetType().GetFields()) {
				string key = p.Name.ToLower ();
				if (p.FieldType == typeof(int))
					p.SetValue (t, reader.GetInt (key, 0));
				else if (p.FieldType == typeof(float))
					p.SetValue (t, reader.GetFloat (key, 0));
				else if (p.FieldType == typeof(string))
					p.SetValue (t, reader.GetString (key));
			} 
		}

	}
}
