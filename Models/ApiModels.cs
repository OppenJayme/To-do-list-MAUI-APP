using System.Text.Json.Serialization;

namespace TodoListApp1.Models
{
    /// <summary>
    /// Generic response for both signup and error payloads.
    /// </summary>
    public class ApiResponse
    {
        [JsonPropertyName("status")]
        public int Status { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }
    }
    
    public class ToDoResponse
    {
        public int Status { get; set; }
        public Dictionary<string, ToDoItem> Data { get; set; }
        public int Count { get; set; }
    }
    /// <summary>
    /// Response returned by your sign-in API on success.
    /// Inherits Status/Message from ApiResponse.
    /// </summary>
    public class SignInResponse : ApiResponse
    {
        [JsonPropertyName("data")]
        public SignInData Data { get; set; }
    }

    public class SignInData
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("fname")]
        public string Fname { get; set; }

        [JsonPropertyName("lname")]
        public string Lname { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("timemodified")]
        public string Timemodified { get; set; }
    }
    
    public class GenericResponse
    {
        public int Status { get; set; }
        public string Message { get; set; }
    }

}
