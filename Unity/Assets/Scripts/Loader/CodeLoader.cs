using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace ET
{
	public class CodeLoader: Singleton<CodeLoader>
	{
		private Assembly model;

		private Dictionary<string, TextAsset> dlls;
		
		public async ETTask DownloadAsync()
		{
			this.dlls = await MonoResourcesComponent.Instance.LoadAllAssetsAsync<TextAsset>($"Assets/Bundles/Code/Model.dll.bytes");
		}
		
		public void Start()
		{
			if (Define.EnableCodes)
			{
				GlobalConfig globalConfig = Resources.Load<GlobalConfig>("GlobalConfig");
				if (globalConfig.CodeMode != CodeMode.ClientServer)
				{
					throw new Exception("ENABLE_CODES mode must use ClientServer code mode!");
				}
				
				Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
				Dictionary<string, Type> types = AssemblyHelper.GetAssemblyTypes(assemblies);
				EventSystem.Instance.Add(types);
				foreach (Assembly ass in assemblies)
				{
					string name = ass.GetName().Name;
					if (name == "Unity.Model.Codes")
					{
						this.model = ass;
					}
				}
			}
			else
			{
				byte[] assBytes = this.dlls["Model.dll"].bytes;
				byte[] pdbBytes = this.dlls["Model.pdb"].bytes;
				if (!Define.IsEditor)
				{
					if (Define.EnableIL2CPP)
					{
						HybridCLRHelper.Load();
					}
				}

				this.model = Assembly.Load(assBytes, pdbBytes);
				this.LoadHotfix();
			}
			
			IStaticMethod start = new StaticMethod(this.model, "ET.Entry", "Start");
			start.Run();
		}

		// 热重载调用该方法
		public void LoadHotfix()
		{
			byte[] assBytes = this.dlls["Hotfix.dll"].bytes;
			byte[] pdbBytes = this.dlls["Hotfix.pdb"].bytes;

			Assembly hotfixAssembly = Assembly.Load(assBytes, pdbBytes);
			
			Dictionary<string, Type> types = AssemblyHelper.GetAssemblyTypes(typeof (Game).Assembly, typeof(Init).Assembly, this.model, hotfixAssembly);
			
			EventSystem.Instance.Add(types);
		}
	}
}