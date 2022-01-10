namespace KWFAuthentication.Implementation.Configuration
{
    public sealed class BearerConfiguration
    {
        private bool _tokenValidateLife = true;
        public string? TokenIdentifier { get; set; } = "Authorization";
        public string? TokenKey { get; set; }
        public bool TokenValidateLife
        {
            get
            {
                return _tokenValidateLife;
            }

            set
            {
                _tokenValidateLife = value;
            }
        }
        public string[]? TokenValidIssuers { get; set; }
        public string[]? TokenValidAudiences { get; set; }

        public bool HasIssuer => TokenValidIssuers is not null && TokenValidIssuers?.Length > 0;
        public bool HasAudience => TokenValidAudiences is not  null && TokenValidAudiences?.Length > 0;

        private bool HasMultipleIssuers => HasIssuer && TokenValidIssuers?.Length > 1;
        private bool HasMultipleAudiences => HasAudience && TokenValidAudiences?.Length > 1;

        public string? GetSingleIssuer => HasIssuer && !HasMultipleIssuers ? TokenValidIssuers?[0] : null;
        public string? GetSingleAudience => HasAudience && !HasMultipleAudiences ? TokenValidAudiences?[0] : null;
        public string[]? GetMultipleIssuers => HasMultipleIssuers ? TokenValidIssuers : null;
        public string[]? GetMultipleAudiences => HasMultipleAudiences ? TokenValidAudiences : null;
    }
}
