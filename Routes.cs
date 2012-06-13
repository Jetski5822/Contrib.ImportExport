using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using Orchard.Mvc.Routes;

namespace Contrib.ImportExport {
    public class Routes : IRouteProvider {

        public void GetRoutes(ICollection<RouteDescriptor> routes) {
            foreach (var routeDescriptor in GetRoutes())
                routes.Add(routeDescriptor);
        }

        public IEnumerable<RouteDescriptor> GetRoutes() {
            return new[] {
                             new RouteDescriptor {
                                                     Priority = 10,
                                                     Route = new Route(
                                                         "Admin/Blogs/Import",
                                                         new RouteValueDictionary {
                                                                                      {"area", "Contrib.ImportExport"},
                                                                                      {"controller", "Admin"},
                                                                                      {"action", "Import"}
                                                                                  },
                                                         new RouteValueDictionary(),
                                                         new RouteValueDictionary {
                                                                                      {"area", "Contrib.ImportExport"}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                         };
        }
    }
}