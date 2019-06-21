using System.Collections.Generic;

namespace MyPromAdapter.Controllers.MyAdapter
{
    /// <summary>
    /// 用來接收AlertManager 吐回來的 Json ，格式、欄位名需一致
    /// </summary>
    public class AlertmanagerMsg
    {
        public string version;
        public string groupKey;
        public string status;  //resolved || firing
        public string receiver;
        public object groupLabels;
        public object commonLabels;
        public object commonAnnotations;
        public object externalURL;
        public List<Alert> alerts;
        public class Alert
        {
            public string status;
            public Dictionary<string, string> labels;
            public object annotations;
            public object startsAt;
            public object endsAt;
            public object generatorURL;
            
            public bool IsResolved()
            {
                return status.ToLower().Contains("resolved");
            }
            
            public bool IsFiring()
            {
                return status.ToLower().Contains("firing");
            }
        }
    }
}