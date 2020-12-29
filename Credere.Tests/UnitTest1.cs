using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.SessionState;
using HttpContext = System.Web.HttpContext;
using HttpRequest = System.Web.HttpRequest;
using HttpResponse = System.Web.HttpResponse;

namespace Credere.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestInitialize]
        public void TestSetup()
        {
            var httpRequest = new HttpRequest("", "http://localhost/", "coordinates=GE,M,M,M,GD,M,M");

            var httpResponce = new HttpResponse(new StringWriter());

            var httpContext = new HttpContext(httpRequest, httpResponce);
            var sessionContainer =
                new HttpSessionStateContainer("id",
                                               new SessionStateItemCollection(),
                                               new HttpStaticObjectsCollection(),
                                               10,
                                               true,
                                               HttpCookieMode.AutoDetect,
                                               SessionStateMode.InProc,
                                               false);
            httpContext.Items["AspSession"] =
                typeof(HttpSessionState)
                .GetConstructor(
                                    BindingFlags.NonPublic | BindingFlags.Instance,
                                    null,
                                    CallingConventions.Standard,
                                    new[] { typeof(HttpSessionStateContainer) },
                                    null)
                .Invoke(new object[] { sessionContainer });

            HttpContext.Current = httpContext;
        }

        public void TestSetupCoordinatesInvalid()
        {
            var httpRequest = new HttpRequest("", "http://localhost/", "coordinates=M,M,M,M,M,M,M");

            var httpResponce = new HttpResponse(new StringWriter());

            var httpContext = new HttpContext(httpRequest, httpResponce);
            var sessionContainer =
                new HttpSessionStateContainer("id",
                                               new SessionStateItemCollection(),
                                               new HttpStaticObjectsCollection(),
                                               10,
                                               true,
                                               HttpCookieMode.AutoDetect,
                                               SessionStateMode.InProc,
                                               false);
            httpContext.Items["AspSession"] =
                typeof(HttpSessionState)
                .GetConstructor(
                                    BindingFlags.NonPublic | BindingFlags.Instance,
                                    null,
                                    CallingConventions.Standard,
                                    new[] { typeof(HttpSessionStateContainer) },
                                    null)
                .Invoke(new object[] { sessionContainer });

            HttpContext.Current = httpContext;
        }

        [TestMethod]
        public void TestResetPosition()
        {
            Object expectedReturnJSON = new
            {
                message = "Posição da sonda reiniciada com sucesso!"
            };

            string expectedReturn = (new JavaScriptSerializer()).Serialize(expectedReturnJSON);

            Sonda sonda = new Sonda();

            sonda.ResetPosition();

            string returnTest = HttpContext.Current.Response.Output.ToString();

            Assert.AreEqual(expectedReturn, returnTest);
        }

        [TestMethod]
        public void TestResetPositionResponseNotNull()
        {
            Sonda sonda = new Sonda();

            sonda.ResetPosition();

            string returnTest = HttpContext.Current.Response.Output.ToString();

            Assert.IsNotNull(returnTest);
        }

        [TestMethod]
        public void TestSetPosition()
        {
            List<Object> movementsPerformed = new List<Object>();

            movementsPerformed.Add("girou para a esquerda");
            movementsPerformed.Add("se moveu 3 vez(es) no eixo X");
            movementsPerformed.Add("girou para a direita");
            movementsPerformed.Add("se moveu 2 vez(es) no eixo Y");

            List<Object> position = new List<Object>
                {
                    new
                    {
                        x = "3"
                    },

                    new
                    {
                        y = "2"
                    },

                    new
                    {
                        face = "D"
                    },

                    new
                    {
                        movementsPerformed = movementsPerformed
                    }
                };

            Object expectedReturnJSON = new
            {
                position
            };

            string expectedReturn = (new JavaScriptSerializer()).Serialize(expectedReturnJSON);

            Sonda sonda = new Sonda();

            sonda.SetPosition();

            string returnTest = HttpContext.Current.Response.Output.ToString();

            Assert.AreEqual(expectedReturn, returnTest);
        }

        [TestMethod]
        public void TestSetPositionInvalid()
        {
            TestSetupCoordinatesInvalid();

            Object expectedReturnJSON = new
            {
                error = "Um movimento inválido foi detectado, infelizmente a sonda ainda não possui a habilidade de voar."
            };

            string expectedReturn = (new JavaScriptSerializer()).Serialize(expectedReturnJSON);

            Sonda sonda = new Sonda();

            sonda.SetPosition();

            string returnTest = HttpContext.Current.Response.Output.ToString();

            Assert.AreEqual(expectedReturn, returnTest);
        }

        [TestMethod]
        public void TestSetPositionResponseNotNull()
        {
            Sonda sonda = new Sonda();

            sonda.SetPosition();

            string returnTest = HttpContext.Current.Response.Output.ToString();

            Assert.IsNotNull(returnTest);
        }

        [TestMethod]
        public void TestGetPosition()
        {
            List<Object> position = new List<Object>
                {
                    new
                    {
                        x = "0"
                    },

                    new
                    {
                        y = "0"
                    },

                    new
                    {
                        face = "D"
                    }
                };

            Object expectedReturnJSON = new
            {
                position
            };

            string expectedReturn = (new JavaScriptSerializer()).Serialize(expectedReturnJSON);

            Sonda sonda = new Sonda();

            sonda.GetPosition();

            string returnTest = HttpContext.Current.Response.Output.ToString();

            Assert.AreEqual(expectedReturn, returnTest);
        }

        [TestMethod]
        public void TestGetPositionResponseNotNull()
        {
            Sonda sonda = new Sonda();

            sonda.GetPosition();

            string returnTest = HttpContext.Current.Response.Output.ToString();

            Assert.IsNotNull(returnTest);
        }
    }
}
