﻿using System;
using System.IO;
using System.Management.Automation;
using PSCompression.Extensions;
using PSCompression.Exceptions;

namespace PSCompression.Commands;

[Cmdlet(VerbsData.Expand, "ZipEntry")]
[OutputType(typeof(FileSystemInfo), ParameterSetName = new[] { "PassThru" })]
public sealed class ExpandZipEntryCommand : PSCmdlet, IDisposable
{
    private readonly ZipArchiveCache _cache = new();

    [Parameter(Mandatory = true, ValueFromPipeline = true)]
    public ZipEntryBase[] InputObject { get; set; } = null!;

    [Parameter(Position = 0)]
    [ValidateNotNullOrEmpty]
    public string? Destination { get; set; }

    [Parameter]
    public SwitchParameter Force { get; set; }

    [Parameter(ParameterSetName = "PassThru")]
    public SwitchParameter PassThru { get; set; }

    protected override void BeginProcessing()
    {
        Destination ??= SessionState.Path.CurrentFileSystemLocation.Path;

        try
        {
            Destination = Destination.NormalizePath(isLiteral: true, this);

            if (File.Exists(Destination))
            {
                ThrowTerminatingError(ExceptionHelpers.NotDirectoryPathError(
                    Destination,
                    nameof(Destination)));
            }
        }
        catch (Exception exception)
        {
            ThrowTerminatingError(exception.ToResolvePathError(Destination));
        }
    }

    protected override void ProcessRecord()
    {
        Dbg.Assert(Destination is not null);

        foreach (ZipEntryBase entry in InputObject)
        {
            try
            {
                (string path, bool isfile) = entry.ExtractTo(
                    _cache.GetOrAdd(entry),
                    Destination,
                    Force.IsPresent);

                if (PassThru.IsPresent)
                {
                    if (isfile)
                    {
                        WriteObject(new FileInfo(path));
                        return;
                    }

                    WriteObject(new DirectoryInfo(path));
                }
            }
            catch (Exception exception)
            {
                WriteError(exception.ToExtractEntryError(entry));
            }
        }
    }

    public void Dispose() => _cache?.Dispose();
}
