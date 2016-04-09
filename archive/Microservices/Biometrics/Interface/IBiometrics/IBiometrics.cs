using LooksFamiliar.Microservices.Biometrics.Models;

namespace LooksFamiliar.Microservices.Biometrics.Interface
{
    public interface IBiometric
    {
        BiometricReadings GetBiometricsByDeviceId(string deviceid, int count);
        BiometricReadings GetBiometricsByParticipantId(string participantid, int count);
        BiometricReadings GetBiometricsByLocationType(string location, string type, int count);
        void CreateAlarm(BiometricReading alarm);
    }
}
