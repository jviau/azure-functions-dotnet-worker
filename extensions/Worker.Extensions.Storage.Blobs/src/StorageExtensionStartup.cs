﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.Functions.Worker.Converters;
using Microsoft.Azure.Functions.Worker.Core;
using Microsoft.Azure.Functions.Worker.Extensions.Storage.Blobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

[assembly: WorkerExtensionStartup(typeof(StorageExtensionStartup))]

namespace Microsoft.Azure.Functions.Worker.Extensions.Storage.Blobs
{
    public class StorageExtensionStartup : WorkerExtensionStartup
    {
        public override void Configure(IFunctionsWorkerApplicationBuilder applicationBuilder)
        {
            applicationBuilder.Services.AddSingleton<BlobStorageConverter>();
        }
    }
}
