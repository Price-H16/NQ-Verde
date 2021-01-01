﻿// WingsEmu
//
// Developed by NosWings Team

using System;
using System.Security.Cryptography;

namespace ChickenAPI.Core.Maths
{
    public class RandomGenerator : IRandomGenerator
    {
        #region Instantiation

        public RandomGenerator() => RngProvider = new RNGCryptoServiceProvider();

        #endregion

        #region Properties

        private RNGCryptoServiceProvider RngProvider { get; }

        #endregion

        #region Methods

        public int Next(int min, int max) => (int)Math.Floor(min + ((double)max - min) * Next());

        public int Next(int max) => Next(0, max);

        public double Next()
        {
            byte[] bytes = new byte[sizeof(uint)];
            GetBytes(bytes);

            uint rnd = BitConverter.ToUInt32(bytes, 0);

            return rnd / (uint.MaxValue + 1.0);
        }

        private void GetBytes(byte[] data)
        {
            RngProvider.GetBytes(data);
        }

        #endregion
    }
}