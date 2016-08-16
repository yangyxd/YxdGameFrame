using UnityEngine;
using System.Collections;

/// <summary>
/// 游戏架构
/// </summary>
namespace GameFrame {
	
	/// <summary>
	/// 消息ID常量, 建议消息以对应的常量为起始值
	/// </summary>
	public static class MsgConst {
		/// <summary>
		/// 系统消息 - 游戏暂停
		/// </summary>
		public const int GamePause = -2;
		/// <summary>
		/// 系统消息 - 游戏暂停后恢复运行
		/// </summary>
		public const int GameResume = -4;
		/// <summary>
		/// 系统消息 - 退出游戏
		/// </summary>
		public const int GameEnd = -1;
		/// <summary>
		/// 系统消息 - 释放游戏 (在GameEnd之后自动触发)
		/// </summary>
		public const int GameDestory = -3;

		/// <summary>
		/// UI 消息
		/// </summary>
		public const int UI = 0;

		/// <summary>
		/// UI 对话框消息
		/// </summary>
		public const int UI_Dialog = 99800;

		/// <summary>
		/// UI 拖放消息
		/// </summary>
		public const int UI_DropDrag = 99900;

		/// <summary>
		/// 资源消息
		/// </summary>
		public const int Res = 100000;

		/// <summary>
		/// 地图用到的消息
		/// </summary>
		public const int Res_Map = 110000;

		/// <summary>
		/// 音效消息
		/// </summary>
		public const int Audio = 200000;

		/// <summary>
		/// 摄像机消息
		/// </summary>
		public const int Carma = 300000;

		/// <summary>
		/// 网络消息
		/// </summary>
		public const int Net = 400000;



	}
}