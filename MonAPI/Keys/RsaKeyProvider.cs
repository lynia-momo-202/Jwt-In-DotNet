using System.Security.Cryptography;

namespace MonAPI.Keys;

public class RsaKeyProvider
{
    public static RSA GetPrivateKey( )
    {
        var rsa = RSA.Create();
        rsa.ImportFromPem(File.ReadAllText(@"Keys\private_key.pem"));
        return rsa;
    }

    public static RSA GetPublicKey( )
    {
        var rsa = RSA.Create();
        rsa.ImportFromPem(File.ReadAllText("Keys/public_key.pem"));
        return rsa;
    }

}
