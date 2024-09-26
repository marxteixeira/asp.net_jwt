using asp.net_jwt.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace asp.net_jwt.Services
{
    public class TokenService
    {
        public string Create()
        {
            //handler
            var handler = new JwtSecurityTokenHandler();

            //chave encriptada
            var key = Encoding.ASCII.GetBytes(Configuration.PrivateKey); //codificar essa string para byte[]

            //montar a credential
            var credentials = new SigningCredentials( //objeto para assinar
                                    new SymmetricSecurityKey(key), //chave simétrica que precisa da chave em array de bytes.
                                    SecurityAlgorithms.HmacSha256);//algoritmo de encriptação

            //montagem do token
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                SigningCredentials = credentials,
                Expires = DateTime.UtcNow.AddHours(2)
            };

            var token = handler.CreateToken(tokenDescriptor);

            return handler.WriteToken(token);
        }

        private static ClaimsIdentity GenerateClaims(User user)
        {
            var ci = new ClaimsIdentity();

            //esse ClaimTypes.Name será o nome de USUÁRIO
            ci.AddClaim(new Claim(ClaimTypes.Name, user.Email));
            ci.AddClaim(new Claim(ClaimTypes.Email, user.Email));
            ci.AddClaim(new Claim(ClaimTypes.GivenName, user.Name));
            //claims customizados
            ci.AddClaim(new Claim("id", user.Id.ToString()));
            ci.AddClaim(new Claim("image", user.Image));

            //para montar os ClaimTypes de roles, vamos fazer um foreach
            foreach (var role in user.Roles)
            {
                ci.AddClaim(new Claim(ClaimTypes.Role, role));
            }

            return ci;
        }
    }
}
