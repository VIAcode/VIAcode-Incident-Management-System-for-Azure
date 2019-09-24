//Licence URLs:
//https://www.viacode.com/viacode-incident-management-license/
//https://www.viacode.com/gnu-affero-general-public-license/

using System.Collections.Generic;
using Zammad.Client.Resources;

namespace DashboardReport
{
    class TicketInfo
    {
        private Ticket _ticket;
        public Ticket Ticket
        {
            get { return _ticket; }
        }
        private List<TicketArticle> _ticketArticles;
        public List<TicketArticle> TicketArticle
        {
            get { return _ticketArticles; }
        }

        public TicketInfo(Ticket ticket, List<TicketArticle> articles)
        {
            _ticket = ticket;
            _ticketArticles = articles;
        }
    }
}
