﻿using BR.Auths.Abstractions.ResponseDTOs;
using BR.Auths.SDK;
using BR.MicroServices.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace BR.MsgHub.OpenApi.Models
{
    public class OpenApiAuthFilter : IAuthorizationFilter
    {
        private OpenAppAuthService _authService = null;

        public OpenApiAuthFilter(OpenAppAuthService authService)
        {
            _authService = authService;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            try
            {
                if (context.HttpContext.Request.Path.Value.Equals(
                    "/api/values", StringComparison.InvariantCultureIgnoreCase))
                    return;
            }
            catch
            {
                return;
            }

            try
            {
                string appid = context.HttpContext.Request.Headers["appid"];
                string tsStr = context.HttpContext.Request.Headers["ts"];
                string sign = context.HttpContext.Request.Headers["sign"];
                if (string.IsNullOrEmpty(appid) || string.IsNullOrEmpty(tsStr)
                    || string.IsNullOrEmpty(sign))
                {
                    ResponseResult result = new ResponseResult();
                    result.Code = (int)HttpStatusCode.Forbidden;
                    context.Result = new JsonResult(result);
                    //context.Result = new ForbidResult();
                    return;
                }

                long ts = long.Parse(tsStr);

                var asyncResult = _authService.CheckAuthAsync(
                    appid, ts, sign, context.HttpContext.Request.Path.Value);
                asyncResult.Wait();

                ActionResponseResult authResult = asyncResult.Result;
                if (authResult.Code != 200)
                {
                    context.Result = new JsonResult(authResult);
                    return;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError(ex.Message, ex);
                context.Result = new ForbidResult();
            }
        }
    }
}
