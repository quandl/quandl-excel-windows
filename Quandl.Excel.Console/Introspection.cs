using System;
using System.IO;
using System.Reflection;

namespace Quandl.Excel.Console
{
    // http://apichange.codeplex.com/SourceControl/changeset/view/76c98b8c7311#ApiChange.Api/src/Introspection/CorFlagsReader.cs
    // Provides basic information about an Assembly derived from its metadata.

    public class Introspection
    {
        public static PEFormat GetPEFormat(string fileName)
        {
            if (String.IsNullOrEmpty(fileName))
            {
                throw new ArgumentException("file name was null or empty");
            }

            using (var fStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                return GetPEFormat(fStream);
            }
        }

        public static PEFormat GetPEFormat(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            long length = stream.Length;
            if (length < 0x40)
                return PEFormat.UNKNOWN;

            BinaryReader reader = new BinaryReader(stream);

            // Read the pointer to the PE header.
            stream.Position = 0x3c;
            uint peHeaderPtr = reader.ReadUInt32();
            if (peHeaderPtr == 0)
                peHeaderPtr = 0x80;

            if (peHeaderPtr > length - 256)
                return PEFormat.UNKNOWN;

            // Check the PE signature.  Should equal 'PE\0\0'.
            stream.Position = peHeaderPtr;
            uint peSignature = reader.ReadUInt32();
            if (peSignature != 0x00004550)
                return PEFormat.UNKNOWN;

            // Read PE header stream point
            for (int i = 0; i < 5; i++)
            {
                reader.ReadUInt32();
            }

            // Read PE magic number from Standard Fields to determine format.
            PEFormat p = (PEFormat)reader.ReadUInt16();
            return p;
        }

        public enum PEFormat : ushort
        {
            PE32 = 0x10b,
            PE32Plus = 0x20b,
            UNKNOWN = 0x000
        }
    }
}