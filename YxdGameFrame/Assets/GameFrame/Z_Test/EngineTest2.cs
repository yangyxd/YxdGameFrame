using UnityEngine;
using System.Collections;
using GameFrame;

public class EngineTest2 : UIBase, IDebugMessage {

	public override void ProcessMsg (IMsgPack msg) {
		this.LOG (msg.Sender.GetType().Name + ": MsgID = " + msg.MsgID);
	}

	void Start() {
		DoStart ();
	}

	void OnGUI() {
		if (GUI.Button (new Rect (450, 200, 200, 30), "反注册消息ID: 888, 123, 456")) {
			UnRegMsg (123);
			UnRegMsg (456);
			UnRegMsg (888);
		}
		if (GUI.Button (new Rect (450, 240, 200, 30), "停止接受消息"))
			UnRegAllMsg ();
		if (GUI.Button (new Rect (450, 280, 200, 30), "开始接受消息"))
			DoStart ();
	}

	void DoStart() {
		RegMsg (123);
		RegMsg (456);
		RegMsg (new int[]{9,8,6,5,999,888});
	}
}
