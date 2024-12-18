// See https://aka.ms/new-console-template for more information
#pragma warning disable CA1416

using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

ServiceProvider serviceProvider = new ServiceCollection()
    .AddLogging((loggingBuilder) => loggingBuilder
        .SetMinimumLevel(LogLevel.Trace)
        .AddSimpleConsole(opts => {
            opts.SingleLine = true;
            opts.IncludeScopes = false;
            opts.TimestampFormat = "hh:mm:ss ";
        })
    )
    .BuildServiceProvider();

var logger = serviceProvider.GetService<ILoggerFactory>()!.CreateLogger<Program>();

Regex startLoadMatcher = new(@"^\d{4}/\d{2}/\d{2} \d{2}:\d{2}:\d{2} \d+ [a-fA-F0-9]+ \[INFO Client \d+\] \[SHADER\] Delay: OFF$", RegexOptions.Compiled);
Regex endLoadMatcher = new(@"^\d{4}/\d{2}/\d{2} \d{2}:\d{2}:\d{2} \d+ [a-fA-F0-9]+ \[INFO Client \d+\] \[SHADER\] Delay: ON", RegexOptions.Compiled);

var fallbackGamePath = @"C:\Program Files (x86)\Grinding Gear Games\Path of Exile 2"; 
var cancellationSource = new CancellationTokenSource();
Console.CancelKeyPress += (_, _) => cancellationSource.Cancel();

// We'll use the PoE process to get the right client.txt path across standalone / Steam.
logger.LogInformation("Waiting for Path of Exile process to launch before doing anything.");
var proc = await WaitForExecutableToLaunch();

string? gameDirectory = Path.GetDirectoryName(proc?.MainModule?.FileName);
if (string.IsNullOrEmpty(gameDirectory)) {
    logger.LogError("Couldn't detect game directory. Falling back to {fallbackGamePath}.", fallbackGamePath);
    gameDirectory = fallbackGamePath;
}

string clientTxtLocation = Path.Combine(gameDirectory, "logs", "client.txt");

await using var logStream = new FileStream(clientTxtLocation, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
logStream.Seek(0, SeekOrigin.End);

using var reader = new StreamReader(logStream);

logger.LogInformation("Reading client data from {clientTxtLocation}", clientTxtLocation);

while (!cancellationSource.IsCancellationRequested) {
    var line = await reader.ReadLineAsync();
    if (String.IsNullOrWhiteSpace(line) || line.Length > 256) {
        await Task.Delay(20);
        continue;
    }

    if (startLoadMatcher.IsMatch(line)) {
        ParkCores();
    } else if (endLoadMatcher.IsMatch(line)) {
        ResumeCores();
    }
}

async Task<Process?> WaitForExecutableToLaunch() {
    while (!cancellationSource.IsCancellationRequested) {
        var proc = GetPathOfExileProcess();
        if (proc is { HasExited: false }) {
            return proc;
        }
        
        await Task.Delay(1000);
    }

    return null;
}

void ParkCores() {
    var affinityBits = new StringBuilder(new string('1', Environment.ProcessorCount)) {
        [^1] = '0',
        [^2] = '0'
    };

    IntPtr affinity = new IntPtr(Convert.ToInt32(affinityBits.ToString(), 2));
    
    var proc = GetPathOfExileProcess();
    if (proc is { HasExited: false }) {
        proc.ProcessorAffinity = affinity;
        logger.LogInformation("Parked cores: {affinityBits}", affinityBits);
    } else {
        logger.LogError("Detected loading screen, but could not find any process to park.");
    }
}

void ResumeCores() {
    var affinityBits = new StringBuilder(new string('1', Environment.ProcessorCount));

    IntPtr affinity = new IntPtr(Convert.ToInt32(affinityBits.ToString(), 2));
    
    var proc = GetPathOfExileProcess();
    if (proc is { HasExited: false }) {
        proc.ProcessorAffinity = affinity;
        logger.LogInformation("Unparked cores: {affinityBits}", affinityBits);
    } else {
        logger.LogError("Detected loading screen, but could not find any process to unpark.");
    }
}

Process? GetPathOfExileProcess() {
    Func<Process, bool> isPoE = (c => c.MainWindowTitle.Equals("Path of Exile 2", StringComparison.InvariantCultureIgnoreCase) && c.ProcessName.Contains("PathOfExile"));
    return Process.GetProcesses().FirstOrDefault(isPoE);
}