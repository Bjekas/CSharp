using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Net.NetworkInformation;
using System.Net;
using System.Text.RegularExpressions;

public static class WWW
{
    public static bool IsValidEmail(string email)
    {        
        Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
        Match match = regex.Match(email);
        if (match.Success)
            return true;
        else
            return false;
    
    }

    public static bool IsValidIPV4(string value)
    {
        var quads = value.Split('.');

        // if we do not have 4 quads, return false
        if (!(quads.Length == 4)) return false;

        // for each quad
        foreach (var quad in quads)
        {
            int q;
            // if parse fails 
            // or length of parsed int != length of quad string (i.e.; '1' vs '001')
            // or parsed int < 0
            // or parsed int > 255
            // return false
            if (!Int32.TryParse(quad, out q)
                || !q.ToString().Length.Equals(quad.Length)
                || q < 0
                || q > 255) { return false; }

        }

        return true;
    }

    public struct PingStatus
    {
        public IPStatus status;
        public int ttl;
        public int rtt;
    }

    /// <summary>
    /// Send's E-Mail
    /// </summary>
    /// <param name="from">Sender Email address</param>
    /// <param name="subject">Mail subject</param>
    /// <param name="message">Message body</param>
    /// <param name="to">Destination addresses separated by ;</param>
    /// <param name="host">Email server (ex: smtp.gmail.com)</param>
    /// <param name="login">Email server: account login</param>
    /// <param name="password">Email server: account password</param>
    public static void sendMail(string from, string subject, string message, string to, string host, string login, string password)
    {
        
        System.Windows.MessageBox.Show(to + ": " + message);
        return;
        
        try
        {
            string[] contacts;
            List<MailAddress> contactList = new List<MailAddress>();
            MailMessage MailClient = new MailMessage();

            //Split emails
            contacts = to.Split(';');

            foreach (string s in contacts) { MailClient.To.Add(new MailAddress(s)); }

            MailClient.Subject = subject;
            MailClient.Body = message;
            MailClient.BodyEncoding = Encoding.GetEncoding("Windows-1254");
            MailClient.From = new MailAddress(from);
            

            System.Net.Mail.SmtpClient Smtp = new SmtpClient();
            Smtp.Host = host;
            Smtp.EnableSsl = true;

            #region wtv
            Smtp.Credentials = new System.Net.NetworkCredential(login, password);
            #endregion
            Smtp.Send(MailClient);            
        }
        catch (Exception ex) { throw ex; }
    }

    public static PingStatus pingTarget(IPAddress ipAddress, int timeOutTime)
    {
        try
        {
            PingStatus status = new PingStatus();
            Ping pingSender = new Ping();
            PingReply pReply;

            pReply = pingSender.Send(ipAddress, timeOutTime);
            status.status = pReply.Status;
            if (status.status == IPStatus.Success)
            {
                status.ttl = pReply.Options.Ttl;
                status.rtt = (int)pReply.RoundtripTime;
            }

            return status;
        }
        catch (Exception e) { throw e; }
    }

    public static PingStatus pingTarget(string hostName, int timeOutTime)
    {
        PingStatus status = new PingStatus();
        try
        {
            Uri url = new Uri("http://" + hostName);           
            Ping pingSender = new Ping();
            PingReply pReply;

            pReply = pingSender.Send(url.Host, timeOutTime);
            status.status = pReply.Status;
            if (status.status == IPStatus.Success)
            {
                status.ttl = pReply.Options.Ttl;
                status.rtt = (int)pReply.RoundtripTime;
            }
            return status;
        }
        catch (Exception e)
        {
            int a = System.Runtime.InteropServices.Marshal.GetExceptionCode();
            if (a == -532462766) { status.status = IPStatus.TimedOut; return status; } //"No such host is known" error code
            if (e.Message.CompareTo("No such host is known") == 0) { status.status = IPStatus.TimedOut; return status; }
            else throw e;
        }
    }
}

/// <summary>
/// Used for encryption and decryption
/// </summary>
public static class Cypher
{
    // This constant string is used as a "salt" value for the PasswordDeriveBytes function calls.
    // This size of the IV (in bytes) must = (keysize / 8).  Default keysize is 256, so the IV must be
    // 32 bytes long.  Using a 16 character string here gives us 32 bytes when converted to a byte array.
    private const string initVector = "tu89geji340t89u2";

    // This constant is used to determine the keysize of the encryption algorithm.
    private const int keysize = 256;

    /// <summary>
    /// Encrypts data
    /// </summary>
    /// <param name="plainText">Data to be encrypted</param>
    /// <param name="passPhrase">Password</param>
    /// <returns></returns>
    public static string Encrypt(string plainText, string passPhrase)
    {
        try
        {
            byte[] initVectorBytes = Encoding.UTF8.GetBytes(initVector);
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, null);
            byte[] keyBytes = password.GetBytes(keysize / 8);
            RijndaelManaged symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;
            ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
            cryptoStream.FlushFinalBlock();
            byte[] cipherTextBytes = memoryStream.ToArray();
            memoryStream.Close();
            cryptoStream.Close();
            return Convert.ToBase64String(cipherTextBytes);
        }
        catch (Exception e) { throw e; }
    }

    /// <summary>
    /// Un-encrypts data
    /// </summary>
    /// <param name="cipherText">Encrypted data</param>
    /// <param name="passPhrase">Password</param>
    /// <returns></returns>
    public static string Decrypt(string cipherText, string passPhrase)
    {
        try { 
            byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
            byte[] cipherTextBytes = Convert.FromBase64String(cipherText);
            PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, null);
            byte[] keyBytes = password.GetBytes(keysize / 8);
            RijndaelManaged symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;
            ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);
            MemoryStream memoryStream = new MemoryStream(cipherTextBytes);
            CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];
            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            memoryStream.Close();
            cryptoStream.Close();
            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
        }
        catch (Exception e) { throw e; }     
    }

    public static bool checkPassword(string encodedPass, string password)
    {
        try { if (Decrypt(encodedPass, password).CompareTo(password) == 0) return true; }
        catch (Exception e) { throw e; }

        return false;
    }
}

public class JsonHelper
{
    /// <summary>
    /// JSON Serialization
    /// </summary>
    public static string JsonSerializer<T>(T t)
    {
        try
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            MemoryStream ms = new MemoryStream();
            
            ser.WriteObject(ms, t);
            string jsonString = Encoding.UTF8.GetString(ms.ToArray());            
            ms.Close();

            return jsonString;
        }
        catch (Exception e) { throw e; }      
    }

    /// <summary>
    /// JSON Deserialization
    /// </summary>
    public static T JsonDeserialize<T>(string jsonString)
    {
        try
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
            T obj = (T)ser.ReadObject(ms);
            return obj;
        }
        catch (Exception e) { throw e; }
    }
}