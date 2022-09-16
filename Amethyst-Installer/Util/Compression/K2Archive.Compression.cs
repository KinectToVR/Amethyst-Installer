using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZstdNet;

namespace amethyst_installer_gui {
	public partial class K2Archive {

		private static byte[] CreateFileBlock(string filePath, ref byte[] content, int compressionLevel) {

			// Decompress the entire file block.
			// var compressor = new Compressor(new CompressionOptions(compressionLevel));
			// var compressedData = compressor.Wrap(content);
			var compressedData = content;

			// Construct the block byte*
			byte[] filePathAsBytes = Encoding.UTF8.GetBytes(filePath);
			byte[] fileSizeInBytes = BitConverter.GetBytes(content.Length);
			byte[] blockSizeInBytes = BitConverter.GetBytes(compressedData.Length);

			byte[] block = new byte[100 + 4 + 4 + compressedData.Length];

			// FilePath
			for ( int i = 0; i < 100; i++ ) {
				if ( i < filePathAsBytes.Length )
					block[i] = filePathAsBytes[i];
				else
					block[i] = 0;
			}

			// FileSize
			block[100] = fileSizeInBytes[0];
			block[101] = fileSizeInBytes[1];
			block[102] = fileSizeInBytes[2];
			block[103] = fileSizeInBytes[3];

			// BlockSize
			block[104] = blockSizeInBytes[0];
			block[105] = blockSizeInBytes[1];
			block[106] = blockSizeInBytes[2];
			block[107] = blockSizeInBytes[3];

			// ZStandard File byte*
			for ( int i = 0; i < compressedData.Length; i++ ) {
				block[107 + i] = compressedData[i];
			}

			return block;
		}
	}
}