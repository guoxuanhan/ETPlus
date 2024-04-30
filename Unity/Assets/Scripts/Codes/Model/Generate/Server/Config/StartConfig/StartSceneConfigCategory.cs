
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Luban;
using System;
using System.Collections.Generic;

namespace ET
{
    [Config]
    public partial class StartSceneConfigCategory : Singleton<StartSceneConfigCategory>
    {
        private readonly Dictionary<int, StartSceneConfig> _dataMap;
        private readonly List<StartSceneConfig> _dataList;

        public StartSceneConfigCategory()
        {
            throw new NotImplementedException();
        }

        public StartSceneConfigCategory(ByteBuf _buf)
        {
            _dataMap = new Dictionary<int, StartSceneConfig>();
            _dataList = new List<StartSceneConfig>();

            for (int n = _buf.ReadSize(); n > 0; --n)
            {
                StartSceneConfig _v;
                _v = StartSceneConfig.DeserializeStartSceneConfig(_buf);
                _dataList.Add(_v);
                _dataMap.Add(_v.Id, _v);
            }

            PostInit();
        }

        public void ForEach(Action<StartSceneConfig> action)
        {
            foreach (var config in _dataMap.Values)
            {
                action.Invoke(config);
            }
        }

        public void ForEach(Func<StartSceneConfig, bool> action)
        {
            foreach (var config in _dataMap.Values)
            {
                if (!action.Invoke(config))
                {
                    break;
                }
            }
        }

        public StartSceneConfig Query(Func<StartSceneConfig, bool> action)
        {
            foreach (var config in _dataMap.Values)
            {
                if (action.Invoke(config))
                {
                    return config;
                }
            }

            return null;
        }

        public List<StartSceneConfig> QueryAll(Func<StartSceneConfig, bool> action)
        {
            List<StartSceneConfig> result = new();
            foreach (var config in _dataMap.Values)
            {
                if (action.Invoke(config))
                {
                    result.Add(config);
                }
            }

            return result;
        }

        public Dictionary<int, StartSceneConfig> DataMap => _dataMap;
        public List<StartSceneConfig> DataList => _dataList;

        public Dictionary<int, StartSceneConfig> GetAll() => _dataMap;
        public StartSceneConfig GetOrDefault(int key) => _dataMap.TryGetValue(key, out var v) ? v : null;
        public StartSceneConfig Get(int key) => _dataMap[key];
        public StartSceneConfig this[int key] => _dataMap[key];

        partial void PostInit();
    }
}
