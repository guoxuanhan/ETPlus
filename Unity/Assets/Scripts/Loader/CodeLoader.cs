using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using HybridCLR;
using UnityEngine;

namespace ET
{
    public class CodeLoader: Singleton<CodeLoader>, ISingletonAwake
    {
        private Assembly modelAssembly;
        private Assembly modelViewAssembly;

        private Dictionary<string, TextAsset> dlls;
        private Dictionary<string, TextAsset> aotDlls;
        private bool enableDll;
        
        public void Awake()
        {
            this.enableDll = GlobalConfig.Instance.EnableDll;
        }

        public async ETTask DownloadAsync()
        {
            if (!Define.IsEditor)
            {
                this.dlls = await ResourcesComponent.Instance.LoadAllAssetsAsync<TextAsset>($"Assets/Bundles/Code/{Define.CodeModelName}.dll.bytes");
                this.aotDlls = await ResourcesComponent.Instance.LoadAllAssetsAsync<TextAsset>($"Assets/Bundles/AotDlls/mscorlib.dll.bytes");
            }
        }

        public void Start()
        {
            if (!Define.IsEditor)
            {
                byte[] modelAssBytes = this.dlls[$"{Define.CodeModelName}.dll"].bytes;
                byte[] modelPdbBytes = this.dlls[$"{Define.CodeModelName}.pdb"].bytes;
                byte[] modelViewAssBytes = this.dlls[$"{Define.CodeModelViewName}.dll"].bytes;
                byte[] modelViewPdbBytes = this.dlls[$"{Define.CodeModelViewName}.pdb"].bytes;
                // 如果需要测试，可替换成下面注释的代码直接加载Assets/Bundles/Code/Unity.Model.dll.bytes，但真正打包时必须使用上面的代码
                //modelAssBytes = File.ReadAllBytes(Path.Combine(Define.CodeDir, $"{Define.CodeModelName}.dll.bytes"));
                //modelPdbBytes = File.ReadAllBytes(Path.Combine(Define.CodeDir, $"{Define.CodeModelName}.pdb.bytes"));
                //modelViewAssBytes = File.ReadAllBytes(Path.Combine(Define.CodeDir, $"{Define.CodeModelViewName}.dll.bytes"));
                //modelViewPdbBytes = File.ReadAllBytes(Path.Combine(Define.CodeDir, $"{Define.CodeModelViewName}.pdb.bytes"));

                if (Define.EnableIL2CPP)
                {
                    foreach (var kv in this.aotDlls)
                    {
                        TextAsset textAsset = kv.Value;
                        RuntimeApi.LoadMetadataForAOTAssembly(textAsset.bytes, HomologousImageMode.SuperSet);
                    }
                }
                this.modelAssembly = Assembly.Load(modelAssBytes, modelPdbBytes);
                this.modelViewAssembly = Assembly.Load(modelViewAssBytes, modelViewPdbBytes);
            }
            else
            {
                if (this.enableDll)
                {
                    byte[] modelAssBytes = File.ReadAllBytes(Path.Combine(Define.CodeDir, $"{Define.CodeModelName}.dll.bytes"));
                    byte[] modelPdbBytes = File.ReadAllBytes(Path.Combine(Define.CodeDir, $"{Define.CodeModelName}.pdb.bytes"));
                    byte[] modelViewAssBytes = File.ReadAllBytes(Path.Combine(Define.CodeDir, $"{Define.CodeModelViewName}.dll.bytes"));
                    byte[] modelViewPdbBytes = File.ReadAllBytes(Path.Combine(Define.CodeDir, $"{Define.CodeModelViewName}.pdb.bytes"));
                    this.modelAssembly = Assembly.Load(modelAssBytes, modelPdbBytes);
                    this.modelViewAssembly = Assembly.Load(modelViewAssBytes, modelViewPdbBytes);
                }
                else
                {
                    Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                    foreach (Assembly ass in assemblies)
                    {
                        string name = ass.GetName().Name;
                        if (name.Contains("Unity.Model"))
                        {
                            this.modelAssembly = ass;
                        }
                        else if (name.Contains("Unity.ModelView"))
                        {
                            this.modelViewAssembly = ass;
                        }

                        if (this.modelAssembly != null && this.modelViewAssembly != null)
                        {
                            break;
                        }
                    }
                }
            }
            
            (Assembly hotfixAssembly, Assembly hotfixViewAssembly) = this.LoadHotfix();

            World.Instance.AddSingleton<CodeTypes, Assembly[]>(new[]
            {
                typeof (World).Assembly, typeof (Init).Assembly, this.modelAssembly, this.modelViewAssembly, hotfixAssembly,
                hotfixViewAssembly
            });

            IStaticMethod start = new StaticMethod(this.modelAssembly, "ET.Entry", "Start");
            start.Run();
        }

        private (Assembly, Assembly) LoadHotfix()
        {
            byte[] hotfixAssBytes;
            byte[] hotfixPdbBytes;
            byte[] hotfixViewAssBytes;
            byte[] hotfixViewPdbBytes;
            Assembly hotfixAssembly = null;
            Assembly hotfixViewAssembly = null;
            if (!Define.IsEditor)
            {
                hotfixAssBytes = this.dlls[$"{Define.CodeHotfixName}.dll"].bytes;
                hotfixPdbBytes = this.dlls[$"{Define.CodeHotfixName}.pdb"].bytes;
                hotfixViewAssBytes = this.dlls[$"{Define.CodeHotfixViewName}.dll"].bytes;
                hotfixViewPdbBytes = this.dlls[$"{Define.CodeHotfixViewName}.pdb"].bytes;
                // 如果需要测试，可替换成下面注释的代码直接加载Assets/Bundles/Code/Hotfix.dll.bytes，但真正打包时必须使用上面的代码
                //hotfixAssBytes = File.ReadAllBytes(Path.Combine(Define.CodeDir, $"{Define.CodeHotfixName}.dll.bytes"));
                //hotfixPdbBytes = File.ReadAllBytes(Path.Combine(Define.CodeDir, $"{Define.CodeHotfixName}.pdb.bytes"));
                //hotfixViewAssBytes = File.ReadAllBytes(Path.Combine(Define.CodeDir, $"{Define.CodeHotfixViewName}.dll.bytes"));
                //hotfixViewPdbBytes = File.ReadAllBytes(Path.Combine(Define.CodeDir, $"{Define.CodeHotfixViewName}.pdb.bytes"));
                hotfixAssembly = Assembly.Load(hotfixAssBytes, hotfixPdbBytes);
                hotfixViewAssembly = Assembly.Load(hotfixViewAssBytes, hotfixViewPdbBytes);
            }
            else
            {
                if (this.enableDll)
                {
                    hotfixAssBytes = File.ReadAllBytes(Path.Combine(Define.CodeDir, $"{Define.CodeHotfixName}.dll.bytes"));
                    hotfixPdbBytes = File.ReadAllBytes(Path.Combine(Define.CodeDir, $"{Define.CodeHotfixName}.pdb.bytes"));
                    hotfixViewAssBytes = File.ReadAllBytes(Path.Combine(Define.CodeDir, $"{Define.CodeHotfixViewName}.dll.bytes"));
                    hotfixViewPdbBytes = File.ReadAllBytes(Path.Combine(Define.CodeDir, $"{Define.CodeHotfixViewName}.pdb.bytes"));
                    hotfixAssembly = Assembly.Load(hotfixAssBytes, hotfixPdbBytes);
                    hotfixViewAssembly = Assembly.Load(hotfixViewAssBytes, hotfixViewPdbBytes);
                }
                else
                {
                    Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                    foreach (Assembly ass in assemblies)
                    {
                        string name = ass.GetName().Name;
                        if (name.Contains("Unity.Hotfix"))
                        {
                            hotfixAssembly = ass;
                        }
                        else if (name.Contains("Unity.HotfixView"))
                        {
                            hotfixViewAssembly = ass;
                        }

                        if (hotfixAssembly != null && hotfixViewAssembly != null)
                        {
                            break;
                        }
                    }
                }
            }
            
            return (hotfixAssembly, hotfixViewAssembly);
        }

        public void Reload()
        {
            (Assembly hotfixAssembly, Assembly hotfixViewAssembly) = this.LoadHotfix();

            CodeTypes codeTypes = World.Instance.AddSingleton<CodeTypes, Assembly[]>(new[]
            {
                typeof (World).Assembly, typeof (Init).Assembly, this.modelAssembly, this.modelViewAssembly, hotfixAssembly,
                hotfixViewAssembly
            });
            codeTypes.CreateCode();

            Log.Info($"reload dll finish!");
        }
    }
}