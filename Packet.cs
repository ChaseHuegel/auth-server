using System;
using System.Collections.Generic;
using System.Text;

public class Packet
{
    public static Packet Create() => new Packet();

    public int Length => data.Count;
    public int UnreadBytes => data.Count - readIndex;

    private int readIndex = 0;
    private List<byte> data = new List<byte>();

    public Packet() { }
    public Packet(byte[] data) => Append(data);

    //  TODO may need to store data as an array the old fashion way
    //  if overhead/garbage are issues when handling large amounts of packets
    public byte[] GetBytes() => data.ToArray();
    public byte[] GetBytes(int index, int length) => data.GetRange(index, length).ToArray();

    public Packet Grab(int index, int length) => new Packet(GetBytes(index, length));

    public byte[] Pack()
    {
        //  Write the packet size to the beginning
        data.InsertRange(0, BitConverter.GetBytes(data.Count));

        return GetBytes();
    }

    public Packet Unpack() { data.RemoveRange(0, 4); return this; }

    public Packet ResetReader() { readIndex = 0; return this; }
    public Packet Reset() { ResetReader(); data.Clear(); return this; }

    public Packet Append(byte[] bytes) { data.AddRange(bytes); return this; }
    public Packet Assign(byte[] bytes) { Reset(); Append(bytes); return this; }

    public Packet Write(object value)
    {
        if      (value is string)   WriteString(value);
        else if (value is int)      WriteInt(value);
        else if (value is float)    WriteFloat(value);
        else if (value is bool)     WriteBool(value);

        else throw new Exception($"Unsupported type [{value.GetType()}] passed to Packet.Write()");

        return this;
    }

    private void WriteInt(object value) => Append( BitConverter.GetBytes((int)value) );
    private void WriteFloat(object value) => Append( BitConverter.GetBytes((float)value) );
    private void WriteBool(object value) => Append( BitConverter.GetBytes((bool)value) );

    private void WriteString(object value)
    {
        string s = (string)value;

        //  Write an int noting the length of the string in bytes
        byte[] bytes = BitConverter.GetBytes(Encoding.Default.GetByteCount(s));
        Append(bytes);

        //  Write the string
        bytes = Encoding.Default.GetBytes(s);
        Append(bytes);
    }

    public string ReadString()
    {
        int length = BitConverter.ToInt32( GetBytes(readIndex, 4), 0 );
        string s = Encoding.Default.GetString( GetBytes(), readIndex+4, length );
        readIndex += length + 4;

        return s;
    }

    public int ReadInt()
    {
        readIndex += 4;
        return BitConverter.ToInt32( GetBytes(readIndex-4, 4), 0 );
    }

    public float ReadFloat()
    {
        readIndex += 4;
        return BitConverter.ToSingle( GetBytes(readIndex-4, 4), 0 );
    }

    public bool ReadBool()
    {
        readIndex += 1;
        return BitConverter.ToBoolean( GetBytes(readIndex-1, 4), 0 );
    }
}