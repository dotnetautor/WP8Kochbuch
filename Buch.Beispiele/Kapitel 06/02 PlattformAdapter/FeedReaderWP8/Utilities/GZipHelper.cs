using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Resources;

namespace TwitterWP8.Utilities {
  public static class GZipHelper {
    public static Stream Decompress(this Stream stream) {
     
      ﻿// Copyright (C) Microsoft Corporation. All Rights Reserved.
      // This code released under the terms of the Microsoft Public License
      // (Ms-PL, http://opensource.org/licenses/ms-pl.html).

      // Read header
      if ((0x1f != stream.ReadByte()) || // ID1
          (0x8b != stream.ReadByte()) || // ID2
          (8 != stream.ReadByte()))    // CM (8 == deflate)
                {
        throw new NotSupportedException("Compressed data not in the expected format.");
      }

      // Read flags
      var flg = stream.ReadByte(); // FLG
      var fhcrc = 0 != (0x2 & flg); // CRC16 present before compressed data
      var fextra = 0 != (0x4 & flg); // extra fields present
      var fname = 0 != (0x8 & flg); // original file name present
      var fcomment = 0 != (0x10 & flg); // file comment present

      // Skip unsupported fields
      stream.ReadByte(); stream.ReadByte(); stream.ReadByte(); stream.ReadByte(); // MTIME
      stream.ReadByte(); // XFL
      stream.ReadByte(); // OS
      if (fextra) {
        // Skip XLEN bytes of data
        var xlen = stream.ReadByte() | (stream.ReadByte() << 8);
        while (0 < xlen) {
          stream.ReadByte();
          xlen--;
        }
      }
      if (fname) {
        // Skip 0-terminated file name
        while (0 != stream.ReadByte()) {
        }
      }
      if (fcomment) {
        // Skip 0-terminated file comment
        while (0 != stream.ReadByte()) {
        }
      }
      if (fhcrc) {
        stream.ReadByte(); stream.ReadByte(); // CRC16
      }

      // Read compressed data
      const int zipHeaderSize = 30 + 1; // 30 bytes + 1 character for file name
      const int zipFooterSize = 68 + 1; // 68 bytes + 1 character for file name

      // Download unknown amount of compressed data efficiently (note: Content-Length header is not always reliable)
      var buffers = new List<byte[]>();
      var buffer = new byte[4096];
      var bytesInBuffer = 0;
      var totalBytes = 0;
      var bytesRead = 0;
      do {
        if (buffer.Length == bytesInBuffer) {
          // Full, allocate another
          buffers.Add(buffer);
          buffer = new byte[buffer.Length];
          bytesInBuffer = 0;
        }
        Debug.Assert(bytesInBuffer < buffer.Length);
        bytesRead = stream.Read(buffer, bytesInBuffer, buffer.Length - bytesInBuffer);
        bytesInBuffer += bytesRead;
        totalBytes += bytesRead;
      } while (0 < bytesRead);
      buffers.Add(buffer);

      // "Trim" crc32 and isize fields off the end
      var compressedSize = totalBytes - 4 - 4;
      if (compressedSize < 0) {
        throw new NotSupportedException("Compressed data not in the expected format.");
      }

      // Create contiguous buffer
      var compressedBytes = new byte[zipHeaderSize + compressedSize + zipFooterSize];
      var offset = zipHeaderSize;
      var remainingBytes = totalBytes;
      foreach (var b in buffers) {
        var length = Math.Min(b.Length, remainingBytes);
        Array.Copy(b, 0, compressedBytes, offset, length);
        offset += length;
        remainingBytes -= length;
      }
      Debug.Assert(0 == remainingBytes);

      // Read footer from end of compressed bytes (note: footer is within zipFooterSize; will be overwritten below)
      Debug.Assert(totalBytes <= compressedSize + zipFooterSize);
      offset = zipHeaderSize + compressedSize;
      var crc32 = compressedBytes[offset + 0] | (compressedBytes[offset + 1] << 8) | (compressedBytes[offset + 2] << 16) | (compressedBytes[offset + 3] << 24);
      var isize = compressedBytes[offset + 4] | (compressedBytes[offset + 5] << 8) | (compressedBytes[offset + 6] << 16) | (compressedBytes[offset + 7] << 24);

      // Create ZIP file stream
      const string fileName = "f"; // MUST be 1 character (offsets below assume this)
      Debug.Assert(1 == fileName.Length);
      var zipFileMemoryStream = new MemoryStream(compressedBytes);
      var writer = new BinaryWriter(zipFileMemoryStream);

      // Local file header
      writer.Write((uint)0x04034b50); // local file header signature
      writer.Write((ushort)20); // version needed to extract (2.0 == compressed using deflate)
      writer.Write((ushort)0); // general purpose bit flag
      writer.Write((ushort)8); // compression method (8: deflate)
      writer.Write((ushort)0); // last mod file time
      writer.Write((ushort)0); // last mod file date
      writer.Write(crc32); // crc-32
      writer.Write(compressedSize); // compressed size
      writer.Write(isize); // uncompressed size
      writer.Write((ushort)1); // file name length
      writer.Write((ushort)0); // extra field length
      writer.Write((byte)fileName[0]); // file name

      // File data (already present)
      zipFileMemoryStream.Seek(compressedSize, SeekOrigin.Current);

      // Central directory structure
      writer.Write((uint)0x02014b50); // central file header signature
      writer.Write((ushort)20); // version made by
      writer.Write((ushort)20); // version needed to extract (2.0 == compressed using deflate)
      writer.Write((ushort)0); // general purpose bit flag
      writer.Write((ushort)8); // compression method
      writer.Write((ushort)0); // last mod file time
      writer.Write((ushort)0); // last mod file date
      writer.Write(crc32); // crc-32
      writer.Write(compressedSize); // compressed size
      writer.Write(isize); // uncompressed size
      writer.Write((ushort)1); // file name length
      writer.Write((ushort)0); // extra field length
      writer.Write((ushort)0); // file comment length
      writer.Write((ushort)0); // disk number start
      writer.Write((ushort)0); // internal file attributes
      writer.Write((uint)0); // external file attributes
      writer.Write((uint)0); // relative offset of local header
      writer.Write((byte)fileName[0]); // file name
      // End of central directory record
      writer.Write((uint)0x06054b50); // end of central dir signature
      writer.Write((ushort)0); // number of this disk
      writer.Write((ushort)0); // number of the disk with the start of the central directory
      writer.Write((ushort)1); // total number of entries in the central directory on this disk
      writer.Write((ushort)1); // total number of entries in the central directory
      writer.Write((uint)(46 + 1)); // size of the central directory (46 bytes + 1 character for file name)
      writer.Write((uint)(zipHeaderSize + compressedSize)); // offset of start of central directory with respect to the starting disk number
      writer.Write((ushort)0); // .ZIP file comment length

      // Reset ZIP file stream to beginning
      zipFileMemoryStream.Seek(0, SeekOrigin.Begin);

      // Return the decompressed stream
      return Application.GetResourceStream(
          new StreamResourceInfo(zipFileMemoryStream, null),
          new Uri(fileName, UriKind.Relative))
          .Stream;
    } 
  }
}
