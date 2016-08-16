using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

/// <summary>
/// 游戏架构
/// </summary>
namespace GameFrame
{
	/// <summary>
	/// UDP 客户端
	/// </summary>
	public class UDPClient : System.Net.Sockets.UdpClient {
		private IPEndPoint ipe;

		public UDPClient() : base() {}

		public UDPClient(string host, int port) : base() {
			SetRemoteAddr (host, port);
		}

		public void SetRemoteAddr(string host, int port) {
			ipe = new IPEndPoint (IPAddress.Parse(host), port);
		}

		protected byte[] getBytes(string data) {
			#if UNITY_EDITOR || UNITY_STANDALONE
			return Encoding.GetEncoding("GB2312").GetBytes (data);
			#else
			return Encoding.Convert(Encoding.UTF8, Encoding.GetEncoding("GB2312"), Encoding.UTF8.GetBytes (data));
			#endif
		}

		/// <summary>
		/// 发送数据，返回成功发送的字节数
		/// </summary>
		public int Send(string data) {
			byte[] buf = getBytes(data);
			return Send (buf, buf.Length, ipe);
		}

		/// <summary>
		/// 发送数据，返回成功发送的字节数
		/// </summary>
		public int Send(string data, string host, int port) {
			byte[] buf = getBytes(data);
			return Send (buf, buf.Length, host, port);
		}

		/// <summary>
		/// 发送数据，返回成功发送的字节数
		/// </summary>
		public int Send(string data, IPEndPoint ipe) {
			byte[] buf = getBytes(data);
			return Send (buf, buf.Length, ipe);
		}
	}
}

