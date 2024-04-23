using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using MinimalChatApp.Model;
using MinimalChatApplication.Model;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MinimalChatApp.Common
{
    public class RequestLoggingMiddleware
    {

        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;


        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger, IHttpContextAccessor httpContextAccessor,IConfiguration configuration)
        {
            _next = next;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }

        public async Task Invoke(HttpContext context, ChatDBContext dbcontext)
        {
            try
            {
                //string token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

                //if (token != null)
                //{
                //    var principal = ValidateToken(token);
                //    if (principal != null)
                //    {
                //        context.User = principal;
                //    }
                //}
                LogRequest(context, dbcontext);
                using var buffer = new MemoryStream();

                var response = context.Response;

                //response.Headers.Append("Access-Control-Allow-Origin", "*");
                //response.Headers.Append("Access-Control-Allow-Headers", "*");
                //response.Headers.Append("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE");
                var stream = response.Body;
                response.Body = buffer;
                await _next(context);
                buffer.Position = 0;
                await buffer.CopyToAsync(stream);

            }
            catch (Exception ex)
            {
                
                var errorInfo = new ErrorInfo()
                {
                    StatusCode = context.Response.StatusCode,
                    ErrorMessage = ex.Message
                };

                var errorLog = new ErrorLogger()
                {
                    ErrorDetails = errorInfo.ErrorMessage,
                    LogDate = DateTime.Now
                };
                await dbcontext.ErrorLogs.AddAsync(errorLog);
                await dbcontext.SaveChangesAsync();
                using var buffer = new MemoryStream();
                var response = context.Response;

                var stream = response.Body;
                response.Body = buffer;
                await _next(context);
                buffer.Position = 0;
                await buffer.CopyToAsync(stream);

            }
        }
        private void LogRequest(HttpContext context, ChatDBContext dbcontext)
        {
            string username = _httpContextAccessor.HttpContext.User.Identity.Name;

            var request = context.Request;
            var method = request.Method;
            var path = request.Path;
            var queryString = request.QueryString;

            var ipAddress = context.Connection.RemoteIpAddress.ToString();


            string requestBody =  ReadRequestBody(context.Request); 


            Logs logs = new Logs()
            {
                Ipaddress = ipAddress,
                RequestBody = "Method:- " + method + " Path:-" + path + " Request:- " + requestBody,
                CreatedDate = DateTime.Now,
                CreatedBy = username
            };

            dbcontext.Logs.AddAsync(logs);
            dbcontext.SaveChangesAsync();

        }

        private string ReadRequestBody(HttpRequest request)
        {
            request.EnableBuffering();
            using (StreamReader reader = new StreamReader(request.Body, Encoding.UTF8, true, 1024, true))
            {
                string requestBody = reader.ReadToEnd();
                request.Body.Seek(0, SeekOrigin.Begin);
                return requestBody;
            }
        }
        

    }
   
    public static class RequestLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestLoggingMiddleware>();
        }
    }
}
