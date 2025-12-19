namespace authDemoApi.Models
{
    public sealed class UserInfo
    {
        public UserInfo(string name, string? email)
        {
            Name = name;
            Email = email;
        }

        public string Name { get; }

        public string? Email { get; }
    }
}
