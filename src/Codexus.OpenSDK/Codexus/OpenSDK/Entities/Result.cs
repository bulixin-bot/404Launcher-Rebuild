namespace Codexus.OpenSDK.Entities;

public class Result(bool isSuccess, string? error = null)
{
	public bool IsSuccess { get; } = isSuccess;

	public bool IsFailure => !IsSuccess;

	public string? Error { get; } = error;

	public static Result Success()
	{
		return new Result(isSuccess: true);
	}

	public static Result Failure(string error)
	{
		return new Result(isSuccess: false, error);
	}

	public static Result Clone(Result result)
	{
		return new Result(result.IsSuccess, result.Error);
	}
}
public class Result<T> : Result
{
	public T? Value { get; }

	private Result(T value)
		: base(isSuccess: true)
	{
		Value = value;
	}

	private Result(string error)
		: base(isSuccess: false, error)
	{
		Value = default;
	}

	public static Result<T> Success(T value)
	{
		return new Result<T>(value);
	}

	public new static Result<T> Failure(string error)
	{
		return new Result<T>(error);
	}
}
