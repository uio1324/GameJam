using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Manager.AssetsManager
{
    public sealed class AssetsManager : Manager<AssetsManager>, IManager
    {
        IEnumerator IManager.PreInit()
        {
            throw new NotImplementedException();
        }
    }
}
