using System;

namespace Core.ActionsResult
{
    public class Result<T>
    {
        public T Value { get; set; }

        public string[] Errors { get; set; }

        public bool IsSuccess => Errors == null || Errors?.Length == 0;

        private Result(T value, string[] errors)
        {
            Value = value;
            Errors = errors;
        }

        public static Result<T> Ok(T value)
        {
            return new Result<T>(value, null);
        }

        public static Result<T> Fail(T value, params string[] errors)
        {
            return new Result<T>(value, errors);
        }
    }
}