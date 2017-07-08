using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Caching;
using System.Threading;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace CurrencyConverter
{

    /// <summary>
    /// Class for implementing Output cache of the response
    /// </summary>
    public class OutputCacheFilterAttribute : ActionFilterAttribute
    {
        private int _timespan;
        
        private string _cachekey;
        
        private static readonly ObjectCache WebApiCache = MemoryCache.Default;

        public OutputCacheFilterAttribute(int timespan)
        {
            _timespan = timespan;
        }
        private bool IsCacheable(HttpActionContext ac)
        {
            if (_timespan > 0)
            {
                    if (Thread.CurrentPrincipal.Identity.IsAuthenticated)
                        return false;
                if (ac.Request.Method == HttpMethod.Get) return true;
            }
            else
            {
                throw new InvalidOperationException("Invalid caching arguments.");
            }
            return false;
        }
        private CacheControlHeaderValue setClientCache()
        {
            var cachecontrol = new CacheControlHeaderValue();
            cachecontrol.MaxAge = TimeSpan.FromSeconds(_timespan);
            cachecontrol.MustRevalidate = true;
            return cachecontrol;
        }
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (actionContext != null)
            {
                if (IsCacheable(actionContext))
                {
                    _cachekey = string.Join(":", new string[] { actionContext.Request.RequestUri.AbsolutePath, actionContext.Request.Headers.Accept.FirstOrDefault().ToString() });
                    if (WebApiCache.Contains(_cachekey))
                    {
                        var val = (string)WebApiCache.Get(_cachekey);
                        if (val != null)
                        {
                            actionContext.Response = actionContext.Request.CreateResponse();
                            actionContext.Response.Content = new StringContent(val);
                            var contenttype = (MediaTypeHeaderValue)WebApiCache.Get(_cachekey + ":responsect");
                            if (contenttype == null)
                                contenttype = new MediaTypeHeaderValue(_cachekey.Split(':')[1]);
                            actionContext.Response.Content.Headers.ContentType = contenttype;
                            actionContext.Response.Headers.CacheControl = setClientCache();
                            return;
                        }
                    }
                }
            }
            else
            {
                throw new ArgumentNullException("HTTPActionContext argument is null.");
            }
        }
        public override void OnActionExecuted(System.Web.Http.Filters.HttpActionExecutedContext actionExecutedContext)
        {
            if (!(WebApiCache.Contains(_cachekey)))
            {
                var body = actionExecutedContext.Response.Content.ReadAsStringAsync().Result;
                WebApiCache.Add(_cachekey, body, DateTime.Now.AddSeconds(_timespan));
                WebApiCache.Add(_cachekey + ":responsect", actionExecutedContext.Response.Content.Headers.ContentType, DateTime.Now.AddSeconds(_timespan));
            }
            if (IsCacheable(actionExecutedContext.ActionContext))
                actionExecutedContext.ActionContext.Response.Headers.CacheControl = setClientCache();
        }
    }
}