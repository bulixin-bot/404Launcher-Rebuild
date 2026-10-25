using System;

namespace Downloader;

public class Chunk
{
	public string Id { get; set; }

	public long Start { get; set; }

	public long End { get; set; }

	public long Position { get; set; }

	public int MaxTryAgainOnFailure { get; set; }

	public int Timeout { get; set; }

	public int FailureCount { get; private set; }

	public long Length => End - Start + 1;

	public long EmptyLength
	{
		get
		{
			if (Length <= 0)
			{
				return long.MaxValue;
			}
			return Length - Position;
		}
	}

	public bool CanWrite
	{
		get
		{
			if (Length > 0)
			{
				return Start + Position < End;
			}
			return true;
		}
	}

	public Chunk()
	{
		Timeout = 1000;
		Id = Guid.NewGuid().ToString("N");
	}

	public Chunk(long start, long end)
		: this()
	{
		Start = start;
		End = end;
	}

	public bool CanTryAgainOnFailure()
	{
		return FailureCount++ < MaxTryAgainOnFailure;
	}

	public void Clear()
	{
		Position = 0L;
		FailureCount = 0;
	}

	public bool IsDownloadCompleted()
	{
		bool flag = Length > 0;
		bool flag2 = Start + Position >= End;
		return flag & flag2;
	}

	public bool IsValidPosition()
	{
		if (Length != 0L)
		{
			if (Position >= 0)
			{
				return Position <= Length;
			}
			return false;
		}
		return true;
	}
}
