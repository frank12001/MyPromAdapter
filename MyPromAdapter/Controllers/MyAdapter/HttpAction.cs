using System;
using System.Collections.Generic;
using RestSharp;

namespace MyPromAdapter.Controllers.MyAdapter
{
    public class HttpAction
    {
        public enum Methods
        {
            Null,Get,Post
        }

        public Methods Method;
        public RestClient RestClient;
        public string Request;
        public Dictionary<string, string> QueryString = new Dictionary<string, string>();

        public HttpAction()
        {
            Method = Methods.Null;
        }

        public HttpAction(Methods method,Uri host,string request,Dictionary<string,string> querystring)
        {
            Method = method;
            RestClient = new RestClient(host);
            Request = request;
            QueryString = querystring;
        }

        public string Go()
        {
            var request = new RestRequest(Request);
            foreach (var keyValuePair in QueryString)
            {
                request.AddParameter(keyValuePair.Key, keyValuePair.Value);
            }
            IRestResponse response;
            switch (Method)
            {
                case Methods.Post:
                    response = RestClient.Post(request);
                    break;
                default:
                    response = RestClient?.Get(request);
                    break;
            }
            Console.WriteLine($"Host: {RestClient?.BaseUrl} Req: {Request}, Result: {response?.IsSuccessful} Content: {response?.Content} , StatusCode {response?.StatusCode.ToString()}");
            Console.WriteLine($"ErrorMsg: {response?.ErrorMessage}");
            return response?.Content;
        }
    }
}