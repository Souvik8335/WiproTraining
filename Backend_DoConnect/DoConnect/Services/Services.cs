using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Model;

namespace DoConnect.Services
{
    public class Service
{  //----------------------------  Reads Jwt Token from app.json ---------------
    private readonly IConfiguration _config;

    public Service(IConfiguration config) => _config = config;
    
   // --------------------------------------------------------------------
        public string CreateToken(User user)
        {
            var jwt = _config.GetSection("Jwt");

            var claims = new List<Claim>
        {
                //============== this feature is used to store name and role of user ====================
                new Claim(ClaimTypes.Name, user.Username ?? "Unknown"),
                new Claim(ClaimTypes.Role, user.Role ?? "User"),
                new Claim("UserId",user.UserId.ToString())
        };
        // ========================== Use the secreat Key to login ======================

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        //====================== Generate user token and all ====================
            var token = new JwtSecurityToken(
                issuer: jwt["Issuer"],
                audience: jwt["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(int.Parse(jwt["ExpiresMinutes"]!)),
                signingCredentials: creds
            );
        // ================= return a string version of token to send it in angular =================
        
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
}
}