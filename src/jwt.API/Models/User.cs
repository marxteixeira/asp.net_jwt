namespace asp.net_jwt.Models
{
    public record User(string Name, int Id, string Email, string Image, string Password, string[] Roles);
}
