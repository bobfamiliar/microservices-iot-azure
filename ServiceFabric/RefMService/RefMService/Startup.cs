// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace RefMService
{
    using System.Web.Http;
    using Owin;

    public class Startup : IOwinAppBuilder
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            HttpConfiguration config = new HttpConfiguration();

            FormatterConfig.ConfigureFormatters(config.Formatters);
            RouteConfig.RegisterRoutes(config.Routes);

            appBuilder.UseWebApi(config);
        }
    }
}