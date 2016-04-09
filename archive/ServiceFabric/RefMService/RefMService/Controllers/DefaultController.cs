// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace RefMService.Controllers
{
    using System.Web.Http;

    /// <summary>
    ///     Default controller.
    /// </summary>
    public class DefaultController : ApiController
    {
        public IHttpActionResult Get()
        {
            return this.Ok("Hello World!");
        }
    }
}