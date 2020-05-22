using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLCApi
{
    public interface IPLCNotify
    {
        void NotifyChanges(string value);
    }
}
