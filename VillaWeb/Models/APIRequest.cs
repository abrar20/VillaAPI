﻿using static VillaUtility.SD;

namespace VillaWeb.Models
{
    public class APIRequest
    {
        public ApiType ApiType { get; set; } = ApiType.GET;
        public string Url { get; set; }
        public object Data { get; set; }
        public string token { get; set; }
    }
}
