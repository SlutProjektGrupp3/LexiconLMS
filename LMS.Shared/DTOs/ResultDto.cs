
namespace LMS.Shared.DTOs;
public class ResultDto
{
    public bool Succeeded { get; set; }
    public List<ErrorDto> Errors { get; set; } = new List<ErrorDto>();

    public static ResultDto Success => new ResultDto { Succeeded = true };

    public static ResultDto Failed(ErrorDto error)
    {
        return new ResultDto
        {
            Succeeded = false,
            Errors = new List<ErrorDto> { error }
        };
    }

    public static ResultDto Failed(List<ErrorDto> errors)
    {
        return new ResultDto
        {
            Succeeded = false,
            Errors = errors
        };
    }
}
public class ResultDto<T> : ResultDto
{
    public T? Data { get; set; }

    public static ResultDto<T> Success(T data) =>
        new ResultDto<T> { Succeeded = true, Data = data };

    public static new ResultDto<T> Failed(ErrorDto error) =>
        new ResultDto<T> { Succeeded = false, Errors = new List<ErrorDto> { error } };

    public static new ResultDto<T> Failed(List<ErrorDto> errors) =>
        new ResultDto<T> { Succeeded = false, Errors = errors };
}
