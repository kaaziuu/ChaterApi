using System;

namespace Chater.Exception
{
    public class RoomWithThisNameExist : System.Exception
    {
        public RoomWithThisNameExist(string message) : base(message)
        {
            
        }
    }
}