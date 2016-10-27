using System.Web.Http;
using Roadkill.Core.Attachments;
using Roadkill.Core.Mvc.Attributes;
using Roadkill.Core.Mvc.Controllers;
using Roadkill.Core.Mvc.ViewModels;
using Roadkill.Core.Mvc.WebApi;
using Roadkill.Core.Mvc.WebViewPages;
using StructureMap;
using StructureMap.Graph;
using UserController = Roadkill.Core.Mvc.Controllers.UserController;

namespace Roadkill.Core.DependencyResolution.StructureMap.Registries
{
    public class MvcRegistry : Registry
    {
        public MvcRegistry()
        {
            Scan(ScanTypes);
            ConfigureInstances();
        }

        private void ScanTypes(IAssemblyScanner scanner)
        {
            scanner.AddAllTypesOf<UserViewModel>();
            scanner.AddAllTypesOf<SettingsViewModel>();
            scanner.AddAllTypesOf<AttachmentRouteHandler>();
            scanner.AddAllTypesOf<ISetterInjected>();
            scanner.AddAllTypesOf<IAuthorizationAttribute>();
            scanner.AddAllTypesOf<RoadkillLayoutPage>();
            scanner.AddAllTypesOf(typeof(RoadkillViewPage<>));
            scanner.ConnectImplementationsToTypesClosing(typeof(RoadkillViewPage<>));

            scanner.AddAllTypesOf<IRoadkillController>();
            scanner.AddAllTypesOf<ControllerBase>();
            scanner.AddAllTypesOf<ApiController>();
            scanner.AddAllTypesOf<ConfigurationTesterController>();
        }

        private void ConfigureInstances()
        {
            // AlwaysUnique is a work around for controllers that use RenderAction() needing to be unique
            // See https://github.com/webadvanced/Structuremap.MVC5/issues/3
            For<HomeController>().AlwaysUnique();
            For<UserController>().AlwaysUnique();
            For<ConfigurationTesterController>().AlwaysUnique();
            For<WikiController>().AlwaysUnique();

            ConfigureSetterInjection();
        }

        private void ConfigureSetterInjection()
        {
            Policies.SetAllProperties(x => x.OfType<ApiKeyAuthorizeAttribute>());
            Policies.SetAllProperties(x => x.OfType<ISetterInjected>());
            Policies.SetAllProperties(x => x.OfType<IAuthorizationAttribute>());
            Policies.SetAllProperties(x => x.TypeMatches(t => t == typeof(RoadkillViewPage<>)));
            Policies.SetAllProperties(x => x.TypeMatches(t => t == typeof(RoadkillLayoutPage)));
        }
    }
}