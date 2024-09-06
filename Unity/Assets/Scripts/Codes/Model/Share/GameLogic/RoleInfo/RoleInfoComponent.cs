using System.Collections.Generic;

namespace ET
{
    [ComponentOf(typeof (Scene))]
    public class RoleInfoComponent: Entity, IAwake, IDestroy
    {
        /// <summary>
        /// 当前选择的游戏角色Id
        /// </summary>
        public long CurrentRoleId { get; set; }

        /// <summary>
        /// 存储玩家所有角色信息
        /// </summary>
        public List<RoleInfoDB> RoleInfosList = new();
    }
}