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
    public class Assets
    {
        private readonly ILogger<Assets> _logger;

        public Assets(ILogger<Assets> log)
        {
            _logger = log;
        }

        [FunctionName("AssetGetAll")]
        [OpenApiOperation(operationId: "Asset/GetAll", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(Asset), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Asset), Description = "The OK response")]

        public async Task<IActionResult> GetAll(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Asset/GetAll")] HttpRequest req)

        {
            var rc = await Utilities.ValidateSession(req, async (CompanyUser companyUser, dynamic data) =>
            {
                using var manager = new BusinessLogic.AssetManager(companyUser);
                return await manager.GetAll();

            }, (CompanyUser companyUser, Exception err) => Utilities.LogError(_logger, companyUser, err, req)).ConfigureAwait(false);
            return rc;
        }

       [FunctionName("AssetGetAllByContractType")]
        [OpenApiOperation(operationId: "Asset/GetAllByContractType", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(Asset), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Asset), Description = "The OK response")]

        public async Task<IActionResult> GetAllByContractType(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Asset/GetAllByContractType")] HttpRequest req)

        {
            var rc = await Utilities.ValidateSession(req, (CompanyUser companyUser, dynamic data) =>
            {
                using var manager = new BusinessLogic.AssetManager(companyUser);
                return manager.GetByAssetType(Utilities.GetGuid(data));

            }, (CompanyUser companyUser, Exception err) => Utilities.LogError(_logger, companyUser, err, req)).ConfigureAwait(false);
            return rc;
        }

        [FunctionName("AssetGet")]
        [OpenApiOperation(operationId: "Asset/Get", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(Asset), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Asset), Description = "The OK response")]
        public async Task<IActionResult> Get(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Asset/Get")] HttpRequest req)
        {
            var rc = await Utilities.ValidateSession(req, async (CompanyUser companyUser, dynamic data) =>
            {

                using var manager = new BusinessLogic.AssetManager(companyUser);
                return await manager.Get(Utilities.GetGuid(data.Id));

            }, (CompanyUser companyUser, Exception err) => Utilities.LogError(_logger, companyUser, err, req)).ConfigureAwait(false);
            return rc;
        }


        [FunctionName("AssetGetPaged")]
        [OpenApiOperation(operationId: "Asset/GetPaged", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(Asset), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Asset), Description = "The OK response")]
        public async Task<IActionResult> GetPaged(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Asset/GetPaged")] HttpRequest req)
        {
            var rc = await Utilities.ValidateSession(req, async (CompanyUser companyUser, dynamic data) =>
            {
                using var manager = new BusinessLogic.AssetManager(companyUser);
                return await manager.GetPaged(Convert.ToInt32(data.PageIndex), Convert.ToInt32(data.PageSize));
            }, (CompanyUser companyUser, Exception err) => Utilities.LogError(_logger, companyUser, err, req)).ConfigureAwait(false);
            return rc;
        }


        [FunctionName("AssetAdd")]
        [OpenApiOperation(operationId: "Asset/Add", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(Asset), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Asset), Description = "The OK response")]
        public async Task<IActionResult> Add(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Asset/Add")] HttpRequest req)
        {
            var rc = await Utilities.ValidateSession(req, async (CompanyUser companyUser, Asset data) =>
            {
                using var manager = new BusinessLogic.AssetManager(companyUser);
                return await manager.Add(data);

            }, (CompanyUser companyUser, Exception err) => Utilities.LogError(_logger, companyUser, err, req)).ConfigureAwait(false);
            return rc;
        }

        [FunctionName("AssetUpdate")]
        [OpenApiOperation(operationId: "Asset/Update", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(Asset), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Asset), Description = "The OK response")]
        public async Task<IActionResult> Update(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Asset/Update")] HttpRequest req)
        {
            var rc = await Utilities.ValidateSession(req, async (CompanyUser companyUser, Asset data) =>
            {
                using var manager = new BusinessLogic.AssetManager(companyUser);
                return await manager.Update(data);
            }, (CompanyUser companyUser, Exception err) => Utilities.LogError(_logger, companyUser, err, req)).ConfigureAwait(false);
            return rc;
        }

        [FunctionName("AssetDelete")]
        [OpenApiOperation(operationId: "Asset/Delete", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(Asset), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Asset), Description = "The OK response")]
        public async Task<IActionResult> Delete(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Asset/Delete")] HttpRequest req)
        {
            var rc = await Utilities.ValidateSession(req, (CompanyUser companyUser, dynamic data) =>
            {
                using var manager = new BusinessLogic.AssetManager(companyUser);
                Guid id = WebThree.Shared.Utilities.GetGuid(data.Id);
                manager.Delete(id);
                return true;
            }, (CompanyUser companyUser, Exception err) => Utilities.LogError(_logger, companyUser, err, req)).ConfigureAwait(false);
            return rc;
        }
    }
}