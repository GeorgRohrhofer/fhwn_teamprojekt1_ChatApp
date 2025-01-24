using System;

public class EncryptionKey
{
	public EncryptionKey(byte[] key, byte[] iv)
	{
		this.key = key;
		this.IV = iv;
	}

	public byte[] key { get; set; }
	public byte[] IV { get; set; }
}
