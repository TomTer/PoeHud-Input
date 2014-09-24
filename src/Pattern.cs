using System;
public struct Pattern
{
	public byte[] Bytes;
	public string Mask;
	public Pattern(byte[] pattern, string mask)
	{
		this.Bytes = pattern;
		this.Mask = mask;
	}
}
