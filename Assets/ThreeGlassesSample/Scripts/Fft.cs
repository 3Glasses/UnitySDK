/* 
 * Free FFT and convolution (C#)
 * 
 * Copyright (c) 2017 Project Nayuki
 * https://www.nayuki.io/page/free-small-fft-in-multiple-languages
 * 
 * (MIT License)
 * Permission is hereby granted, free of charge, to any person obtaining a copy of
 * this software and associated documentation files (the "Software"), to deal in
 * the Software without restriction, including without limitation the rights to
 * use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
 * the Software, and to permit persons to whom the Software is furnished to do so,
 * subject to the following conditions:
 * - The above copyright notice and this permission notice shall be included in
 *   all copies or substantial portions of the Software.
 * - The Software is provided "as is", without warranty of any kind, express or
 *   implied, including but not limited to the warranties of merchantability,
 *   fitness for a particular purpose and noninfringement. In no event shall the
 *   authors or copyright holders be liable for any claim, damages or other
 *   liability, whether in an action of contract, tort or otherwise, arising from,
 *   out of or in connection with the Software or the use or other dealings in the
 *   Software.
 */

using System;
using UnityEngine;

// ReSharper disable MemberCanBePrivate.Global


public sealed class Fft
{
    public static void Transform(float[] real, float[] imag)
    {
        if (real.Length != imag.Length)
        {
            throw new ArgumentException("Input array mismatched lengths");
        }

        var n = real.Length;
        if (n == 0)
        {
            return;
        }

        if ((n & (n - 1)) == 0)
        {
            TransformRadix2(real, imag);
        }
        else // More complicated algorithm for arbitrary sizes
        {
            TransformBluestein(real, imag);
        }
    }


    /* 
	 * Computes the inverse discrete Fourier transform (IDFT) of the given complex vector, storing the result back into the vector.
	 * The vector can have any length. This is a wrapper function. This transform does not perform scaling, so the inverse is not a true inverse.
	 */
    public static void InverseTransform(float[] real, float[] imag)
    {
        Transform(imag, real);
    }


    /* 
	 * Computes the discrete Fourier transform (DFT) of the given complex vector, storing the result back into the vector.
	 * The vector's length must be a power of 2. Uses the Cooley-Tukey decimation-in-time radix-2 algorithm.
	 */
    private static void TransformRadix2(float[] real, float[] imag)
    {
        // Initialization
        var n = real.Length;
        var levels = 31 - NumberOfLeadingZeros(n);  // Equal to floor(log2(n))
        if (1 << levels != n)
        {
            throw new ArgumentException("Length is not a power of 2");
        }

        var cosTable = new float[n / 2];
        var sinTable = new float[n / 2];
        for (var i = 0; i < n / 2; i++)
        {
            cosTable[i] = Mathf.Cos(2 * Mathf.PI * i / n);
            sinTable[i] = Mathf.Sin(2 * Mathf.PI * i / n);
        }

        // Bit-reversed addressing permutation
        for (var i = 0; i < n; i++)
        {
            var j = (int)((uint)ReverseBits(i) >> (32 - levels));
            if (j <= i) continue;

            var temp = real[i];
            real[i] = real[j];
            real[j] = temp;
            temp = imag[i];
            imag[i] = imag[j];
            imag[j] = temp;
        }

        // Cooley-Tukey decimation-in-time radix-2 FFT
        for (var size = 2; size <= n; size *= 2)
        {
            var halfsize = size / 2;
            var tablestep = n / size;
            for (var i = 0; i < n; i += size)
            {
                for (int j = i, k = 0; j < i + halfsize; j++, k += tablestep)
                {
                    var tpre = real[j + halfsize] * cosTable[k] + imag[j + halfsize] * sinTable[k];
                    var tpim = -real[j + halfsize] * sinTable[k] + imag[j + halfsize] * cosTable[k];
                    real[j + halfsize] = real[j] - tpre;
                    imag[j + halfsize] = imag[j] - tpim;
                    real[j] += tpre;
                    imag[j] += tpim;
                }
            }

            if (size == n)
            {
                break;
            }
        }
    }


    /* 
	 * Computes the discrete Fourier transform (DFT) of the given complex vector, storing the result back into the vector.
	 * The vector can have any length. This requires the convolution function, which in turn requires the radix-2 FFT function.
	 * Uses Bluestein's chirp z-transform algorithm.
	 */
    private static void TransformBluestein(float[] real, float[] imag)
    {
        // Find a power-of-2 convolution length m such that m >= n * 2 + 1

        var n = real.Length;
        if (n >= 0x20000000)
        {
            throw new ArgumentException("Array too large");
        }

        var m = HighestOneBit(n * 2 + 1) << 1;

        // Trignometric tables
        var cosTable = new float[n];
        var sinTable = new float[n];
        for (var i = 0; i < n; i++)
        {
            var j = (int)((long)i * i % (n * 2));  // This is more accurate than j = i * i
            cosTable[i] = Mathf.Cos(Mathf.PI * j / n);
            sinTable[i] = Mathf.Sin(Mathf.PI * j / n);
        }

        // Temporary vectors and preprocessing
        var areal = new float[m];
        var aimag = new float[m];
        for (var i = 0; i < n; i++)
        {
            areal[i] = real[i] * cosTable[i] + imag[i] * sinTable[i];
            aimag[i] = -real[i] * sinTable[i] + imag[i] * cosTable[i];
        }
        var breal = new float[m];
        var bimag = new float[m];
        breal[0] = cosTable[0];
        bimag[0] = sinTable[0];
        for (var i = 1; i < n; i++)
        {
            breal[i] = breal[m - i] = cosTable[i];
            bimag[i] = bimag[m - i] = sinTable[i];
        }

        // Convolution
        var creal = new float[m];
        var cimag = new float[m];
        Convolve(areal, aimag, breal, bimag, creal, cimag);

        // Postprocessing
        for (var i = 0; i < n; i++)
        {
            real[i] = creal[i] * cosTable[i] + cimag[i] * sinTable[i];
            imag[i] = -creal[i] * sinTable[i] + cimag[i] * cosTable[i];
        }
    }


    /* 
	 * Computes the circular convolution of the given complex vectors. Each vector's length must be the same.
	 */
    private static void Convolve(float[] xreal, float[] ximag, float[] yreal, float[] yimag, float[] outreal, float[] outimag)
    {
        var n = xreal.Length;
        xreal = (float[])xreal.Clone();
        ximag = (float[])ximag.Clone();
        yreal = (float[])yreal.Clone();
        yimag = (float[])yimag.Clone();

        Transform(xreal, ximag);
        Transform(yreal, yimag);
        for (var i = 0; i < n; i++)
        {
            var temp = xreal[i] * yreal[i] - ximag[i] * yimag[i];
            ximag[i] = ximag[i] * yreal[i] + xreal[i] * yimag[i];
            xreal[i] = temp;
        }
        InverseTransform(xreal, ximag);
        for (var i = 0; i < n; i++)
        {  // Scaling (because this FFT implementation omits it)
            outreal[i] = xreal[i] / n;
            outimag[i] = ximag[i] / n;
        }
    }


    private static int NumberOfLeadingZeros(int val)
    {
        if (val == 0)
        {
            return 32;
        }
        var result = 0;
        for (; val >= 0; val <<= 1)
        {
            result++;
        }
        return result;
    }


    private static int HighestOneBit(int val)
    {
        for (var i = 1 << 31; i != 0; i = (int)((uint)i >> 1))
        {
            if ((val & i) != 0)
            {
                return i;
            }
        }
        return 0;
    }


    private static int ReverseBits(int val)
    {
        var result = 0;
        for (var i = 0; i < 32; i++, val >>= 1)
        {
            result = (result << 1) | (val & 1);
        }
        return result;
    }

}