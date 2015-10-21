// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace RefMService
{
    using System.Web.Http;

    public static class RouteConfig
    {
        /// <summary>
        ///     Routing registration.
        /// </summary>
        /// <param name="routes"></param>
        public static void RegisterRoutes(HttpRouteCollection routes)
        {
            routes.MapHttpRoute(
                name: "Default",
                routeTemplate: "{action}",
                defaults: new { controller = "Default", action = "Index" },
                constraints: new { }
                );

            routes.MapHttpRoute(
               name: "GetAllByDomain",
               routeTemplate: "entities/domain/{domain}",
               defaults: new { controller = "RefM", action = "GetAllByDomain" },
               constraints: new { }
               );

            routes.MapHttpRoute(
               name: "GetByCode",
               routeTemplate: "entities/code/{code}",
               defaults: new { controller = "RefM", action = "GetByCode" },
               constraints: new { }
               );

            routes.MapHttpRoute(
               name: "GetByCodeValue",
               routeTemplate: "entities/codevalue/{codevalue}",
               defaults: new { controller = "RefM", action = "GetByCodeValue" },
               constraints: new { }
               );

            routes.MapHttpRoute(
               name: "GetAllByLink",
               routeTemplate: "entities/link/{link}",
               defaults: new { controller = "RefM", action = "GetAllByLink" },
               constraints: new { }
               );
        }
    }
}