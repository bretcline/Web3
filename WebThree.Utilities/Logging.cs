using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebThree.Shared
{
    public class Logging
    {

//        public static void LogAction(ExecutionContext context, Guid sessionId, HttpRequest request, Guid actionLogId)
//        {
//            try
//            {
//                var config = new ConfigurationBuilder().SetBasePath(context.FunctionAppDirectory)
//                    .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true).AddEnvironmentVariables()
//                    .Build();
//                var connString = config.GetConnectionString("ErrorLogDB");
//                if (!string.IsNullOrWhiteSpace(connString))
//                {
//                    using (var connection = new SqlConnection(connString))
//                    {
//                        connection.Open();

//                        var sql = @$"INSERT INTO [dbo].[ActionLog] 
//([ActionLogID],[ActionLogDate],[SessionID],[IpAddresses],[FunctionName],[Request])
//VALUES
//(@ActionLogId,GETUTCDATE(),@SessionId,@IpAddresses,@FunctionName,@Request)";

//                        using (var command = new SqlCommand(sql, connection))
//                        {
//                            var requestBody = "";
//                            if (request.Body.CanSeek)
//                            {
//                                request.Body.Position = 0;
//                                requestBody = new StreamReader(request.Body).ReadToEnd();
//                            }
//                            else
//                            {
//                                requestBody = request.QueryString.Value;
//                            }

//                            if (!context.FunctionName.Equals("VerifyUser"))
//                            {
//                                Regex regex = new Regex("\"Password\":\".+\"");
//                                requestBody = regex.Replace(requestBody, "\"Password\":\"#########\"");
//                            }

//                            command.Parameters.Add(new SqlParameter { ParameterName = "@ActionLogId", Value = actionLogId });
//                            command.Parameters.Add(new SqlParameter { ParameterName = "@SessionId", Value = sessionId });
//                            command.Parameters.Add(new SqlParameter { ParameterName = "@FunctionName", Value = $"{context.FunctionName} - {request.Host}{request.Path}" });
//                            command.Parameters.Add(new SqlParameter { ParameterName = "@Request", Value = requestBody });

//                            command.Parameters.Add(new SqlParameter { ParameterName = "@IpAddresses", Value = $"{request.HttpContext.Connection.RemoteIpAddress.ToString()} -> {request.HttpContext.Connection.LocalIpAddress.ToString()}" });

//                            command.ExecuteNonQuery();
//                        }
//                    }
//                }
//            }
//            catch (Exception e)
//            {
//            }
//        }


//        public static async void UpdateActionAsync(ExecutionContext context, Guid actionLogId, HttpRequest request, OkObjectResult result, long elapsedTime)
//        {
//            try
//            {
//                var config = new ConfigurationBuilder().SetBasePath(context.FunctionAppDirectory)
//                   .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true).AddEnvironmentVariables()
//                   .Build();
//                var connString = config.GetConnectionString("ErrorLogDB");
//                if (!string.IsNullOrWhiteSpace(connString))
//                {
//                    using (var connection = new SqlConnection(connString))
//                    {
//                        connection.Open();

//                        var sql = @$"UPDATE [dbo].[ActionLog] SET [Response] = @Response, ElapsedTime = @ElapsedTime WHERE ActionLogID = @ActionLogId";

//                        using (var command = new SqlCommand(sql, connection))
//                        {
//                            var responseBody = "";
//                            if (!context.FunctionName.StartsWith("Get")) // space saving logic...dont write all the data from a Get call.
//                            {
//                                responseBody = result?.Value?.ToString() ?? "";
//                            }
//                            command.Parameters.Add(new SqlParameter { ParameterName = "@ActionLogId", Value = actionLogId });
//                            command.Parameters.Add(new SqlParameter { ParameterName = "@Response", Value = responseBody });
//                            command.Parameters.Add(new SqlParameter { ParameterName = "@ElapsedTime", Value = elapsedTime });

//                            await command.ExecuteNonQueryAsync().ConfigureAwait(false);
//                        }
//                    }
//                }
//            }
//            catch (Exception e)
//            {
//                LogError(context, Guid.Empty, e, request);
//            }
//        }

    }
}
