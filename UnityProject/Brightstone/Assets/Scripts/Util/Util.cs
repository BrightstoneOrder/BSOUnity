using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
#if UNITY
using UnityEngine;
#endif

namespace Brightstone
{
    public static class Util
    {
        public const int INVALID_INT = -1;

        [Test]
        public class UtilTest : Test
        {
            private enum EnumTest
            {
                A,
                B,
                C
            }

            public override void RunTest()
            {
                const int HEX_INT = 0xDEB4;
                const long HEX_LONG = 0xDEB4CAD7EFL;
                const float HEX_FLOAT = 45567.232320f;
                string hexInt = Util.GetHexString(HEX_INT);
                string hexLong = Util.GetHexString(HEX_LONG);
                string hexFloat = Util.GetHexString(HEX_FLOAT);
                int hexIntVal = Util.GetHex32(hexInt);
                long hexLongVal = Util.GetHex64(hexLong);
                float hexFloatVal = Util.GetHexFloat(hexFloat);

                TEST(GetEnumCount<EnumTest>() == 3);
                TEST(hexInt == "DEB4");
                TEST(hexLong == "DEB4CAD7EF");
                TEST(hexFloat == "4731FF3B");
                TEST(hexIntVal == HEX_INT);
                TEST(hexLongVal == HEX_LONG);
                TEST(hexFloatVal == HEX_FLOAT);

                string quoteA = "Hello World"; // HelloWorld
                string quoteB = "\"Hello World\""; // Hello World
                string quoteC = "\"\\\"\\\"Rub\\\" \\\"Ish \""; //""Rub "Ish "
                string quoteD = "\t\tmName:String(\"Sword\")"; // mName:String(Sword)

                string quoteResA = StrStripWhitespace(quoteA, true);
                string quoteResB = StrStripWhitespace(quoteB, true);
                string quoteResC = StrStripWhitespace(quoteC, true);
                string quoteResD = StrStripWhitespace(quoteD, true);

                TEST(quoteResA == "HelloWorld");
                TEST(quoteResB == "Hello World");
                TEST(quoteResC == "\"\"Rub\" \"Ish ");
                TEST(quoteResD == "mName:String(Sword)");
            }
        }

        public static int SizeOf(Type type)
        {
            return System.Runtime.InteropServices.Marshal.SizeOf(type);
        }

        /**
        *  Gets the number of enums in a enum for T.
        */
        public static int GetEnumCount<T>() where T : struct, IConvertible
        {
            Type enumType = typeof(T);
            if (!enumType.IsEnum)
            {
                return -1;
            }
            return Enum.GetValues(enumType).Length;
        }

        public static T GetEnumValueFromString<T>(string valueString) where T : struct, IConvertible
        {
            Type enumType = typeof(T);
            if(!enumType.IsEnum)
            {
                return default(T);
            }
            return (T)Enum.Parse(enumType, valueString);
        }
        /**
        * Converts string hex value to int
        */
        public static int GetHex32(string stringValue)
        {
            return Convert.ToInt32(stringValue, 16);
        }

        /**
        * Converts string hex value to long
        */
        public static long GetHex64(string stringValue)
        {
            return Convert.ToInt64(stringValue, 16);
        }

        /**
        * Converts string hex value to float  (does bit-wise conversion from uint > float)
        */
        public static float GetHexFloat(string stringValue)
        {
            uint bitValue = Convert.ToUInt32(stringValue, 16);
            return BitConverter.ToSingle(BitConverter.GetBytes(bitValue), 0);
        }

        /**
        * Converts number to a hex string.
        */
        public static string GetHexString(int number)
        {
            return number.ToString("X");
        }

        /**
        * Converts number to a hex string
        */
        public static string GetHexString(long number)
        {
            return number.ToString("X");
        }

        /**
        * Converts number to a hex string. (Note: number is converted to uint before converted to hex)
        */
        public static string GetHexString(float number)
        {
            uint bitValue = BitConverter.ToUInt32(BitConverter.GetBytes(number), 0);
            return bitValue.ToString("X");
        }

        /**
        * Retrieves a list of 'Types' using the attribType
        */
        public static List<Type> GetAttributeTypes(Type attribType)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            List<Type> attribTypes = new List<Type>();
            if(assembly != null)
            {
                Type[] types = assembly.GetTypes();
                for(int i = 0; i < types.Length; ++i)
                {
                    if(types[i].GetCustomAttributes(attribType, true).Length > 0)
                    {
                        attribTypes.Add(types[i]);
                    }
                }
            }
            return attribTypes;
        } 
        

        public static string GetCurrentDirectory()
        {
            return Directory.GetCurrentDirectory();
        }

        /**
        * Strips whitespace from a string.
        * If quotes is true it will respect quotes unless \" is inside the string.
        * A literal string should use \\\"
        */
        public static string StrStripWhitespace(string s, bool quotes)
        {
            string output = string.Empty;
            char lastChar = '\0';
            char c = '\0';
            bool inQuotes = false;

            for (int i = 0; i < s.Length; ++i)
            {
                c = s[i];
                if(quotes)
                {
                    if(!inQuotes)
                    {
                        if(c == '\"')
                        {
                            inQuotes = !inQuotes;
                        }
                        else if (c != ' ' && c != '\t' && c != '\\')
                        {
                            output += c;
                        }
                    }
                    else
                    {
                        // \" style qutoe..
                        if (c == '\"' && lastChar == '\\')
                        {
                            output += c;
                        }
                        else if (c == '\"') // Actual quote, lets get out of quotes.
                        {
                            inQuotes = !inQuotes;
                        }
                        else if (c != '\\')
                        {
                            output += c;
                        }
                    }
                }
                else
                {
                    if(c != ' ' && c != '\t')
                    {
                        output += c;
                    }
                }
                lastChar = c;
            }
            return output;
        }

#if UNITY
        public static StringBuilder Clear(this StringBuilder self)
        {
            self.Length = 0;
            return self;
        }

#endif
        public static string GetUserDataDirectory(string filePath)
        {
#if UNITY
            string path = Application.dataPath;
            return path + "/UserData" + filePath;
#else
            return filePath;
#endif
        }
        public static bool Valid(int value)
        {
            return value != INVALID_INT;
        }

        public static bool Invalid(int value)
        {
            return value == INVALID_INT;
        }
    }
}