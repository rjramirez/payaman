using Common.DataTransferObjects.ErrorLog;
using Newtonsoft.Json;

namespace WebApp.Extensions
{
    public static class ResponseMessageExtension
    {
        public static async Task<ErrorMessage> GetErrorMessage(this HttpResponseMessage httpResponseMessage)
        {
            return JsonConvert.DeserializeObject<ErrorMessage>(await httpResponseMessage.Content.ReadAsStringAsync());
        }
    }
}
