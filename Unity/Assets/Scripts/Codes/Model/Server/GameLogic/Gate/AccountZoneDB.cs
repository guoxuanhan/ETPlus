namespace ET.Server
{
    /// <summary>
    /// 区服账号信息
    /// </summary>
    [ComponentOf(typeof (GateUser))]
    public class AccountZoneDB: Entity, IAwake, IDestroy
    {
        /// <summary>
        /// 游戏账号
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 登录的区服Id
        /// </summary>
        public int LoginZoneId { get; set; }
        
        /// <summary>
        /// 最后一次登录的角色Id
        /// </summary>
        public long LastLoginRoleId { get; set; }
    }
}