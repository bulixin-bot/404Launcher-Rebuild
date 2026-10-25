using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Downloader;

public class TaskStateManagement
{
	private readonly ConcurrentQueue<Exception> _exceptions = new ConcurrentQueue<Exception>();

	protected readonly ILogger Logger;

	public AggregateException Exception { get; private set; }

	public bool IsCompleted
	{
		get
		{
			if (Status != TaskStatus.RanToCompletion && Status != TaskStatus.Faulted)
			{
				return Status == TaskStatus.Canceled;
			}
			return true;
		}
	}

	public bool IsCanceled => Status == TaskStatus.Canceled;

	public bool IsCompletedSuccessfully => Status == TaskStatus.RanToCompletion;

	public bool IsFaulted => Status == TaskStatus.Faulted;

	public TaskStatus Status { get; private set; }

	public TaskStateManagement(ILogger logger = null)
	{
		Logger = logger;
	}

	internal void StartState()
	{
		Status = TaskStatus.Running;
	}

	internal void CompleteState()
	{
		Status = TaskStatus.RanToCompletion;
	}

	internal void CancelState()
	{
		Status = TaskStatus.Canceled;
	}

	internal void SetException(Exception exp, [CallerMemberName] string callerName = null)
	{
		Logger?.LogCritical(exp, "TaskStateManagement: SetException catch an exception on " + callerName);
		Status = TaskStatus.Faulted;
		_exceptions.Enqueue(exp);
		Exception = new AggregateException(_exceptions);
	}
}
