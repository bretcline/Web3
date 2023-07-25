using System;
using System.IO;
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
using Newtonsoft.Json;
using ProtonPack.Data;
using WebThree.Shared;
using static WebThree.Shared.Utilities;

namespace ProtonPack.API
{
    public class Customers
    {
        private readonly ILogger<Customers> _logger;

        public Customers(ILogger<Customers> log)
        {
            _logger = log;
        }

        [FunctionName("CustomerGetAll")]
        [OpenApiOperation(operationId: "Customer/GetAll", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(Customer), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Customer), Description = "The OK response")]
        public async Task<IActionResult> GetAll(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Customer/GetAll")] HttpRequest req)
        {
            var rc = await Utilities.ValidateSession(req, async (CompanyUser companyUser, dynamic data) =>
            {
                using var manager = new BusinessLogic.CustomerManager(companyUser);
                return await manager.GetAll();
            }, (CompanyUser companyUser, Exception err) => Utilities.LogError(_logger, companyUser, err, req)).ConfigureAwait(false);
            return rc;
        }

        [FunctionName("CustomerGetPaged")]
        [OpenApiOperation(operationId: "Customer/GetPaged", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(Customer), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Customer), Description = "The OK response")]
        public async Task<IActionResult> GetPaged(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Customer/GetPaged")] HttpRequest req)
        {
            var rc = await Utilities.ValidateSession(req, async (CompanyUser companyUser, dynamic data) =>
            {
                using var manager = new BusinessLogic.CustomerManager(companyUser);
                return await manager.GetPaged(Convert.ToInt32( data.PageIndex ), Convert.ToInt32( data.PageSize ));
            }, (CompanyUser companyUser, Exception err) => Utilities.LogError(_logger, companyUser, err, req)).ConfigureAwait(false);
            return rc;
        }

        [FunctionName("CustomerGet")]
        [OpenApiOperation(operationId: "Customer/Get", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(Customer), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Customer), Description = "The OK response")]
        public async Task<IActionResult> Get(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Customer/Get")] HttpRequest req)
        {
            var rc = await Utilities.ValidateSession(req, async (CompanyUser companyUser, dynamic data) =>
            {
                using var manager = new BusinessLogic.CustomerManager(companyUser);
                return await manager.Get(Utilities.GetGuid(data.Id));
            }, (CompanyUser companyUser, Exception err) => Utilities.LogError(_logger, companyUser, err, req)).ConfigureAwait(false);
            return rc;
        }

        [FunctionName("CustomerAdd")]
        [OpenApiOperation(operationId: "Customer/Add", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(Customer), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Customer), Description = "The OK response")]
        public async Task<IActionResult> Add(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Customer/Add")] HttpRequest req)
        {
            var rc = await Utilities.ValidateSession(req, async (CompanyUser companyUser, Customer data) =>
            {
                using var manager = new BusinessLogic.CustomerManager(companyUser);
                return await manager.Add( data );

            }, (CompanyUser companyUser, Exception err) => Utilities.LogError(_logger, companyUser, err, req)).ConfigureAwait(false);
            return rc;
        }

        [FunctionName("CustomerUpdate")]
        [OpenApiOperation(operationId: "Customer/Update", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(Customer), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Customer), Description = "The OK response")]
        public async Task<IActionResult> Update(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Customer/Update")] HttpRequest req)
        {
            var rc = await Utilities.ValidateSession(req, async (CompanyUser companyUser, Customer data) =>
            {
                using var manager = new BusinessLogic.CustomerManager(companyUser);
                return await manager.Update(data);
            }, (CompanyUser companyUser, Exception err) => Utilities.LogError(_logger, companyUser, err, req)).ConfigureAwait(false);
            return rc;
        }

        [FunctionName("CustomerDelete")]
        [OpenApiOperation(operationId: "Customer/Delete", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(Customer), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Customer), Description = "The OK response")]
        public async Task<IActionResult> Delete(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Customer/Delete")] HttpRequest req)
        {
            var rc = await Utilities.ValidateSession(req, (CompanyUser companyUser, dynamic data) =>
            {
                using var manager = new BusinessLogic.CustomerManager(companyUser);
                Guid id = WebThree.Shared.Utilities.GetGuid(data.Id);
                manager.Delete(id);
                return true;
            }, (CompanyUser companyUser, Exception err) => Utilities.LogError(_logger, companyUser, err, req)).ConfigureAwait(false);
            return rc;
        }



        [FunctionName("CustomerGetSummary")]
        [OpenApiOperation(operationId: "Customer/GetSummary", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(Customer), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Customer), Description = "The OK response")]
        public async Task<IActionResult> GetSummary(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Customer/GetSummary")] HttpRequest req)
        {
            var rc = await Utilities.ValidateSession(req, async (CompanyUser companyUser, dynamic data) =>
            {
                using var manager = new BusinessLogic.CustomerManager(companyUser);
                return await manager.GetSummary(Utilities.GetGuid(data.Id));
            }, (CompanyUser companyUser, Exception err) => Utilities.LogError(_logger, companyUser, err, req), false).ConfigureAwait(false);
            return rc;
        }
    }
}

