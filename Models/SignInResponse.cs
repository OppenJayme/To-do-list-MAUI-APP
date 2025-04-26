namespace TodoListApp1.Models

{
    public class SignInResponse
    {
        public int Status { get; set; }
        public string Message { get; set; }
        public SignInData Data { get; set; }
    }
}
