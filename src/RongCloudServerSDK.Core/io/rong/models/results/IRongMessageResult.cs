using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace donet.io.rong.models
{

    public interface IRongMessageResult
    {

        int getCode();

        string getErrorMessage();

    }

}