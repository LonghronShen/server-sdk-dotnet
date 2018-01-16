using donet.io.rong.models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace RongCloudServerSDK.Core.Tests
{

    public class HistoryItemTest
    {

        [Fact]
        public void HistoryItemDeserializeTest()
        {
            var item = "{\"appId\":\"8w7jv4qb7k5wy\",\"fromUserId\":\"99921\",\"targetId\":\"4974\",\"targetType\":3,\"GroupId\":\"4971\",\"classname\":\"RC:TxtMsg\",\"content\":{\"content\":\"求帮助\"},\"dateTime\":\"2015-05-2708:18:30.709\",\"source\":\"iOS\",\"isDiscard\":\"false\",\"isSensitiveWord\":\"false\",\"isForbidden\":\"false\",\"isNotForward\":\"false\",\"msgUID\":\"596E-P5PG-4FS2-7OJK\"}";
            var obj = JsonConvert.DeserializeObject<HistoryItem>(item);
            Assert.NotNull(obj?.Content);
        }

    }

}