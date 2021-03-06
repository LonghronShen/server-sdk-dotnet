﻿using donet.io.rong.messages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace donet.io.rong.models
{

    public class HistoryItem
    {

        private static Dictionary<string, Type> MessageTypes { get; }

        static HistoryItem()
        {
            var asm = typeof(HistoryItem).GetTypeInfo().Assembly;
            MessageTypes = asm.GetExportedTypes().Where(x => x.Name.EndsWith("Message")).Select(x =>
            {
                var pi = x.GetTypeInfo().GetField("TYPE",
                    BindingFlags.IgnoreCase | BindingFlags.NonPublic | BindingFlags.Static);
                var type = pi?.GetValue(null) as string;
                return Tuple.Create(type, x);
            }).Where(x => x.Item1 != null).ToDictionary(x => x.Item1, x => x.Item2);
        }

        public string appId { get; set; }

        public string fromUserId { get; set; }

        public string targetId { get; set; }

        public int targetType { get; set; }

        public string GroupId { get; set; }

        public string classname { get; set; }

        public JObject content { get; set; }

        public string dateTime { get; set; }

        public string source { get; set; }

        public string isDiscard { get; set; }

        public string isSensitiveWord { get; set; }

        public string isForbidden { get; set; }

        public string isNotForward { get; set; }

        public string msgUID { get; set; }

        [JsonIgnore]
        public IRongMessage Content
        {
            get
            {
                var type = MessageTypes[this.classname];
                return this.content?.ToObject(type) as IRongMessage;
            }
        }

    }

}