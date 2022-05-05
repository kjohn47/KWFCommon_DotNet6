namespace KWFWebApi.Implementation.Query
{
    using KWFWebApi.Abstractions.Query;

    internal class FileQueryResponse : IFileQueryResponse
    {
        private FileQueryResponse(byte[] fileBytes, string mimeType, string? fileName)
        {
            FileBytes = fileBytes;
            MimeType = mimeType;
            FileName = fileName?? string.Empty;
            HasFileName = !string.IsNullOrEmpty(fileName);
        }

        public string MimeType { get; init; }

        public byte[] FileBytes { get; init; }

        public string FileName { get; init; }

        public bool HasFileName { get; init; }

        public FileQueryResponse Initialize(byte[] fileBytes, string mimeType)
        {
            return new FileQueryResponse(fileBytes, mimeType, null);
        }

        public FileQueryResponse Initialize(byte[] fileBytes, string mimeType, string fileName)
        {
            return new FileQueryResponse(fileBytes, mimeType, fileName);
        }
    }
}
