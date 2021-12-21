using Daftari.API.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Web;

namespace Daftari.API.Services
{
    public class AuthService : IAuthService
    {
        public int GenerateRandomNo()
        {
            int _min = 1000;
            int _max = 9999;
            Random _rdm = new Random();
            return _rdm.Next(_min, _max);
        }

        public string GetSecurityToken(string userNames, string userId, string role)
        {
            var Secret = ConfigurationManager.AppSettings["JwtKey"];
            var Audience = ConfigurationManager.AppSettings["JwtIssuer"];
            var Issuer = ConfigurationManager.AppSettings["JwtIssuer"];

            //var tokenHandler = new JwtSecurityTokenHandler();
            //var key = Convert.FromBase64String(Secret);

            var claimsIdentity = new[] {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, userNames),
                new Claim(ClaimTypes.Sid, userId),
                new Claim(ClaimTypes.Role, role),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Secret));
            var token = new JwtSecurityToken(
                issuer: Issuer,
                audience: Audience,
                claims: claimsIdentity,
                expires: DateTime.Now.AddDays(30),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
                );

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);

            //string key = "my_secret_key_12345"; //Secret key which will be used later during validation    
            //var issuer = "http://mysite.com";  //normally this will be your site URL    

            //var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Secret));
            //var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            ////Create a List of Claims, Keep claims name short    
            //var permClaims = new List<Claim>();
            //permClaims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            //permClaims.Add(new Claim("valid", "1"));
            //permClaims.Add(new Claim("userid", "1"));
            //permClaims.Add(new Claim("name", "bilal"));

            ////Create Security Token object by giving required parameters    
            //var token = new JwtSecurityToken(issuer, //Issure    
            //                issuer,  //Audience    
            //                permClaims,
            //                expires: DateTime.Now.AddDays(1),
            //                signingCredentials: credentials);
            //var jwt_token = new JwtSecurityTokenHandler().WriteToken(token);

            //string key = "my_secret_key_12345"; //Secret key which will be used later during validation    
            //var issuer = "http://mysite.com";  //normally this will be your site URL    

            //var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            //var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            ////Create a List of Claims, Keep claims name short    
            //var permClaims = new List<Claim>();
            //permClaims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            //permClaims.Add(new Claim("valid", "1"));
            //permClaims.Add(new Claim("userid", "1"));
            //permClaims.Add(new Claim("name", "bilal"));

            ////Create Security Token object by giving required parameters    
            //var token = new JwtSecurityToken(issuer, //Issure    
            //                issuer,  //Audience    
            //                permClaims,
            //                expires: DateTime.Now.AddDays(1),
            //                signingCredentials: credentials);
            //var jwt_token = new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}