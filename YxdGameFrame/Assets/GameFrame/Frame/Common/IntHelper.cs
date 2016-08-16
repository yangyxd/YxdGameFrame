using UnityEngine;
using System.Collections;
using System.IO;
using System;

/// <summary>
/// 游戏架构
/// </summary>
namespace GameFrame {

	/// <summary>
	/// int 类型助手
	/// </summary>
	public static class IntHelper {

		public static Int32 init(short high, short low) {
			return (Int32)(high << 16) + low;
		}

		public static short high(Int32 value) {
			return (short)(value >> 16);
		}

		public static short low(Int32 value) {
			return (short)(value & 0xffff);
		}

	}
}
