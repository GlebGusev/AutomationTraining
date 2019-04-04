using System.Collections;
using Objectivity.Test.Automation.Tests.NUnit.DataDriven;

namespace Initialize.DataDriven
{
    public static class TestData
    {
        public static IEnumerable LoginCredentials
        {
            get { return DataDrivenHelper.ReadDataDriveFile(ProjectBaseConfiguration.DataDrivenFile, "credential", new[] { "user", "password" }, "LoginGmail"); }
        }

        public static IEnumerable LogoutCredentials
        {
            get { return DataDrivenHelper.ReadDataDriveFile(ProjectBaseConfiguration.DataDrivenFile, "credential", new[] { "user", "password" }, "LogoutGmail"); }
        }
    }
}