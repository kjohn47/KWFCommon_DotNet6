namespace KWFCommon.Implementation.Configuration
{
    public sealed class KestrelConfiguration
    {
        public int? HttpPort { get; set; }
        public int HttpsPort { get; set; }
        public IEnumerable<string>? ListenIpAddresses { get; set; }
        public KestrelCertificateSettings? KestrelCertificateSettings { get; set; }
    }
}
