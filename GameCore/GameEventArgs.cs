#region

using System;
using GameCore.Utils.Timers;

#endregion

namespace GameCore
{
    public class GameEventArgs : EventArgs
    {
        public enum Types
        {
            /// <summary>
            /// A fps status has been received.
            /// </summary>
            Status,
            Message,
            /// <summary>
            /// A new map has been loaded (Message = filePath).
            /// </summary>
            MapLoaded,
            /// <summary>
            /// Map has been saved (Message = filePath).
            /// </summary>
            MapSaved
        }

        private Types theType;

        public OpStatus TheOpStatus;

        public string Message = string.Empty;

        public GameEventArgs(Types theType) : base()
        {
            this.theType = theType;
        }

        public Types TheType
        {
            get { return theType; }
        }

        public override string ToString()
        {
            string outStr = "";
            outStr += theType + ":";
            if (TheOpStatus != null)
            {
                outStr += " " + TheOpStatus;
            }
            if (!string.IsNullOrEmpty(Message))
            {
                outStr += " " + Message;
            }
            return outStr;
        }
    }
}