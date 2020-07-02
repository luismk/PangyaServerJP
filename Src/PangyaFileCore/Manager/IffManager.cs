using PangyaFileCore.Manager;
using System.Runtime.CompilerServices;

namespace PangyaFileCore
{
    public class IffBaseManager
    {
        public static IFFFile IffEntry = new IFFFile();

        [MethodImpl(MethodImplOptions.NoInlining)]
        public IffBaseManager()
        {
            IffEntry.LoadIff();
        }
    }
}
