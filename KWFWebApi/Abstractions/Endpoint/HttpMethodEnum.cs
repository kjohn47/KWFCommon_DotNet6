namespace KWFWebApi.Abstractions.Endpoint
{
    public enum HttpMethodEnum
    {
        Get,
        Post,
        Put,
        Delete
    }

    public static class HttpMethodExtensions
    {
        public static string GetMethodName(this HttpMethodEnum httpMethodEnum)
        {
            return httpMethodEnum switch
            {
                HttpMethodEnum.Get => nameof(HttpMethodEnum.Get),
                HttpMethodEnum.Post => nameof(HttpMethodEnum.Post),
                HttpMethodEnum.Put => nameof(HttpMethodEnum.Put),
                HttpMethodEnum.Delete => nameof(HttpMethodEnum.Delete),
                _ => string.Empty,
            };
        }
    }
}
