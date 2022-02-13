namespace KWFCommon.Abstractions.Models
{
    public class PropertyValidationError
    {
        public PropertyValidationError(string parameter, string errorCode, string errorMessage)
        {
            Parameter = parameter;
            ErrorCode = errorCode;
            Message = errorMessage;
        }

        public string ErrorCode { get; set; }
        public string Parameter { get; set; }
        public string Message { get; set; }
    }
}
