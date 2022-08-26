using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace InstallerTools.Ame_Installer {
    public static class AmeUtil {
        public static string GetChecksum(string filePath) {
            // 1MB read buffer, seems *200ms faster*
            using ( var stream = new BufferedStream(File.OpenRead(filePath), 1024 * 1024 * 1) ) {
                using ( var md5 = MD5.Create() ) {
                    byte[] checksum = md5.ComputeHash(stream);
                    return BitConverter.ToString(checksum).Replace("-", String.Empty);
                }
            }
        }
    }
}
