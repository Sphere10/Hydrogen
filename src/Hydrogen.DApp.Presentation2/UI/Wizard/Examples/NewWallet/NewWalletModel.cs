namespace Hydrogen.DApp.Presentation2.UI.Wizard.Examples.NewWallet
{

    public class NewWalletModel
    {
        public WalletType Type { get; set; } = WalletType.Standard;

        public string Password { get; set; }

        public string Name { get; set; } = "wallet_1";
        
        public string Seed { get; set; } 
    }

    public enum WalletType
    {
        Standard,
        Restore
    }

}