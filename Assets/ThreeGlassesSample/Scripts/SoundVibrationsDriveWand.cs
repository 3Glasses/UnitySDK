using System;
using UnityEngine;
using System.Collections.Generic;
using ThreeGlasses;

public class SoundVibrationsDriveWand : MonoBehaviour
{
    [Range(0, 100.0f)]
    public float level = 0;
    public float strength = 0;

#if TEST_FFT
    public void Awake()
    {
        var real = new float[]
        {
            0.672957f, -0.453061f, -0.835088f, 0.980334f,
            0.972232f, 0.640295f, 0.791619f, -0.042803f,
            0.282745f, 0.153629f, 0.939992f, 0.588169f,
            0.189058f, 0.461301f, -0.667901f, -0.314791f
        };
        var imag = new float[16];

        var output = new float[16];

        DFT(real,ref output);
        Fft.Transform(real, imag);

        Debug.Log("DFT = " +
                  string.Join("", new List<float>(output)
                      .ConvertAll(i => i.ToString("##.###") + " ")
                      .ToArray()));

        Debug.Log("FFT real = " +
                  string.Join("", new List<float>(real)
                      .ConvertAll(i => i.ToString("##.###") + " ")
                      .ToArray()));

        Debug.Log("FFT imag = " +
                  string.Join("", new List<float>(imag)
                      .ConvertAll(i => i.ToString("##.###") + " ")
                      .ToArray()));
    }
#endif

    public void Update()
    {
        var v = (ushort)(Mathf.Clamp(strength, 0, 1.0f) * 100);
        foreach (var wand in ThreeGlassesManager.joyPad)
        {
            wand.SetMotor(v);
        }
    }

    public void OnAudioFilterRead(float[] data, int channels)
    {
        if (channels <= 0) return;

        var sliceLength = data.Length / channels;
        if (sliceLength < 4)
        {
            return;
        }

        var real = new float[sliceLength];
        var imag = new float[sliceLength];
        Array.Copy(data, real, sliceLength);

        Fft.Transform(real, imag);
        var index = (int)((level / 100.0f) * (sliceLength - 1));
        strength = (real[index] * real[index] + imag[index] * imag[index]) / sliceLength;
    }

#if TEST_FFT
    private void DFT(float[] input, ref float[] output)
    {
        int size = input.Length;
        float pi2 = 2.0f * Mathf.PI;

        float[] real = new float[size];
        float[] imag = new float[size];

        for (var y = 0; y < size; y++)
        {
            real[y] = 0;
            imag[y] = 0;

            for (var x = 0; x < size; x++)
            {
                var angleTerm = pi2 * y * x / size;
                var cosineA = Mathf.Cos(angleTerm);
                var sineA = Mathf.Sin(angleTerm);
                real[y] += input[x] * cosineA;
                imag[y] -= input[x] * sineA;
            }
            output[y] = real[y] * real[y] + imag[y] * imag[y];
        }
    }
#endif
}
