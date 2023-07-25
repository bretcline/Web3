﻿using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using ProtonPack.Data;
using WebThree.Shared;
using static WebThree.Shared.Utilities;

namespace ProtonPack.API
{
    public class Registrations
    {
        private readonly ILogger<Registrations> _logger;

        public Registrations(ILogger<Registrations> log)
        {
            _logger = log;
        }

        [FunctionName("RegistrationsSignup")]
        [OpenApiOperation(operationId: "Registrations/Signup", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(Company), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Company), Description = "The OK response")]
        public async Task<IActionResult> Signup(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Registrations/Signup")] HttpRequest req)
        {
            var rc = await Utilities.ValidateSession(req, async (CompanyUser companyUser, dynamic data) =>
            {
                using var manager = new BusinessLogic.AssetManager(companyUser);
                var assets = await manager.GetAllByWalletAddress(data);

                return assets;

            }, (CompanyUser companyUser, Exception err) => Utilities.LogError(_logger, companyUser, err, req)).ConfigureAwait(false);
            return rc;
        }

        [FunctionName("RegistrationsHello")]
        [OpenApiOperation(operationId: "Registrations/Hello", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(Group), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Group), Description = "The OK response")]
        public async Task<IActionResult> Hello(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Registrations/Hello")] HttpRequest req)
        {
            var rc = await Utilities.ValidateSession(req, (CompanyUser companyUser, dynamic data) =>
            {
                return "Hello Johnny!!";
            }, (CompanyUser companyUser, Exception err) => Utilities.LogError(_logger, companyUser, err, req)).ConfigureAwait(false);
            return rc;
        }
    }
}

