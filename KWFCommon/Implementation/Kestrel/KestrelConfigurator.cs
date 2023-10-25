namespace KWFCommon.Implementation.Kestrel
{
    using KWFCommon.Implementation.Configuration;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Server.Kestrel.Core;
    using Microsoft.AspNetCore.Server.Kestrel.Https;

    using System.Net;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;

    public static class KestrelConfigurator
    {
        public static ConfigureWebHostBuilder ConfigureKestrel(
            this ConfigureWebHostBuilder builder,
            KestrelConfiguration configuration,
            bool isDev = false)
        {
            builder.ConfigureKestrel(serverOptions =>
            {
                bool hasHttps = configuration.HasHttpsAvailable;
                if (isDev)
                {
                    if (configuration.HttpPort is not null) serverOptions.ListenLocalhost(configuration.HttpPort.Value);
                    if (hasHttps) serverOptions.ListenLocalhost(configuration.HttpsPort, l => l.ConfigureHttps());
                    return;
                }

                if (configuration.ListenIpAddresses is not null && configuration.ListenIpAddresses.Any())
                {
                    foreach (var listenIpAddress in configuration.ListenIpAddresses)
                    {
                        var ip = IPAddress.Parse(listenIpAddress);
                        if (configuration.HttpPort is not null) serverOptions.Listen(ip, configuration.HttpPort.Value);

                        if (hasHttps) 
                            serverOptions.Listen(
                                ip, 
                                configuration.HttpsPort,
                                l => l.ConfigureHttps(configuration.KestrelCertificateSettings));
                    }
                    return;
                }

                if (configuration.HttpPort is not null) serverOptions.Listen(IPAddress.Any, configuration.HttpPort.Value);

                if (hasHttps)
                    serverOptions.Listen(
                        IPAddress.Any, 
                        configuration.HttpsPort,
                        l => l.ConfigureHttps(configuration.KestrelCertificateSettings));

            });

            return builder;
        }

        private static void ConfigureHttps(
            this ListenOptions opt,
            KestrelCertificateSettings? kestrelCertificateSettings = null)
        {
            if (kestrelCertificateSettings is not null)
            {
                if (!string.IsNullOrEmpty(kestrelCertificateSettings.Path))
                {
                    opt.UseHttps(kestrelCertificateSettings.Path, kestrelCertificateSettings.Password);
                    return;
                }

                if (!string.IsNullOrEmpty(kestrelCertificateSettings.Base64EncodedPemPublic))
                {
                    if (!string.IsNullOrEmpty(kestrelCertificateSettings.Base64EncodedPemPrivate))
                    {
                        if (!string.IsNullOrEmpty(kestrelCertificateSettings.Password))
                        {
                            opt.UseHttps(
                                X509Certificate2.CreateFromEncryptedPem(
                                    kestrelCertificateSettings.Base64EncodedPemPublic.GetStringFromBase64(),
                                    kestrelCertificateSettings.Base64EncodedPemPrivate.GetStringFromBase64(),
                                    kestrelCertificateSettings.Password));
                            return;
                        }

                        opt.UseHttps(
                            X509Certificate2.CreateFromPem(
                                kestrelCertificateSettings.Base64EncodedPemPublic.GetStringFromBase64(), 
                                kestrelCertificateSettings.Base64EncodedPemPrivate.GetStringFromBase64()));
                        return;
                    }

                    opt.UseHttps(X509Certificate2.CreateFromPem(kestrelCertificateSettings.Base64EncodedPemPublic.GetStringFromBase64()));
                    return;
                }

                if (kestrelCertificateSettings.StoreName.HasValue && !string.IsNullOrEmpty(kestrelCertificateSettings.StoreSubject))
                {
                    opt.UseHttps(kestrelCertificateSettings.StoreName.Value, kestrelCertificateSettings.StoreSubject);
                    return;
                }
            }

            opt.UseHttps(o =>
            {
                o.AllowAnyClientCertificate();
                o.ClientCertificateMode = ClientCertificateMode.NoCertificate;
            });
        }

        private static string GetStringFromBase64(this string? encodedString)
        {
            if (string.IsNullOrEmpty(encodedString))
            {
                return string.Empty;
            }

            return Encoding.UTF8.GetString(Convert.FromBase64String(encodedString));
        }
    }
}
