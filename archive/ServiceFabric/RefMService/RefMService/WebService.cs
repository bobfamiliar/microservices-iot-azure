// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace RefMService
{
    using Microsoft.ServiceFabric.Services;

    public class RefMService : StatelessService
    {
        public const string ServiceTypeName = "RefMServiceType";

        protected override ICommunicationListener CreateCommunicationListener()
        {
            return new OwinCommunicationListener("refm", new Startup());
        }
    }
}