//Licence URLs:
//https://www.viacode.com/viacode-incident-management-license/
//https://www.viacode.com/gnu-affero-general-public-license/

using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using System;

namespace DashboardReport
{
    public class AppInsightService
    {
        private static readonly string _key =
            TelemetryConfiguration.Active.InstrumentationKey = 
            System.Environment.GetEnvironmentVariable("APPINSIGHTS_INSTRUMENTATIONKEY");

        private static readonly TelemetryClient _telemetryClient =
            new TelemetryClient() { InstrumentationKey = _key };

        public AppInsightService()
        {

        }

        public void LogMessage(String message)
        {
            _telemetryClient.TrackTrace(message);
        }

        public void MetricMessage(String metricName, int number)
        {
            _telemetryClient.GetMetric(metricName).TrackValue(number);
        }
    }
}
