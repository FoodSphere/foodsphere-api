namespace FoodSphere.Common.Service;

public enum ResultError
{
    NotFound,
    Argument,
    State,
    External,
    Forbidden,
    Authentication,
}

public readonly struct ResultNone;

public record ResultErrorObject(
    ResultError Error, string Message, object? Data = null);

public record ResultObject
{
    static readonly ResultObject _success = new();

    readonly List<ResultErrorObject> _errors = [];

    public ResultErrorObject[] Errors => [.. _errors];
    public bool IsSucceeded => _errors.Count is 0;
    public bool IsFailed => !IsSucceeded;
    public bool HasMultipleErrors => _errors.Count > 1;

    public static ResultNone None() => default;
    public static ResultObject Success() => _success;
    public static ResultObject<T> Success<T>(T value) => new(value);

    public static ResultErrorObject Fail(
        ResultError status, string message = "", object? data = null)
        => new(status, message, data);

    public static ResultErrorObject NotFound(IEntityKey key)
        => new(ResultError.NotFound, $"{key} was not found.", key);

    public ResultObject AddError(ResultErrorObject err)
    {
        _errors.Add(err);
        return this;
    }

    public ResultObject AddError(
        ResultError status, string message = "", object? data = null)
    {
        _errors.Add(new(status, message, data));
        return this;
    }

    public static implicit operator ResultObject(ResultErrorObject err)
    {
        var result = new ResultObject();
        result.AddError(err);

        return result;
    }

    public static implicit operator ResultObject(ResultErrorObject[] errors)
    {
        var result = new ResultObject();

        foreach (var err in errors)
            result.AddError(err);

        return result;
    }
}

public record ResultObject<T> : ResultObject
{
    readonly T _value;

    public T Value => IsSucceeded ? _value :
        throw new InvalidOperationException(
            "Cannot access Value of an error result.");

    public ResultObject(T value) { _value = value; }

    public bool TryGetValue(out T value)
    {
        value = _value;

        return IsSucceeded;
    }

    public static implicit operator ResultObject<T>(ResultNone _) => new(default!);
    public static implicit operator ResultObject<T>(T vaule) => new(vaule);
    public static implicit operator ResultObject<T>(ResultErrorObject err)
    {
        var result = new ResultObject<T>(default!);
        result.AddError(err);

        return result;
    }

    public static implicit operator ResultObject<T>(ResultErrorObject[] errors)
    {
        var result = new ResultObject<T>(default!);

        foreach (var err in errors)
            result.AddError(err);

        return result;
    }
}