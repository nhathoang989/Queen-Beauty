﻿// Licensed to the Swastika I/O Foundation under one or more agreements.
// The Swastika I/O Foundation licenses this file to you under the GNU General Public License v3.0 license.
// See the LICENSE file in the project root for more information.

//using Messenger.Lib.SignalR.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace QueenBeauty
{
    public partial class Startup
    {
        public void ConfigureSignalRServices(IServiceCollection services)
        {
            //services.BuildServiceProvider();
            //services.AddSignalR();
        }

        public void ConfigurationSignalR(IApplicationBuilder app)
        {
            //app.UseSignalR(routes => routes.MapHub<MessengerHub>("Messenger"));
        }
    }
}