using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace UOBCMS.Classes
{
    public class CustomJsonResult : JsonResult
    {
        public CustomJsonResult(object value) : base(value)
        {
            // Set the desired JsonSerializerSettings
            SerializerSettings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore
            };
        }
    }
}
