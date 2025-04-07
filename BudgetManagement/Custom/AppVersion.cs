using System.Reflection;

namespace ExpenseManagment.Custom
{
    public static class AppVersion
    {
        public static string GetAppVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
    }
}
