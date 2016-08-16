using UnityEngine;
using System.Collections;
using GameFrame;

public class EngineTestSend : UIBase {

	public override void ProcessMsg (IMsgPack msg) {}

	public int SendCount = 100000;
	public bool RandomMsg = true;

	void OnGUI() {
		if (GUI.Button (new Rect (200, 50, 100, 30), "测试")) {
			if (RandomMsg) {
				for (int i = 0; i < SendCount; i++) {
					SendMsg (new MsgBase (Random.Range (MsgConst.UI, 1100)));
				}
			} else {
				for (int i = 0; i < SendCount; i++) {
					SendMsg (new MsgBase (i & 2));
				}
			}
		}
		if (GUI.Button (new Rect (200, 90, 100, 30), "测试 888")) {
			SendMsg (new MsgBase (888));
		}
	}
}
