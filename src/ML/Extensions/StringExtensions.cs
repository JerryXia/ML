using System;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Mail;

namespace ML.Extensions
{
    /// <summary>
    /// String Extentensions
    /// </summary>
    public static class StringExtensions
    {
        #region FormatWith

        /// <summary>
        /// Formats a string with one literal placeholder.
        /// </summary>
        /// <param name="text">The extension text</param>
        /// <param name="arg0">Argument 0</param>
        /// <returns>The formatted string</returns>
        public static string FormatWith(this string text, object arg0)
        {
            return string.Format(text, arg0);
        }

        /// <summary>
        /// Formats a string with two literal placeholders.
        /// </summary>
        /// <param name="text">The extension text</param>
        /// <param name="arg0">Argument 0</param>
        /// <param name="arg1">Argument 1</param>
        /// <returns>The formatted string</returns>
        public static string FormatWith(this string text, object arg0, object arg1)
        {
            return string.Format(text, arg0, arg1);
        }

        /// <summary>
        /// Formats a string with tree literal placeholders.
        /// </summary>
        /// <param name="text">The extension text</param>
        /// <param name="arg0">Argument 0</param>
        /// <param name="arg1">Argument 1</param>
        /// <param name="arg2">Argument 2</param>
        /// <returns>The formatted string</returns>
        public static string FormatWith(this string text, object arg0, object arg1, object arg2)
        {
            return string.Format(text, arg0, arg1, arg2);
        }

        /// <summary>
        /// Formats a string with a list of literal placeholders.
        /// </summary>
        /// <param name="text">The extension text</param>
        /// <param name="args">The argument list</param>
        /// <returns>The formatted string</returns>
        public static string FormatWith(this string text, params object[] args)
        {
            return string.Format(text, args);
        }

        /// <summary>
        /// Formats a string with a list of literal placeholders.
        /// </summary>
        /// <param name="text">The extension text</param>
        /// <param name="provider">The format provider</param>
        /// <param name="args">The argument list</param>
        /// <returns>The formatted string</returns>
        public static string FormatWith(this string text, IFormatProvider provider, params object[] args)
        {
            return string.Format(provider, text, args);
        }

        #endregion

        #region XmlSerialize XmlDeserialize

        /// <summary>Serialises an object of type T in to an xml string</summary>
        /// <typeparam name="T">Any class type</typeparam>
        /// <param name="objectToSerialise">Object to serialise</param>
        /// <returns>A string that represents Xml, empty oterwise</returns>
        public static string XmlSerialize<T>(this T objectToSerialise) where T : class
        {
            var serialiser = new XmlSerializer(typeof (T));
            string xml;
            using (var memStream = new MemoryStream())
            {
                using (var xmlWriter = new XmlTextWriter(memStream, Encoding.UTF8))
                {
                    serialiser.Serialize(xmlWriter, objectToSerialise);
                    xml = Encoding.UTF8.GetString(memStream.GetBuffer());
                }
            }

            // ascii 60 = '<' and ascii 62 = '>'
            xml = xml.Substring(xml.IndexOf(Convert.ToChar(60)));
            xml = xml.Substring(0, (xml.LastIndexOf(Convert.ToChar(62)) + 1));
            return xml;
        }

        /// <summary>Deserialises an xml string in to an object of Type T</summary>
        /// <typeparam name="T">Any class type</typeparam>
        /// <param name="xml">Xml as string to deserialise from</param>
        /// <returns>A new object of type T is successful, null if failed</returns>
        public static T XmlDeserialize<T>(this string xml) where T : class
        {
            var serialiser = new XmlSerializer(typeof (T));
            T newObject;

            using (var stringReader = new StringReader(xml))
            {
                using (var xmlReader = new XmlTextReader(stringReader))
                {
                    try
                    {
                        newObject = serialiser.Deserialize(xmlReader) as T;
                    }
                    catch (InvalidOperationException) // String passed is not Xml, return null
                    {
                        return null;
                    }

                }
            }

            return newObject;
        }

        #endregion

        #region To X conversions

        /// <summary>
        /// Parses a string into an Enum
        /// </summary>
        /// <typeparam name="T">The type of the Enum</typeparam>
        /// <param name="value">String value to parse</param>
        /// <returns>The Enum corresponding to the stringExtensions</returns>
        public static T ToEnum<T>(this string value)
        {
            return ToEnum<T>(value, false);
        }

        /// <summary>
        /// Parses a string into an Enum
        /// </summary>
        /// <typeparam name="T">The type of the Enum</typeparam>
        /// <param name="value">String value to parse</param>
        /// <param name="ignorecase">Ignore the case of the string being parsed</param>
        /// <returns>The Enum corresponding to the stringExtensions</returns>
        public static T ToEnum<T>(this string value, bool ignorecase)
        {
            if (value == null)
                throw new ArgumentNullException("Value");

            value = value.Trim();

            if (value.Length == 0)
                throw new ArgumentNullException("Must specify valid information for parsing in the string.", "value");

            Type t = typeof (T);
            if (!t.IsEnum)
                throw new ArgumentException("Type provided must be an Enum.", "T");

            return (T) Enum.Parse(t, value, ignorecase);
        }

        /// <summary>
        /// Toes the integer.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="defaultvalue">The defaultvalue.</param>
        /// <returns></returns>
        public static int ToInteger(this string value, int defaultvalue)
        {
            return (int) ToDouble(value, defaultvalue);
        }

        /// <summary>
        /// Toes the integer.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static int ToInteger(this string value)
        {
            return ToInteger(value, 0);
        }

        ///// <summary>
        ///// Toes the U long.
        ///// </summary>
        ///// <param name="value">The value.</param>
        ///// <returns></returns>
        //public static ulong ToULong(this string value)
        //{
        //    ulong def = 0;
        //    return value.ToULong(def);
        //}
        ///// <summary>
        ///// Toes the U long.
        ///// </summary>
        ///// <param name="value">The value.</param>
        ///// <param name="defaultvalue">The defaultvalue.</param>
        ///// <returns></returns>
        //public static ulong ToULong(this string value, ulong defaultvalue)
        //{
        //    return (ulong)ToDouble(value, defaultvalue);
        //}

        /// <summary>
        /// Toes the double.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="defaultvalue">The defaultvalue.</param>
        /// <returns></returns>
        public static double ToDouble(this string value, double defaultvalue)
        {
            double result;
            if (double.TryParse(value, out result))
            {
                return result;
            }
            else return defaultvalue;
        }

        /// <summary>
        /// Toes the double.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static double ToDouble(this string value)
        {
            return ToDouble(value, 0);
        }

        /// <summary>
        /// Toes the date time.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="defaultvalue">The defaultvalue.</param>
        /// <returns></returns>
        public static DateTime? ToDateTime(this string value, DateTime? defaultvalue)
        {
            DateTime result;
            if (DateTime.TryParse(value, out result))
            {
                return result;
            }
            else return defaultvalue;
        }

        /// <summary>
        /// Toes the date time.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static DateTime? ToDateTime(this string value)
        {
            return ToDateTime(value, null);
        }

        /// <summary>
        /// Converts a string value to bool value, supports "T" and "F" conversions.
        /// </summary>
        /// <param name="value">The string value.</param>
        /// <returns>A bool based on the string value</returns>
        public static bool? ToBoolean(this string value)
        {
            if (string.Compare("T", value, true) == 0)
            {
                return true;
            }
            if (string.Compare("F", value, true) == 0)
            {
                return false;
            }
            bool result;
            if (bool.TryParse(value, out result))
            {
                return result;
            }
            else return null;
        }

        #endregion

        #region ValueOrDefault

        public static string GetValueOrEmpty(this string value)
        {
            return GetValueOrDefault(value, string.Empty);
        }

        public static string GetValueOrDefault(this string value, string defaultvalue)
        {
            if (value != null) return value;
            return defaultvalue;
        }

        #endregion

        #region ToUpperLowerNameVariant

        /// <summary>
        /// Converts string to a Name-Format where each first letter is Uppercase.
        /// </summary>
        /// <param name="value">The string value.</param>
        /// <returns></returns>
        public static string ToUpperLowerNameVariant(this string value)
        {
            if (string.IsNullOrEmpty(value)) return "";
            char[] valuearray = value.ToLower().ToCharArray();
            bool nextupper = true;
            for (int i = 0; i < (valuearray.Count() - 1); i++)
            {
                if (nextupper)
                {
                    valuearray[i] = char.Parse(valuearray[i].ToString().ToUpper());
                    nextupper = false;
                }
                else
                {
                    switch (valuearray[i])
                    {
                        case ' ':
                        case '-':
                        case '.':
                        case ':':
                        case '\n':
                            nextupper = true;
                            break;
                        default:
                            nextupper = false;
                            break;
                    }
                }
            }
            return new string(valuearray);
        }

        #endregion

        #region Encrypt Decrypt

        /// <summary>
        /// Encryptes a string using the supplied key. Encoding is done using RSA encryption.
        /// </summary>
        /// <param name="stringToEncrypt">String that must be encrypted.</param>
        /// <param name="key">Encryptionkey.</param>
        /// <returns>A string representing a byte array separated by a minus sign.</returns>
        /// <exception cref="ArgumentException">Occurs when stringToEncrypt or key is null or empty.</exception>
        public static string Encrypt(this string stringToEncrypt, string key)
        {
            if (string.IsNullOrEmpty(stringToEncrypt))
            {
                throw new ArgumentException("An empty string value cannot be encrypted.");
            }

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Cannot encrypt using an empty key. Please supply an encryption key.");
            }

            System.Security.Cryptography.CspParameters cspp = new System.Security.Cryptography.CspParameters();
            cspp.KeyContainerName = key;

            System.Security.Cryptography.RSACryptoServiceProvider rsa = new System.Security.Cryptography.RSACryptoServiceProvider(cspp);
            rsa.PersistKeyInCsp = true;

            byte[] bytes = rsa.Encrypt(System.Text.UTF8Encoding.UTF8.GetBytes(stringToEncrypt), true);

            return BitConverter.ToString(bytes);
        }

        /// <summary>
        /// Decryptes a string using the supplied key. Decoding is done using RSA encryption.
        /// </summary>
        /// <param name="key">Decryptionkey.</param>
        /// <returns>The decrypted string or null if decryption failed.</returns>
        /// <exception cref="ArgumentException">Occurs when stringToDecrypt or key is null or empty.</exception>
        public static string Decrypt(this string stringToDecrypt, string key)
        {
            string result = null;

            if (string.IsNullOrEmpty(stringToDecrypt))
            {
                throw new ArgumentException("An empty string value cannot be encrypted.");
            }

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Cannot decrypt using an empty key. Please supply a decryption key.");
            }

            try
            {
                System.Security.Cryptography.CspParameters cspp = new System.Security.Cryptography.CspParameters();
                cspp.KeyContainerName = key;

                System.Security.Cryptography.RSACryptoServiceProvider rsa = new System.Security.Cryptography.RSACryptoServiceProvider(cspp);
                rsa.PersistKeyInCsp = true;

                string[] decryptArray = stringToDecrypt.Split(new string[] {"-"}, StringSplitOptions.None);
                byte[] decryptByteArray = Array.ConvertAll<string, byte>(decryptArray, (s => Convert.ToByte(byte.Parse(s, System.Globalization.NumberStyles.HexNumber))));


                byte[] bytes = rsa.Decrypt(decryptByteArray, true);

                result = System.Text.UTF8Encoding.UTF8.GetString(bytes);

            }
            finally
            {
                // no need for further processing
            }

            return result;
        }

        #endregion

        #region IsValidUrl

        /// <summary>
        /// Determines whether it is a valid URL.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if [is valid URL] [the specified text]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsValidUrl(this string text)
        {
            System.Text.RegularExpressions.Regex rx = new System.Text.RegularExpressions.Regex(@"http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?");
            return rx.IsMatch(text);
        }

        #endregion

        #region IsValidEmailAddress

        /// <summary>
        /// Determines whether it is a valid email address
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if [is valid email address] [the specified s]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsValidEmailAddress(this string email)
        {
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$");
            return regex.IsMatch(email);
        }

        #endregion

        #region Email

        /// <summary>
        /// Send an email using the supplied string.
        /// </summary>
        /// <param name="body">String that will be used i the body of the email.</param>
        /// <param name="subject">Subject of the email.</param>
        /// <param name="sender">The email address from which the message was sent.</param>
        /// <param name="recipient">The receiver of the email.</param> 
        /// <param name="server">The server from which the email will be sent.</param>  
        /// <returns>A boolean value indicating the success of the email send.</returns>
        public static bool Email(this string body, string subject, string sender, string recipient, string server)
        {
            try
            {
                // To
                MailMessage mailMsg = new MailMessage();
                mailMsg.To.Add(recipient);

                // From
                MailAddress mailAddress = new MailAddress(sender);
                mailMsg.From = mailAddress;

                // Subject and Body
                mailMsg.Subject = subject;
                mailMsg.Body = body;

                // Init SmtpClient and send
                SmtpClient smtpClient = new SmtpClient(server);
                System.Net.NetworkCredential credentials = new System.Net.NetworkCredential();
                smtpClient.Credentials = credentials;

                smtpClient.Send(mailMsg);
            }
            catch (Exception ex)
            {
                throw new Exception("Could not send mail from: " + sender + " to: " + recipient + " thru smtp server: " + server + "\n\n" + ex.Message, ex);
            }

            return true;
        }

        #endregion

        #region Truncate

        /// <summary>
        /// Truncates the string to a specified length and replace the truncated to a ...
        /// </summary>
        /// <param name="maxLength">total length of characters to maintain before the truncate happens</param>
        /// <returns>truncated string</returns>
        public static string Truncate(this string text, int maxLength)
        {
            // replaces the truncated string to a ...
            const string suffix = "...";
            string truncatedString = text;

            if (maxLength <= 0) return truncatedString;
            int strLength = maxLength - suffix.Length;

            if (strLength <= 0) return truncatedString;

            if (text == null || text.Length <= maxLength) return truncatedString;

            truncatedString = text.Substring(0, strLength);
            truncatedString = truncatedString.TrimEnd();
            truncatedString += suffix;
            return truncatedString;
        }

        #endregion

        #region HTMLHelper

        /// <summary>
        /// Converts to a HTML-encoded string
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public static string HtmlEncode(this string data)
        {
            return System.Web.HttpUtility.HtmlEncode(data);
        }

        /// <summary>
        /// Converts the HTML-encoded string into a decoded string
        /// </summary>
        public static string HtmlDecode(this string data)
        {
            return System.Web.HttpUtility.HtmlDecode(data);
        }

        /// <summary>
        /// Parses a query string into a System.Collections.Specialized.NameValueCollection
        /// using System.Text.Encoding.UTF8 encoding.
        /// </summary>
        public static System.Collections.Specialized.NameValueCollection ParseQueryString(this string query)
        {
            return System.Web.HttpUtility.ParseQueryString(query);
        }

        /// <summary>
        /// Encode an Url string
        /// </summary>
        public static string UrlEncode(this string url)
        {
            return System.Web.HttpUtility.UrlEncode(url);
        }

        /// <summary>
        /// Converts a string that has been encoded for transmission in a URL into a
        /// decoded string.
        /// </summary>
        public static string UrlDecode(this string url)
        {
            return System.Web.HttpUtility.UrlDecode(url);
        }

        /// <summary>
        /// Encodes the path portion of a URL string for reliable HTTP transmission from
        /// the Web server to a client.
        /// </summary>
        public static string UrlPathEncode(this string url)
        {
            return System.Web.HttpUtility.UrlPathEncode(url);
        }

        #endregion

        #region Format

        /// <summary>
        /// Replaces the format item in a specified System.String with the text equivalent
        /// of the value of a specified System.Object instance.
        /// </summary>
        /// <param name="arg">The arg.</param>
        /// <param name="additionalArgs">The additional args.</param>
        public static string Format(this string format, object arg, params object[] additionalArgs)
        {
            if (additionalArgs == null || additionalArgs.Length == 0)
            {
                return string.Format(format, arg);
            }
            else
            {
                return string.Format(format, new object[] {arg}.Concat(additionalArgs).ToArray());
            }
        }

        #endregion

        #region IsNullOrEmpty

        /// <summary>
        /// Determines whether [is not null or empty] [the specified input].
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if [is not null or empty] [the specified input]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNotNullOrEmpty(this string input)
        {
            return !String.IsNullOrEmpty(input);
        }

        #endregion


        /// <summary>
        /// Parses a string into an Enum
        /// <para></para>
        /// <para>public enum TestEnum</para>
        /// <para>{</para>
        /// <para>    Bar,</para>
        /// <para>    Test</para>
        /// <para>}</para>
        /// <para></para>
        /// <para>public class Test</para>
        /// <para>{</para>
        /// <para>    public void Test()</para>
        /// <para>    {</para>
        /// <para>        TestEnum foo =
        /// &quot;Test&quot;.EnumParse&lt;TestEnum&gt;();</para>
        /// <para>    }</para>
        /// <para>}</para>
        /// </summary>
        /// <typeparam name="T">The type of the Enum</typeparam>
        /// <param name="value">String value to parse</param>
        /// <returns>
        /// The Enum corresponding to the stringExtensions
        /// </returns>
        public static T EnumParse<T>(this string value)
        {
            return StringExtensions.EnumParse<T>(value, false);
        }

        public static T EnumParse<T>(this string value, bool ignorecase)
        {

            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            value = value.Trim();

            if (value.Length == 0)
            {
                throw new ArgumentException("Must specify valid information for parsing in the string.", "value");
            }

            Type t = typeof (T);

            if (!t.IsEnum)
            {
                throw new ArgumentException("Type provided must be an Enum.", "T");
            }

            return (T) Enum.Parse(t, value, ignorecase);
        }


        public static string ToTitleCase(this string mText)
        {
            if (mText == null) return mText;

            System.Globalization.CultureInfo cultureInfo = System.Threading.Thread.CurrentThread.CurrentCulture;
            System.Globalization.TextInfo textInfo = cultureInfo.TextInfo;

            // TextInfo.ToTitleCase only operates on the string if is all lower case, otherwise it returns the string unchanged.
            return textInfo.ToTitleCase(mText.ToLower());
        }

        /// <summary>
        /// Reverse a String
        /// </summary>
        /// <param name="input">The string to Reverse</param>
        /// <returns>The reversed String</returns>
        public static string Reverse(this string input)
        {
            char[] array = input.ToCharArray();
            Array.Reverse(array);
            return new string(array);
        }



        /// <summary> 
        /// Checks string object's value to array of string values 
        /// </summary>         
        /// <param name="stringValues">Array of string values to compare</param> 
        /// <returns>Return true if any string value matches</returns> 
        public static bool In(this string value, params string[] stringValues)
        {
            foreach (string otherValue in stringValues)
                if (string.Compare(value, otherValue) == 0)
                    return true;

            return false;
        }

        /// <summary> 
        /// Converts string to enum object 
        /// </summary> 
        /// <typeparam name="T">Type of enum</typeparam> 
        /// <param name="value">String value to convert</param> 
        /// <returns>Returns enum object</returns> 
        //public static T ToEnum<T>(this string value)
        //    where T : struct
        //{
        //    return (T) System.Enum.Parse(typeof (T), value, true);
        //}

        /// <summary> 
        /// Returns characters from right of specified length 
        /// </summary> 
        /// <param name="value">String value</param> 
        /// <param name="length">Max number of charaters to return</param> 
        /// <returns>Returns string from right</returns> 
        public static string Right(this string value, int length)
        {
            return value != null && value.Length > length ? value.Substring(value.Length - length) : value;
        }

        /// <summary> 
        /// Returns characters from left of specified length 
        /// </summary> 
        /// <param name="value">String value</param> 
        /// <param name="length">Max number of charaters to return</param> 
        /// <returns>Returns string from left</returns> 
        public static string Left(this string value, int length)
        {
            return value != null && value.Length > length ? value.Substring(0, length) : value;
        }

        /// <summary> 
        ///  Replaces the format item in a specified System.String with the text equivalent 
        ///  of the value of a specified System.Object instance. 
        /// </summary> 
        /// <param name="value">A composite format string</param> 
        /// <param name="arg0">An System.Object to format</param> 
        /// <returns>A copy of format in which the first format item has been replaced by the 
        /// System.String equivalent of arg0</returns> 
        public static string Format(this string value, object arg0)
        {
            return string.Format(value, arg0);
        }

        /// <summary> 
        ///  Replaces the format item in a specified System.String with the text equivalent 
        ///  of the value of a specified System.Object instance. 
        /// </summary> 
        /// <param name="value">A composite format string</param> 
        /// <param name="args">An System.Object array containing zero or more objects to format.</param> 
        /// <returns>A copy of format in which the format items have been replaced by the System.String 
        /// equivalent of the corresponding instances of System.Object in args.</returns> 
        public static string Format(this string value, params object[] args)
        {
            return string.Format(value, args);
        }
        

        public static T Parse<T>(this string value)
        {
            // Get default value for type so if string 
            // is empty then we can return default value. 
            T result = default(T);
            if (!string.IsNullOrEmpty(value))
            {
                // we are not going to handle exception here 
                // if you need SafeParse then you should create 
                // another method specially for that. 
                TypeConverter tc = TypeDescriptor.GetConverter(typeof (T));
                result = (T) tc.ConvertFrom(value);
            }
            return result;

        }

        public static  System.IO.MemoryStream ToStream(this string Source)
        {
            byte[] Bytes = System.Text.Encoding.ASCII.GetBytes(Source);
            return new System.IO.MemoryStream(Bytes);
        }

    }
}
