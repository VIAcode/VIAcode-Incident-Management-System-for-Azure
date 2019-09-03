//Licence URLs:
//https://www.viacode.com/viacode-incident-management-license/
//https://www.viacode.com/gnu-affero-general-public-license/

using System.Collections.Generic;
using Zammad.Client.Resources;

namespace DashboardReport
{
    class Tickets
    {
        private readonly List<TicketInfo> _list = new List<TicketInfo>();

        public Tickets(IList<Ticket> tickets, IList<TicketArticle> ticketsArticles)
        {
            foreach (Ticket t in tickets)
            {
                List<TicketArticle> ta = new List<TicketArticle>();

                foreach (TicketArticle a in ticketsArticles)
                {
                    if (t.Id == a.TicketId)
                    {
                        ta.Add(a);
                    }
                }
                if (ta.Count > 0)
                {
                    TicketInfo ti = new TicketInfo(ticket: t, articles: ta);
                    _list.Add(ti);
                }
            }
        }

        public (int, int) TotalRespondedSLA()
        {
            int totalTickets = _list.Count;
            int respondedTickets = 0;
            foreach (TicketInfo t in _list)
            {
                if (t.Ticket.FirstResponseAt.HasValue)
                {
                    respondedTickets++;
                }
                if (t.TicketArticle.Count > 1)
                {
                    respondedTickets++;
                }
            }
            return (totalTickets, respondedTickets);
        }
    }
}
