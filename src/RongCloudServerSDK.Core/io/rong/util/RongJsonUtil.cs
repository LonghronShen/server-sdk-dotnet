using System;
using Newtonsoft.Json;

namespace donet.io.rong.util
{

    public class RongJsonUtil
    {

        public static ObjType JsonStringToObj<ObjType>(string JsonString) where ObjType : class
        {
            ObjType s = JsonConvert.DeserializeObject<ObjType>(JsonString);
            return s;
        }

    }

}