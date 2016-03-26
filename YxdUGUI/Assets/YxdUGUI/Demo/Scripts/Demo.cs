﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Demo : MonoBehaviour {

	public void SetText (string v) {
		GetComponent<ButtonEx> ().text = v;
		Debug.Log ("SetText: " + v);
	}

	public void ShowTime() {
		Debug.Log (string.Format("currentTimeMillis: {0}, {1}, {2}", 
			Common.currentTimeMillis(), 
			System.DateTime.UtcNow.Millisecond,
			System.DateTime.Now.Millisecond
		));
		Debug.Log(string.Format("getDateTime: {0:u}", Common.getDateTime(Common.currentTimeMillis())));
		Debug.Log(string.Format("getDateTime: {0:u}", Common.getDateTimeUTC(Common.currentTimeMillis())));
	}

	/// 测试CurrentTimeMillis性能
	public void TestCurrentTimeMillis(int count) {
		long t = Common.currentTimeMillis ();
		for (int i = 0; i < count; i++) {
			long v = Common.currentTimeMillis ();
		}
		string msg = string.Format ("{0:D}", Common.currentTimeMillis () - t);
		Debug.Log(msg);
		GameObject.Find ("Text").GetComponent<Text> ().text = msg;
	}

	/// 测试CurrentTimeMillis性能
	public void TestGetTicks(int count) {
		long t = Common.currentTimeMillis ();
		for (int i = 0; i < count; i++) {
			long v = Common.getTicks ();
		}
		Debug.Log(string.Format ("{0:D}, {1:D}", Common.currentTimeMillis () - t, Common.getTicks ()));

	}
}
