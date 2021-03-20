using System;

namespace Chater.JWT
{
    public class JwtTokenConfig
    {
        public String Secret { get; set; }
        public String Issuer { get; set; }
        public String Audience { get; set; }
        public int AccessTokenExpiration { get; set; }
        public int RefreshTokenExpiration { get; set; }
    }
}