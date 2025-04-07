using Newtonsoft.Json;

namespace ExpenseManagment.Models
{
    public class ResponseData<T>
    {
        [JsonProperty("isValid")]
        public bool IsValid { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("data")]
        public T Data { get; set; }
    }
}
