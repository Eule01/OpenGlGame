namespace GameCore.UserInterface
{
    public class KeyBinding
    {
        public KeyBindings.Ids Id;
        public int Key;
        public string Description;
        public string KeyName;

        public KeyBinding()
        {
        }

        public KeyBinding(KeyBindings.Ids anId, byte aKey, string aDescription, string aKeyName)
        {
            Id = anId;
            Key = aKey;
            Description = aDescription;
            KeyName = aKeyName;
        }

        public override string ToString()
        {
            string outStr = "";
            outStr += Id;
            outStr += ": " + KeyName;
            outStr += " [" + Key + "]";
            outStr += ": " + Description;
            return outStr;
        }
    }
}