
using System.Collections.Generic;
using System.Web;
using System;
using Adhesive.Common;

namespace Adhesive.AppInfoCenter.Imp
{
    public class HttpContextInfoProvider : IInfoProvider
    {
        public void ProcessInfo(IncludeInfoStrategy strategy, AbstractInfo info)
        {
            if (!strategy.IncludeInfoStrategyForHttpContextInfo.Include) return;

            try
            {
                var httpContext = HttpContext.Current;
                if (httpContext != null)
                {
                    var httpContextInfo = new HttpContextInfo()
                    {
                        Handler = httpContext.Handler == null ? "" : httpContext.Handler.GetType().FullName,
                        IsCustomErrorEnabled = httpContext.IsCustomErrorEnabled,
                        IsDebuggingEnabled = httpContext.IsDebuggingEnabled,
                        SkipAuthorization = httpContext.SkipAuthorization,
                    };

                    try
                    {
                        if (httpContext.Session != null && strategy.IncludeInfoStrategyForHttpContextInfo.IncludeHttpContextSessions)
                        {
                            var session = new Dictionary<string, string>();
                            foreach (var key in httpContext.Session.Keys)
                            {
                                if (!session.ContainsKey(key.ToString()))
                                    session.Add(key.ToString(), httpContext.Session[key.ToString()] == null ? "" :
                                        httpContext.Session[key.ToString()].ToString());
                            }
                            httpContextInfo.Sessions = session;
                        }
                    }
                    catch (Exception ex)
                    {
                        LocalLoggingService.Error("httpContext.Session" + ex.ToString());
                    }

                    try
                    {
                        if (httpContext.Items != null && strategy.IncludeInfoStrategyForHttpContextInfo.IncludeHttpContextItems)
                        {
                            var items = new Dictionary<string, string>();
                            foreach (var key in httpContext.Items.Keys)
                            {
                                if (!items.ContainsKey(key.ToString()))
                                    items.Add(key.ToString(), httpContext.Items[key] == null ? "" :
                                        httpContext.Items[key].ToString());
                            }
                            httpContextInfo.Items = items;
                        }
                    }
                    catch (Exception ex)
                    {
                        LocalLoggingService.Error("httpContext.Items" + ex.ToString());
                    }

                    try
                    {
                        var request = httpContext.Request;
                        if (request != null && strategy.IncludeInfoStrategyForHttpContextInfo.IncludeInfoStrategyForRequestInfo.Include)
                        {
                            var requestInfo = strategy.IncludeInfoStrategyForHttpContextInfo.IncludeInfoStrategyForRequestInfo.IncludeBasicInfo ?
                                new RequestInfo()
                                {
                                    AcceptTypes = request.AcceptTypes == null ? "" : string.Join(",", request.AcceptTypes),
                                    AnonymousID = request.AnonymousID,
                                    ApplicationPath = request.ApplicationPath,
                                    AppRelativeCurrentExecutionFilePath = request.AppRelativeCurrentExecutionFilePath,
                                    Browser = request.Browser == null ? "" : request.Browser.ToString(),
                                    ContentEncoding = request.ContentEncoding == null ? "" : request.ContentEncoding.ToString(),
                                    ContentLength = request.ContentLength,
                                    ContentType = request.ContentType,
                                    CurrentExecutionFilePath = request.CurrentExecutionFilePath,
                                    FilePath = request.FilePath,
                                    HttpMethod = request.HttpMethod,
                                    IsAuthenticated = request.IsAuthenticated,
                                    IsLocal = request.IsLocal,
                                    IsSecureConnection = request.IsSecureConnection,
                                    LogonUserIdentity = request.LogonUserIdentity == null ? "" : request.LogonUserIdentity.ToString(),
                                    Path = request.Path,
                                    PathInfo = request.PathInfo,
                                    PhysicalApplicationPath = request.PhysicalApplicationPath,
                                    PhysicalPath = request.PhysicalPath,
                                    Url = request.Url == null ? "" : request.Url.ToString(),
                                    UrlReferrer = request.UrlReferrer == null ? "" : request.UrlReferrer.ToString(),
                                    RequestType = request.RequestType,
                                    UserAgent = request.UserAgent,
                                    UserHostAddress = request.UserHostAddress,
                                    UserHostName = request.UserHostName,
                                    UserLanguages = request.UserLanguages == null ? "" : string.Join(",", request.UserLanguages),
                                } : new RequestInfo();

                            try
                            {
                                if (request.Cookies != null && strategy.IncludeInfoStrategyForHttpContextInfo.IncludeInfoStrategyForRequestInfo.IncludeRequestCookies)
                                {
                                    var cookies = new Dictionary<string, string>();
                                    foreach (var key in request.Cookies.Keys)
                                    {
                                        if (!cookies.ContainsKey(key.ToString()))
                                            cookies.Add(key.ToString(), request.Cookies[key.ToString()] == null ? "" :
                                                request.Cookies[key.ToString()].ToString());
                                    }
                                    requestInfo.Cookies = cookies;
                                }
                            }
                            catch (Exception ex)
                            {
                                LocalLoggingService.Error("httpContext.Request.Cookies" + ex.ToString());
                            }

                            try
                            {
                                if (request.Form != null && strategy.IncludeInfoStrategyForHttpContextInfo.IncludeInfoStrategyForRequestInfo.IncludeRequestForms)
                                {
                                    var form = new Dictionary<string, string>();
                                    foreach (var key in request.Form.Keys)
                                    {
                                        if (!form.ContainsKey(key.ToString()))
                                            form.Add(key.ToString(), request.Form[key.ToString()] == null ? "" :
                                                request.Form[key.ToString()].ToString());
                                    }
                                    requestInfo.Forms = form;
                                }
                            }
                            catch (Exception ex)
                            {
                                LocalLoggingService.Error("httpContext.Request.Form" + ex.ToString());
                            }

                            try
                            {
                                if (request.Headers != null && strategy.IncludeInfoStrategyForHttpContextInfo.IncludeInfoStrategyForRequestInfo.IncludeRequestHeaders)
                                {
                                    var headers = new Dictionary<string, string>();
                                    foreach (var key in request.Headers.Keys)
                                    {
                                        if (!headers.ContainsKey(key.ToString()))
                                            headers.Add(key.ToString(), request.Headers[key.ToString()] == null ? "" :
                                                request.Headers[key.ToString()].ToString());
                                    }
                                    requestInfo.Headers = headers;
                                }
                            }
                            catch (Exception ex)
                            {
                                LocalLoggingService.Error("httpContext.Request.Headers" + ex.ToString());
                            }

                            try
                            {
                                if (request.QueryString != null && strategy.IncludeInfoStrategyForHttpContextInfo.IncludeInfoStrategyForRequestInfo.IncludeRequestQueryStrings)
                                {
                                    var querystring = new Dictionary<string, string>();
                                    foreach (var key in request.QueryString.Keys)
                                    {
                                        if (!querystring.ContainsKey(key.ToString()))
                                            querystring.Add(key.ToString(), request.QueryString[key.ToString()] == null ? "" :
                                                request.QueryString[key.ToString()].ToString());
                                    }
                                    requestInfo.QueryStrings = querystring;
                                }
                            }
                            catch (Exception ex)
                            {
                                LocalLoggingService.Error("httpContext.Request.QueryString" + ex.ToString());
                            }

                            httpContextInfo.RequestInfo = requestInfo;
                        }
                    }
                    catch (Exception ex)
                    {
                        LocalLoggingService.Error("httpContext.Request" + ex.ToString());
                    }

                    try
                    {
                        var response = httpContext.Response;
                        if (response != null && strategy.IncludeInfoStrategyForHttpContextInfo.IncludeInfoStrategyForResponseInfo.Include)
                        {
                            var responseInfo = strategy.IncludeInfoStrategyForHttpContextInfo.IncludeInfoStrategyForResponseInfo.IncludeBasicInfo ?
                                new ResponseInfo()
                                {

                                    CacheControl = response.CacheControl,
                                    Charset = response.Charset,
                                    ContentEncoding = response.ContentEncoding == null ? "" : response.ContentEncoding.ToString(),
                                    ContentType = response.ContentType,
                                    ExpiresAbsolute = response.ExpiresAbsolute,
                                    HeaderEncoding = response.HeaderEncoding == null ? "" : response.HeaderEncoding.ToString(),
                                    IsClientConnected = response.IsClientConnected,
                                    IsRequestBeingRedirected = response.IsRequestBeingRedirected,
                                    RedirectLocation = response.RedirectLocation,
                                    Status = response.Status,
                                    StatusCode = response.StatusCode,
                                    StatusDescription = response.StatusDescription,
                                    SuppressContent = response.SuppressContent,
                                    TrySkipIisCustomErrors = response.TrySkipIisCustomErrors,
                                } : new ResponseInfo();

                            try
                            {
                                if (response.Cookies != null && strategy.IncludeInfoStrategyForHttpContextInfo.IncludeInfoStrategyForResponseInfo.IncludeResponseCookies)
                                {
                                    var cookies = new Dictionary<string, string>();
                                    foreach (var key in response.Cookies.Keys)
                                    {
                                        if (!cookies.ContainsKey(key.ToString()))
                                            cookies.Add(key.ToString(), response.Cookies[key.ToString()] == null ? "" :
                                                response.Cookies[key.ToString()].ToString());
                                    }
                                    responseInfo.Cookies = cookies;
                                }
                            }
                            catch (Exception ex)
                            {
                                LocalLoggingService.Error("httpContext.Response.Cookies" + ex.ToString());
                            }

                            httpContextInfo.ResponseInfo = responseInfo;
                        }
                    }
                    catch (Exception ex)
                    {
                        LocalLoggingService.Error("httpContext.Response" + ex.ToString());
                    }

                    info.HttpContextInfo = httpContextInfo;
                }
            }
            catch (Exception ex)
            {
                LocalLoggingService.Error("httpContext" + ex.ToString());
            }
        }

    }
}
