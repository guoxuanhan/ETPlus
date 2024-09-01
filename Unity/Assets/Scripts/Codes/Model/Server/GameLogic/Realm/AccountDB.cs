namespace ET.Server
{
    [ChildOf(typeof (AccountDBZoneComponent))]
    public class AccountDB: Entity, IAwake, IDestroy
    {
        /// <summary>
        /// 账号
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
    }
}