using System;
using System.IO.Compression;

namespace PSCompression;

internal class ZipContentWriter : ZipContentOpsBase
{
    internal ZipContentWriter(string source) : base(source, ZipArchiveMode.Update)
    { }


    public void Dispose(bool disposing)
    {
        throw new NotImplementedException();
    }
}
