using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _404NEL;

internal static class Acquire_0020Resolve_0020Validate_002EEvaluate
{
	internal static Thread Worker_0020Observe_0020Acquire_002EResolve(Initialize_0020Finalize_0020Process_002EEvaluate Observe_0020Synchronize_0020Evaluate_002EResolve, string Process_0020Initialize_0020Validate_002ESchedule, int Evaluate_0020Transform_0020Observe_002EFinalize, Action<string, string> Resolve_0020Validate_0020Observe_002EObserve)
	{
		Thread thread = new Thread(() =>
		{
			using CancellationTokenSource cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(Observe_0020Synchronize_0020Evaluate_002EResolve.ShutdownToken);
			cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(3L));
			try
			{
				if (Observe_0020Synchronize_0020Evaluate_002EResolve.ShutdownToken.IsCancellationRequested)
				{
					return;
				}
				Resolve_0020Validate_0020Observe_002EObserve("INFO", $"[Proxy.Upstream] 后台测试新上游：{Process_0020Initialize_0020Validate_002ESchedule}:{Evaluate_0020Transform_0020Observe_002EFinalize}");
				using (Observe_0020Synchronize_0020Evaluate_002EResolve.ConnectUpstreamAsync(Process_0020Initialize_0020Validate_002ESchedule, Evaluate_0020Transform_0020Observe_002EFinalize, cancellationTokenSource.Token).GetAwaiter().GetResult())
				{
					if (!cancellationTokenSource.IsCancellationRequested && Observe_0020Synchronize_0020Evaluate_002EResolve.Worker_0020Process_0020Initialize_002EAcquire(Process_0020Initialize_0020Validate_002ESchedule, Evaluate_0020Transform_0020Observe_002EFinalize))
					{
						Resolve_0020Validate_0020Observe_002EObserve("INFO", $"[Proxy.Upstream] TCP 测试通过，新连接使用：{Process_0020Initialize_0020Validate_002ESchedule}:{Evaluate_0020Transform_0020Observe_002EFinalize}");
					}
				}
			}
			catch (Exception ex)
			{
				if (!Observe_0020Synchronize_0020Evaluate_002EResolve.ShutdownToken.IsCancellationRequested)
				{
					Resolve_0020Validate_0020Observe_002EObserve("WARN", "[Proxy.Upstream] TCP 测试未通过，继续使用原上游：" + ex.Message);
				}
			}
		});
		thread.IsBackground = true;
		thread.Name = Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_020000C3.Transform_0020Validate_0020Process_002EEvaluate, Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_02000063.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_02000062.Evaluate_0020Resolve_0020Evaluate_002ETransform);
		thread.Start();
		return thread;
	}

	internal static async Task<TcpClient> ConnectThroughSocksAsync(string host, int port, string socksHost, int socksPort, string username, string password, CancellationToken cancellation)
	{
		TcpClient client = new TcpClient();
		try
		{
			await client.ConnectAsync(socksHost, socksPort, cancellation);
			NetworkStream stream = client.GetStream();
			byte[] userBytes = Encoding.UTF8.GetBytes(username);
			byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
			if (userBytes.Length > 255 || passwordBytes.Length > 255)
			{
				throw new IOException("SOCKS5 credentials exceed the protocol limit");
			}
			await stream.WriteAsync((userBytes.Length != 0) ? new byte[4] { 5, 2, 0, 2 } : new byte[3] { 5, 1, 0 }, cancellation);
			byte[] selection = new byte[2];
			await stream.ReadExactlyAsync(selection, cancellation);
			bool flag = selection[0] != 5;
			if (!flag)
			{
				byte b = selection[1];
				bool flag2 = ((b == 0 || b == 2) ? true : false);
				flag = !flag2;
			}
			if (flag)
			{
				throw new IOException("SOCKS5 negotiation failed");
			}
			if (selection[1] == 2)
			{
				if (userBytes.Length == 0)
				{
					throw new IOException("SOCKS5 requires authentication");
				}
				using MemoryStream authentication = new MemoryStream();
				authentication.WriteByte(1);
				authentication.WriteByte((byte)userBytes.Length);
				authentication.Write(userBytes);
				authentication.WriteByte((byte)passwordBytes.Length);
				authentication.Write(passwordBytes);
				await stream.WriteAsync(authentication.ToArray(), cancellation);
				await stream.ReadExactlyAsync(selection, cancellation);
				if (selection[0] != 1 || selection[1] != 0)
				{
					throw new IOException("SOCKS5 authentication failed");
				}
			}
			using MemoryStream request = new MemoryStream();
			request.Write(new byte[3] { 5, 1, 0 });
			if (IPAddress.TryParse(host, out IPAddress address))
			{
				request.WriteByte((byte)((address.AddressFamily == AddressFamily.InterNetwork) ? 1 : 4));
				request.Write(address.GetAddressBytes());
			}
			else
			{
				byte[] bytes = Encoding.ASCII.GetBytes(host);
				int num = bytes.Length;
				if ((num < 1 || num > 255) ? true : false)
				{
					throw new IOException("SOCKS5 hostname exceeds the protocol limit");
				}
				request.WriteByte(3);
				request.WriteByte((byte)bytes.Length);
				request.Write(bytes);
			}
			request.WriteByte((byte)(port >> 8));
			request.WriteByte((byte)port);
			await stream.WriteAsync(request.ToArray(), cancellation);
			byte[] reply = new byte[4];
			await stream.ReadExactlyAsync(reply, cancellation);
			if (reply[0] != 5 || reply[1] != 0 || reply[2] != 0)
			{
				throw new IOException("SOCKS5 upstream connection failed");
			}
			int num2 = reply[3] switch
			{
				1 => 4, 
				4 => 16, 
				3 => 0, 
				_ => throw new IOException("Invalid SOCKS5 address type"), 
			};
			if (reply[3] == 3)
			{
				byte[] length = new byte[1];
				await stream.ReadExactlyAsync(length, cancellation);
				num2 = length[0];
			}
			await stream.ReadExactlyAsync(new byte[num2 + 2], cancellation);
			return client;
		}
		catch
		{
			client.Dispose();
			throw;
		}
	}
}
