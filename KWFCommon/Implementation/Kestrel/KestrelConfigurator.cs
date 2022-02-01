namespace KWFCommon.Implementation.Kestrel
{
    using KWFCommon.Implementation.Configuration;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Server.Kestrel.Core;
    using Microsoft.AspNetCore.Server.Kestrel.Https;

    using System.Net;

    public static class KestrelConfigurator
    {
        public static ConfigureWebHostBuilder ConfigureKestrel(
            this ConfigureWebHostBuilder builder,
            KestrelConfiguration configuration,
            bool isDev = false)
        {
            builder.ConfigureKestrel(serverOptions =>
            {
                if (isDev)
                {
                    if (configuration.HttpPort is not null) serverOptions.ListenLocalhost(configuration.HttpPort.Value);

                    serverOptions.ListenLocalhost(configuration.HttpsPort, l => l.ConfigureHttps());
                }
                else
                {
                    if (configuration.ListenIpAddresses is not null && configuration.ListenIpAddresses.Any())
                    {
                        foreach (var listenIpAddress in configuration.ListenIpAddresses)
                        {
                            var ip = IPAddress.Parse(listenIpAddress);
                            if (configuration.HttpPort is not null) serverOptions.Listen(ip, configuration.HttpPort.Value);

                            serverOptions.Listen(
                                ip, 
                                configuration.HttpsPort,
                                l => l.ConfigureHttps(configuration.KestrelCertificateSettings));
                        }
                    }
                    else
                    {
                        if (configuration.HttpPort is not null) serverOptions.Listen(IPAddress.Any, configuration.HttpPort.Value);

                        serverOptions.Listen(
                            IPAddress.Any, 
                            configuration.HttpsPort,
                            l => l.ConfigureHttps(configuration.KestrelCertificateSettings));
                    }
                }

            });

            return builder;
        }

        private static void ConfigureHttps(
            this ListenOptions opt,
            KestrelCertificateSettings? kestrelCertificateSettings = null)
        {
            if (kestrelCertificateSettings?.Path is not null)
            {
                opt.UseHttps(kestrelCertificateSettings.Path, kestrelCertificateSettings.Password);
            }
            else
            {
                opt.UseHttps(o =>
                {
                    o.AllowAnyClientCertificate();
                    o.ClientCertificateMode = ClientCertificateMode.NoCertificate;
                });
            }
        }
    }
}
