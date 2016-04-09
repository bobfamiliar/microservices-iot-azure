using System;
using LooksFamiliar.Microservices.Biometrics.Interface;
using LooksFamiliar.Microservices.Biometrics.Models;

namespace LooksFamiliar.Microservice.Biometrics.Service
{
    public class BiometricService : IBiometric
    {
        DataAccess _dac;

        public BiometricService(string connStr)
        {
            _dac = new DataAccess();
            _dac.Connect(connStr);
        }

        public BiometricReadings GetBiometricsByDeviceId(string deviceid, int count)
        {
            BiometricReadings readings = null;
            try
            {
                readings = _dac.GetBiometricsByDeviceId(deviceid, count);
            }
            catch (Exception)
            {
                throw;
            }
            return readings;
        }

        public BiometricReadings GetBiometricsByParticipantId(string participantid, int count)
        {
            BiometricReadings readings = null;
            try
            {
                readings = _dac.GetBiometricsByParticipantId(participantid, count);
            }
            catch (Exception)
            {
                throw;
            }
            return readings;
        }

        public BiometricReadings GetBiometricsByLocationType(string location, string type, int count)
        {
            BiometricReadings readings = null;
            var readingType = 0;

            try
            {
                switch (type)
                {
                    case "glucose":
                        readingType = 1;
                        break;
                    case "heartrate":
                        readingType = 2;
                        break;
                    case "bloodoxygen":
                        readingType = 3;
                        break;
                    case "temperature":
                        readingType = 4;
                        break;
                }

                if (location == "all")
                {
                    var readingsBoston = _dac.GetBiometricsByLocationType("boston", readingType, count);
                    var readingsChicago = _dac.GetBiometricsByLocationType("chicago", readingType, count);
                    var readingsNewYork = _dac.GetBiometricsByLocationType("newyork", readingType, count);

                    readings = new BiometricReadings();

                    readings.AddRange(readingsBoston);
                    readings.AddRange(readingsChicago);
                    readings.AddRange(readingsNewYork);
                }
                else
                {
                    readings = _dac.GetBiometricsByLocationType(location, readingType, count);
                }

            }
            catch (Exception)
            {
                throw;
            }
            return readings;
        }

        public void CreateAlarm(BiometricReading alarm)
        {
            _dac.Insert(alarm);
        }
    }
}