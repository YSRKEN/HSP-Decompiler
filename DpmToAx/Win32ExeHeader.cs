using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using BYTE = System.Byte;
using WORD = System.UInt16;
using DWORD = System.UInt32;
namespace KttK.HspDecompiler.DpmToAx
{
	/// <summary>
	/// "PE\0\0"で始まるやつ
	/// </summary>
	internal sealed class IMAGE_NT_HEADERS
	{
		/// <summary>
		/// リトルエンディアンでPE(50 45)
		/// </summary>
		const int IMAGE_NT_SIGNATURE = 0x4550;
		internal DWORD Signature;
		internal IMAGE_FILE_HEADER FileHeader;
		internal IMAGE_OPTIONAL_HEADER OptionalHeader;

		internal static IMAGE_NT_HEADERS FromBinaryReader(BinaryReader reader)
		{
			IMAGE_NT_HEADERS ret = new IMAGE_NT_HEADERS();
			ret.Signature = reader.ReadUInt32();
			if (ret.Signature != IMAGE_NT_SIGNATURE)
				return null;
			ret.FileHeader = IMAGE_FILE_HEADER.FromBinaryReader(reader);
			if (ret.FileHeader == null)
				return null;
			ret.OptionalHeader = IMAGE_OPTIONAL_HEADER.FromBinaryReader(reader);
			if (ret.OptionalHeader == null)
				return null;
			return ret;


		}
	}
	internal sealed class IMAGE_FILE_HEADER
	{
		internal WORD Machine;
		internal WORD NumberOfSections;
		internal DWORD TimeDateStamp;
		internal DWORD PointerToSymbolTable;
		internal DWORD NumberOfSymbols;
		internal WORD SizeOfOptionalHeader;
		internal WORD Characteristics;

		internal static IMAGE_FILE_HEADER FromBinaryReader(BinaryReader reader)
		{
			IMAGE_FILE_HEADER ret = new IMAGE_FILE_HEADER();
			try
			{
				ret.Machine = reader.ReadUInt16();
				ret.NumberOfSections = reader.ReadUInt16();
				ret.TimeDateStamp = reader.ReadUInt32();
				ret.PointerToSymbolTable = reader.ReadUInt32();
				ret.NumberOfSymbols = reader.ReadUInt32();
				ret.SizeOfOptionalHeader = reader.ReadUInt16();
				ret.Characteristics = reader.ReadUInt16();
			}
			catch
			{
				return null;
			}
			return ret;
		}
	}
	internal sealed class IMAGE_OPTIONAL_HEADER
	{
		const int IMAGE_NUMBEROF_DIRECTORY_ENTRIES = 16;
		internal WORD Magic;
		internal BYTE MajorLinkerVersion;
		internal BYTE MinorLinkerVersion;
		internal DWORD SizeOfCode;
		internal DWORD SizeOfInitializedData;
		internal DWORD SizeOfUninitializedData;
		internal DWORD AddressOfEntryPoint;
		internal DWORD BaseOfCode;
		internal DWORD BaseOfData;
		internal DWORD ImageBase;
		internal DWORD SectionAlignment;
		internal DWORD FileAlignment;
		internal WORD MajorOperatingSystemVersion;
		internal WORD MinorOperatingSystemVersion;
		internal WORD MajorImageVersion;
		internal WORD MinorImageVersion;
		internal WORD MajorSubsystemVersion;
		internal WORD MinorSubsystemVersion;
		internal DWORD Win32VersionValue;
		internal DWORD SizeOfImage;
		internal DWORD SizeOfHeaders;
		internal DWORD CheckSum;
		internal WORD Subsystem;
		internal WORD DllCharacteristics;
		internal DWORD SizeOfStackReserve;
		internal DWORD SizeOfStackCommit;
		internal DWORD SizeOfHeapReserve;
		internal DWORD SizeOfHeapCommit;
		internal DWORD LoaderFlags;
		internal DWORD NumberOfRvaAndSizes;
		internal IMAGE_DATA_DIRECTORY[] DataDirectory = new IMAGE_DATA_DIRECTORY[IMAGE_NUMBEROF_DIRECTORY_ENTRIES];
		internal static IMAGE_OPTIONAL_HEADER FromBinaryReader(BinaryReader reader)
		{
			IMAGE_OPTIONAL_HEADER ret = new IMAGE_OPTIONAL_HEADER();
			try
			{
				ret.Magic = reader.ReadUInt16();
				ret.MajorLinkerVersion = reader.ReadByte();
				ret.MinorLinkerVersion = reader.ReadByte();
				ret.SizeOfCode = reader.ReadUInt32();
				ret.SizeOfInitializedData = reader.ReadUInt32();
				ret.SizeOfUninitializedData = reader.ReadUInt32();
				ret.AddressOfEntryPoint = reader.ReadUInt32();
				ret.BaseOfCode = reader.ReadUInt32();
				ret.BaseOfData = reader.ReadUInt32();
				ret.ImageBase = reader.ReadUInt32();
				ret.SectionAlignment = reader.ReadUInt32();
				ret.FileAlignment = reader.ReadUInt32();
				ret.MajorOperatingSystemVersion = reader.ReadUInt16();
				ret.MinorOperatingSystemVersion = reader.ReadUInt16();
				ret.MajorImageVersion = reader.ReadUInt16();
				ret.MinorImageVersion = reader.ReadUInt16();
				ret.MajorSubsystemVersion = reader.ReadUInt16();
				ret.MinorSubsystemVersion = reader.ReadUInt16();
				ret.Win32VersionValue = reader.ReadUInt32();
				ret.SizeOfImage = reader.ReadUInt32();
				ret.SizeOfHeaders = reader.ReadUInt32();
				ret.CheckSum = reader.ReadUInt32();
				ret.Subsystem = reader.ReadUInt16();
				ret.DllCharacteristics = reader.ReadUInt16();
				ret.SizeOfStackReserve = reader.ReadUInt32();
				ret.SizeOfStackCommit = reader.ReadUInt32();
				ret.SizeOfHeapReserve = reader.ReadUInt32();
				ret.SizeOfHeapCommit = reader.ReadUInt32();
				ret.LoaderFlags = reader.ReadUInt32();
				ret.NumberOfRvaAndSizes = reader.ReadUInt32();

				for (int i = 0; i < IMAGE_NUMBEROF_DIRECTORY_ENTRIES; i++)
				{
					ret.DataDirectory[i] = new IMAGE_DATA_DIRECTORY();
					ret.DataDirectory[i].VirtualAddress = reader.ReadUInt32();
					ret.DataDirectory[i].Size = reader.ReadUInt32();
				}
			}
			catch
			{
				return null;
			}
			return ret;
		}
	}
	internal sealed class IMAGE_DATA_DIRECTORY
	{
		internal DWORD VirtualAddress;
		internal DWORD Size;
	}
	internal sealed class IMAGE_SECTION_HEADER
	{
		internal const int IMAGE_SIZEOF_SHORT_NAME = 8;
		internal BYTE[] Name = new BYTE[IMAGE_SIZEOF_SHORT_NAME];
		internal DWORD PhysicalAddress;
		internal DWORD VirtualSize { get { return PhysicalAddress; } set { PhysicalAddress = value; } }
		internal DWORD VirtualAddress;
		internal DWORD SizeOfRawData;
		internal DWORD PointerToRawData;
		internal DWORD PointerToRelocations;
		internal DWORD PointerToLinenumbers;
		internal WORD NumberOfRelocations;
		internal WORD NumberOfLinenumbers;
		internal DWORD Characteristics;
		internal static IMAGE_SECTION_HEADER FromBinaryReader(BinaryReader reader)
		{
			IMAGE_SECTION_HEADER ret = new IMAGE_SECTION_HEADER();
			try
			{
				bool allZero = true;
				for (int i = 0; i < IMAGE_SIZEOF_SHORT_NAME; i++)
				{
					ret.Name[i] = reader.ReadByte();
					allZero &= (ret.Name[i] == 0);
				}
				if (allZero)
					return null;
				ret.PhysicalAddress = reader.ReadUInt32();
				ret.VirtualAddress = reader.ReadUInt32();
				ret.SizeOfRawData = reader.ReadUInt32();
				ret.PointerToRawData = reader.ReadUInt32();
				ret.PointerToRelocations = reader.ReadUInt32();
				ret.PointerToLinenumbers = reader.ReadUInt32();
				ret.NumberOfRelocations = reader.ReadUInt16();
				ret.NumberOfLinenumbers = reader.ReadUInt16();
				ret.Characteristics = reader.ReadUInt32();
			}
			catch
			{
				return null;
			}
			return ret;
		}
	}
	internal sealed class IMAGE_DOS_HEADER
	{      // DOS .EXE header

		/// <summary>
		/// リトルエンディアンでMZ(4D 5A)
		/// </summary>
		const int IMAGE_DOS_SIGNATURE = 0x5A4D;
		internal WORD e_magic;                     // Magic number
		internal WORD e_cblp;                      // Bytes on last page of file
		internal WORD e_cp;                        // Pages in file
		internal WORD e_crlc;                      // Relocations
		internal WORD e_cparhdr;                   // Size of header in paragraphs
		internal WORD e_minalloc;                  // Minimum extra paragraphs needed
		internal WORD e_maxalloc;                  // Maximum extra paragraphs needed
		internal WORD e_ss;                        // Initial (relative) SS value
		internal WORD e_sp;                        // Initial SP value
		internal WORD e_csum;                      // Checksum
		internal WORD e_ip;                        // Initial IP value
		internal WORD e_cs;                        // Initial (relative) CS value
		internal WORD e_lfarlc;                    // File address of relocation table
		internal WORD e_ovno;                      // Overlay number
		internal WORD[] e_res = new WORD[4];                    // Reserved words
		internal WORD e_oemid;                     // OEM identifier (for e_oeminfo)
		internal WORD e_oeminfo;                   // OEM information; e_oemid specific
		internal WORD[] e_res2 = new WORD[10];                    // Reserved words
		internal DWORD e_lfanew;                    // File address of new exe header
		internal static IMAGE_DOS_HEADER FromBinaryReader(BinaryReader reader)
		{
			IMAGE_DOS_HEADER ret = new IMAGE_DOS_HEADER();
			try
			{
				ret.e_magic = reader.ReadUInt16();                     // Magic number
				if (ret.e_magic != IMAGE_DOS_SIGNATURE)
					return null;
				ret.e_cblp = reader.ReadUInt16();                      // Bytes on last page of file
				ret.e_cp = reader.ReadUInt16();                        // Pages in file
				ret.e_crlc = reader.ReadUInt16();                     // Relocations
				ret.e_cparhdr = reader.ReadUInt16();                   // Size of header in paragraphs
				ret.e_minalloc = reader.ReadUInt16();                  // Minimum extra paragraphs needed
				ret.e_maxalloc = reader.ReadUInt16();                  // Maximum extra paragraphs needed
				ret.e_ss = reader.ReadUInt16();                        // Initial (relative) SS value
				ret.e_sp = reader.ReadUInt16();                        // Initial SP value
				ret.e_csum = reader.ReadUInt16();                      // Checksum
				ret.e_ip = reader.ReadUInt16();                        // Initial IP value
				ret.e_cs = reader.ReadUInt16();                        // Initial (relative) CS value
				ret.e_lfarlc = reader.ReadUInt16();                    // File address of relocation table
				ret.e_ovno = reader.ReadUInt16();                      // Overlay number
				for (int i = 0; i < 4; i++)
					ret.e_res[i] = reader.ReadUInt16();
				ret.e_oemid = reader.ReadUInt16();
				ret.e_oeminfo = reader.ReadUInt16();

				for (int i = 0; i < 10; i++)
					ret.e_res2[i] = reader.ReadUInt16();
				ret.e_lfanew = reader.ReadUInt32();
			}
			catch
			{
				return null;
			}
			return ret;
		}
	}
	internal sealed class IMAGE_IMPORT_DESCRIPTOR
	{
		internal DWORD OriginalFirstThunk;
		internal DWORD TimeDataStamp;
		internal DWORD ForwarderChain;
		internal DWORD Name;
		internal DWORD FirstThunk;
		internal static IMAGE_IMPORT_DESCRIPTOR FromBinaryReader(BinaryReader reader)
		{
			IMAGE_IMPORT_DESCRIPTOR ret = new IMAGE_IMPORT_DESCRIPTOR();
			try
			{
				ret.OriginalFirstThunk = reader.ReadUInt32();
				ret.TimeDataStamp = reader.ReadUInt32();
				ret.ForwarderChain = reader.ReadUInt32();
				ret.Name = reader.ReadUInt32();
				ret.FirstThunk = reader.ReadUInt32();
			}
			catch
			{
				return null;
			}
			return ret;
		}
	}

	internal sealed class Win32ExeHeader
	{
		IMAGE_DOS_HEADER DosHeader;
		IMAGE_NT_HEADERS NtHeader;
		List<IMAGE_SECTION_HEADER> SectionHeaders = new List<IMAGE_SECTION_HEADER>();

		internal long EndOfExecutableRegion
		{
			get
			{
				if (SectionHeaders.Count == 0)
					return -1;
				IMAGE_SECTION_HEADER section = SectionHeaders[SectionHeaders.Count - 1];
				return (section.PointerToRawData + section.SizeOfRawData);
			}
		}


		internal static Win32ExeHeader FromBinaryReader(BinaryReader reader)
		{
			try
			{
				long startPosition = reader.BaseStream.Position;
				long length = reader.BaseStream.Length - startPosition;
				if (length < 0x1000)
					return null;
				Win32ExeHeader ret = new Win32ExeHeader();
				ret.DosHeader = IMAGE_DOS_HEADER.FromBinaryReader(reader);
				if (ret.DosHeader == null)
					return null;
				if (ret.DosHeader.e_lfanew <= 0)
					return ret;
				reader.BaseStream.Seek(startPosition + ret.DosHeader.e_lfanew, SeekOrigin.Begin);
				ret.NtHeader = IMAGE_NT_HEADERS.FromBinaryReader(reader);
				if (ret.NtHeader == null)
					return null;
				IMAGE_SECTION_HEADER section = IMAGE_SECTION_HEADER.FromBinaryReader(reader);
				while (section != null)
				{
					ret.SectionHeaders.Add(section);
					section = IMAGE_SECTION_HEADER.FromBinaryReader(reader);
				}
				return ret;
			}
			catch
			{
				return null;
			}
		}

	}


}
