//Licence URLs:
//https://www.viacode.com/viacode-incident-management-license/
//https://www.viacode.com/gnu-affero-general-public-license/

using System.Collections.Generic;
using System.Threading.Tasks;
using Zammad.Client;
using Zammad.Client.Resources;

namespace DashboardReport
{
    public class ZammadService
    {
        private TicketClient _ticketClient;
        private UserClient _userClient;

        private const string DateTimeFormat = "G";
        private const string ZammadUrlSetting = "ZammadUrl";

        public ZammadService()
        {
            string zammadUrl = System.Environment.GetEnvironmentVariable(ZammadUrlSetting);
            
            var account = ZammadAccount.CreateBasicAccount(zammadUrl, KeyVault.Login, KeyVault.Password);

            _ticketClient = account.CreateTicketClient();
            _userClient = account.CreateUserClient();
        }

        public async Task<IList<Ticket>> GetTickets()
        {
            return await _ticketClient.GetTicketListAsync();
        }

        public async Task<IList<TicketArticle>> GetTicketArticle()
        {
            return await _ticketClient.GetTicketArticleListAsync();
        }

        public async Task<int> GetUsersCount()
        {
            IList<User> users = await _userClient.GetUserListAsync();
            return users.Count;
        }
    }
}
