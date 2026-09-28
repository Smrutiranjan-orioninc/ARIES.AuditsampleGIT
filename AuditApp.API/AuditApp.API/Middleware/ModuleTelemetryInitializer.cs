using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace AuditApp.API.Middleware
{
    public class ModuleTelemetryInitializer : ITelemetryInitializer
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ModuleTelemetryInitializer(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void Initialize(ITelemetry telemetry)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            
            if (httpContext != null)
            {
                var controller = httpContext.GetRouteValue("controller")?.ToString();
                var requestPath = httpContext.Request.Path.Value;

                // Request telemetry is not always initialized after route values are populated,
                // so prefer path-based mapping and fall back to the controller name.
                var moduleName = ResolveModuleName(controller, requestPath);
                
                // Set CloudRoleName for Application Insights
                telemetry.Context.Cloud.RoleName = moduleName;
                
                // Also add to custom properties for easier querying
                if (telemetry is ISupportProperties supportProperties)
                {
                    if (!supportProperties.Properties.ContainsKey("ModuleName"))
                    {
                        supportProperties.Properties.Add("ModuleName", moduleName);
                    }

                    if (!supportProperties.Properties.ContainsKey("CloudRoleName"))
                    {
                        supportProperties.Properties.Add("CloudRoleName", moduleName);
                    }

                    if (!string.IsNullOrWhiteSpace(requestPath) && !supportProperties.Properties.ContainsKey("RequestPath"))
                    {
                        supportProperties.Properties.Add("RequestPath", requestPath!);
                    }
                }
            }
        }

        private static string ResolveModuleName(string? controller, string? requestPath)
        {
            var fromPath = MapPathToModule(requestPath);
            if (fromPath != "Unknown")
            {
                return fromPath;
            }

            return MapControllerToModule(controller);
        }

        private static string MapPathToModule(string? requestPath)
        {
            if (string.IsNullOrWhiteSpace(requestPath))
            {
                return "Unknown";
            }

            var path = requestPath.Trim().ToLowerInvariant();

            return path switch
            {
                _ when path.StartsWith("/api/settlements") => "Settlement",
                _ when path.StartsWith("/api/datapersistences") => "DataPersistence",
                _ when path.StartsWith("/api/periodicfailovers") => "PeriodicFailover",
                _ when path.StartsWith("/api/auditsitecreations") => "AuditSiteCreation",
                _ when path.StartsWith("/api/auditsitecopies") => "AuditSiteCopy",
                _ when path.StartsWith("/api/restoreauditsites") => "RestoreAuditSite",
                _ when path.StartsWith("/api/dataanalyses") => "DataAnalysis",
                _ when path.StartsWith("/api/datasamplings") => "DataSampling",
                _ when path.StartsWith("/api/audits") => "Audit",
                _ when path.StartsWith("/api/clients") => "Client",
                _ when path.StartsWith("/api/auditors") => "Auditor",
                _ when path.StartsWith("/api/findings") => "Finding",
                _ when path.StartsWith("/api/checklist") => "Checklist",
                _ when path.StartsWith("/api/dashboard") => "Dashboard",
                _ => "Unknown"
            };
        }

        private static string MapControllerToModule(string? controller)
        {
            return controller?.Trim() switch
            {
                "Settlements" => "Settlement",
                "DataPersistences" => "DataPersistence",
                "PeriodicFailovers" => "PeriodicFailover",
                "AuditSiteCreations" => "AuditSiteCreation",
                "AuditSiteCopies" => "AuditSiteCopy",
                "RestoreAuditSites" => "RestoreAuditSite",
                "DataAnalyses" => "DataAnalysis",
                "DataSamplings" => "DataSampling",
                "Audits" => "Audit",
                "Clients" => "Client",
                "Auditors" => "Auditor",
                "Findings" => "Finding",
                "Checklist" => "Checklist",
                "Checklists" => "Checklist",
                "Dashboard" => "Dashboard",
                _ => "Unknown"
            };
        }
    }
}
