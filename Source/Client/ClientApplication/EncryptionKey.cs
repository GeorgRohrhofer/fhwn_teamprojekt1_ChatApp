using System;

/// <summary>
/// Class to hold session key information.
/// </summary>
public class EncryptionKey
{
	/// <summary>
	/// Constructor for a new Encryption key.
	/// </summary>
	/// <param name="key">Byte array of the key.</param>
	/// <param name="iv">Byte array of the initialization vector.</param>
	public EncryptionKey(byte[] key, byte[] iv)
	{
		this.key = key;
		this.IV = iv;
	}

	/// <summary>
	/// Key property of the session key. 
	/// </summary>
	public byte[] key { get; set; }
	/// <summary>
	/// Initialization Vector  property of the session key.
	/// </summary>
	public byte[] IV { get; set; }
}
