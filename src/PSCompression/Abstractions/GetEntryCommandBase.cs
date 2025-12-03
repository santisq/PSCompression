using System.Linq;
using System.Management.Automation;
using System.IO;
using System.ComponentModel;
using System.Collections.Generic;
using System;
using PSCompression.Exceptions;
using ICSharpCode.SharpZipLib.Tar;
using ZstdSharp;

namespace PSCompression.Abstractions;

[EditorBrowsable(EditorBrowsableState.Never)]
public abstract class GetEntryCommandBase : CommandWithPathBase
{
    internal abstract ArchiveType ArchiveType { get; }

    [Parameter(
        ParameterSetName = "Stream",
        Position = 0,
        Mandatory = true,
        ValueFromPipeline = true,
        ValueFromPipelineByPropertyName = true)]
    [Alias("RawContentStream")]
    public Stream? InputStream { get; set; }

    private WildcardPattern[]? _includePatterns;

    private WildcardPattern[]? _excludePatterns;

    [Parameter]
    public EntryType? Type { get; set; }

    [Parameter]
    [SupportsWildcards]
    public string[]? Include { get; set; }

    [Parameter]
    [SupportsWildcards]
    public string[]? Exclude { get; set; }

    protected override void BeginProcessing()
    {
        if (Exclude is null && Include is null)
        {
            return;
        }

        const WildcardOptions Options = WildcardOptions.Compiled
            | WildcardOptions.CultureInvariant
            | WildcardOptions.IgnoreCase;

        if (Exclude is not null)
        {
            _excludePatterns = [.. Exclude.Select(e => new WildcardPattern(e, Options))];
        }

        if (Include is not null)
        {
            _includePatterns = [.. Include.Select(e => new WildcardPattern(e, Options))];
        }
    }

    protected override void ProcessRecord()
    {
        if (InputStream is not null)
        {
            HandleFromStream(InputStream);
            return;
        }

        foreach (string path in EnumerateResolvedPaths())
        {
            if (path.WriteErrorIfNotArchive(
                IsLiteral ? nameof(LiteralPath) : nameof(Path), this))
            {
                continue;
            }

            try
            {
                WriteObject(
                    GetEntriesFromFile(path).ToEntrySort(),
                    enumerateCollection: true);
            }
            catch (Exception _) when (_ is PipelineStoppedException or FlowControlException)
            {
                throw;
            }
            catch (Exception exception) when (IsInvalidArchive(exception))
            {
                ThrowTerminatingError(exception.ToInvalidArchive(ArchiveType));
            }
            catch (Exception exception)
            {
                WriteError(exception.ToOpenError(path));
            }
        }
    }

    private void HandleFromStream(Stream stream)
    {
        try
        {
            if (stream.CanSeek)
            {
                stream.Seek(0, SeekOrigin.Begin);
            }

            WriteObject(
                GetEntriesFromStream(stream).ToEntrySort(),
                enumerateCollection: true);
        }
        catch (Exception _) when (_ is PipelineStoppedException or FlowControlException)
        {
            throw;
        }
        catch (Exception exception) when (IsInvalidArchive(exception))
        {
            ThrowTerminatingError(exception.ToInvalidArchive(ArchiveType, isStream: true));
        }
        catch (Exception exception)
        {
            WriteError(exception.ToOpenError("InputStream"));
        }
    }

    protected abstract IEnumerable<EntryBase> GetEntriesFromFile(string path);

    protected abstract IEnumerable<EntryBase> GetEntriesFromStream(Stream stream);

    private static bool MatchAny(
        string name,
        WildcardPattern[] patterns)
    {
        foreach (WildcardPattern pattern in patterns)
        {
            if (pattern.IsMatch(name))
            {
                return true;
            }
        }

        return false;
    }

    protected bool ShouldInclude(string name)
    {
        if (_includePatterns is null)
        {
            return true;
        }

        return MatchAny(name, _includePatterns);
    }

    protected bool ShouldExclude(string name)
    {
        if (_excludePatterns is null)
        {
            return false;
        }

        return MatchAny(name, _excludePatterns);
    }

    protected bool ShouldSkipEntry(bool isDirectory) =>
        isDirectory && Type is EntryType.Archive || !isDirectory && Type is EntryType.Directory;

    private bool IsInvalidArchive(Exception exception) =>
        exception is InvalidDataException or TarException or ZstdException or IOException;
}
