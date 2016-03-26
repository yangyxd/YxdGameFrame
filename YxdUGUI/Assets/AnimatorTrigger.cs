using UnityEngine;
using System.Collections;

public class AnimatorTrigger : MonoBehaviour {

    Animator ani;

    void Awake() {
        GameObject obj = GameObject.Find("TextView");
        ani = obj.GetComponent<Animator>();
    }

	public void Trigger(string name) {
        ani.SetTrigger(name);
    }
}
