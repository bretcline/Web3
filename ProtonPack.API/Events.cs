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
    public class Events
    {
        private readonly ILogger<Events> _logger;

        public Events(ILogger<Events> log)
        {
            _logger = log;
        }

        [FunctionName("eventGetAll")]
        [OpenApiOperation(operationId: "Events/GetAll", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(Event), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Event), Description = "The OK response")]
        public async Task<IActionResult> GetAll(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Events/GetAll")] HttpRequest req)
        {
            var rc = await Utilities.ValidateSession( req, async (CompanyUser companyUser, dynamic data) =>
            {
                using var manager = new BusinessLogic.EventManager(companyUser);
                return await manager.GetAll();
            }, (CompanyUser companyUser, Exception err) => Utilities.LogError(_logger, companyUser, err, req)).ConfigureAwait(false); 
            return rc;

        }

        [FunctionName("eventGet")]
        [OpenApiOperation(operationId: "Events/Get", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(Event), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Event), Description = "The OK response")]
        public async Task<IActionResult> Get(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Events/Get")] HttpRequest req)
        {
            var rc = await Utilities.ValidateSession(req, async (CompanyUser companyUser, dynamic data) =>
            {
                using var manager = new BusinessLogic.EventManager(companyUser);
                Guid eventId = WebThree.Shared.Utilities.GetGuid(data.Id);
                return await manager.Get(eventId);
            }, (CompanyUser companyUser, Exception err) => Utilities.LogError(_logger, companyUser, err, req)).ConfigureAwait(false);
            return rc;
        }

        [FunctionName("eventAdd")]
        [OpenApiOperation(operationId: "Events/Add", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(Event), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Event), Description = "The OK response")]
        public async Task<IActionResult> Add(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Events/Add")] HttpRequest req)
        {
            var rc = await Utilities.ValidateSession(req, async (CompanyUser companyUser, Event data) =>
            {
                using var manager = new BusinessLogic.EventManager(companyUser);
                return await manager.Add( data );
            }, (CompanyUser companyUser, Exception err) => Utilities.LogError(_logger, companyUser, err, req)).ConfigureAwait(false); 
            return rc;
        }

        [FunctionName("eventUpdate")]
        [OpenApiOperation(operationId: "Events/Update", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(Event), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Event), Description = "The OK response")]
        public async Task<IActionResult> Update(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Events/Update")] HttpRequest req)
        {
            var rc = await Utilities.ValidateSession(req, async (CompanyUser companyUser, Event data) =>
            {
                using var manager = new BusinessLogic.EventManager(companyUser);
                return await manager.Update(data);
            }, (CompanyUser companyUser, Exception err) => Utilities.LogError(_logger, companyUser, err, req)).ConfigureAwait(false);
            return rc;
        }

        [FunctionName("eventDelete")]
        [OpenApiOperation(operationId: "Events/Delete", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(Event), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Event), Description = "The OK response")]
        public async Task<IActionResult> Delete(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Events/Delete")] HttpRequest req)
        {
            var rc = await Utilities.ValidateSession(req, (CompanyUser companyUser, dynamic data) =>
            {
                using var manager = new BusinessLogic.EventManager(companyUser);
                Guid id = WebThree.Shared.Utilities.GetGuid(data.Id);
                manager.Delete(id);

                return true;

            }, (CompanyUser companyUser, Exception err) => Utilities.LogError(_logger, companyUser, err, req)).ConfigureAwait(false);
            return rc;
        }

    }
}

