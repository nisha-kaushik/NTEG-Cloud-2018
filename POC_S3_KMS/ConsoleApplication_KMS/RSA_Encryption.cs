using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication_KMS
{
    class RSA_Encryption
    {
        private static readonly byte[] Salt =
                   new byte[] { 10, 20, 30, 40, 50, 60, 70, 80 };

        public static byte[] RSAEncryptionFromPublicKey(MemoryStream keyfromAWS)
        {
            // Asn1Object obj = Asn1Object.FromByteArray(Convert.FromBase64String(Encoding.Unicode.GetString(keyfromAWS.ToArray())));

            Asn1Object obj = Asn1Object.FromByteArray(keyfromAWS.ToArray());

            DerSequence publicKeySequence = (DerSequence)obj;

            // DerBitString encodedPublicKey0000 = (DerBitString)publicKeySequence[1];

            DerBitString encodedPublicKey = (DerBitString)publicKeySequence[1];

            // DerBitString encodedPublicKey = new DerBitString(keyfromAWS.ToArray());
            DerSequence publicKey = (DerSequence)Asn1Object.FromByteArray(encodedPublicKey.GetBytes());

            DerInteger modulus = (DerInteger)publicKey[0];
            DerInteger exponent = (DerInteger)publicKey[1];

            RsaKeyParameters keyParameters = new RsaKeyParameters(false, modulus.PositiveValue, exponent.PositiveValue);


            RSAParameters parameters = DotNetUtilities.ToRSAParameters(keyParameters);

            // Just to verify XML .
            //string pubKeyString;
            //{
            //    //we need some buffer
            //    var sw = new System.IO.StringWriter();
            //    //we need a serializer
            //    var xs = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
            //    //serialize the key into the stream
            //    xs.Serialize(sw, parameters);
            //    //get the string from the stream
            //    pubKeyString = sw.ToString();
            //}

            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048);
            rsa.ImportParameters(parameters);

            byte[] dataToEncrypt = CreateInternalKey("Panorama");
            // byte[] dataToEncrypt = Encoding.UTF8.GetBytes("Panorama");
            byte[] encryptedData = rsa.Encrypt(dataToEncrypt, false);
            //return Convert.ToBase64String(encryptedData);
           
            return encryptedData;
        }       

        private static byte[] CreateInternalKey(string password, int keyBytes = 32)
        {
            const int Iterations = 32;
            var keyGenerator = new Rfc2898DeriveBytes(password, Salt, Iterations);
            return keyGenerator.GetBytes(keyBytes);
        }

        //public static byte[] EncryptionMix(MemoryStream key)
        //{

        //    //  Asn1Object obj = Asn1Object.FromByteArray(Convert.FromBase64String(Encoding.Unicode.GetString(keyfromAWS.ToArray())));

        //    Asn1Object obj = Asn1Object.FromByteArray(key.ToArray());

        //    DerSequence publicKeySequence = (DerSequence)obj;

        //    // DerBitString encodedPublicKey0000 = (DerBitString)publicKeySequence[1];

        //    DerBitString encodedPublicKey = (DerBitString)publicKeySequence[1];

        //    // DerBitString encodedPublicKey = new DerBitString(keyfromAWS.ToArray());
        //    DerSequence publicKey = (DerSequence)Asn1Object.FromByteArray(encodedPublicKey.GetBytes());

        //    DerInteger modulus = (DerInteger)publicKey[0];
        //    DerInteger exponent = (DerInteger)publicKey[1];

        //    RsaKeyParameters keyParameters = new RsaKeyParameters(false, modulus.PositiveValue, exponent.PositiveValue);


        //    RSAParameters parameters = DotNetUtilities.ToRSAParameters(keyParameters);







        //    //lets take a new CSP with a new 2048 bit rsa key pair
        //    var csp = new RSACryptoServiceProvider(2048);

        //    //how to get the private key
        //    var privKey = csp.ExportParameters(true);

        //    //and the public key ...
        //    var pubKey = csp.ExportParameters(false);
        //    //pubKey.Modulus = key.ToArray();
        //    //pubKey.Modulus = modulus.PositiveValue.ToByteArray();

        //    pubKey.Modulus = parameters.Modulus;
        //    //converting the public key into a string representation
        //    string pubKeyString;
        //    {
        //        //we need some buffer
        //        var sw = new System.IO.StringWriter();
        //        //we need a serializer
        //        var xs = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
        //        //serialize the key into the stream
        //        xs.Serialize(sw, pubKey);
        //        //get the string from the stream
        //        pubKeyString = sw.ToString();
        //    }

        //    //converting it back
        //    {
        //        //get a stream from the string
        //        var sr = new System.IO.StringReader(pubKeyString);
        //        //we need a deserializer
        //        var xs = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
        //        //get the object back from the stream
        //        pubKey = (RSAParameters)xs.Deserialize(sr);
        //    }

        //    //conversion for the private key is no black magic either ... omitted

        //    //we have a public key ... let's get a new csp and load that key
        //    csp = new RSACryptoServiceProvider(2048);
        //    csp.ImportParameters(pubKey);

        //    //we need some data to encrypt
        //    var plainTextData = "Panorama";

        //    //for encryption, always handle bytes...
        //    var bytesPlainTextData = System.Text.Encoding.Unicode.GetBytes(plainTextData);

        //    //apply pkcs#1.5 padding and encrypt our data 
        //    var bytesCypherText = csp.Encrypt(bytesPlainTextData, false);

        //    //we might want a string representation of our cypher text... base64 will do
        //    var cypherText = Convert.ToBase64String(bytesCypherText);



        //    /*
        //     * some transmission / storage / retrieval
        //     * 
        //     * and we want to decrypt our cypherText
        //     */

        //    //first, get our bytes back from the base64 string ...
        //    bytesCypherText = Convert.FromBase64String(cypherText);
        //    privKey.Modulus = parameters.Modulus;
        //    //we want to decrypt, therefore we need a csp and load our private key
        //    csp = new RSACryptoServiceProvider(2048);
        //    csp.ImportParameters(privKey);

        //    //decrypt and strip pkcs#1.5 padding
        //    bytesPlainTextData = csp.Decrypt(bytesCypherText, false);

        //    //get our original plainText back...
        //    plainTextData = System.Text.Encoding.Unicode.GetString(bytesPlainTextData);

        //    return bytesCypherText;
        //}



        //public static byte[] Encrytion(MemoryStream key)
        //{
        //    //lets take a new CSP with a new 2048 bit rsa key pair
        //    var csp = new RSACryptoServiceProvider(2048);

        //    //how to get the private key
        //    var privKey = csp.ExportParameters(true);

        //    //and the public key ...
        //    var pubKey = csp.ExportParameters(false);
        //   // pubKey.Modulus = key.ToArray();
        //    //converting the public key into a string representation
        //    string pubKeyString;
        //    {
        //        //we need some buffer
        //        var sw = new System.IO.StringWriter();
        //        //we need a serializer
        //        var xs = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
        //        //serialize the key into the stream
        //        xs.Serialize(sw, pubKey);
        //        //get the string from the stream
        //        pubKeyString = sw.ToString();
        //    }

        //    //converting it back
        //    {
        //        //get a stream from the string
        //        var sr = new System.IO.StringReader(pubKeyString);
        //        //we need a deserializer
        //        var xs = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
        //        //get the object back from the stream
        //        pubKey = (RSAParameters)xs.Deserialize(sr);
        //    }

        //    //conversion for the private key is no black magic either ... omitted

        //    //we have a public key ... let's get a new csp and load that key
        //    csp = new RSACryptoServiceProvider(2048);
        //    csp.ImportParameters(pubKey);

        //    //we need some data to encrypt
        //    var plainTextData = "Panorama";

        //    //for encryption, always handle bytes...
        //    var bytesPlainTextData = System.Text.Encoding.Unicode.GetBytes(plainTextData);

        //    //apply pkcs#1.5 padding and encrypt our data 
        //    var bytesCypherText = csp.Encrypt(bytesPlainTextData, false);

        //    //we might want a string representation of our cypher text... base64 will do
        //    var cypherText = Convert.ToBase64String(bytesCypherText);



        //    /*
        //     * some transmission / storage / retrieval
        //     * 
        //     * and we want to decrypt our cypherText
        //     */

        //    //first, get our bytes back from the base64 string ...
        //    bytesCypherText = Convert.FromBase64String(cypherText);
        //    //privKey.Modulus = key.ToArray();
        //    //we want to decrypt, therefore we need a csp and load our private key
        //    csp = new RSACryptoServiceProvider(2048);
        //    csp.ImportParameters(privKey);

        //    //decrypt and strip pkcs#1.5 padding
        //    bytesPlainTextData = csp.Decrypt(bytesCypherText, false);

        //    //get our original plainText back...
        //    plainTextData = System.Text.Encoding.Unicode.GetString(bytesPlainTextData);

        //    return bytesCypherText;
        //}
    }
}
