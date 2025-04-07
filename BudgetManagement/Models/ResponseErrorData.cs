using Newtonsoft.Json;

namespace ExpenseManagment.Models
{
    public class ResponseErrorData
    {
        [JsonProperty("isValid")]
        public bool IsValid { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
