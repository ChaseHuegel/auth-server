//------------------------------------------------------------------------------//
//	Usage (C#):
//		Initialize your ByteMessage with:
//			ByteMessage bytes = new ByteMessage();
//
//		Write variables to the message using:
//			bytes.Write(myVariable);
//
//		Read variables from the message using:
//			int myInt = bytes.ReadInt();
//			float myFloat = bytes.ReadFloat();
//			bool myBool = bytes.ReadBool();
//			string myString = bytes.ReadString();
//
//		Get/Set/Reset the message using:
//			byte[] myMessage = bytes.GetMessage();
//			bytes.SetMessage(myMessage);
//			bytes.ResetMessage();
//
//------------------------------------------------------------------------------
using System;
using System.Text;

public class ByteMessage
{
	private byte[] message = new byte[0];
	private int readIndex = 0;

	public int Length { get => message.Length; }

	public int UnreadBytes()
	{
		return message.Length - readIndex;
    }

	//  Retrieve the message (Byte array)
	public byte[] GetMessage() { return message; }

	//  Set the message (Byte array), and reset the read index
	public void SetMessage(byte[] temp) { message = temp; readIndex = 0; }

	//  Reset the message (Byte array), and reset the read index
	public void ResetMessage() { message = new byte[0]; readIndex = 0; }

	public override string ToString()
	{
        string s = "";

		foreach (byte b in message)
            s += b.ToString();

        return s;
    }

    //  Convert a variable to bytes then write them to the message
    public void Write(object value)
	{
		if (value is string)
		{
			string tempString = (string)value;

			//  First write an integer to state the # of bytes used by the string
			byte[] bytes = BitConverter.GetBytes(Encoding.Default.GetByteCount(tempString));
			BytesToMessage(bytes);

            //	Write the string to the message
            bytes = Encoding.Default.GetBytes(tempString);
            BytesToMessage(bytes);
		}

		if (value is int)
		{
			byte[] bytes = BitConverter.GetBytes((int)value);
			BytesToMessage(bytes);
		}

		if (value is float)
		{
			byte[] bytes = BitConverter.GetBytes((float)value);
			BytesToMessage(bytes);
		}

		if (value is bool)
		{
			byte[] bytes = BitConverter.GetBytes((bool)value);
			BytesToMessage(bytes);
		}
	}

	//  Write bytes to the message
	private void BytesToMessage(byte[] bytes)
	{
		byte[] temp = new byte[message.Length + bytes.Length];

		for (int i = 0; i < message.Length; i++)
		{
			temp[i] = message[i];
		}

		for (int i = 0; i < bytes.Length; i++)
		{
			temp[message.Length + i] = bytes[i];
		}

		message = temp;
	}

	//  Read a string from the message
	public string ReadString()
	{
		byte[] temp = new byte[4];

		for (int i = 0; i < temp.Length; i++)
		{
			temp[i] = message[readIndex + i];
		}

		readIndex += temp.Length;

		int length = BitConverter.ToInt32(temp, 0);

        string s = Encoding.Default.GetString(message, readIndex, length);

        readIndex += length;

        return s;
	}

	//  Read an int from the message
	public int ReadInt()
	{
		byte[] temp = new byte[4];

		for (int i = 0; i < temp.Length; i++)
		{
			temp[i] = message[readIndex + i];
		}

		readIndex += temp.Length;

		return BitConverter.ToInt32(temp, 0);
	}

	//  Read a float from the message
	public float ReadFloat()
	{
		byte[] temp = new byte[4];

		for (int i = 0; i < temp.Length; i++)
		{
			temp[i] = message[readIndex + i];
		}

		readIndex += temp.Length;

		return BitConverter.ToSingle(temp, 0);
	}

	//  Read a boolean from the message
	public bool ReadBool()
	{
		byte[] temp = new byte[1];

		temp[0] = message[readIndex];
		readIndex += 1;

		return BitConverter.ToBoolean(temp, 0);
	}
}