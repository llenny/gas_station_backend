using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gas_station.Models
{
    public class AuthOptions
    {
        public const string ISSUER = "BirstTime"; // издатель токена
        public const string AUDIENCE = "BirstTimeClient"; // потребитель токена
        const string KEY = "ne_privlekay_sotrudnikov_stryahni_sam";   // ключ для шифрации
        public const int LIFETIME = 100; // время жизни токена - 1 минута
        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }
    }
}
