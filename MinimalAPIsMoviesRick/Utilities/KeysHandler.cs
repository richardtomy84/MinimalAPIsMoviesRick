using Microsoft.IdentityModel.Tokens;

namespace MinimalAPIsMoviesRick.Utilities
{
    public class KeysHandler
    {
        public const string OurIssuer = "Lkh878h";

        private const string KeysSection = "Authentication:Schemes:Bearer:SigningKeys";
        private const string KeysSection_Issuer = "Issuer";
        private const string keysSection_value = "Value";

        public static IEnumerable<SecurityKey> GetKey(IConfiguration configuration)
            => GetKey(configuration, OurIssuer);

        public static IEnumerable<SecurityKey> GetKey (IConfiguration configuration,string issuer)
        {
            
           var signingKey = configuration.GetSection(KeysSection)
                .GetChildren()
                .SingleOrDefault(key => key[KeysSection_Issuer] == issuer);
            //Rick testing
            if (signingKey is not null && signingKey[keysSection_value] is string secretkey) 
            { 
                yield return new SymmetricSecurityKey(Convert.FromBase64String(secretkey));
            
            }
        
        }

        public static IEnumerable<SecurityKey> GetAllKeys(IConfiguration configuration)
        {
            var signingKeys = configuration.GetSection(KeysSection)
              .GetChildren();

            foreach (var signingKey in signingKeys)
            {
                if (signingKey[keysSection_value] is string secretkey)
                {
                    yield return new SymmetricSecurityKey(Convert.FromBase64String(secretkey));

                }

            }


        }
    }
}
