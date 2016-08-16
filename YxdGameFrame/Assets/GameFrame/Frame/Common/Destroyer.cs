using UnityEngine;
using System.Collections;

/// <summary>
/// 物体释放脚本
/// </summary>
public class Destroyer : MonoBehaviour {
    /// <summary>
    /// 在创建时就延地释放
    /// </summary>
    public bool destroyOnAwake;
    /// <summary>
    /// 创建时延时释放的时间
    /// </summary>
    public float awakeDestroyDelay;
    /// <summary>
    /// 是否查找名称为namedChild的子元素，并立即释放
    /// </summary>
    public bool findChild = false; 
    /// <summary>
    /// 在子元素中检测名称
    /// </summary>
    public string namedChild;

    void Awake() {
        if (destroyOnAwake) {
            // 在创建时就需要释放自己
            if (findChild) {
                Destroy(transform.Find(namedChild).gameObject);
            } else
                Destroy(gameObject, awakeDestroyDelay);
        }
    }

    /// <summary>
    /// 释放子游戏对象
    /// </summary>
    void DestroyChildGameObject() {
        // 释放指定的子元素对象，可能是从动画事件中调用
        if (transform.Find(namedChild).gameObject != null)
            Destroy(transform.Find(namedChild).gameObject);
    }

    void DisableChildGameObject() {
        // 禁用（隐藏）指定的子元素对象，可能是从动画事件中调用
        if (transform.Find(namedChild).gameObject.activeSelf == true)
            transform.Find(namedChild).gameObject.SetActive(false);
    }

    void DestroyGameObject() {
        // 释放自己的游戏对象，可能是从动画事件中调用
        Destroy(gameObject);
    }

}
