namespace ET
{
    /// <summary>
    /// 玩家的角色信息
    /// </summary>
    [ChildOf]
    public class RoleInfoDB: Entity, IAwake, IDestroy
    {
        /// <summary>
        /// 玩家账号
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 记录挂载在哪个区服账号信息中（AccountZoneDB的Id）
        /// 可控制角色在不同区服的转移
        /// </summary>
        public long AccountZoneId { get; set; }
        
        /// <summary>
        /// 登录的逻辑区服
        /// </summary>
        public int LoginZoneId { get; set; }

        /// <summary>
        /// 角色是否已删除
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// 角色名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 角色等级
        /// </summary>
        public int Level { get; set; }
    }
}