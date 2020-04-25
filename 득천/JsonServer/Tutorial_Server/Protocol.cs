using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tutorial_Server
{
    public enum TProtocol
    {
        None,
        ReqAuth,
        Join,
        Leave,
        Spell,
        ReqAccountCreation
    }

    public enum ResultCode
    {
        OK,
        Error,
        Empty,
    }

    public enum AuthType
    {
        None,
        Google,
        GameCenter,
        Guest
    }

    public class CommonRequest
    {
        public TProtocol protocol;
    }

    internal class CommandClassAttribute : Attribute
    {
        public string Description;
        public CommandClassAttribute(string desc)
        {
            this.Description = desc;
        }
    }

    internal class CommandAttribute : Attribute
    {
        public string Description;
        public CommandAttribute(string desc)
        {
            this.Description = desc;
        }
    }

    public struct DataInventory
    {
        public int itemIdx;
        public int itemID;
        public int count;
        public int option;
    }


    [ProtocolAttribute(TProtocol.ReqAuth)]
    public class ReqAuth : CommonRequest
    {
        public string accountKey;
        public DateTime creation;
        public int gender;
        public int age;
        public AuthType authType;
    }
    public class ResAccountCreation : CommonResponse
    {
        public string accountKey;
        public string sessionKey;
    }
    public class CommonResponse
    {
        public ResultCode result;
        private ResultCode oK;


    }
    public class ResAuth :CommonResponse
    {
        public bool success;
        public string accessKey;
        public List<DataInventory> inventory; //Json은 public 된것만 변환한다.
        public string sessionKey;

        [Protocol(TProtocol.ReqAccountCreation)]
        public class ReqAccountCreation : CommonRequest
        {
            public AuthType authType;
            public string accountKey;
        }
    }

    public class ProtocolAttribute : Attribute
    {
        public TProtocol protocol;
        public ProtocolAttribute(TProtocol protocol)
        {
            this.protocol = protocol;
        }
    }

    public class ReqItemGet : CommonRequest
    {
        public string sessionKey;
        public int itemID;
        
    }
}
