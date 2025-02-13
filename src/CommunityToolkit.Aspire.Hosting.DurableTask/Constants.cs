using System.Runtime.InteropServices;

namespace CommunityToolkit.Aspire.Hosting.DurableTask;

static class Constants
{
    public static class Scheduler
    {
        public static class Dashboard
        {
            public static readonly Uri Endpoint = new Uri("https://dashboard.durabletask.io");
        }

        public static class Emulator
        {
            public static class Container
            {
                public const string Image = "mcr.microsoft.com/durable-task/dts-emulator";

                public static string Tag =>
                    RuntimeInformation.OSArchitecture == Architecture.Arm64
                        ? "latest-arm64"
                        : "latest-amd64";
            }

            public static class Endpoints
            {
                public const string Worker = "worker";
                public const string Dashboard = "dashboard";
            }
        }

        public static class TaskHub
        {
            public const string DefaultName = "default";
        }
    }
}