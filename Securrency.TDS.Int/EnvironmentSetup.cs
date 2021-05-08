using NUnit.Framework;

namespace Securrency.TDS.Int
{
    [SetUpFixture]
    public class EnvironmentSetup
    {
        [OneTimeSetUp]
        public void RunBeforeAnyTests()
        {
            Integration.StartWebApplication();
        }

        [OneTimeTearDown]
        public void RunAfterAnyTests()
        {
            Integration.StopWebApplication();
        }
    }
}
