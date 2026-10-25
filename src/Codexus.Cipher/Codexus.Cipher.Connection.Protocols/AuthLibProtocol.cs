using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Codexus.Development.SDK.Entities;
using Codexus.Development.SDK.Manager;
using Serilog;

namespace Codexus.Cipher.Connection.Protocols;

public class AuthLibProtocol(IPAddress address, int port, string modList, string version, string accessToken) : IDisposable
{
	private readonly CancellationTokenSource _cts = new CancellationTokenSource();

	private TcpListener? _listener;

	private Task? _acceptLoopTask;

	private bool _disposed;

	~AuthLibProtocol()
	{
		Dispose(disposing: false);
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (_disposed)
		{
			return;
		}
		if (disposing)
		{
			_cts.Cancel();
			_listener?.Stop();
			try
			{
				_acceptLoopTask?.Wait(TimeSpan.FromSeconds(5L));
			}
			catch (Exception ex)
			{
				Log.Error("Authentication failed. {Message}", ex.Message);
			}
			_cts.Dispose();
		}
		_disposed = true;
	}

	public void Start()
	{
		if (_disposed)
		{
			throw new ObjectDisposedException("AuthLibProtocol");
		}
		_listener = new TcpListener(address, port);
		_listener.Start();
		_acceptLoopTask = AcceptLoopAsync(_cts.Token);
	}

	public void Stop()
	{
		if (!_disposed)
		{
			Dispose();
		}
	}

	private async Task AcceptLoopAsync(CancellationToken token)
	{
		while (!token.IsCancellationRequested && !_disposed)
		{
			try
			{
				HandleClientAsync(await _listener.AcceptTcpClientAsync(token).ConfigureAwait(continueOnCapturedContext: false), token);
			}
			catch (ObjectDisposedException)
			{
				break;
			}
			catch (Exception ex2)
			{
				Log.Warning("Accept loop error: {Message}", ex2.Message);
				break;
			}
		}
	}

	private static async Task ReadExactAsync(NetworkStream stream, byte[] buffer, int offset, int count, CancellationToken token)
	{
		int num;
		for (int read = 0; read < count; read += num)
		{
			num = await stream.ReadAsync(buffer.AsMemory(offset + read, count - read), token).ConfigureAwait(continueOnCapturedContext: false);
			if (num == 0)
			{
				throw new EndOfStreamException();
			}
		}
	}

	private async Task HandleClientAsync(TcpClient client, CancellationToken token)
	{
		using (client)
		{
			await using NetworkStream stream = client.GetStream();
			uint responseCode = 1u;
			try
			{
				_ = 6;
				try
				{
					byte[] lenBuf = new byte[4];
					await ReadExactAsync(stream, lenBuf, 0, 4, token).ConfigureAwait(continueOnCapturedContext: false);
					int num = BitConverter.ToInt32(lenBuf, 0);
					byte[] gameIdBuf = new byte[num];
					await ReadExactAsync(stream, gameIdBuf, 0, num, token).ConfigureAwait(continueOnCapturedContext: false);
					string gameId = Encoding.UTF8.GetString(gameIdBuf);
					await ReadExactAsync(stream, lenBuf, 0, 4, token).ConfigureAwait(continueOnCapturedContext: false);
					int num2 = BitConverter.ToInt32(lenBuf, 0);
					byte[] userIdBuf = new byte[num2];
					await ReadExactAsync(stream, userIdBuf, 0, num2, token).ConfigureAwait(continueOnCapturedContext: false);
					string userId = Encoding.UTF8.GetString(userIdBuf);
					await ReadExactAsync(stream, lenBuf, 0, 4, token).ConfigureAwait(continueOnCapturedContext: false);
					int num3 = BitConverter.ToInt32(lenBuf, 0);
					byte[] certBuf = new byte[num3];
					await ReadExactAsync(stream, certBuf, 0, num3, token).ConfigureAwait(continueOnCapturedContext: false);
					string text = Encoding.Unicode.GetString(certBuf);
					EntityAvailableUser entityAvailableUser = IUserManager.Instance?.GetAvailableUser(userId);
					if (entityAvailableUser == null)
					{
						throw new Exception("User not found");
					}
					if (!string.IsNullOrEmpty(text))
					{
						await NetEaseConnection.CreateAuthenticatorAsync(text, gameId, version, modList, accessToken, int.Parse(userId), entityAvailableUser.AccessToken, "45.253.165.190", NetEaseConnection.RandomAuthPort(), () =>
						{
							responseCode = 0u;
						}).ConfigureAwait(continueOnCapturedContext: false);
					}
				}
				catch (Exception ex)
				{
					Log.Warning("Client handling error: {Message}", ex.Message);
				}
			}
			finally
			{
				try
				{
					byte[] bytes = BitConverter.GetBytes(responseCode);
					await stream.WriteAsync(bytes, token).ConfigureAwait(continueOnCapturedContext: false);
				}
				catch (Exception ex2)
				{
					Log.Warning("Response writing error: {Message}", ex2.Message);
				}
			}
		}
	}
}
