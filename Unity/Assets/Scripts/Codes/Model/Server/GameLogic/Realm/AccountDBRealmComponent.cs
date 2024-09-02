namespace ET.Server
{
    [ComponentOf(typeof (Session))]
    public class AccountDBRealmComponent: Entity, IAwake, IDestroy
    {
        private EntityRef<AccountDB> accountDB;

        public AccountDB AccountDB
        {
            get
            {
                return this.accountDB;
            }
            set
            {
                this.accountDB = value;
            }
        }
    }
}