using System;
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
    public class Behaviors
    {
        private readonly ILogger<Companies> _logger;

        public Behaviors(ILogger<Companies> log)
        {
            _logger = log;
        }

        [FunctionName("behaviorsLike")]
        [OpenApiOperation(operationId: "Behaviors/Like", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(Company), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Company), Description = "The OK response")]
        public async Task<IActionResult> ProcessLike(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Behaviors/Like")] HttpRequest req)
        {
            var rc = await Utilities.ValidateSession(req, async (CompanyUser companyUser, UserAssetData data) =>
            {
                companyUser.CompanyOnly = true;

                var manager = new BusinessLogic.BehaviorManager(companyUser);
                return await manager.ProcessLike(data);
            }, (CompanyUser companyUser, Exception err) => Utilities.LogError(_logger, companyUser, err, req), false).ConfigureAwait(false);
            return rc;
        }


        [FunctionName("behaviorsComment")]
        [OpenApiOperation(operationId: "Behaviors/Comment", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(Company), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Company), Description = "The OK response")]
        public async Task<IActionResult> ProcessComment(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Behaviors/Comment")] HttpRequest req)
        {
            var rc = await Utilities.ValidateSession(req, async (CompanyUser companyUser, UserAssetData data) =>
            {
                companyUser.CompanyOnly = true;

                var manager = new BusinessLogic.BehaviorManager(companyUser);
                return await manager.ProcessComment(data);
            }, (CompanyUser companyUser, Exception err) => Utilities.LogError(_logger, companyUser, err, req), false).ConfigureAwait(false);
            return rc;
        }

        [FunctionName("behaviorsTip")]
        [OpenApiOperation(operationId: "Behaviors/Tip", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(Company), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Company), Description = "The OK response")]
        public async Task<IActionResult> ProcessTip(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Behaviors/Tip")] HttpRequest req)
        {
            var rc = await Utilities.ValidateSession(req, async (CompanyUser companyUser, UserTipData data) =>
            {
                companyUser.CompanyOnly = true;

                var manager = new BusinessLogic.BehaviorManager(companyUser);
                return await manager.ProcessTip(data);
            }, (CompanyUser companyUser, Exception err) => Utilities.LogError(_logger, companyUser, err, req), false).ConfigureAwait(false);
            return rc;
        }

    }

}

