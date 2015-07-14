using System;
using System.Web;
using DevExpress.Web.ASPxClasses;
using Memos.Framework.Logging;
using System.Collections.Generic;
using System.Collections;
using DevExpress.Web.ASPxClasses.Internal;
using System.Configuration;

namespace CanonWebApp.Code
{
    public class CallbackErrorModule : IHttpModule
    {
        /// <summary>
        /// You will need to configure this module in the web.config file of your
        /// web and register it with IIS before being able to use it. For more information
        /// see the following link: http://go.microsoft.com/?linkid=8101007
        /// </summary>
        protected bool IsCallBack(HttpRequest request)
        {
            return !string.IsNullOrEmpty(request.Params["__CALLBACKID"]);
        }

        protected bool IsErrorCode(int code)
        {
            if ((code != 404) && (code != 500))
            {
                return false;
            }
            return true;
        }


        protected void PrepareContent(ref HttpResponse response)
        {
            List<HttpCookie> list = new List<HttpCookie>(response.Cookies.Count);
            for (int i = 0; i < response.Cookies.Count; i++)
            {
                list.Add(response.Cookies[i]);
            }
            //response.ClearHeaders();
            response.ClearContent();
            response.ContentType = "text/html";
            for (int j = 0; j < list.Count; j++)
            {
                response.AppendCookie(list[j]);
            }
            response.Cache.SetCacheability(HttpCacheability.NoCache);
        }


        protected string EncodeRedirect(string redirectUrl)
        {
            Hashtable hashtable = new Hashtable();
            hashtable["redirect"] = redirectUrl;
            return this.Encode("/*^^^DX^^^*/" + HtmlConvertor.ToJSON(hashtable));
        }


        protected string Encode(string request)
        {
            return ("0|" + request);
        }


        protected string EncodeError(string error)
        {
            Hashtable hashtable = new Hashtable();
            hashtable["generalError"] = error;
            return this.Encode("/*^^^DX^^^*/" + HtmlConvertor.ToJSON(hashtable));
        }

        #region IHttpModule Members


        void context_PreSendRequestContent(object sender, EventArgs e)
        {
            //HttpApplication application = (HttpApplication)sender;
            //HttpRequest request = application.Request;
            //HttpResponse response = application.Response;

            //if (application == null)
            //{
            //    return;
            //}

            //if (IsErrorCode(application.Response.StatusCode) == false)
            //{
            //    return;
            //}

            //if (IsCallBack(request) && HttpContext.Current != null && HttpContext.Current.Error != null)
            //{
            //    string error;

            //    if (HttpContext.Current.Error.InnerException != null)
            //    {
            //        error = HttpContext.Current.Error.InnerException.Message;
            //        Logger.Log(HttpContext.Current.Error.InnerException.ToString(), LogLevel.Error);
            //    }
            //    else
            //    {
            //        error = HttpContext.Current.Error.Message;
            //        Logger.Log(HttpContext.Current.Error.ToString(), LogLevel.Error);
            //    }

            //    //this.PrepareContent(ref response);
            //    response.Output.Write(this.EncodeError(error));

            //    string redirectLocation = response.RedirectLocation;
            //    if (!string.IsNullOrEmpty(redirectLocation))
            //    {
            //        this.PrepareContent(ref response);
            //        response.Output.Write(this.EncodeRedirect(redirectLocation));
            //    }


            //}

            HttpApplication application = (HttpApplication)sender;
            HttpResponse response = application.Response;
            HttpRequest request = application.Request;
            if (this.IsCallBack(request))
            {
                if (this.IsErrorCode(response.StatusCode))
                {
                    string error = "";
                    SystemException exception = (SystemException)HttpContext.Current.Error;
                    if ((exception != null) && (exception.InnerException != null))
                    {
                        error = exception.InnerException.Message;
                    }
                    else
                    {
                        error = response.Status;
                    }
                    string str2 = ConfigurationManager.AppSettings["DXCallbackErrorRedirectUrl"];
                    if (str2 != null)
                    {
                        response.RedirectLocation = str2;
                    }
                    else
                    {
                        this.PrepareContent(ref response);
                        response.Output.Write(this.EncodeError(error));
                    }

                    if (HttpContext.Current.Error.InnerException != null)
                    {
                        Logger.Log(HttpContext.Current.Error.InnerException.ToString(), LogLevel.Error);
                    }
                    else
                    {
                        Logger.Log(HttpContext.Current.Error.ToString(), LogLevel.Error);
                    }
                }
                string redirectLocation = response.RedirectLocation;
                if (!string.IsNullOrEmpty(redirectLocation))
                {
                    this.PrepareContent(ref response);
                    response.Output.Write(this.EncodeRedirect(redirectLocation));
                }
            }

        }

        #endregion

        #region IHttpModule Members

        public void Dispose()
        {
            ;
        }

        void IHttpModule.Init(HttpApplication context)
        {
            context.PreSendRequestContent += context_PreSendRequestContent;
        }

        #endregion
    }
}