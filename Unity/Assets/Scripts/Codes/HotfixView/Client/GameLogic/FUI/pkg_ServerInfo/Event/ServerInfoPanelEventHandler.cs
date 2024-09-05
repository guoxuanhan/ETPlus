using System;
using UnityEngine;

namespace ET.Client
{
	[FriendOf(typeof(PanelCoreData))]
	[FriendOf(typeof(FUIEntity))]
	[FUIEvent(PanelId.ServerInfoPanel, "pkg_ServerInfo", "ServerInfoPanel")]
	public class ServerInfoPanelEventHandler: IFUIEventHandler
	{
		public void OnInitPanelCoreData(FUIEntity fuiEntity)
		{
			fuiEntity.PanelCoreData.panelType = UIPanelType.Normal;
		}

		public void OnInitComponent(FUIEntity fuiEntity)
		{
			ServerInfoPanel panel = fuiEntity.AddComponent<ServerInfoPanel>();
			panel.Awake();
		}

		public void OnRegisterUIEvent(FUIEntity fuiEntity)
		{
			fuiEntity.GetComponent<ServerInfoPanel>().RegisterUIEvent();
		}

		public void OnShow(FUIEntity fuiEntity, Entity contextData = null)
		{
			fuiEntity.GetComponent<ServerInfoPanel>().OnShow(contextData);
		}

		public void OnHide(FUIEntity fuiEntity)
		{
			fuiEntity.GetComponent<ServerInfoPanel>().OnHide();
		}

		public void BeforeUnload(FUIEntity fuiEntity)
		{
			fuiEntity.GetComponent<ServerInfoPanel>().BeforeUnload();
		}
	}
}