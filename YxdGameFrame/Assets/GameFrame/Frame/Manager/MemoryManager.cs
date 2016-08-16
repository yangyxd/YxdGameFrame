using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

/// <summary>
/// 游戏架构
/// </summary>
namespace GameFrame {
	
	/// <summary>
	/// 内存管理器，使用池化技术
	/// </summary>
	public class MemoryManager : SingletonEntire<MemoryManager>, IDebugMessage {

		List<object> structs = new List<object>();
		Dictionary<string, List<object>> structList = new Dictionary<string, List<object>>();

		/// <summary>
		/// 对象池大小
		/// </summary>
		public int PoolSize = 100;

		/// <summary>
		/// 创建一个本地结构
		/// </summary>
		/// <param name="className">类名</param>
		public object CreateNativeStruct(string className) {
			this.STARTMETHOD ("CreateNativeStruct");
			Type type = Type.GetType (className);
			if (structs.Count <= PoolSize) {
				if (!structList.ContainsKey (className))
					structList.Add (className, structs);
				else
					structs = structList [className];

				object obj = Activator.CreateInstance (type);
				structs.Add (obj);
				this.ENDMETHOD ("CreateNativeStruct");
				return obj;
			}
			throw new UnityException ("Try to create wrong struct.");
		}

		protected override void Awake() {
			base.Awake ();
		}

	}
}
