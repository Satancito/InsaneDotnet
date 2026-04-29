using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsaneIO.Insane;

public class Constants
{
    public const string UrlEncodedPlusSign = "%2B";
    public const string UrlEncodedSlash = "%2F";
    public const string UrlEncodedEqualSign = "%3D";
    public const string PlusSign = "+";
    public const string MinusSign = "-";
    public const string Slash = "/";
    public const string Underscore = "_";
    public const string EqualSign = "=";
    public const string LineFeed = "\n";
    public const string CarriageReturn = "\r";
    public const string NewLineWindows = "\r\n";

    public const uint Base64NoLineBreaks = 0;
    public const uint Base64MimeLineBreaksLength = 76;
    public const uint Base64PemLineBreaksLength = 64;

    public const uint Md5HashSizeInBytes = 16;
    public const uint Sha1HashSizeInBytes = 20;
    public const uint Sha256HashSizeInBytes = 32;
    public const uint Sha384HashSizeInBytes = 48;
    public const uint Sha512HashSizeInBytes = 64;

    public const uint HmacKeySize = 16;

    public const uint ScryptIterationsForInteractiveLogin = 2048;
    public const uint ScryptIterationsForEncryption = 1048576;
    public const uint ScryptIterations = 16384;
    public const uint ScryptBlockSize = 8;
    public const uint ScryptParallelism = 1;
    public const uint ScryptDerivedKeyLength = 64;
    public const uint ScryptSaltSize = 16;

    public const uint Argon2DerivedKeyLength = 64;
    public const uint Argon2SaltSize = 16;
    public const uint Argon2Iterations = 2;
    public const uint Argon2MemorySizeInKiB = 16384;
    public const uint Argon2DegreeOfParallelism = 4;

    public const string Base64ValueRegexPattern = @"^(?:(?:[A-Za-z0-9+\/]{4})*)(?:[A-Za-z0-9+\/]{2}==|[A-Za-z0-9+\/]{3}=)?$";

    public const string RsaPemKeyInitialTextHeader = "-----BEGIN ";
    public const string RsaPemPrivateKeyHeader = "-----BEGIN PRIVATE KEY-----";
    public const string RsaPemPrivateKeyFooter = "-----END PRIVATE KEY-----";

    public const string RsaPemPublicKeyHeader = "-----BEGIN PUBLIC KEY-----";
    public const string RsaPemPublicKeyFooter = "-----END PUBLIC KEY-----";

    public const string RsaPemRsaPrivateKeyHeader = "-----BEGIN RSA PRIVATE KEY-----";
    public const string RsaPemRsaPrivateKeyFooter = "-----END RSA PRIVATE KEY-----";

    public const string RsaPemRsaPublicKeyHeader = "-----BEGIN RSA PUBLIC KEY-----";
    public const string RsaPemRsaPublicKeyFooter = "-----END RSA PUBLIC KEY-----";

    public const string RsaPemPublicKeyRegexPattern = @"^(?:(-----BEGIN PUBLIC KEY-----)(?:\r|\n|\r\n)((?:(?:(?:[A-Za-z0-9+\/]{4}){16}(?:\r|\n|\r\n))+)(?:(?:[A-Za-z0-9+\/]{4}){0,15})(?:(?:[A-Za-z0-9+\/]{4}|[A-Za-z0-9+\/]{2}==|[A-Za-z0-9+\/]{3}=)))(?:\r|\n|\r\n)(-----END PUBLIC KEY-----))$";
    public const string RsaPemPrivateKeyRegexPattern = @"^(?:(-----BEGIN PRIVATE KEY-----)(?:\r|\n|\r\n)((?:(?:(?:[A-Za-z0-9+\/]{4}){16}(?:\r|\n|\r\n))+)(?:(?:[A-Za-z0-9+\/]{4}){0,15})(?:(?:[A-Za-z0-9+\/]{4}|[A-Za-z0-9+\/]{2}==|[A-Za-z0-9+\/]{3}=)))(?:\r|\n|\r\n)(-----END PRIVATE KEY-----))$";

    public const string RsaPemRsaPublicKeyRegexPattern = @"^(?:(-----BEGIN RSA PUBLIC KEY-----)(?:\r|\n|\r\n)((?:(?:(?:[A-Za-z0-9+\/]{4}){16}(?:\r|\n|\r\n))+)(?:(?:[A-Za-z0-9+\/]{4}){0,15})(?:(?:[A-Za-z0-9+\/]{4}|[A-Za-z0-9+\/]{2}==|[A-Za-z0-9+\/]{3}=)))(?:\r|\n|\r\n)(-----END RSA PUBLIC KEY-----))$";
    public const string RsaPemRsaPrivateKeyRegexPattern = @"^(?:(-----BEGIN RSA PRIVATE KEY-----)(?:\r|\n|\r\n)((?:(?:(?:[A-Za-z0-9+\/]{4}){16}(?:\r|\n|\r\n))+)(?:(?:[A-Za-z0-9+\/]{4}){0,15})(?:(?:[A-Za-z0-9+\/]{4}|[A-Za-z0-9+\/]{2}==|[A-Za-z0-9+\/]{3}=)))(?:\r|\n|\r\n)(-----END RSA PRIVATE KEY-----))$";

    public const string RsaXmlKeyMainTag = "<RSAKeyValue>";
    public const string RsaXmlPublicKeyRegexPattern = @"^(<RSAKeyValue>\s*(?:\s*<Modulus>[a-zA-Z0-9\+\/]+={0,2}<\/Modulus>()|\s*<Exponent>[a-zA-Z0-9\+\/]+={0,2}<\/Exponent>()){2}\s*<\/\s*RSAKeyValue\s*>\s*\2\3)$";
    public const string RsaXmlPrivateKeyRegexPattern = @"^(\s*<RSAKeyValue>\s*(?:\s*<Modulus>[a-zA-Z0-9\+\/]+={0,2}<\/Modulus>()|\s*<Exponent>[a-zA-Z0-9\+\/]+={0,2}<\/Exponent>()|\s*<P>[a-zA-Z0-9\+\/]+={0,2}<\/P>()|\s*<Q>[a-zA-Z0-9\+\/]+={0,2}<\/Q>()|\s*<DP>[a-zA-Z0-9\+\/]+={0,2}<\/DP>()|\s*<DQ>[a-zA-Z0-9\+\/]+={0,2}<\/DQ>()|\s*<InverseQ>[a-zA-Z0-9\+\/]+={0,2}<\/InverseQ>()|\s*<D>[a-zA-Z0-9\+\/]+={0,2}<\/D>()){8}\s*<\/RSAKeyValue>\2\3\4\5\6\7\8\9)$";

}
