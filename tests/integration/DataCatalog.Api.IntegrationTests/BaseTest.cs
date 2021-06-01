using System.IO;
using System.Net;
using System.Text;

using DataCatalog.Data.Model;
using System;
using System.Drawing;

namespace DataCatalog.Api.IntegrationTests
{
    public class BaseTest
    {
        protected enum HttpVerb
        {
            GET,
            POST,
            PUT,
            DELETE
        }

        protected enum Controller
        {
            Category
        }

        protected Response SendRequest(HttpVerb method, Controller controller, string url, string content, bool useAuthHeader = true)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create($"https://localhost:44396/api/{controller}" + url);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = method.ToString();
            if (useAuthHeader)
            {
                httpWebRequest.Headers.Add("Authorization", "Bearer D81769D1-B643-41E6-AD5A-EC6B86F7E608");
            }

            if (method == HttpVerb.POST || method == HttpVerb.PUT)
            {
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(content);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
            }

            var response = new Response();
            try
            {

                response.HttpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();

            }
            catch (WebException ex)
            {
                response.HttpWebResponse = (HttpWebResponse)ex.Response;
                return response;
            }

            var stream = response.HttpWebResponse.GetResponseStream();
            if (stream != null)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    stream.CopyTo(ms);
                    response.BinaryContent = ms.ToArray();
                }
            }
            response.Content = Encoding.UTF8.GetString(response.BinaryContent);

            return response;
        }

        protected IdentityProvider CreateIdentityProvider()
        {
            return new IdentityProvider
            {
                Id = Guid.NewGuid(),
                Name = Guid.NewGuid().ToString(),
                TenantId = Guid.NewGuid().ToString()
            };
        }

        protected Member CreateMember(IdentityProvider identityProvider)
        {
            return new Member
            {
                Id = Guid.NewGuid(),
                ExternalId = Guid.NewGuid().ToString(),
                IdentityProvider = identityProvider,
                IdentityProviderId = identityProvider.Id,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now
            };
        }
    }
}
