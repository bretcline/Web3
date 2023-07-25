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
using ProtonPack.BusinessLogic;
using ProtonPack.Data;
using WebThree.Shared;
using static WebThree.Shared.Utilities;

namespace ProtonPack.API
{
    public class Blockchain
    {
        private readonly ILogger<Companies> _logger;

        public Blockchain(ILogger<Companies> log)
        {
            _logger = log;
        }


        //[FunctionName("QueueCleanup")]
        //public static void QueueCleanup([TimerTrigger("0 */1 * * * *")] TimerInfo myTimer, ILogger log) // every 1 minutes
        //{
            // TODO: HACK - This should loop through all of the companies and call cleanup for each...or at least each that uses the queue.

        //    var cu = new CompanyUser { CompanyId = new Guid("4C3E33FB-889A-4908-9798-FF368A29254C"), CompanyOnly = true };

        //    var auth = new BlockchainManager(cu);
        //    auth.QueueCleanup();
        //}

        ////#if !DEBUG
        //[FunctionName("CryptoPunkWalker")]
        //public static void CryptoPunkWalker([TimerTrigger("0 0 * * * *")] TimerInfo myTimer, ILogger log) // every 1 minutes
        //{
        //    // TODO: HACK - This should loop through all of the companies and call cleanup for each...or at least each that uses the queue.

        //    var cu = new CompanyUser { CompanyId = new Guid("4C3E33FB-889A-4908-9798-FF368A29254C"), CompanyOnly = true };
        //    var auth = new BlockchainManager(cu);
        //    auth.WalkCryptoPunks();
        //}
        //#endif

        [FunctionName("blockchainVerify")]
        [OpenApiOperation(operationId: "Blockchain/Verify", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(Company), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Company), Description = "The OK response")]
        public async Task<IActionResult> Verify(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Blockchain/Verify")] HttpRequest req)
        {
            try
            {
                _logger.LogDebug("Verify");
                req.Headers.TryGetValue("ContractType", out var assetTypeString);

                var contractType = Guid.Empty;
                if (!req.Headers.TryGetValue("CompanyId", out var companyIdString))
                {
                    throw new MissingCompanyIdException();
                }
                if (!string.IsNullOrEmpty(assetTypeString))
                {
                    contractType = new Guid(assetTypeString);
                }

                _logger.LogDebug($"Verify {assetTypeString}");

                var cu = new CompanyUser { CompanyId = new Guid(companyIdString), CompanyOnly = true };

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync().ConfigureAwait(false);
                dynamic data = JsonConvert.DeserializeObject(requestBody, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore, DateTimeZoneHandling = DateTimeZoneHandling.Utc });

                var auth = new BlockchainManager(cu);
                var responseMessage = auth.AuthenticateAndQueue(data.WalletAddress.ToString(), cu.CompanyId, contractType);

                return new OkObjectResult(responseMessage);
            }
            catch (Exception e)
            {
                return new UnauthorizedObjectResult(e.Message);
            }
        }


        [FunctionName("blockchainCheckAdmin")]
        [OpenApiOperation(operationId: "Blockchain/CheckAdmin", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(Company), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Company), Description = "The OK response")]
        public async Task<IActionResult> CheckAdmin(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Blockchain/CheckAdmin")] HttpRequest req)
        {
            try
            {
                var contractType = Guid.Empty;
                if (!req.Headers.TryGetValue("CompanyId", out var companyIdString))
                {
                    throw new MissingCompanyIdException();
                }
                if (req.Headers.TryGetValue("ContractType", out var assetTypeString))
                {
                    contractType = new Guid(assetTypeString);
                }
                var cu = new CompanyUser { CompanyId = new Guid(companyIdString), CompanyOnly = true };

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync().ConfigureAwait(false);
                dynamic data = JsonConvert.DeserializeObject(requestBody, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore, DateTimeZoneHandling = DateTimeZoneHandling.Utc });

                var auth = new BlockchainManager(cu);
                var responseMessage = auth.CheckAdministrativeRights(data.WalletAddress.ToString(), cu.CompanyId, contractType);

                return new OkObjectResult(responseMessage);
            }
            catch (Exception e)
            {
                return new UnauthorizedObjectResult(e.Message);
            }
        }

        [FunctionName("blockchainUpdateEnvironment")]
        [OpenApiOperation(operationId: "Blockchain/UpdateEnvironment", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(Company), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Company), Description = "The OK response")]
        public async Task<IActionResult> UpdateEnvironment(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Blockchain/UpdateEnvironment")] HttpRequest req)
        {

            var rc = await Utilities.ValidateSession(req, async (CompanyUser companyUser, EnvironmentData data) =>
            {
                using var manager = new BusinessLogic.CompanyManager(companyUser);
                return await manager.UpdateEnvironment(data);
            }, (CompanyUser companyUser, Exception err) => Utilities.LogError(_logger, companyUser, err, req), true).ConfigureAwait(false);
            return rc;
        }


        [FunctionName("blockchainManualDequeue")]
        [OpenApiOperation(operationId: "Blockchain/ManualDequeue", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(Company), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Company), Description = "The OK response")]
        public async Task<IActionResult> ManualDequeue(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Blockchain/ManualDequeue")] HttpRequest req)
        {

            var rc = await Utilities.ValidateSession(req, async (CompanyUser companyUser, dynamic data) =>
            {
                bool clearAll = data?.All ?? false;

                var auth = new BlockchainManager(companyUser);
                return await auth.ManualDequeue(clearAll);
            }, (CompanyUser companyUser, Exception err) => Utilities.LogError(_logger, companyUser, err, req), true).ConfigureAwait(false);
            return rc;
        }


        [FunctionName("blockchainVerifyOwnership")]
        [OpenApiOperation(operationId: "Blockchain/VerifyOwnership", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(Company), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Company), Description = "The OK response")]
        public async Task<IActionResult> VerifyOwnership(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Blockchain/VerifyOwnership")] HttpRequest req)
        {
            try
            {
                _logger.LogDebug("Verify Ownership");

                var origin = string.Empty;
                var fromWebThree = false;

                foreach( var key in req.Headers.Keys )
                {
                    if( key == "Origin" && 
                        ( 
                        req.Headers[key].ToString().EndsWith("billmurray1000.com", StringComparison.InvariantCultureIgnoreCase ) ||
                        req.Headers[key].ToString().EndsWith("venkmanholdings.com", StringComparison.InvariantCultureIgnoreCase ) ||
                        req.Headers[key].ToString().Contains("localhost:", StringComparison.InvariantCultureIgnoreCase)) )
                    {
                        _logger.LogDebug($"FOUND IT! {key} - {req.Headers[key]}");
                        fromWebThree = true;
                    }
                }

                if (!req.Headers.TryGetValue("Origin", out var originString))
                {
                    _logger.LogDebug($"Origin {originString}");
                    origin = originString.ToString();
                    if( origin.Contains("venkmanholdings.com", StringComparison.InvariantCultureIgnoreCase) || origin.Contains("billmurray1000.com", StringComparison.InvariantCultureIgnoreCase))
                    {
                        fromWebThree = true;
                    }
                }

                _logger.LogDebug($"Origin {origin} - From Venk {fromWebThree}");
                if ( fromWebThree )
                {
                    var contractType = Guid.Empty;
                    if (!req.Headers.TryGetValue("CompanyId", out var companyIdString))
                    {
                        throw new MissingCompanyIdException();
                    }
                    if (req.Headers.TryGetValue("ContractType", out var assetContractType))
                    {
                        contractType = new Guid(assetContractType);
                    }
                    var cu = new CompanyUser { CompanyId = new Guid(companyIdString), CompanyOnly = true, ForceAuthDb = true };

                    string requestBody = await new StreamReader(req.Body).ReadToEndAsync().ConfigureAwait(false);


                    var data = JsonConvert.DeserializeObject<CryptoAddress>(requestBody, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore, DateTimeZoneHandling = DateTimeZoneHandling.Utc });
                    var auth = new BlockchainManager(cu);

                    _logger.LogDebug($"Verify Ownership - {data.WalletAddress}, {cu.CompanyId}, {contractType}");

                    var responseMessage = auth.VerifyOwnership(data, cu.CompanyId, contractType);

                    _logger.LogDebug(JsonConvert.SerializeObject(responseMessage));

                    return new OkObjectResult(responseMessage);
                }
                else
                    throw new WebThreeException("Invalid Request. Origin is not from Project WebThree");
            }
            catch (Exception e)
            {
                _logger.LogDebug($"ERROR: {e.Message}");
                _logger.LogDebug($"ERROR: {e.StackTrace}");
                return new UnauthorizedObjectResult(e.Message);
            }
        }


        [FunctionName("blockchainConfirm")]
        [OpenApiOperation(operationId: "Blockchain/Confirm", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(Company), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Company), Description = "The OK response")]
        public async Task<IActionResult> Confirm(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Blockchain/Confirm")] HttpRequest req)
        {
            try
            {
                _logger.LogDebug("Confirm");
                
                if (!req.Headers.TryGetValue("CompanyId", out var companyIdString))
                {
                    throw new MissingCompanyIdException();
                }
                if (!req.Headers.TryGetValue("ContractType", out var contractTypeIDString))
                {
                    throw new MissingContractTypeException();
                }

                _logger.LogDebug($"Confirm Company  |{companyIdString}|");
                _logger.LogDebug($"Confirm Contract |{contractTypeIDString}|");


                var cu = new CompanyUser { CompanyId = new Guid(companyIdString), ContractTypeId = new Guid(contractTypeIDString), CompanyOnly = true };

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync().ConfigureAwait(false);
                dynamic data = JsonConvert.DeserializeObject(requestBody, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore, DateTimeZoneHandling = DateTimeZoneHandling.Utc });

                _logger.LogDebug($"VerifyQueueItem {data.QueueToken}");
                var auth = new BlockchainManager(cu);
                var responseMessage = auth.VerifyQueueItem(GetGuid(data.QueueToken));

                return new OkObjectResult(responseMessage);
            }
            catch (Exception e)
            {
                _logger.LogDebug($"Error {e.Message} {e.StackTrace}");
                return new BadRequestObjectResult(e.Message);
            }
        }

        [FunctionName("blockchainDequeue")]
        [OpenApiOperation(operationId: "Blockchain/Dequeue", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(Company), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Company), Description = "The OK response")]
        public async Task<IActionResult> Dequeue(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Blockchain/Dequeue")] HttpRequest req)
        {
            try
            {
                if (!req.Headers.TryGetValue("CompanyId", out var companyIdString))
                {
                    throw new MissingCompanyIdException();
                }
                var cu = new CompanyUser { CompanyId = new Guid(companyIdString), CompanyOnly = true };

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync().ConfigureAwait(false);
                dynamic data = JsonConvert.DeserializeObject(requestBody, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore, DateTimeZoneHandling = DateTimeZoneHandling.Utc });

                var auth = new BlockchainManager(cu);
                var responseMessage = auth.DequeueToken(GetGuid(data.QueueToken));

                return new OkObjectResult(responseMessage);
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(e.Message);
            }
        }


        [FunctionName("blockchainCheckPosition")]
        [OpenApiOperation(operationId: "Blockchain/CheckPosition", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(Company), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Company), Description = "The OK response")]
        public async Task<IActionResult> CheckPosition(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Blockchain/CheckPosition")] HttpRequest req)
        {
            try
            {
                if (!req.Headers.TryGetValue("CompanyId", out var companyIdString))
                {
                    throw new MissingCompanyIdException();
                }
                if (!req.Headers.TryGetValue("ContractType", out var contractTypeIDString))
                {
                    throw new MissingContractTypeException();
                }

                var contractTypeID = GetGuid(contractTypeIDString);
                var cu = new CompanyUser { CompanyId = new Guid(companyIdString), ContractTypeId = contractTypeID, CompanyOnly = true };

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync().ConfigureAwait(false);
                dynamic data = JsonConvert.DeserializeObject(requestBody, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore, DateTimeZoneHandling = DateTimeZoneHandling.Utc });

                var auth = new BlockchainManager(cu);
                var responseMessage = auth.GetQueuePosition(GetGuid(data.QueueToken));

                return new OkObjectResult(responseMessage);
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(e.Message);
            }
        }


        [FunctionName("blockchainGetTopToken")]
        [OpenApiOperation(operationId: "Blockchain/GetTopToken", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(Company), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Company), Description = "The OK response")]
        public async Task<IActionResult> GetTopToken(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Blockchain/GetTopToken")] HttpRequest req)
        {
            try
            {
                if (!req.Headers.TryGetValue("CompanyId", out var companyIdString))
                {
                    throw new MissingCompanyIdException();
                }
                var cu = new CompanyUser { CompanyId = new Guid(companyIdString), CompanyOnly = true };

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync().ConfigureAwait(false);
                dynamic data = JsonConvert.DeserializeObject(requestBody, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore, DateTimeZoneHandling = DateTimeZoneHandling.Utc });

                var auth = new BlockchainManager(cu);
                var responseMessage = auth.GetTopToken();

                return new OkObjectResult(responseMessage);
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(e.Message);
            }
        }

        public class ChangeOwner
        {
            public string assetNumber { get; set; }
            public string walletAddress { get; set; }
        }


        [FunctionName("blockchainUpdateOwner")]
        [OpenApiOperation(operationId: "Blockchain/UpdateOwner", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(Company), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Company), Description = "The OK response")]
        public async Task<IActionResult> UpdateOwner(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Blockchain/UpdateOwner")] HttpRequest req)
        {
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync().ConfigureAwait(false);
                var data = JsonConvert.DeserializeObject<ChangeOwner>(requestBody, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore, DateTimeZoneHandling = DateTimeZoneHandling.Utc });

                if (!req.Headers.TryGetValue("CompanyId", out var companyIdString))
                {
                    throw new MissingCompanyIdException();
                }
                var cu = new CompanyUser { CompanyId = new Guid(companyIdString), CompanyOnly = true };

                var manager = new BlockchainManager(cu);

                var responseMessage = manager.UpdateOwner(data.assetNumber, data.walletAddress);
                return new OkObjectResult(responseMessage);
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(e.Message);
            }
        }


        [FunctionName("blockchainEnvironmentData")]
        [OpenApiOperation(operationId: "Blockchain/EnvironmentData", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(Company), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Company), Description = "The OK response")]
        public async Task<IActionResult> EnvironmentData(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Blockchain/EnvironmentData")] HttpRequest req)
        {
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync().ConfigureAwait(false);
                var data = JsonConvert.DeserializeObject<ChangeOwner>(requestBody, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore, DateTimeZoneHandling = DateTimeZoneHandling.Utc });

                if (!req.Headers.TryGetValue("CompanyId", out var companyIdString))
                {
                    throw new MissingCompanyIdException();
                }
                var cu = new CompanyUser { CompanyId = new Guid(companyIdString), CompanyOnly = true };

                var manager = new CompanyManager(cu);

                var responseMessage = manager.Get(cu.CompanyId).GetAwaiter().GetResult();

                return new OkObjectResult(responseMessage.CompanyData);
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(e.Message);
            }
        }

        [FunctionName("blockchainWalkCryptoPunks")]
        [OpenApiOperation(operationId: "Blockchain/WalkCryptoPunks", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(Company), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Company), Description = "The OK response")]
        public async Task<IActionResult> WalkCryptoPunks(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Blockchain/WalkCryptoPunks")] HttpRequest req)
        {
            try
            {
                var contractType = Guid.Empty;
                if (!req.Headers.TryGetValue("CompanyId", out var companyIdString))
                {
                    throw new MissingCompanyIdException();
                }
                if (req.Headers.TryGetValue("ContractType", out var assetContractType))
                {
                    contractType = new Guid(assetContractType);
                }
                var cu = new CompanyUser { CompanyId = new Guid(companyIdString), CompanyOnly = true };

                var auth = new BlockchainManager(cu);
                auth.WalkCryptoPunks();

                return new OkObjectResult("Ok");
            }
            catch (Exception e)
            {
                return new UnauthorizedObjectResult(e.Message);
            }
        }


        [FunctionName("blockchainWalkBAYC")]
        [OpenApiOperation(operationId: "Blockchain/WalkCryptoPunks", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(Company), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Company), Description = "The OK response")]
        public async Task<IActionResult> WalkBAYC(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Blockchain/WalkBAYC")] HttpRequest req)
        {
            try
            {
                var contractType = Guid.Empty;
                if (!req.Headers.TryGetValue("CompanyId", out var companyIdString))
                {
                    throw new MissingCompanyIdException();
                }
                if (req.Headers.TryGetValue("ContractType", out var assetContractType))
                {
                    contractType = new Guid(assetContractType);
                }
                var cu = new CompanyUser { CompanyId = new Guid(companyIdString), CompanyOnly = true };

                var auth = new BlockchainManager(cu);
                auth.WalkCryptoPunks();

                return new OkObjectResult("Ok");
            }
            catch (Exception e)
            {
                return new UnauthorizedObjectResult(e.Message);
            }
        }

        [FunctionName("blockchainGetContractTypes")]
        [OpenApiOperation(operationId: "Blockchain/GetContractTypes", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(Company), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Company), Description = "The OK response")]
        public async Task<IActionResult> GetContractTypes(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Blockchain/GetContractTypes")] HttpRequest req)
        {
            var rc = await Utilities.ValidateSession(req, async (CompanyUser companyUser, dynamic data) =>
            {
                var manager = new BusinessLogic.BlockchainManager(companyUser);
                return await manager.GetByContractType();
            }, (CompanyUser companyUser, Exception err) => Utilities.LogError(_logger, companyUser, err, req)).ConfigureAwait(false);
            return rc;
        }

        [FunctionName("blockchainWalkNfts")]
        [OpenApiOperation(operationId: "Blockchain/WalkNfts", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(Company), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Company), Description = "The OK response")]
        public async Task<IActionResult> WalkNfts(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Blockchain/WalkNfts")] HttpRequest req)
        {
            try
            {
                var contractType = Guid.Empty;
                if (!req.Headers.TryGetValue("CompanyId", out var companyIdString))
                {
                    throw new MissingCompanyIdException();
                }
                if (req.Headers.TryGetValue("ContractType", out var assetContractType))
                {
                    contractType = new Guid(assetContractType);
                }
                var cu = new CompanyUser { CompanyId = new Guid(companyIdString), ContractTypeId = contractType, CompanyOnly = true };

                var auth = new BlockchainManager(cu);
                auth.WalkContract(contractType);

                return new OkObjectResult("Ok");
            }
            catch (Exception e)
            {
                return new UnauthorizedObjectResult(e.Message);
            }
        }
    }
}

