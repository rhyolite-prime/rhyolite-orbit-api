using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Abp.Domain.Services;
using Microsoft.Extensions.Configuration;

namespace RhyoliteOrbit.DomainServices.Dms.Security;

/// <summary>
/// Antivirus scanner that talks to a ClamAV daemon (clamd) over the INSTREAM protocol.
/// Configure with "Antivirus:Host" and "Antivirus:Port" (default 3310).
/// </summary>
public class ClamAvScanner: DomainService, IAntivirusScanner
{
    private readonly string _host;
    private readonly int _port;

    public ClamAvScanner(IConfiguration configuration)
    {
        _host = configuration["Antivirus:Host"] ?? "localhost";
        _port = configuration.GetValue<int?>("Antivirus:Port") ?? 3310;
    }

    public async Task<ScanVerdict> ScanAsync(Stream content, string fileName, CancellationToken cancellationToken = default)
    {
        try
        {
            using var client = new TcpClient();
            await client.ConnectAsync(_host, _port);

            using var stream = client.GetStream();
            stream.ReadTimeout = 30000;
            stream.WriteTimeout = 30000;

            // zINSTREAM (z = null terminated session)
            var command = Encoding.ASCII.GetBytes("zINSTREAM\0");
            await stream.WriteAsync(command, 0, command.Length, cancellationToken);

            if (content.CanSeek)
            {
                content.Position = 0;
            }

            var buffer = new byte[8192];
            int read;
            while ((read = await content.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) > 0)
            {
                var lengthPrefix = BitConverter.GetBytes(read);
                await stream.WriteAsync(lengthPrefix, 0, 4, cancellationToken);
                await stream.WriteAsync(buffer, 0, read, cancellationToken);
            }

            var terminator = BitConverter.GetBytes(0);
            await stream.WriteAsync(terminator, 0, 4, cancellationToken);
            await stream.FlushAsync(cancellationToken);

            using var reader = new StreamReader(stream, Encoding.ASCII);
            var response = await reader.ReadToEndAsync();

            Logger.Info($"ClamAV verdict for {fileName}: {response.Trim()}");

            if (response.IndexOf("FOUND", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return new ScanVerdict
                {
                    IsInfected = true,
                    ScannerAvailable = true,
                    Signature = response.Split(':').Length > 1 ? response.Split(':')[1].Trim() : "unknown"
                };
            }

            return ScanVerdict.Clean();
        }
        catch (Exception ex)
        {
            Logger.Warn($"ClamAV scan failed for {fileName}: {ex.Message}");
            return ScanVerdict.Unavailable();
        }
    }
}
