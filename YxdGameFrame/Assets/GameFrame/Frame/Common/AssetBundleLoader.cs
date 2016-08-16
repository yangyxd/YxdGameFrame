using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// 游戏架构
/// </summary>
namespace GameFrame {
	
	/// <summary>
	/// 资源加载器 (加截使用AssetBuilder打包的文件)
	/// </summary>
	public class AssetBundleLoader : SingletonEntire<AssetBundleLoader>, IDebugMessage {
	    const string assetTail = ".unity3d";

	    /// <summary>
	    /// 加载目标资源
	    /// </summary>
	    /// <param name="name"></param>
	    /// <param name="callback"></param>
	    public void LoadAssetBundle(string name, Action<UnityEngine.Object> callback) {
	        name = name + assetTail; // eg: ui/panel.unity3d

	        Action<List<AssetBundle>> action = (depenceAssetBundles) => {

	            string realName = Common.getRuntimePlatform() + "/" + name;//eg:Windows/ui/panel.unity3d

	            LoadResReturnWWW(realName, (www) => {
					string assetName = Common.getFileName(realName, false);	                
					UnityEngine.Object obj = null;
					if (www == null || www.assetBundle == null) {
						this.LOG("Load Failed. " + www.error);
					} else {
						AssetBundle assetBundle = www.assetBundle;
	                  	obj = assetBundle.LoadAsset(assetName);//LoadAsset(name）,这个name没有后缀,eg:panel

		                //卸载资源内存
		                assetBundle.Unload(false);
		                for (int i = 0; i < depenceAssetBundles.Count; i++) {
		                    depenceAssetBundles[i].Unload(false);
		                }
					}

	                //加载目标资源完成的回调
	                callback(obj);
	            });

	        };

	        LoadDependenceAssets(name, action);
	    }

	    /// <summary>
	    /// 加载目标资源的依赖资源
	    /// </summary>
	    /// <param name="targetAssetName"></param>
	    /// <param name="action"></param>
	    private void LoadDependenceAssets(string targetAssetName, Action<List<AssetBundle>> action) {
			this.LOG("Load Assets: " + targetAssetName);//ui/panel.unity3d
	        Action<AssetBundleManifest> dependenceAction = (manifest) => {
	            List<AssetBundle> depenceAssetBundles = new List<AssetBundle>();//用来存放加载出来的依赖资源的AssetBundle

	            string[] dependences = manifest.GetAllDependencies(targetAssetName);
				this.LOG("Dependence Files: " + dependences.Length);
	            int length = dependences.Length;
	            int finishedCount = 0;
	            if (length == 0) {
	                //没有依赖
	                action(depenceAssetBundles);
	            } else {
	                //有依赖，加载所有依赖资源
	                for (int i = 0; i < length; i++) {
	                    string dependenceAssetName = dependences[i];
	                    dependenceAssetName = Common.getRuntimePlatform() + "/" + dependenceAssetName;//eg:Windows/altas/heroiconatlas.unity3d

	                    //加载，加到assetpool
	                    LoadResReturnWWW(dependenceAssetName, (www) => {
	                        int index = dependenceAssetName.LastIndexOf("/");
	                        string assetName = dependenceAssetName.Substring(index + 1);
	                        assetName = assetName.Replace(assetTail, "");
	                        AssetBundle assetBundle = www.assetBundle;
	                        //UnityEngine.Object obj = 
							assetBundle.LoadAsset(assetName);
	                        //assetBundle.Unload(false);
	                        depenceAssetBundles.Add(assetBundle);

	                        finishedCount++;

	                        if (finishedCount == length) {
	                            //依赖都加载完了
	                            action(depenceAssetBundles);
	                        }
	                    });
	                }
	            }
	        };
	        LoadAssetBundleManifest(dependenceAction);
	    }

	    /// <summary>
	    /// 加载AssetBundleManifest
	    /// </summary>
	    /// <param name="action"></param>
	    private void LoadAssetBundleManifest(Action<AssetBundleManifest> action) {
	        string manifestName = Common.getRuntimePlatform();
	        manifestName = manifestName + "/" + manifestName;//eg:Windows/Windows
	        LoadResReturnWWW(manifestName, (www) => {
	            AssetBundle assetBundle = www.assetBundle;
	            if (assetBundle != null) {
	                UnityEngine.Object obj = assetBundle.LoadAsset("AssetBundleManifest");
	                assetBundle.Unload(false);
	                AssetBundleManifest manif = obj as AssetBundleManifest;
	                action(manif);
				} else
					this.LOG("Load Failed. " + www.error);
	        });
	    }

	    #region ExcuteLoader
	    public void LoadResReturnWWW(string name, Action<WWW> callback) {
	        //string path = "file://" + this.m_assetPath + "/" + name;
			string path = Common.streamingAssetsURL + name;
			this.LOG("Load: " + path);
			Coroutine ct = StartCoroutine(LoaderRes(path, callback));
			if (ct == null)
				this.LOG("Load Fialed: " + path);
	    }

	    IEnumerator LoaderRes(string path, Action<WWW> callback) {
	        WWW www = new WWW(path);
	        yield return www;
			if (www.isDone) {
				callback (www);
			}
	    }
	    #endregion

	}
}
