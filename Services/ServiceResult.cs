namespace Challenge_1_Build_APIS.Services
{
    public enum ServiceStatus
    {
        Success,
        NotFound,
        Conflict
    }

    public class ServiceResult<T>
    {
        public ServiceStatus Status { get; private init; }
        public T? Data { get; private init; }
        public string? Field { get; private init; }
        public string? Error { get; private init; }
       
    

        public bool IsSuccess => Status == ServiceStatus.Success;

        public static ServiceResult<T> Ok(T data) =>
            new() { Status = ServiceStatus.Success, Data =  data };

        public static ServiceResult<T> NotFound() =>
            new() { Status = ServiceStatus.NotFound, };

        public static ServiceResult<T> Conflict(string field, string error) =>
            new() { Status = ServiceStatus.Conflict, Field = field, Error = error };

    }
}
