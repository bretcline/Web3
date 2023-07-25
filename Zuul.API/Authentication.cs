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
using WebThree.Shared;
using Zuul.BusinessLogic.Data;

namespace Zuul.API
{
    public class 
        Authentication
    {
        private readonly ILogger<Authentication> _logger;

        public Authentication(ILogger<Authentication> log)
        {
            _logger = log;
        }

        [FunctionName("Login")]
        [OpenApiOperation(operationId: "Login", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> Login(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Auth/Login")] HttpRequest req)
        {
            try
            {
                _logger.LogDebug("Auth-Login");
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync().ConfigureAwait(false);

                var data = JsonConvert.DeserializeObject<UidPwd>(requestBody, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore, DateTimeZoneHandling = DateTimeZoneHandling.Utc });

                var auth = new BusinessLogic.Authentication();
                var responseMessage = auth.Login(data);

                GenerateJWTToken generateJWTToken = new();
                responseMessage.Token = generateJWTToken.IssuingJWT(responseMessage);
                //return await Task.FromResult(new OkObjectResult(token)).ConfigureAwait(false);

                return new OkObjectResult(responseMessage);
            }
            catch( Exception e )
            {
                _logger.LogError($"Auth-Login : {e.Message}");
                return new BadRequestObjectResult( e.Message );
            }
        }
    }
}

