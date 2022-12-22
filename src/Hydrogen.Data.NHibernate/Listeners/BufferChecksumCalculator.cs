using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hydrogen.Data.NHibernate {
    /// <summary>
    /// Cusomize this class to control how the busiess object checksum is computed
    /// </summary>
    public class BufferChecksumCalculator {
        private const int MurMur3HashSeed = 37;
        public int ComputeChecksum(byte[] data) {
            // TODO: change to MurMur3(Blake2(data)) or MurMur3(MD5(data))
            return data.GetMurMurHash3(MurMur3HashSeed);
        }
    }
}
