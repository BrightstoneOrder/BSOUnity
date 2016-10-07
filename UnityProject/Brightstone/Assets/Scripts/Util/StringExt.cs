using System;

namespace Brightstone
{

    /** Extensions to the string class*/
    public static class StringExt
    {
        public static void Something()
        {

        }
        /** Searches for first occurance of c in thisString
        * "Basea" search 'a' == 1
        * @return Returns index of char or -1 if not found.
        */
        public static int Find(this string thisString, char c)
        {
            for (int i = 0; i < thisString.Length; ++i)
            {
                if (thisString[i] == c)
                {
                    return i;
                }
            }
            return -1;
        }

        /** Searches for first occurance of c in thisString
        * "Basea" search 'a' == 4
        * @return Returns index of char or -1 if not found.
        */
        public static int FindLast(this string thisString, char c)
        {
            for (int i = thisString.Length - 1; i >= 0; --i)
            {
                if (thisString[i] == c)
                {
                    return i;
                }
            }
            return -1;
        }

        /** Searches for first occurance of c in thisString
        * "Big One Wolf" search 'One' == 4
        * @return Returns index of char or -1 if not found.
        */
        public static int Find(this string thisString, string str)
        {
            // not possible, exit early. eg. "Big Worlew" find in "world"
            if (str.Length > thisString.Length || str == string.Empty || thisString == string.Empty)
            {
                return -1;
            }
            for (int i = 0; i < thisString.Length; ++i)
            {
                if (thisString[i] == str[0])
                {
                    ++i;
                    // i == 4
                    // j == 0
                    bool match = true;
                    int index = i;
                    for (int j = 1; j < str.Length && i < thisString.Length; ++i, ++j)
                    {
                        if (thisString[i] != str[j])
                        {
                            match = false;
                            --i; // sub one so we can read next char properly instead of skipping in outter loop
                            break;
                        }
                    }
                    if (match)
                    {
                        return index - 1;
                    }
                }
            }
            return -1;
        }

        public static int FindLast(this string thisString, string str)
        {
            // not possible, exit early. eg. "Big Bor Bor Ror" find in "Bor"
            if (str.Length > thisString.Length)
            {
                return -1;
            }
            int lastStrIndex = str.Length - 1;
            for (int i = thisString.Length - 1; i >= 0; --i)
            {
                if (thisString[i] == str[lastStrIndex])
                {
                    --i;
                    bool match = true;
                    for (int j = lastStrIndex - 1; j >= 0 && i >= 0; --i, --j)
                    {
                        if (thisString[i] != str[j])
                        {
                            match = false;
                            ++i;
                            break;
                        }
                    }
                    if (match)
                    {
                        return i + 1;
                    }
                }
            }
            return -1;
        }
    }
}