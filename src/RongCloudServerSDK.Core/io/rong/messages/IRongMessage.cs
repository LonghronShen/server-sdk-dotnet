using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace donet.io.rong.messages
{

    public interface IRongMessage
    {

        string getType();

        string toString();

    }

}