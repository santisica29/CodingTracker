using System.Configuration;

namespace CodingTracker;
internal static class Helpers
{
    internal static string GetConnectionString()
    {
        return ConfigurationManager.AppSettings["ConnectionString"];
    } 
}
