//Licence URLs:
//https://www.viacode.com/viacode-incident-management-license/
//https://www.viacode.com/gnu-affero-general-public-license/

using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using Zammad.Client.Resources;

namespace DashboardReport
{
    public static class GetSLAReport
	{
        [FunctionName("GetSLAReport")]
        public static void Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger 'DashboardReport' function stared execution at: {DateTime.Now}");

            ZammadService zammadService = new ZammadService();

            IList<Ticket> ticketsList = zammadService.GetTickets().Result;

            IList<TicketArticle> ticketArticles = zammadService.GetTicketArticle().Result;

            Tickets tickets = new Tickets(tickets: ticketsList, ticketsArticles: ticketArticles);

            (int totalTickets, int respondedTickets) = tickets.TotalRespondedSLA();

            AppInsightService appInsightService = new AppInsightService();

            //Total responded sla
            appInsightService.LogMessage(message: $"TRSLA:{totalTickets};{respondedTickets}");
            //Passes total responded sla violations  % 
            int respondedViolationsPercent = (totalTickets - respondedTickets) * 100 / totalTickets;
            appInsightService.LogMessage(message: $"TRSLAPercent:100;{respondedViolationsPercent}");
            //Responded within SLA %
            appInsightService.LogMessage(message: $"RWSLAPercent:100;{respondedTickets * 100 / totalTickets}");
            //Respond SLA Violated
            appInsightService.LogMessage(message: $"RSLAV:{totalTickets};{totalTickets - respondedTickets}");

            // Add data to metrics
            // Users Count
            int usersCount = zammadService.GetUsersCount().Result;
            //users
            appInsightService.MetricMessage("Users", usersCount);
            // tickets count
            appInsightService.MetricMessage("Tickets", ticketsList.Count);
        }
    }
}
