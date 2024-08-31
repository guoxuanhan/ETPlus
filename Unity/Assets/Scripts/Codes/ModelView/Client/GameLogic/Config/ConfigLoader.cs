using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Luban;

namespace ET.Client
{
    [Invoke]
    public class GetAllConfigBytes: AInvokeHandler<ConfigComponent.GetAllConfigBytes, Dictionary<Type, ByteBuf>>
    {
        public override Dictionary<Type, ByteBuf> Handle(ConfigComponent.GetAllConfigBytes args)
        {
            Dictionary<Type, ByteBuf> output = new Dictionary<Type, ByteBuf>();
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
                    
                    try
                    {
                        output[configType] = new ByteBuf(File.ReadAllBytes(configFilePath));
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }
                }
            }
            else
            {
                foreach (Type configType in configTypes)
                {
                    TextAsset v = MonoResourcesComponent.Instance.LoadAssetSync<TextAsset>($"Assets/Bundles/Config/GameConfig/{configType.Name}.bytes");
                    output[configType] = new ByteBuf(v.bytes);
                }
            }

            return output;
        }
    }

    [Invoke]
    public class GetOneConfigBytes: AInvokeHandler<ConfigComponent.GetOneConfigBytes, ByteBuf>
    {
        public override ByteBuf Handle(ConfigComponent.GetOneConfigBytes args)
        {
            ByteBuf buf = null;

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
                        buf = new ByteBuf(File.ReadAllBytes(configFilePath));
                        break;
                    }
                }
            }
            else
            {
                TextAsset v = MonoResourcesComponent.Instance.LoadAssetSync<TextAsset>($"Assets/Bundles/Config/GameConfig/{args.ConfigName}.bytes");
                buf = new ByteBuf(v.bytes);
            }

            return buf;
        }
    }
}