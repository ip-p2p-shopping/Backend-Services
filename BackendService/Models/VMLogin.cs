namespace BackendService.Models
{
    public class VMLogin
    {
        public String Email {get; set;}
        public String PassWord {get; set;}

        public bool KeepLoggedIn {get; set;}
    }
}
