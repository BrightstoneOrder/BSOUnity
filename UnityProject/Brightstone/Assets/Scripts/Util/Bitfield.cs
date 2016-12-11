using System; // IConvertable

namespace Brightstone
{
    /** Generic bitfield class intended to ease enum bitfields operations at the cost of operation overhead. */
    public class Bitfield<T> where T : struct, IConvertible
    {
        /** Value of the bitfield. */
        public int value = 0;
        /** Default constructor*/
        public Bitfield()
        {
            value = 0;
        }
        /** Value initialized constructor */
        public Bitfield(int initValue)
        {
            value = initValue;
        }
        /** Helper function designed to get the integral value of the enum.*/
        private static int GetInteger(T value)
        {
            Type enumType = typeof(T);
            Array values = Enum.GetValues(enumType);
            foreach (var temp in values)
            {
                if (((T)temp).Equals(value))
                {
                    return (int)temp;
                }
            }
            return -1;
        }
        /** Helper function designed to get the 'bit' value of 'value' */
        public static int GetBit(T value)
        {
            return 1 << GetInteger(value);
        }
        /** Sets the bit associated with 'flag' in the bitfield as on.*/
        public void Set(T flag)
        {
            value |= GetBit(flag);
        }
        /** Sets all bits in mask as on.*/
        public void SetMask(int mask)
        {
            value |= mask;
        }
        /** Sets the bit associated with 'flag' in the bitfield as off.*/
        public void Unset(T flag)
        {
            value = value & ~(GetBit(flag));
        }
        /** Sets all bits in mask as off.*/
        public void UnsetMask(int mask)
        {
            value = value & ~(mask);
        }
        /** Returns true if the bit associated with 'flag' is on in the bitfield.*/
        public bool Has(T flag)
        {
            return (value & GetBit(flag)) == (GetBit(flag));
        }
        /** Returns true if the mask is exactly the same as the bitfield mask. */
        public bool Is(int mask)
        {
            return (value & mask) == value;
        }
        /** Returns true if any bits in the mask are enabled in the bitfield.*/
        public bool Any(int mask)
        {
            return (value & mask) != 0;
        }
        /** Sets the bitfield to zero.*/
        public void SetZero()
        {
            value = 0;
        }

        public int GetBitsOn()
        {
            int numOn = 0;
            for (int i = 0, size = Util.GetEnumCount<T>(); i < size; ++i)
            {
                if ((value & (1 << i)) == (1 << i))
                {
                    ++numOn;
                }
            }
            return numOn;
        }
    }
}