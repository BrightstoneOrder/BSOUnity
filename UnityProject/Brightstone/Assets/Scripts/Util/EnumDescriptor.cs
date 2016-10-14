namespace Brightstone
{
    public struct EnumDescriptor
    {
        public string name;
        public int id;

        public EnumDescriptor(string inName, int inID)
        {
            name = inName;
            id = inID;
        }
    }
}