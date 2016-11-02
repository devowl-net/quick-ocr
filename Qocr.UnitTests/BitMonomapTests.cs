﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Qocr.Core.Data;
using Qocr.Core.Interfaces;
using Qocr.Core.Recognition;

namespace Qocr.UnitTests
{
    [TestClass]
    public class BitMonomapTests
    {
        private readonly bool[,] _bitImage = new bool[3, 2]
        {
            // { false, true, false },
            // { true, false, true }

            { false, true },
            { true, false },
            { false, true }
        };

        [TestMethod]
        public void EqualMatrix()
        {
            IMonomap monomap = new BitMonomap(_bitImage);

            Assert.AreEqual(monomap[0, 0], false);
            Assert.AreEqual(monomap[1, 0], true);
            Assert.AreEqual(monomap[2, 0], false);

            Assert.AreEqual(monomap[0, 1], true);
            Assert.AreEqual(monomap[1, 1], false);
            Assert.AreEqual(monomap[2, 1], true);
        }

        [TestMethod]
        public void MatrixSize()
        {
            IMonomap monomap = new BitMonomap(_bitImage);
            Assert.AreEqual(monomap.Width, 3);
            Assert.AreEqual(monomap.Height, 2);
        }

        [TestMethod]
        public void MalevichEuler()
        {
            IMonomap monomap = new BitMonomap(_bitImage);
            var euler = EulerCharacteristicComputer.Compute2D(monomap);
            Assert.AreEqual(euler.S0, 0);
            Assert.AreEqual(euler.S1, 0);
            Assert.AreEqual(euler.S2, 0);
            Assert.AreEqual(euler.S3, 0);

            Assert.AreEqual(euler.S4, 0);
            Assert.AreEqual(euler.S5, 0);
            Assert.AreEqual(euler.S6, 0);
            Assert.AreEqual(euler.S7, 0);

            Assert.AreEqual(euler.S8, 1);
            Assert.AreEqual(euler.S9, 1);

            Assert.AreEqual(euler.S10, 0);
            Assert.AreEqual(euler.S11, 0);
            Assert.AreEqual(euler.S12, 0);
            Assert.AreEqual(euler.S13, 0);

            Assert.AreEqual(euler.S14, 0);
        }
    }
}
