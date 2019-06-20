using System;
using System.Collections.Generic;

namespace MyPromAdapter.Controllers.MyAdapter
{
    public class StrategyScaleGaming : Strategy
    {
        private static uint _gamingServerReplicas = 1;
        private static List<string> player_morethan_100_pods = new List<string>();
        private static List<string> player_equal_0_pods = new List<string>();

        private const string AlertName1 = "player_morethan_10";
        private const string AlertName2 = "player_equal_0";
        bool TryGetLabel(string name,AlertmanagerMsg.Alert alert, out string value)
        {
            return alert.labels.TryGetValue(name, out value);
        }
        public override HttpAction[] GetActions(AlertmanagerMsg alertMsg)
        {
            List<HttpAction> result = new List<HttpAction>();
            try
            {
                List<AlertmanagerMsg.Alert> alerts = alertMsg.alerts;
                foreach (var alert in alerts)
                {
                    #region 短函式
                    bool TryAdd<T>(List<T> list, T value)
                    {
                        bool result = false;
                        if (!list.Contains(value))
                        {
                            list.Add(value);
                            result = true;
                        }
                        return result;
                    }
                    bool TryRemove<T>(List<T> list, T value)
                    {
                        return list.Remove(value);
                    }
                    string DeployNameByPod(string podName)
                    {
                        return podName.Split('-')[0];
                    }

                    void IsMatchExistAlert(string alertName,Action match1, Action match2)
                    {
                        if (alertName == AlertName1)
                        {
                            match1();
                        }
                        if (alertName == AlertName2)
                        {
                            match2();
                        }
                    }

                    #endregion

                    if (TryGetLabel("pod",alert, out string podName) &&
                        TryGetLabel("namespace",alert, out string ns) &&
                        TryGetLabel("alertname",alert, out string alertName))
                    {
                        if (!alert.IsResolved())
                        {
                            IsMatchExistAlert(alertName, () =>
                                {
                                    bool addSuccess = TryAdd<string>(player_morethan_100_pods, podName);
                                    if (addSuccess)
                                    {
                                        _gamingServerReplicas++;
                                        HttpAction action = new HttpAction(HttpAction.Methods.Get,
                                            new Uri("http://controller:80"), "/api/scale/1",
                                            new Dictionary<string, string>()
                                            {
                                                {"namespace", ns},
                                                {"name", DeployNameByPod(podName)},
                                                {"replicas", _gamingServerReplicas.ToString()}
                                            });
                                        result.Add(action);
                                    }
                                },
                                () => { TryAdd<string>(player_equal_0_pods, podName); });
                        }
                        else
                        {
                            IsMatchExistAlert(alertName, () => { TryRemove(player_morethan_100_pods, podName); }, () => { TryRemove(player_equal_0_pods, podName); });
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return result.ToArray();
        }
    }
}