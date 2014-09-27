using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace ExileHUD.Framework
{
	public class Memory : IDisposable
	{
		public readonly int BaseAddress;
		private bool closed;
		private IntPtr procHandle;
		private Dictionary<string, int> modules;
		public Process Process { get; private set; }
		public Memory(int pId)
		{
			this.Process = Process.GetProcessById(pId);
			this.BaseAddress = this.Process.MainModule.BaseAddress.ToInt32();
			this.Open();
			this.modules = new Dictionary<string, int>();
		}
		public Memory(string name) : this(Process.GetProcessesByName(name)[0].Id)
		{
		}
		public void Dispose()
		{
			this.Close();
		}
		~Memory()
		{
			this.Close();
		}
		public int GetModule(string name)
		{
			if (this.modules.ContainsKey(name))
			{
				return this.modules[name];
			}
			int num = this.Process.Modules.Cast<ProcessModule>().First(m => m.ModuleName == name).BaseAddress.ToInt32();
			this.modules.Add(name, num);
			return num;
		}
		public bool IsInvalid()
		{
			return this.Process.HasExited || this.closed;
		}
		public int ReadInt(int addr)
		{
			return BitConverter.ToInt32(this.ReadMem(addr, 4), 0);
		}
		public int ReadInt(int addr, params int[] offsets)
		{
			int num = this.ReadInt(addr);
			for (int i = 0; i < offsets.Length; i++)
			{
				int num2 = offsets[i];
				num = this.ReadInt(num + num2);
			}
			return num;
		}
		public float ReadFloat(int addr)
		{
			return BitConverter.ToSingle(this.ReadMem(addr, 4), 0);
		}
		public long ReadLong(int addr)
		{
			return BitConverter.ToInt64(this.ReadMem(addr, 8), 0);
		}
		public uint ReadUInt(int addr)
		{
			return BitConverter.ToUInt32(this.ReadMem(addr, 4), 0);
		}
		public short ReadShort(int addr)
		{
			return BitConverter.ToInt16(this.ReadMem(addr, 2), 0);
		}
		public string ReadString(int addr, int length, bool replaceNull = true)
		{
			if (addr <= 65536 && addr >= -1)
			{
				return string.Empty;
			}
			string @string = Encoding.ASCII.GetString(this.ReadMem(addr, length));
			if (replaceNull)
			{
				int num = @string.IndexOf('\0');
				if (num > 0)
				{
					return @string.Substring(0, num);
				}
			}
			return @string;
		}
		public string ReadStringU(int addr, int length, bool replaceNull = true)
		{
			if (addr <= 65536 && addr >= -1)
			{
				return string.Empty;
			}
			string @string = Encoding.Unicode.GetString(this.ReadMem(addr, length));
			if (replaceNull)
			{
				int num = @string.IndexOf('\0');
				if (num > 0)
				{
					return @string.Substring(0, num);
				}
			}
			return @string;
		}
		public byte ReadByte(int addr)
		{
			return this.ReadBytes(addr, 1)[0];
		}
		public byte[] ReadBytes(int addr, int length)
		{
			return this.ReadMem(addr, length);
		}
        public unsafe T Read<T>(int address) where T : struct
        {
            fixed (byte* numRef = this.ReadMem(address, Marshal.SizeOf(typeof(T))))
            {
                return (T)Marshal.PtrToStructure(new IntPtr((void*)numRef), typeof(T));
            }
        }

		public void WriteStringU(int addr, string str)
		{
			if (addr <= 4096 && addr >= -1)
			{
				return;
			}
			byte[] bytes = Encoding.Unicode.GetBytes(str);
			this.WriteMem(addr, bytes);
		}
		public void WriteInt(int addr, int value)
		{
			this.WriteMem(addr, BitConverter.GetBytes(value));
		}
		public void WriteFloat(int addr, float value)
		{
			this.WriteMem(addr, BitConverter.GetBytes(value));
		}
		public void WriteBytes(int addr, byte[] bytes)
		{
			this.WriteMem(addr, bytes);
		}
		private void Open()
		{
			this.procHandle = Imports.OpenProcess(2035711u, false, this.Process.Id);
		}
		private bool Close()
		{
			if (!this.closed)
			{
				this.closed = true;
				return Imports.CloseHandle(this.procHandle) != 0;
			}
			return true;
		}
		private byte[] ReadMem(int addr, int size)
		{
			byte[] array = new byte[size];
			Imports.ReadProcessMemory(this.procHandle, (IntPtr)addr, array, size, 0);
			return array;
		}
		private void WriteMem(int addr, byte[] data)
		{
			int num = 0;
			if (!Imports.WriteProcessMemory(this.procHandle, new IntPtr(addr), data, (uint)data.Length, out num))
			{
				Console.WriteLine(string.Concat(new object[]
				{
					"mem write addr ",
					addr.ToString("X"),
					" failed ",
					Marshal.GetLastWin32Error()
				}));
			}
		}
		public void MakeMemoryWriteable(int addr, int length)
		{
			uint num = 0u;
			if (!Memory.VirtualProtectEx(this.procHandle, new IntPtr(addr), new IntPtr(length), 64u, ref num))
			{
				Console.WriteLine("VirtualProtectEx failed! " + Marshal.GetLastWin32Error());
			}
		}
		[DllImport("kernel32.dll")]
		private static extern bool VirtualProtectEx(IntPtr hProcess, IntPtr lpAddress, IntPtr dwSize, uint flNewProtect, ref uint lpflOldProtect);
		public int[] FindPatterns(params Pattern[] patterns)
		{
			byte[] array = this.ReadBytes(this.BaseAddress, 33554432);
			int[] array2 = new int[patterns.Length];
			bool[] array3 = new bool[patterns.Length];
			for (int i = 0; i < array.Length; i += 4)
			{
				for (int j = 0; j < patterns.Length; j++)
				{
					if (!array3[j])
					{
						Pattern pattern = patterns[j];
						if (this.CompareData(pattern, array, i))
						{
							array2[j] = i;
							array3[j] = true;
							Console.WriteLine("Pattern " + j + " is done");
						}
					}
				}
			}
			return array2;
		}
		private bool CompareData(Pattern pattern, byte[] data, int offset)
		{
			if (data.Length - offset <= pattern.Bytes.Length)
			{
				return false;
			}
			for (int i = 0; i < pattern.Bytes.Length; i++)
			{
				if (pattern.Mask[i] == 'x' && pattern.Bytes[i] != data[offset + i])
				{
					return false;
				}
			}
			return true;
		}
	}
}
