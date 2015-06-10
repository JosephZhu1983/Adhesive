
using System;
namespace Adhesive.DistributedService.TestService.Host
{
    public class FuckService : IFuckService
    {
        public string YouWannaFuckWho(string name, int time)
        {
            if (time > 100)
                throw new Exception("You really can fuck so mang times?");
            return string.Format("Fuck {0} {1} times!", name, time);
        }
    }
}
