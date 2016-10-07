namespace Brightstone
{
	public class TypeMgr 
	{
        /** Singleton stuff..*/
        private static TypeMgr sTypeMgr = null;
        public static TypeMgr GetInstance()
        {
            if(sTypeMgr == null)
            {
                sTypeMgr = new TypeMgr();
                sTypeMgr.PreInit();
            }
            return sTypeMgr;
        }

        // Consider using dictoinary when dealing with 5k + types.
        private ObjectType[] mTypeMap = null;


        private void PreInit()
        {
            
        }

        public void InitFromTypeMap()
        {
            Log.Game.Info("Initializing types...");
            string path = Project.GetResourcePath("/TypeMap.txt");
            string[] lines = Project.ReadAllLines(path);
            if(lines == null || lines.Length == 0)
            {
                return;
            }
            mTypeMap = new ObjectType[lines.Length];

            for(int i = 0; i < lines.Length; ++i)
            {
                string[] args = lines[i].Split(':');
                if(args != null && args.Length == 3)
                {
                    
                }

            }
        }


        public void InitFromTypeMapText(string allText)
        {
            // Type map... t1,t2=id

            const int READ_GARBAGE = 0;
            const int READ_HEADER = 1;
            const int READ_TYPES = 2;
            const char END_LINE = '\n';
            int cursor = 0;
            int mode = READ_GARBAGE;
            int braceCount = 0;
            int mapSize = 0;
            for(int i = 0; i < allText.Length; ++i)
            {
                if(mode == READ_TYPES && mTypeMap == null)
                {
                    Log.Game.Error("Failed to parse. Missing type map.");
                    break;
                }
                switch(mode)
                {
                    case READ_GARBAGE:
                        if (allText[i] == '{')
                        {
                            mode = READ_HEADER;
                            ++braceCount;
                        }
                        break;
                    case READ_HEADER:
                        if(allText[i] == '{')
                        {
                            ++braceCount;
                        }
                        if (allText[i] == '}')
                        {
                            --braceCount;
                            if(braceCount == 0)
                            {
                                mode = READ_TYPES;
                                cursor = i + 2;
                            }
                        }
                        if(mode ==  READ_HEADER)
                        {
                            if(allText[i] == END_LINE)
                            {
                                string line = allText.Substring(cursor, i - 1);
                                if(line.Find("MAP_SIZE") != -1)
                                {
                                    int valSep = line.Find('=');
                                    if(valSep != -1)
                                    {
                                        string val = line.Substring(valSep + 1);
                                        if(int.TryParse(val, out mapSize))
                                        {
                                            mTypeMap = new ObjectType[mapSize];
                                        }
                                    }
                                }
                                cursor = i + 1;
                            }
                        }
                        break;
                    case READ_TYPES:
                        if(mTypeMap == null)
                        {
                            
                            break;
                        }
                        if (allText[i] == END_LINE && i > cursor)
                        {
                            // get line...
                            string line = allText.Substring(cursor, i - cursor);
                            int typeSep = line.Find(',');
                            int idSep = line.Find('=');
                            if(typeSep == -1 || idSep == -1)
                            {
                                Log.Game.Error("Failed to parse line " + line + " missing token , or =");
                            }
                            else
                            {
                                string t1 = line.Substring(0, typeSep);
                                string t2 = line.Substring(typeSep + 1, idSep - typeSep - 1);
                                string t3 = line.Substring(idSep + 1);
                                int id = 0;
                                if(int.TryParse(t3, out id))
                                {
                                    InternalCreateType(t1, t2, id);
                                }
                                else
                                {
                                    Log.Game.Error("Failed to parse line " + line + "id is not a number.");
                                }
                            }
                            cursor = i + 1;
                        }
                        break;
                }
            }
        }

        private void InternalCreateType(string derived, string type, int id)
        {
            if(id >= mTypeMap.Length)
            {
                Log.Game.Error("Failed to create type " + type + " because id is out of range. " + id.ToString());
            }
            else if(mTypeMap[id] != null)
            {
                Log.Game.Error("Failed to create type " + type + " because one already exists with that id. " + id.ToString());
            }
            else
            {
                if(derived == string.Empty)
                {
                    Log.Game.Warning("Creating type " + type + " with no derived type. This type is Isolated.");
                }
                ObjectType objectType = new ObjectType();
                objectType.InternalInit(derived, type, id);
                mTypeMap[id] = objectType;
            }
        }
	}
}