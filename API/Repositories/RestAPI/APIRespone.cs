namespace API.Repositories.RestAPI
{
    public class APIRespone<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
    }
}
