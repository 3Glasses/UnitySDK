using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using ThreeGlasses;

public class SoundVibrationsDriveWand : MonoBehaviour
{
    public int Window = 2048;

    [Range(0, 100.0f)]
    public float Frequency = 0;
    public float Scale = 1.0f;
    public int Channel = 0;

    public float Strength = 0;

    private readonly object _dataLock = new object();
    private readonly List<float> _data = new List<float>();

    private float[] real;
    private float[] imag;
    private int _w;

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
        ThreeGlassesDllInterface.FFT(real, imag, (uint)real.Length);

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


    public void Start()
    {
        _w = Window;
    }

    public void Update()
    {
        lock (_dataLock)
        {
            if (_data.Count < _w)
            {
                return;
            }

            if (Scale < 0)
            {
                return;
            }

            var index = (int)((Frequency / 100.0f) * (_w - 1));

            real = new float[_w];
            imag = new float[_w];
            Array.Copy(_data.ToArray(), real, _w);
            ThreeGlassesDllInterface.FFT(real, imag, (uint)real.Length);
            Strength = (real[index] * real[index]
                            + imag[index] * imag[index]) * Scale;
        }

        var v = (ushort)(Mathf.Clamp(Strength, 0.0f, 100.0f));
        foreach (var wand in ThreeGlassesManager.joyPad)
        {
            wand.SetMotor(v);
        }
    }

    public void OnAudioFilterRead(float[] data, int channels)
    {
        if (channels <= 0) return;

        var sliceLength = data.Length / channels;
        if (sliceLength < 1)
        {
            return;
        }

        var dataSlice = new float[sliceLength];
        Array.Copy(data, sliceLength * Channel, dataSlice, 0, sliceLength);

        lock (_dataLock)
        {
            _data.AddRange(dataSlice);
            if (_data.Count <= _w) return;
            var removeNum = _data.Count - _w;
            _data.RemoveRange(0, removeNum);
        }
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
