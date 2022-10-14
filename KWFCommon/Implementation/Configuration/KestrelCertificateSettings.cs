namespace KWFCommon.Implementation.Configuration
{
    using System.Security.Cryptography.X509Certificates;

    public sealed class KestrelCertificateSettings
    {
        public string? Path { get; set; }
        public string? StoreSubject { get; set; }
        public StoreName? StoreName { get; set; }
        public string? Password { get; set; }
        public string? Base64EncodedPemPublic { get; set; }
        public string? Base64EncodedPemPrivate { get; set; }
    }
}
