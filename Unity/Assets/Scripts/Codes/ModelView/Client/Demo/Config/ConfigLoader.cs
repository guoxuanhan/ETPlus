using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ET.Client
{
    [Invoke]
    public class GetAllConfigBytes: AInvokeHandler<ConfigComponent.GetAllConfigBytes, Dictionary<Type, byte[]>>
    {
        public override Dictionary<Type, byte[]> Handle(ConfigComponent.GetAllConfigBytes args)
        {
            Dictionary<Type, byte[]> output = new Dictionary<Type, byte[]>();
            HashSet<Type> configTypes = EventSystem.Instance.GetTypes(typeof (ConfigAttribute));

            if (Define.IsEditor)
            {
                string ct = "cs";
                CodeMode codeMode = GlobalConfig.Instance.CodeMode;
                ct = codeMode switch
                {
                    CodeMode.Client => "c",
                    CodeMode.Server => "s",
                    CodeMode.ClientServer => "cs",
                    _ => throw new ArgumentOutOfRangeException()
                };

                List<string> startConfigs = new List<string>()
                {
                    "StartMachineConfigCategory", "StartProcessConfigCategory", "StartSceneConfigCategory", "StartZoneConfigCategory",
                };
                foreach (Type configType in configTypes)
                {
                    string configFilePath;
                    if (startConfigs.Contains(configType.Name))
                    {
                        configFilePath = $"../Config/Excel/{ct}/{Options.Instance.StartConfig}/{configType.Name}.bytes";
                    }
                    else
                    {
                        configFilePath = $"../Config/Excel/{ct}/{configType.Name}.bytes";
                    }

                    output[configType] = File.ReadAllBytes(configFilePath);
                }
            }
            else
            {
                foreach (Type configType in configTypes)
                {
                    TextAsset v = MonoResourcesComponent.Instance.LoadAssetSync<TextAsset>($"Assets/Bundles/Config/GameConfig/{configType.Name}.bytes");
                    output[configType] = v.bytes;
                }
            }

            return output;
        }
    }

    [Invoke]
    public class GetOneConfigBytes: AInvokeHandler<ConfigComponent.GetOneConfigBytes, byte[]>
    {
        public override byte[] Handle(ConfigComponent.GetOneConfigBytes args)
        {
            byte[] buf = null;

            if (Define.IsEditor)
            {
                string ct = "cs";
                CodeMode codeMode = GlobalConfig.Instance.CodeMode;
                ct = codeMode switch
                {
                    CodeMode.Client => "c",
                    CodeMode.Server => "s",
                    CodeMode.ClientServer => "cs",
                    _ => throw new ArgumentOutOfRangeException()
                };

                HashSet<Type> configTypes = EventSystem.Instance.GetTypes(typeof (ConfigAttribute));
                foreach (Type configType in configTypes)
                {
                    if (args.ConfigName == configType.Name)
                    {
                        string configFilePath = $"../Config/Excel/{ct}/{configType.Name}.bytes";
                        buf = File.ReadAllBytes(configFilePath);
                        break;
                    }
                }
            }
            else
            {
                TextAsset v = MonoResourcesComponent.Instance.LoadAssetSync<TextAsset>($"Assets/Bundles/Config/GameConfig/{args.ConfigName}.bytes");
                buf = v.bytes;
            }

            return buf;
        }
    }
}