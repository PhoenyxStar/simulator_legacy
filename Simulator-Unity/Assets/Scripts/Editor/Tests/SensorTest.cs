// Team Robosub
using UnityEngine;
using NUnit.Framework;
using System;

namespace TeamStark.RobosubSimulator
{
    [TestFixture]
    [Category("Sensor Tests")]
    public class SensorTest
    {
        private Sensors sensor;

        public SensorTest()
        {
            sensor = new Sensors();
        }

        /*
            // Depth test
            for (int i = 0; i < 15; i++)
            {
                sensor.Depth = i;
                double d = ((sensor.Depth * 3.28084) + 10.341) / .1728;
                if (Math.Abs(sensor.GetDepthOutput() - d) >= 0.0001)
                {
                    Debug.LogError("output error. depth not equal");
                    return false;
                }
            }
        */
        [Test]
        [Category("Depth Test")]
        public void TestDepth([NUnit.Framework.Range(0, 15, 1)] int d)
        {
            double error = 0.0001;
            sensor.Depth = d;
            double depth = ((sensor.Depth * 3.28084) + 10.341) / .1728;
            Assert.Less(Math.Abs(sensor.GetDepthOutput() - depth), error, depth.ToString() + "!=" + sensor.GetDepthOutput().ToString());
        }

        /*
            // GetGyroscopeOutput test
            for (float i = -5.0f; i < 5.0f; i += 0.1f)
            {
                sensor.AngularVelocity = new Vector3(i, i, i);
                Vector3 v = sensor.AngularVelocity / 0.0174533f * 14.375f;
                if (Math.Abs(sensor.GetGyroscopeOutput().x - v.x) >= 0.1f ||
                    Math.Abs(sensor.GetGyroscopeOutput().y - v.y) >= 0.1f ||
                    Math.Abs(sensor.GetGyroscopeOutput().z - v.z) >= 0.1f)
                {
                    Debug.LogError("output error. angularVelocity not equal");
                    return false;
                }
            }
        */
        [Test]
        [Category("Gyroscope Test")]
        public void TestGyroscope([NUnit.Framework.Range(-5.0f, 5.0f, 0.1f)] float av)
        {
            float error = 0.1f;
            sensor.AngularVelocity = new Vector3(av, av, av);
            Vector3 v = sensor.AngularVelocity / 0.0174533f * 14.375f;
            Assert.Less(Math.Abs(sensor.GetGyroscopeOutput().x - v.x), error);
            Assert.Less(Math.Abs(sensor.GetGyroscopeOutput().y - v.y), error);
            Assert.Less(Math.Abs(sensor.GetGyroscopeOutput().z - v.z), error);
        }

        /*
            for (float i = -5.0f; i < 5.0f; i += 0.1f)
            {
                Vector3 v = new Vector3(i, i, i);
                sensor.Acceleration = v;
                if (v != sensor.GetAccelerometerOutput() / (256.0f / 9.81f))
                {
                    Debug.LogError("output error. GetAccelerometerOutput not equal");
                    return false;
                }
            }
         */
        [Test]
        [Category("Accelerometer Test")]
        public void TestAccelerometer([NUnit.Framework.Range(-5.0f, 5.0f, 0.1f)] float am)
        {
            float error = 0.0001f;
            Vector3 newam = new Vector3(am, am, am);
            sensor.Acceleration = newam;
            //Assert.AreEqual(newam, sensor.GetAccelerometerOutput() / (256.0f / 9.81f));
            Assert.Less(Math.Abs(sensor.GetAccelerometerOutput().x / (256.0f / 9.81f) - newam.x), error);
            Assert.Less(Math.Abs(sensor.GetAccelerometerOutput().y / (256.0f / 9.81f) - newam.y), error);
            Assert.Less(Math.Abs(sensor.GetAccelerometerOutput().z / (256.0f / 9.81f) - newam.z), error);
        }
    }
}