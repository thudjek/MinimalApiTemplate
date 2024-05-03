using Microsoft.AspNetCore.Mvc;

namespace MinimalApiTemplate.REST.Common;

public class Result
{
    protected Result(bool isSuccess)
    {
        IsSuccess = isSuccess;
    }

    protected Result(bool isSuccess, string successMessage)
    {
        IsSuccess = isSuccess;
        SuccessMessage = successMessage;
    }

    protected Result(string errorMessage)
    {
        IsSuccess = false;
        ErrorMessage = errorMessage;
    }

    public bool IsSuccess { get; protected set; }
    public string SuccessMessage { get; protected set; }
    public string ErrorMessage { get; protected set; }

    public static Result Success()
    {
        return new Result(true);
    }

    public static Result Success(string successMessage)
    {
        return new Result(true, successMessage);
    }

    public static Result Fail(string errorMessage)
    {
        return new Result(errorMessage);
    }

    public ProblemDetails ToProblemDetails(int statusCode = StatusCodes.Status400BadRequest)
    {
        return new ProblemDetails() 
        {
            Status = statusCode,
            Detail = ErrorMessage
        };
    }
}

public class Result<T> : Result where T : class
{
    private Result(bool isSuccess, string successMessage) : base(isSuccess, successMessage)
    {
    }

    private Result(T value) : base(true)
    {
        Value = value;
    }

    private Result(T value, string successMessage) : base(true, successMessage)
    {
        Value = value;
    }

    private Result(string errorMessage) : base(errorMessage)
    {
    }

    public T Value { get; private set; }

    public new static Result<T> Success(string successMessage)
    {
        return new Result<T>(true, successMessage);
    }

    public static Result<T> Success(T value)
    {
        return new Result<T>(value);
    }

    public static Result<T> Success(T value, string successMessage)
    {
        return new Result<T>(value, successMessage);
    }

    public new static Result<T> Fail(string errorMessage)
    {
        return new Result<T>(errorMessage);
    }
}