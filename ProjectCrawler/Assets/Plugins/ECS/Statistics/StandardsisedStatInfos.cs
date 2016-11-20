namespace ECS.Statistics
{
    using System.Collections.Generic;
    using UnityEngine;

    public static class StandardisedStatInfo
    {
        private static StatInfo defaultStatInfo = new StatInfo();

        private static Dictionary<string, StatInfo> statInfoMap;

        public static void RegisterStatInfo(string name, StatInfo statInfo)
        {
            statInfoMap.Add(name, statInfo);
        }

        public static StatInfo GetStatInfo(string name)
        {
            name = name.ToLower();

            if (statInfoMap == null)
            {
                GetStatInfoMapJSON();
            }

            if (statInfoMap.ContainsKey(name))
            {
                return statInfoMap[name];
            }

            Debug.LogError("No key in StatisticInfo.StatInfoMap named \"" + name +
                           "\".Could not load SmartStatInfo. Returning default values instead.");
            return defaultStatInfo;
        }

        #region GetStatInfoMapJson definition
        private static void GetStatInfoMapJSON()
        {
            statInfoMap = new Dictionary<string, StatInfo>();
            TextAsset jsonText = new TextAsset();
            jsonText = (TextAsset)Resources.Load("StatisticDataJSON");

            JSONObject jsonDocObj = new JSONObject(jsonText.text);

            if (jsonDocObj.type == JSONObject.Type.OBJECT)
            {
                for (int i = 0; i < jsonDocObj.list.Count; i++)
                {
                    RegisterStatInfo(jsonDocObj.keys[i].ToLower(), new StatInfo(jsonDocObj.list[i]));
                }
            }
            else
            {
                Debug.LogError(@"Error -- The JSON file - ""StatisticDataJSON""
                            - should be of JSONObject.Type.OBJECT - meaning it should contain multiple elements - 
                            {element1: data, element2: data} etc...");
            }
        }
        #endregion
    }
}