//Licence URLs:
//https://www.viacode.com/viacode-incident-management-license/
//https://www.viacode.com/gnu-affero-general-public-license/

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing.Template;
using Zammad.Client;
using Zammad.Client.Resources;

namespace ITSMConnector
{
    public class ZammadService
    {
        private readonly TicketClient _ticketClient;
        private readonly TagClient _tagClient;

        // G: 6/15/2008 9:15:07 PM
        private const string DateTimeFormat = "G";
        private const string ZammadUrlSetting = "ZammadUrl";
        
        public ZammadService()
        {
            var zammadUrl = System.Environment.GetEnvironmentVariable(ZammadUrlSetting);

            var account = ZammadAccount.CreateBasicAccount(zammadUrl, KeyVault.Login, KeyVault.Password);

            _ticketClient = account.CreateTicketClient();
            _tagClient = account.CreateTagClient();
        }

        public async Task<Ticket> GetTicketByTitleAsync(string title)
        {
            return (await _ticketClient.SearchTicketAsync($"title:{title} state_id:1", 1, "created_at", "desc")).FirstOrDefault();
        }

        public async Task<IEnumerable<Ticket>> GetClosedTicketsAsync()
        {
            return await _ticketClient.SearchTicketAsync($"state_id:4 close_at:>now-60m", 100, "created_at", "asc");
        }

        public async Task<IEnumerable<string>> GetTicketTagsAsync(int id)
        {
            return await _tagClient.GetTagListAsync("Ticket", id);
        }

        public async Task CreateTicketAsync(string title, string body)
        {
            var ticketArticle = new TicketArticle()
            {
                Subject = title,
                Body = body,
                Type = "note",
            };
            await _ticketClient.CreateTicketAsync(
                new Ticket
                {
                    Title = title,
                    GroupId = 1,
                    CustomerId = 1,
                    OwnerId = 1

                }, ticketArticle);
        }

        public async Task CreateTicketAsync(Alert alert)
        {
            var ticketArticle = await MakeTicketArticle(alert);
            var ticket = await _ticketClient.CreateTicketAsync(
                new Ticket
                {
                    Title = $"{alert.Data.Essentials.AlertRule}",
                    GroupId = 1,
                    CustomerId = 1,
                    OwnerId = 1

                }, ticketArticle);

            await _tagClient.AddTagAsync("Ticket", ticket.Id, $"AlertId:{alert.Data.Essentials.AlertId}");
            await _tagClient.AddTagAsync("Ticket", ticket.Id, $"{alert.Data.Essentials.MonitoringService}");
            await _tagClient.AddTagAsync("Ticket", ticket.Id, $"{alert.Data.Essentials.AlertRule.Replace(" ", "_")}");
        }

        private static async Task<TicketArticle> MakeTicketArticle(Alert alert)
        {
            var service = alert.Data.Essentials.MonitoringService;
            if (alert.Data.AlertContext.SearchQuery != null && alert.Data.AlertContext.SearchQuery.Equals("CustomEvent_CL", StringComparison.CurrentCultureIgnoreCase))
            {
                service = MonitoringService.Feed;
            }
            if (alert.Data.AlertContext.SearchQuery != null && alert.Data.AlertContext.SearchQuery.Equals("CustomEvent_UPSELL", StringComparison.CurrentCultureIgnoreCase))
            {
                service = MonitoringService.Upsell;
            }
            var body = await GenerateArticleFromTemplate(alert.Data, service);
            return new TicketArticle
            {
                Subject = "Default Subject",
                Body = body,
                Type = "note",
                ContentType = "text/html"
            };
            
        }

        public async Task CreateTicketIfNotExistsAsync(Alert alert)
        {
            //If there is an existing open ticket for the rule and the source of the incoming alert,
            //the connector should create a new article for the existing incident.
            var ticket = (await _ticketClient.SearchTicketAsync($"tags:{alert.Data.Essentials.MonitoringService}%20AND%20{alert.Data.Essentials.AlertRule.Replace(" ", "_")}",
                1)).FirstOrDefault();

            if (ticket == null || ticket.Id == 0 || ticket.CloseAt.HasValue)
            {
                await CreateTicketAsync(alert);
            }
            else
            {
                TicketArticle article;
                if (alert.Data.Essentials.MonitorCondition == MonitorCondition.Resolved)
                {
                    article = new TicketArticle
                    {
                        Subject = alert.Data.AlertContext.AlertType,
                        Body = "Alert resolved",
                        Type = "note"
                    };
                }
                else
                {
                    article = await MakeTicketArticle(alert);
                    await _tagClient.AddTagAsync("Ticket", ticket.Id, $"AlertId:{alert.Data.Essentials.AlertId}");
                }
                article.TicketId = ticket.Id;

                await _ticketClient.CreateTicketArticleAsync(article);
            }
        }

        private static string FormatSearchResult(SearchResults searchResults)
        {
            var result = string.Empty;
            foreach (var table in searchResults.Tables)
            {
                result += $"Table Name: {table.Name} \n";
                foreach (var column in table.Columns)
                {
                    result += $"{column.Name} ({column.Type})|";
                }
                result += "\n";
                foreach (var row in table.Rows)
                {
                    foreach (var value in row)
                    {
                        result += $"{value}|";
                    }
                    result += "\n";
                }
                result += "\n\n";
            }
            return result;
        }

        private static async Task<string> GetArticleTemplate(string name)
        {
            var baseTemplate = await File.ReadAllTextAsync(AppendFilenameToPath("BaseTemplate.md"));
            var templatePath = AppendFilenameToPath($"{name}.md");
            var template = "";

            if (!File.Exists(templatePath))
            {
                template = baseTemplate;
            }
            else
            {
                template = await File.ReadAllTextAsync(templatePath);
                template = template.Replace("###BaseTemplate###", baseTemplate);
            }
            return template;

            string AppendFilenameToPath(string fileName)
            {
                var rootPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                return Path.Combine(rootPath.Substring(0, rootPath.Length - 4), @"AlertArticleTemplates\" + fileName);
            }
        }

        private static async Task<string> GenerateArticleFromTemplate(Data data,
            MonitoringService monitoringService)
        {
            var articleTemplate = await GetArticleTemplate(monitoringService.ToString());

            articleTemplate = FillPlaceholdersReqursive(data.Essentials, articleTemplate);
            articleTemplate = FillPlaceholdersReqursive(data.AlertContext, articleTemplate);

            string FillPlaceholdersReqursive(object obj, string template)
            {
                if (obj == null) return template;
                var fields = obj.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance).Where(f => Attribute.IsDefined(f, typeof(DataMemberAttribute)));
                foreach (var field in fields)
                {
                    if (field.FieldType.IsClass && Attribute.IsDefined(field.FieldType, typeof(DataContractAttribute)))
                    {
                        template = FillPlaceholdersReqursive(field.GetValue(obj), template);
                        continue;
                    }

                    if (field.GetValue(obj) is IEnumerable objVal && field.FieldType.IsGenericType)
                    {
                        var gT = field.FieldType.GetGenericTypeDefinition();
                        if (
                            gT.FullName != null && (gT.FullName.StartsWith("System", StringComparison.CurrentCultureIgnoreCase) ||
                                                    gT.FullName.StartsWith("Microsoft", StringComparison.CurrentCultureIgnoreCase)))
                        {
                            var atr = (DataMemberAttribute)field.GetCustomAttribute(typeof(DataMemberAttribute));
                            var pl = $"###{atr.Name}###";
                            if (template.Contains(pl, StringComparison.CurrentCultureIgnoreCase))
                            {
                                if (atr.Name.Equals("alertTargetIDs", StringComparison.CurrentCultureIgnoreCase))
                                    template = template.Replace(pl, string.Join("; ", objVal.Cast<object>().Select(o => $"\r\n  * [{o.ToString()}]({data.GetLinkToTarget(o.ToString())})")));
                                else
                                    template = template.Replace(pl, string.Join("; ", objVal.Cast<object>().Select(o => $"\r\n  * {o.ToString()}")));
                            }
                        }

                        template = objVal.Cast<object>().Aggregate(template, (current, o) => FillPlaceholdersReqursive(o, current));
                        continue;
                    }

                    var attr = (DataMemberAttribute)field.GetCustomAttribute(typeof(DataMemberAttribute));
                    var placeholder = $"###{attr.Name}###";
                    if (template.Contains(placeholder, StringComparison.CurrentCultureIgnoreCase))
                    {
                        template = template.Replace(placeholder, (field.GetValue(obj) ?? "undefined").ToString(), StringComparison.CurrentCultureIgnoreCase);
                    }
                }
                return template;
            }

            return CommonMark.CommonMarkConverter.Convert(articleTemplate);
        }
    }
}
