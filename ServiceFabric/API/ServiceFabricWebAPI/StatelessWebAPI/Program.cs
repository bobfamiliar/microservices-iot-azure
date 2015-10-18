using System;
using System.Fabric;
using System.Fabric.Query;
using System.Threading;

namespace StatelessWebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                using (var fabricRuntime = FabricRuntime.Create())
                {
                    fabricRuntime.RegisterServiceType(StatelessWebAPI.ServiceTypeName, typeof(StatelessWebAPI));

                    Thread.Sleep(Timeout.Infinite);
                }
            }
            catch (Exception e)
            {
                ServiceEventSource.Current.ServiceHostInitializationFailed(e);
                throw;
            }
        }
    }
}
