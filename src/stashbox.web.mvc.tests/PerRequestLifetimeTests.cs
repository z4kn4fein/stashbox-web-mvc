using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stashbox;
using Stashbox.Web.Mvc;

namespace stashbox.web.mvc.tests
{
    [TestClass]
    public class PerRequestLifetimeTests
    {
        [TestMethod]
        public void PerRequestLifetimeTests_Resolve()
        {
            var container = new StashboxContainer();
            container.PrepareType<ITest, Test>().WithLifetime(new PerRequestLifetime()).Register();

            using (container.BeginScope())
            {
                var inst1 = container.Resolve<ITest>();
                var inst2 = container.Resolve<ITest>();

                Assert.AreSame(inst1, inst2);
            }
        }

        public interface ITest { }

        public class Test : ITest { }
    }
}
