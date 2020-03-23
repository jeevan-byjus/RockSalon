using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.TestTools;
using Byjus.RockSalon.Verticals;
using Byjus.RockSalon.Ctrls;
using Byjus.RockSalon.Views;
using Byjus.RockSalon.Util;
using System.IO;


namespace Byjus.RockSalon.Tests {
    public class UtilTest : BaseTestSuite {

        [SetUp]
        public void Setup() {
            BaseInit();
            CreateMainCamera();
        }

        [TearDown]
        public void Teardown() {
            BaseTearDown();
        }

        [Test]
        public void SamePositionTests() {
            Assert.AreEqual(true, GenUtil.EqualPositionSw(new Vector2(-2.99f, 1.6f), new Vector2(-2.98f, 1.6f)));
        }
    }
}