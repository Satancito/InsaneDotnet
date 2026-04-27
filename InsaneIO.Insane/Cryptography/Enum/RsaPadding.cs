using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsaneIO.Insane.Cryptography
{
    public enum RsaPadding
    {
        Pkcs1,
        OaepSha1,
        OaepSha256,
        OaepSha384,
        OaepSha512
    }
}
