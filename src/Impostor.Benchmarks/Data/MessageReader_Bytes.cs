﻿using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace Impostor.Benchmarks.Data;

public class MessageReader_Bytes
{
    public MessageReader_Bytes(byte[] buffer, int position = 0, int length = 0)
    {
        Tag = byte.MaxValue;
        Buffer = buffer;
        Position = position;
        Length = length;
    }

    public MessageReader_Bytes(byte tag, byte[] buffer, int position = 0, int length = 0)
    {
        Tag = tag;
        Buffer = buffer;
        Position = position;
        Length = length;
    }

    public byte Tag { get; }
    public byte[] Buffer { get; }
    public int Position { get; set; }
    public int Length { get; set; }
    public int BytesRemaining => Length - Position;

    public MessageReader_Bytes ReadMessage()
    {
        var length = ReadUInt16();
        var tag = FastByte();
        var pos = Position;

        Position += length;
        return new MessageReader_Bytes(tag, Buffer, pos, length);
    }

    public bool ReadBoolean()
    {
        var val = FastByte();
        return val != 0;
    }

    public sbyte ReadSByte()
    {
        return (sbyte)FastByte();
    }

    public byte ReadByte()
    {
        return FastByte();
    }

    public ushort ReadUInt16()
    {
        return (ushort)(FastByte() |
                        (FastByte() << 8));
    }

    public short ReadInt16()
    {
        return (short)(FastByte() |
                       (FastByte() << 8));
    }

    public uint ReadUInt32()
    {
        return FastByte()
               | ((uint)FastByte() << 8)
               | ((uint)FastByte() << 16)
               | ((uint)FastByte() << 24);
    }

    public int ReadInt32()
    {
        return FastByte()
               | (FastByte() << 8)
               | (FastByte() << 16)
               | (FastByte() << 24);
    }

    public unsafe float ReadSingle()
    {
        float output = 0;
        fixed (byte* bufPtr = &Buffer[Position])
        {
            var outPtr = (byte*)&output;

            *outPtr = *bufPtr;
            *(outPtr + 1) = *(bufPtr + 1);
            *(outPtr + 2) = *(bufPtr + 2);
            *(outPtr + 3) = *(bufPtr + 3);
        }

        Position += 4;
        return output;
    }

    public string ReadString()
    {
        var len = ReadPackedInt32();

        if (BytesRemaining < len)
        {
            throw new InvalidDataException($"Read length is longer than message length: {len} of {BytesRemaining}");
        }

        var output = Encoding.UTF8.GetString(Buffer, Position, len);
        Position += len;
        return output;
    }

    public Span<byte> ReadBytesAndSize()
    {
        var len = ReadPackedInt32();
        return ReadBytes(len);
    }

    public Span<byte> ReadBytes(int length)
    {
        var output = Buffer.AsSpan(Position, length);
        Position += length;
        return output;
    }

    public int ReadPackedInt32()
    {
        return (int)ReadPackedUInt32();
    }

    public uint ReadPackedUInt32()
    {
        var readMore = true;
        var shift = 0;
        uint output = 0;

        while (readMore)
        {
            var b = FastByte();
            if (b >= 0x80)
            {
                readMore = true;
                b ^= 0x80;
            }
            else
            {
                readMore = false;
            }

            output |= (uint)(b << shift);
            shift += 7;
        }

        return output;
    }

    public MessageReader_Bytes Slice(int start, int length)
    {
        return new MessageReader_Bytes(Tag, Buffer, start, length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private byte FastByte()
    {
        return Buffer[Position++];
    }
}
