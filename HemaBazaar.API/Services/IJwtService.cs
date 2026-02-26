namespace HemaBazaar.API.Services
{
    public interface IJwtService
    {
        string CreateToken(string userId,string email);

    }
}
