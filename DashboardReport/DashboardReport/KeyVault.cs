//Licence URLs:
//https://www.viacode.com/viacode-incident-management-license/
//https://www.viacode.com/gnu-affero-general-public-license/

using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;

namespace DashboardReport
{
    internal static class KeyVault
    {
        public static string Login { get; private set; }
        public static string Password { get; private set; }

        private static string vaultUrl = System.Environment.GetEnvironmentVariable("KeyVaultUrl");

        static KeyVault()
        {
            GetSecrets();
        }

        private static void GetSecrets()
        {
            var azureServiceTokenProvider = new AzureServiceTokenProvider();
            using (KeyVaultClient keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback)))
            {
                var secretLogin = keyVaultClient.GetSecretAsync(vaultUrl, nameof(Login));
                secretLogin.Wait();
                Login = secretLogin.Result.Value;
                var secretPassword = keyVaultClient.GetSecretAsync(vaultUrl, nameof(Password));
                secretPassword.Wait();
                Password = secretPassword.Result.Value;
            }
        }
    }
}
