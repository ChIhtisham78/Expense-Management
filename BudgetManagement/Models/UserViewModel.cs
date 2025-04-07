namespace ExpenseManagment.Models
{
    public class UserViewModel
    {
        public string Id { get; set; }
        public string Address { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public string Password { get; set; }
        public bool IsCompleted { get; set; }
        public string ZipCode { get; set; }
        public string Email { get; set; }
    }
}
