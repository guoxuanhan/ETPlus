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
			this.dlls = await MonoResourcesComponent.Instance.LoadAllAssetsAsync<TextAsset>($"Assets/Bundles/Code/Model_{GlobalConfig.Instance.ModelVersion}.dll.bytes");
		}
		
		public void Start()
		{
			if (Define.EnableCodes)
			{
				if (GlobalConfig.Instance.CodeMode != CodeMode.ClientServer)
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
				byte[] assBytes = this.dlls[$"Model_{GlobalConfig.Instance.ModelVersion}.dll"].bytes;
				byte[] pdbBytes = this.dlls[$"Model_{GlobalConfig.Instance.ModelVersion}.pdb"].bytes;
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

			IStaticMethod start;
			if (GlobalConfig.Instance.CodeMode == CodeMode.Client)
			{
				start = new StaticMethod(this.model, "ET.Entry", "StartOnlyClient");
			}
			else
			{
				start = new StaticMethod(this.model, "ET.Entry", "Start");
			}

			start.Run();
		}

		// 热重载调用该方法
		public void LoadHotfix()
		{
			byte[] assBytes = this.dlls[$"Hotfix_{GlobalConfig.Instance.HotfixVersion}.dll"].bytes;
			byte[] pdbBytes = this.dlls[$"Hotfix_{GlobalConfig.Instance.HotfixVersion}.pdb"].bytes;

			Assembly hotfixAssembly = Assembly.Load(assBytes, pdbBytes);
			
			Dictionary<string, Type> types = AssemblyHelper.GetAssemblyTypes(typeof (Game).Assembly, typeof(Init).Assembly, this.model, hotfixAssembly);
			
			EventSystem.Instance.Add(types);
		}
	}
}