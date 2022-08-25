using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace InstallerTools {
    public static class Util {
        public static bool IsAny<T>(this IEnumerable<T> data) {
            return data != null && data.Any();
        }

        public static Type[] LoadVerbs() {
            return Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.GetCustomAttribute<VerbAttribute>() != null).ToArray();
        }
    }
}
