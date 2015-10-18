using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using LooksFamiliar.Microservices.Biometrics.Models;

namespace LooksFamiliar.Microservice.Biometrics.Service
{
    public class DataAccess
    {
        private SqlConnection _conn;

        public DataAccess()
        {
            _conn = null;
        }

        public void Connect(string connStr)
        {
            _conn = new SqlConnection(connStr);
            _conn.Open();
        }

        private static BiometricReadings Transform(IDataReader rdr)
        {
            var readings = new BiometricReadings();

            while (rdr.Read())
            {
                var reading = new BiometricReading
                {
                    deviceid = rdr[0].ToString(),
                    participantid = rdr[1].ToString(),
                    longitude = (double) rdr[2],
                    latitude = (double) rdr[3],
                    reading = (DateTime) rdr[4],
                    value = (double) rdr[6]
                };

                var sensorType = Convert.ToInt32(rdr[5]);

                switch (sensorType)
                {
                    case 1:
                        reading.type = BiometricType.Glucose;
                        break;
                    case 2:
                        reading.type = BiometricType.Heartrate;
                        break;
                    case 3:
                        reading.type = BiometricType.Bloodoxygen;
                        break;
                    case 4:
                        reading.type = BiometricType.Temperature;
                        break;
                }

                readings.Add(reading);
            }

            rdr.Close();

            return readings;
        }

        public BiometricReadings GetBiometricsByDeviceId(string deviceid, int count)
        {
            // read the latest ROW number of rows
            var cmd = new SqlCommand("select top " + count + " * from biometrics where deviceid = '" + deviceid + "' order by reading desc", _conn);
            var rdr = cmd.ExecuteReader();
            return Transform(rdr);
        }

        public BiometricReadings GetBiometricsByParticipantId(string participantid, int count)
        {
            // read the latest ROW number of rows
            var cmd = new SqlCommand("select top " + count + " * from biometrics where participantid = '" + participantid + "' order by reading desc", _conn);
            var rdr = cmd.ExecuteReader();
            return Transform(rdr);
        }

        public BiometricReadings GetBiometricsByLocationType(string location, int type, int count)
        {
            var sql = string.Empty;

            switch (location)
            {
                case "boston":
                    sql = "select top " + count + " * from biometrics where longitude < -71 and longitude > -72 and latitude > 42 and type = " + type + " order by reading desc";
                    break;
                case "newyork":
                    sql = "select top " + count + " * from biometrics where longitude < -73 and longitude > -74 and latitude > 40 and type = " + type + " order by reading desc";
                    break;
                case "chicago":
                    sql = "select top " + count + " * from biometrics where longitude < -87 and longitude > -89 and latitude > 41 and type = " + type + " order by reading desc";
                    break;
            }

            var cmd = new SqlCommand(sql, _conn);
            var rdr = cmd.ExecuteReader();
            return Transform(rdr);
        }

        public void Insert(BiometricReading alarm)
        {
            var type = 0;
            var sql = string.Empty;

            try
            {
                switch (alarm.type)
                {
                    case BiometricType.Glucose:
                        type = 1;
                        break;
                    case BiometricType.Heartrate:
                        type = 2;
                        break;
                    case BiometricType.Bloodoxygen:
                        type = 3;
                        break;
                    case BiometricType.Temperature:
                        type = 4;
                        break;
                    case BiometricType.NotSet:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                sql = $"INSERT INTO [dbo].[alarms]([deviceid],[participantid],[longitude],[latitude],[reading],[type],[value]) VALUES('{alarm.deviceid}','{alarm.participantid}',{alarm.longitude},{alarm.latitude},'{alarm.reading}',{type},{alarm.value})";
                var cmd = new SqlCommand(sql, _conn);
                cmd.ExecuteReader();
            }
            catch (Exception err)
            {
                Trace.TraceError(sql);
                Trace.TraceError(err.Message);
            }
        }

        public void Disconnect()
        {
            _conn.Close();
        }
    }
}
