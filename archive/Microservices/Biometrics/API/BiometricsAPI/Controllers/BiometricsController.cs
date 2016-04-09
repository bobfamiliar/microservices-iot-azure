using System.Configuration;
using System.Web.Http;
using LooksFamiliar.Microservice.Biometrics.Service;
using LooksFamiliar.Microservices.Biometrics.Models;

namespace BiometricsAPI.Controllers
{
    public class BiometricsController : ApiController
    {
        private readonly BiometricService _biometrics;

        public BiometricsController()
        {
            var sqlConnStr = ConfigurationManager.AppSettings["SQLConnStr"];
            _biometrics = new BiometricService(sqlConnStr);
        }

        [Route("biometrics/device/{deviceid}/count/{count}")]
        [HttpGet]
        public BiometricReadings GetReadingsByDeviceId(string deviceid, int count)
        {
            return _biometrics.GetBiometricsByDeviceId(deviceid, count);
        }

        [Route("biometrics/participant/{participantid}/count/{count}")]
        [HttpGet]
        public BiometricReadings GetReadingsByParticipantId(string participantid, int count)
        {
            return _biometrics.GetBiometricsByParticipantId(participantid, count);
        }

        [Route("biometrics/city/{city}/type/{type}/count/{count}")]
        [HttpGet]
        public BiometricReadings GetReadingsByLocationType(string city, string type, int count)
        {
            return _biometrics.GetBiometricsByLocationType(city, type, count);
        }

        [Route("biometrics/alarm")]
        [HttpPost]
        public void CreateAlarm([FromBody] BiometricReading alarm)
        {
            _biometrics.CreateAlarm(alarm);
        }
    }
}
