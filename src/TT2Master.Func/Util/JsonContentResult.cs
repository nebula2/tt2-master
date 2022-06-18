using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TT2MasterFunc.Util
{
    public class JsonContentResult : ContentResult
    {
        public JsonContentResult(object obj)
        {
            ContentType = "application/json";
            Content = JsonConvert.SerializeObject(obj);
        }
    }
}