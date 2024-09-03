using System;
using System.Collections.Generic;
using System.Net;

namespace ET
{
    public partial class StartSceneConfigCategory
    {
        public MultiMap<int, StartSceneConfig> Gates = new MultiMap<int, StartSceneConfig>();
        
        public MultiMap<int, StartSceneConfig> ProcessScenes = new MultiMap<int, StartSceneConfig>();
        
        public Dictionary<long, Dictionary<string, StartSceneConfig>> ClientScenesByName = new Dictionary<long, Dictionary<string, StartSceneConfig>>();

        public StartSceneConfig LocationConfig;

        public List<StartSceneConfig> Realms = new List<StartSceneConfig>();
        
        public List<StartSceneConfig> Routers = new List<StartSceneConfig>();
        
        public List<StartSceneConfig> Robots = new List<StartSceneConfig>();

        public StartSceneConfig BenchmarkServer;

        public StartSceneConfig[,] SceneTypeByScenes = new StartSceneConfig[IdGenerater.MaxZone, SceneType.Max.ToInt()];
        
        public List<StartSceneConfig> GetByProcess(int process)
        {
            return this.ProcessScenes[process];
        }
        
        public StartSceneConfig GetBySceneName(int zone, string name)
        {
            return this.ClientScenesByName[zone][name];
        }

        public StartSceneConfig GetBySceneType(int zone, SceneType sceneType)
        {
            var config = this.SceneTypeByScenes[zone, sceneType.ToInt()];

            if (config == null)
            {
                Log.Error($"无法获取对应区服的配置：Zone={zone} SceneType={sceneType}");
            }

            return config;
        }

        public long GetSceneInstanceId(int zone, SceneType sceneType)
        {
            try
            {
                StartSceneConfig startSceneConfig = Instance.GetBySceneType(zone, sceneType);
                return startSceneConfig.InstanceId;
            }
            catch (Exception e)
            {
                throw new Exception($"not find scene: {zone} {sceneType}", e);
            }
        }

        partial void PostInit()
        {
            foreach (StartSceneConfig startSceneConfig in this.GetAll().Values)
            {
                this.ProcessScenes.Add(startSceneConfig.Process, startSceneConfig);
                
                if (!this.ClientScenesByName.ContainsKey(startSceneConfig.Zone))
                {
                    this.ClientScenesByName.Add(startSceneConfig.Zone, new Dictionary<string, StartSceneConfig>());
                }
                this.ClientScenesByName[startSceneConfig.Zone].Add(startSceneConfig.Name, startSceneConfig);
                
                switch (startSceneConfig.Type)
                {
                    case SceneType.Realm:
                        this.Realms.Add(startSceneConfig);
                        break;
                    case SceneType.Gate:
                        this.Gates.Add(startSceneConfig.Zone, startSceneConfig);
                        break;
                    case SceneType.Location:
                        this.LocationConfig = startSceneConfig;
                        break;
                    case SceneType.Robot:
                        this.Robots.Add(startSceneConfig);
                        break;
                    case SceneType.Router:
                        this.Routers.Add(startSceneConfig);
                        break;
                    case SceneType.BenchmarkServer:
                        this.BenchmarkServer = startSceneConfig;
                        break;
                    default:
                        this.SceneTypeByScenes[startSceneConfig.Zone, startSceneConfig.Type.ToInt()] = startSceneConfig;
                        break;
                }
            }
        }
    }
    
    public partial class StartSceneConfig
    {
        public long InstanceId;
        
        public SceneType Type;

        public StartProcessConfig StartProcessConfig
        {
            get
            {
                return StartProcessConfigCategory.Instance.Get(this.Process);
            }
        }
        
        public StartZoneConfig StartZoneConfig
        {
            get
            {
                return StartZoneConfigCategory.Instance.Get(this.Zone);
            }
        }

        // 内网地址外网端口，通过防火墙映射端口过来
        private IPEndPoint innerIPOutPort;

        public IPEndPoint InnerIPOutPort
        {
            get
            {
                if (innerIPOutPort == null)
                {
                    this.innerIPOutPort = NetworkHelper.ToIPEndPoint($"{this.StartProcessConfig.InnerIP}:{this.OuterPort}");
                }

                return this.innerIPOutPort;
            }
        }

        private IPEndPoint outerIPPort;

        // 外网地址外网端口
        public IPEndPoint OuterIPPort
        {
            get
            {
                if (this.outerIPPort == null)
                {
                    this.outerIPPort = NetworkHelper.ToIPEndPoint($"{this.StartProcessConfig.OuterIP}:{this.OuterPort}");
                }

                return this.outerIPPort;
            }
        }

        partial void PostInit()
        {
            this.Type = EnumHelper.FromString<SceneType>(this.SceneType);
            InstanceIdStruct instanceIdStruct = new InstanceIdStruct(this.Process, (uint) this.Id);
            this.InstanceId = instanceIdStruct.ToLong();
        }
    }
}