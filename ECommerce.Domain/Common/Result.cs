namespace ECommerce.Domain.Common;

public class Result
{
    public bool IsSuccess { get; }
    public Error? Error { get; }

    public bool IsFailure => !IsSuccess;

    protected Result(bool isSuccess, Error? error = null)
    {
        if (isSuccess && error is not null)
            throw new InvalidOperationException("Success Result cannot have an error");

        if (!isSuccess && error is null)
            throw new InvalidOperationException("Failure Result must have an error");

        IsSuccess = isSuccess;
        Error = error;
    }
    public static Result Success()
        => new Result(true);

    public static Result Failure(Error error)
        => new Result(false, error);


    public override string ToString()
    {
        return IsSuccess
            ? "Success"
            : $"Failure: {Error!.Code} - {Error.Message}";
    }
}

public sealed class Result<TValue> : Result
{
    private readonly TValue? _value;

    protected internal Result(TValue? value, bool isSuccess, Error? error)
        : base(isSuccess, error)
    {
        _value = value;
    }
        


    public static Result<TValue> Success(TValue value)
        => new Result<TValue>(value, true, null);

    public static new Result<TValue> Failure(Error error)
        => new Result<TValue>(default, false, error);

    public TResult Match<TResult>(
    Func<TValue, TResult> onSuccess,
    Func<Error, TResult> onFailure)
    {
        return IsSuccess
            ? onSuccess(_value!)
            : onFailure(Error!);
    }
    public override string ToString()
    {
        return IsSuccess
            ? $"Success: {_value}"
            : $"Failure: {Error!.Code} - {Error.Message}";
    }
}
