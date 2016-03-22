using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Demo : MonoBehaviour {

	public void SetText (string v) {
		GetComponent<ButtonEx> ().text = v;
		Debug.Log ("SetText: " + v);
	}

}
