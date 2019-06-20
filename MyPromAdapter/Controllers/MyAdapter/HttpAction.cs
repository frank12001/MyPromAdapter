using System;
using System.Collections.Generic;
using RestSharp;

namespace MyPromAdapter.Controllers.MyAdapter
{
    public class HttpAction
    {
        public enum Methods
        {
            Get,Post
        }

        public Methods Method;
        public Uri Host;
        public string Request;
        public Dictionary<string, string> QueryString;

        public HttpAction(Methods method,Uri host,string request,Dictionary<string,string> querystring)
        {
            Method = method;
            Host = host;
            Request = request;
            QueryString = querystring;
        }

        public void Go()
        {
            var client = new RestClient(Host);
            var request = new RestRequest(Request);
            foreach (KeyValuePair<string,string> keyValuePair in QueryString)
            {
                request.AddQueryParameter(keyValuePair.Key, keyValuePair.Value);
            }

            IRestResponse response;
            switch (Method)
            {
                case Methods.Post:
                    response = client.Post(request);
                    break;
                default:
                    response = client.Get(request);
                    break;
            }
            Console.WriteLine($"Host: {Host} Req: {Request}, Result: {response.IsSuccessful} Content: {response.Content}");

        }
    }
}