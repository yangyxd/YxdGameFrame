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
	/// TCP 客户端
	/// </summary>
	public class TCPClient {
		private const string E_NOLink = "连接还未建立，不能发送数据";
		private const string E_InitFail = "Socket 创建失败!";
		private const string E_InvalidConnParam = "无效的连接参数";

		// 设置一次接收数据的大小, 如果不设置, 默认为64k
		private const int receiveMaxSize = 64 * 1024; 

		/// <summary>
		/// 最大接收数据的大小（小于1时使用默认值64k)
		/// </summary>
		public int MaxRecvDataSize = 0;

		private string m_Host;
		private int m_Port;
		private IPEndPoint ipe;
		private int m_RecvTimeOut;

		protected Socket so;

		/// <summary>
		/// 远程地址
		/// </summary>
		public string Host {
			get { return m_Host; }
			set { m_Host = value; ipe = null; }
		}

		/// <summary>
		/// 远程端口
		/// </summary>
		public int Port {
			get { return m_Port; }
			set { m_Port = value; ipe = null; }
		}

		/// <summary>
		/// 是否已经连接
		/// </summary>
		public bool Connected {
			get { return (so != null && so.Connected); }
		}

		/// <summary>
		/// 接收超时
		/// </summary>
		public int RecvTimeOut {
			get { return m_RecvTimeOut; }
			set { m_RecvTimeOut = value; }
		}

		public TCPClient () {}

		public TCPClient (string host, int port) {
			m_Host = host;
			m_Port = port;
		}

		~TCPClient() {
			Close ();
		}

		protected virtual Socket createSocket() {
			return new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		}

		/// <summary>
		/// 连接到主机
		/// </summary>
		public bool Connect() {
			if (ipe == null) {
				if (string.IsNullOrEmpty (m_Host) || m_Port <= 0) {
					throw new Exception (E_InvalidConnParam);
				}
				ipe = new IPEndPoint(IPAddress.Parse(m_Host), m_Port);
			}
			if (so == null) {
				so = createSocket ();
				if (so == null)
					throw new Exception (E_InitFail);
			}
			so.Connect (ipe);
			return so.Connected;
		}

		/// <summary>
		/// 关闭连接
		/// </summary>
		public void Close() {
			if (Connected)
				so.Close ();
		}

		/// <summary>
		/// 连接到主机
		/// </summary>
		public bool Connect(string host, int port) {
			m_Host = host;
			m_Port = port;
			ipe = null;
			return Connect ();
		}

		/// <summary>
		/// 发送数据，返回成功发送的字节数
		/// </summary>
		public int Send(string data) {
			if (!Connected)
				throw new Exception (E_NOLink);
			byte[] buf = Encoding.ASCII.GetBytes (data);
			return so.Send (buf);
		}

		/// <summary>
		/// 发送数据，返回成功发送的字节数
		/// </summary>
		public int Send(byte[] buffer) {
			if (!Connected)
				throw new Exception (E_NOLink);
			return so.Send (buffer);
		}

		/// <summary>
		/// 发送数据，返回成功发送的字节数
		/// </summary>
		public int Send(byte[] buffer, int offset, int size) {
			if (!Connected)
				throw new Exception (E_NOLink);
			return so.Send (buffer, offset, size, SocketFlags.None);
		}

		/// <summary>
		/// 发送数据，返回成功发送的字节数
		/// </summary>
		public int Send(byte[] buffer, int offset) {
			if (!Connected)
				throw new Exception (E_NOLink);
			int size = buffer.Length - offset;
			if (size > 0)
				return so.Send (buffer, offset, size, SocketFlags.None);
			else
				return 0;
		}

		/// <summary>
		/// 发送数据，返回成功发送的字节数
		/// </summary>
		public int Send(char[] chars, int index = 0, int count = -1) {
			if (!Connected)
				throw new Exception (E_NOLink);
			if (chars == null || chars.Length == 0)
				return 0;
			byte[] buf;
			if (count == -1)
				buf = Encoding.ASCII.GetBytes (chars);
			else
				buf = Encoding.ASCII.GetBytes (chars, index, count);
			return so.Send (buf);
		}

		/// <summary>
		/// 接收数据 (原始接口)
		/// </summary>
		public int Receive(byte[] buffer, int len, SocketFlags socketFlags = SocketFlags.None) {
			if (!Connected)
				throw new Exception (E_NOLink);
			return so.Receive (buffer, len, socketFlags);
		}

		/// <summary>
		/// 接收数据
		/// </summary>
		public byte[] Receive() {
			return Receive (receiveMaxSize, RecvTimeOut);
		}

		/// <summary>
		/// 接收数据
		/// </summary>
		public byte[] Receive(int len, int timeOut) {
			if (!Connected)
				throw new Exception (E_NOLink);
			long starttime = Common.Ticks;
			byte[] bufIn = new byte[len];

			if (MaxRecvDataSize > receiveMaxSize)
				so.ReceiveTimeout = 2000;
			else
				so.ReceiveTimeout = 1000;

			int bytesLen = 0;
			int totalCount = 0;
			int j = 0;
			int k = 0;
			int maxK = timeOut / 2 / 100;

			while (totalCount < len) {
				try {
					k++;
					bytesLen = so.Receive(bufIn, totalCount, len - totalCount, SocketFlags.None);
					if (bytesLen == 0) {
						j++;
					} else if (bytesLen < 0) {
						break;
					} else {
						totalCount += bytesLen;
						if (totalCount == len)
							break;
						j = 0;
					}
				} catch (Exception e) {
					j++; 
					Debug.Log(e.Message);
				}

				if ((totalCount == 0) && (k > maxK)) break; 
				if (((j > 2) && (totalCount > 0)) || (starttime - Common.Ticks >= timeOut) || (!Connected))
					break;
			}

			if (totalCount > 0) {
				byte[] stores = new byte[totalCount];
				System.Buffer.BlockCopy (bufIn, 0, stores, 0, totalCount);
				return stores;
			} else 
				return null;
		}

		/// <summary>
		/// 接收字符串
		/// </summary>
		public string Receive(Encoding charset, int len = -1) {
			byte[] receiveData = Receive();
			if (receiveData == null || receiveData.Length == 0)
				return null;
			return charset.GetString (receiveData);
		}

	}
}

