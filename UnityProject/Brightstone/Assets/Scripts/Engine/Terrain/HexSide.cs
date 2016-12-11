namespace Brightstone
{
    public enum HexSide
    {
        HS_NORTH_EAST,
        HS_EAST,
        HS_SOUTH_EAST,
        HS_SOUTH_WEST,
        HS_WEST,
        HS_NORTH_WEST
    }

    public class HexSideBitfield : Bitfield<HexSide>
    {
        public HexSideBitfield(int value) : base(value)
        {

        }

        public HexSideBitfield(HexSide[] sides) : base()
        {
            for(int i = 0; i < sides.Length; ++i)
            {
                Set(sides[i]);
            }
        }
    }
}