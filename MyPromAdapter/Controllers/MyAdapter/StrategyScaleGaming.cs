using System;
using System.Collections.Generic;

namespace MyPromAdapter.Controllers.MyAdapter
{
    public class StrategyScaleGaming : Strategy
    {
        private static List<string> player_morethan_100_pods = new List<string>();
        private static List<string> player_equal_0_pods = new List<string>();

        private const string AlertName1 = "player_morethan_10";
        private const string AlertName2 = "player_equal_0";

        public override HttpAction[] GetActions(AlertmanagerMsg alertMsg)
        {
            var result = new List<HttpAction>();
            foreach (var alert in alertMsg.alerts)
            {
                result.Add(GenerateAction(alert));
            }
            return result.ToArray();
        }
        
        private HttpAction GenerateAction(AlertmanagerMsg.Alert alert)
        {
            var result = new HttpAction();
            if (!TryGetLabel("pod", alert, out var podName) || !TryGetLabel("namespace", alert, out var ns) ||
                !TryGetLabel("alertname", alert, out var alertName)) return result;
            if (alert.IsFiring())
            {
                IsMatchExistAlert(alertName, () =>
                    {
                        var addSuccess = TryAdd(player_morethan_100_pods, podName);
                        if (!addSuccess) return;
                        result = ScaleDeploy(alert,(player_morethan_100_pods.Count+1));
                    },
                    () =>
                    {
                        var addSuccess = TryAdd(player_equal_0_pods, podName);
                        if (!addSuccess || !int.TryParse(DeployCount(alert).Go(), out var count) ||
                            player_equal_0_pods.Count != count) return;
                        player_equal_0_pods.Clear();
                        result = ScaleDeploy(alert, 1);
                    });
            }

            if (alert.IsResolved())
            {
                IsMatchExistAlert(alertName, () => { TryRemove(player_morethan_100_pods, podName); },
                    match2: () => TryRemove(player_equal_0_pods, podName));
            }

            return result;
        }

        private static HttpAction ScaleDeploy(AlertmanagerMsg.Alert alert,int number)
        {
            var result = new HttpAction();
            if (!TryGetLabel("pod", alert, out string podName) || !TryGetLabel("namespace", alert, out string ns) ||
                !TryGetLabel("alertname", alert, out string alertName)) return result;
            result = new HttpAction(HttpAction.Methods.Get,
                new Uri("http://controller:80"), "api/scale/1",
                new Dictionary<string, string>()
                {
                    {"namespace", ns},
                    {"name", DeployNameByPod(podName)},
                    {"replicas", number.ToString()}
                });
            return result;
        }

        private HttpAction DeployCount(AlertmanagerMsg.Alert alert)
        {
            var result = new HttpAction();
            if (!TryGetLabel("pod", alert, out string podName) || !TryGetLabel("namespace", alert, out string ns) ||
                !TryGetLabel("alertname", alert, out string alertName)) return result;
            result = new HttpAction(HttpAction.Methods.Get,
                new Uri("http://controller:80"), "api/count/1",
                new Dictionary<string, string>()
                {
                    {"namespace", ns},
                    {"name", DeployNameByPod(podName)},
                });
            return result;
        }

        private static bool TryGetLabel(string name,AlertmanagerMsg.Alert alert, out string value)
        {
            return alert.labels.TryGetValue(name, out value);
        }
        private static bool TryAdd<T>(ICollection<T> list, T value)
        {
            lock (list)
            {
                if (list.Contains(value)) return false;
                list.Add(value);
            }
            return true;
        }

        private static void TryRemove<T>(ICollection<T> list, T value)
        {
            lock (list)
            {
                list.Remove(value);
            }
        }

        private static string DeployNameByPod(string podName)
        {
            return podName.Split('-')[0];
        }

        private static void IsMatchExistAlert(string alertName,Action match1, Action match2)
        {
            switch (alertName)
            {
                case AlertName1:
                    match1();
                    break;
                case AlertName2:
                    match2();
                    break;
            }
        }
    }
}