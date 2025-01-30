﻿using System.Net.Http;

namespace AnglingClubWebsite.Services
{
    public abstract class DataServiceBase
    {
        private readonly string _controllerName;
        private readonly IHttpClientFactory _httpClientFactory;

        protected DataServiceBase(
            string controllerName, 
            IHttpClientFactory httpClientFactory
            )
        {
            _controllerName = controllerName;
            _httpClientFactory = httpClientFactory;
        }

        private HttpClient? _http = null;
        protected HttpClient Http
        {
            get
            {
                if (_http == null || _http.BaseAddress == null)
                {
                    _http = _httpClientFactory.CreateClient(Constants.HTTP_CLIENT_KEY);
                    _http.BaseAddress = new Uri($"{_http.BaseAddress.ToString()}api/{_controllerName}/");
                }

                return _http;

            }
        }

    }
}
