namespace ET.Server
{
    [ComponentOf(typeof (Session))]
    public class AccountDBRealmComponent: Entity, IAwake, IDestroy
    {
        public AccountDB AccountDB { get; set; }
    }
}