using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Tutorial_Server
{
    
    public class RedisProvider
    {
        static ConnectionMultiplexer manager;
        static int counter;
        static public void Open(string host, int port, string password)
        {
            manager = ConnectionMultiplexer.Connect($"{host}:{port},password={password},allowadmin=true");
        }
        static public string CreateSession(int accountID)
        {
            //쓰레드 상에서 값을 추가 해준다.
            Interlocked.Increment(ref counter);
            long sessionCounter = (int)DateTime.UtcNow.Ticks << 32 | counter; // FFFF(tick값) FFFF(counter값)
            var value = string.Format("{0:X}", sessionCounter);
            manager.GetDatabase(0).StringSet(value, accountID, TimeSpan.FromMinutes(1));
            return value; //SessionKey를 return
        }

        static public string GetSession(string sessionKey)
        {
           return manager.GetDatabase(0).StringGet(sessionKey); // SessionKey를 넣어서 아이디를 가져온다.
        }
    }


}
