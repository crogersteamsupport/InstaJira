using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSupport.ServiceLibrary
{

    public class IndexLocks
    {
        private static object _staticLock = new object();
        private static List<string> _lockedPaths = new List<string>();

        public static bool AquireLock(string indexPath) {
            bool result = false;
            lock (_staticLock)
            {
                if (_lockedPaths.IndexOf(indexPath) < 0)
                {
                    _lockedPaths.Add(indexPath);
                    result = true;
                }
            }
            return result;
        }

        public static void ReleaseLock(string indexPath)
        {
            lock (_staticLock)
            {
                _lockedPaths.Remove(indexPath);
            }
        }
    }


}
