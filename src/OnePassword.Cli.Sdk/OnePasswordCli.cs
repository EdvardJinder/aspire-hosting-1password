using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace OnePassword.Cli.Sdk;
internal sealed class OnePasswordCli
{
    private readonly string _cliPath;
    public OnePasswordCli(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException("CLI path must be provided", nameof(filePath));
        }

        _cliPath = filePath;
    }
    internal Task<int> GetFieldValue(string? accountId, string vaultId, string itemId, string field, TextWriter? outputWriter = null, TextWriter? errorWriter = null, ILogger? logger = default, CancellationToken cancellationToken = default)
    {
        List<string> args = ["read", $"op://{vaultId}/{itemId}/{field}"];
        if (accountId is not null)
        {
            args.Add("--account");
            args.Add(accountId);
        }
        return RunAsync([.. args], outputWriter, errorWriter, logger, cancellationToken);
    }

    private Task<int> RunAsync(string[] args, TextWriter? outputWriter = null, TextWriter? errorWriter = null, ILogger? logger = default, CancellationToken cancellationToken = default)
            => RunAsync(args, outputWriter, errorWriter, useShellExecute: false, logger, cancellationToken);
    private Task<int> RunAsync(string[] args, TextWriter? outputWriter = null, TextWriter? errorWriter = null, bool useShellExecute = false, ILogger? logger = default, CancellationToken cancellationToken = default)
    {
        return RunAsync((isError, line) =>
        {
            if (isError)
            {
                errorWriter?.WriteLine(line);
            }
            else
            {
                outputWriter?.WriteLine(line);
            }
        }, args, useShellExecute, logger, cancellationToken);
    }
    private async Task<int> RunAsync(Action<bool, string> onOutput, string[] args, bool useShellExecute = false, ILogger? logger = default, CancellationToken cancellationToken = default)
    {
        using var process = new Process
        {
            StartInfo = BuildStartInfo(args, useShellExecute),
            EnableRaisingEvents = true
        };

        var stdoutTask = Task.CompletedTask;
        var stderrTask = Task.CompletedTask;

        logger?.LogTrace("Invoking 1Password CLI{ShellExecuteInfo}: {FileName} {Arguments}", useShellExecute ? " (UseShellExecute=true)" : "", process.StartInfo.FileName, EscapeArgList(process.StartInfo.ArgumentList));

        try
        {
            if (!process.Start())
            {
                throw new InvalidOperationException("Failed to start 1Password process.");
            }

            if (!useShellExecute)
            {
                stdoutTask = PumpAsync(process.StandardOutput, line => onOutput(false, line), cancellationToken);
                stderrTask = PumpAsync(process.StandardError, line => onOutput(true, line), cancellationToken);
            }

            using var ctr = cancellationToken.Register(() =>
            {
                try
                {
                    if (!process.HasExited)
                    {
                        logger?.LogTrace("Cancellation requested, killing 1Password process tree.");
                        process.Kill(entireProcessTree: true);
                    }
                }
                catch
                {
                    // ignored
                }
            });

            await process.WaitForExitAsync(cancellationToken).ConfigureAwait(false);

            logger?.LogTrace("1Password CLI exited with exit code '{ExitCode}'.", process.ExitCode);
            return process.ExitCode;
        }
        finally
        {
            if (!useShellExecute)
            {
                try
                {
                    logger?.LogTrace("1Password CLI exited, draining stdout/stderr.");
                    await Task.WhenAll(stdoutTask, stderrTask).ConfigureAwait(false);
                }
                catch
                {
                    // ignored
                }
            }
        }
    }
    private static string? EscapeArgList(Collection<string> args)
    {
        if (args.Count == 0)
        {
            return null;
        }

        StringBuilder? sb = null;

        foreach (var a in args)
        {
            if (string.IsNullOrWhiteSpace(a))
            {
                continue;
            }

            sb ??= new StringBuilder(args.Count * 2);
            if (sb.Length > 0)
            {
                sb.Append(' ');
            }

            var needsQuotes = a.Any(char.IsWhiteSpace) || a.Contains('"');
            if (needsQuotes)
            {
                sb.Append('"');
            }

            foreach (var c in a)
            {
                if (c == '"')
                {
                    sb.Append('\\'); // Escape quote
                }
                sb.Append(c);
            }

            if (needsQuotes)
            {
                sb.Append('"');
            }
        }

        return sb?.ToString();
    }
    private ProcessStartInfo BuildStartInfo(IEnumerable<string> args, bool useShellExecute = false)
    {
        var psi = new ProcessStartInfo
        {
            FileName = _cliPath,
            UseShellExecute = useShellExecute,
            RedirectStandardOutput = !useShellExecute,
            RedirectStandardError = !useShellExecute,
            RedirectStandardInput = false,
            CreateNoWindow = true,
            WindowStyle = ProcessWindowStyle.Hidden
        };

        // Prefer ArgumentList to avoid quoting issues.
        foreach (var a in args)
        {
            if (string.IsNullOrWhiteSpace(a))
            {
                continue;
            }

            psi.ArgumentList.Add(a);
        }

        // Ensure consistent encoding on Windows terminals
        if (!useShellExecute && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            psi.StandardOutputEncoding = Encoding.UTF8;
            psi.StandardErrorEncoding = Encoding.UTF8;
        }

        return psi;
    }
    private static async Task PumpAsync(StreamReader reader, Action<string> onLine, CancellationToken cancellationToken = default)
    {
        while (!reader.EndOfStream && !cancellationToken.IsCancellationRequested)
        {
            var line = await reader.ReadLineAsync(cancellationToken).ConfigureAwait(false);
            if (line is null)
            {
                break;
            }
            if (line.Length > 0)
            {
                onLine(line);
            }
        }
    }

    
}