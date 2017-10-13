//using Accord.Audio;
//using Accord.Audio.Windows;
//using Accord.DirectSound;
//using NAudio.Dsp;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using NAudio.CoreAudioApi;
//using System.Threading;
//using NAudio.Dsp;
//using NAudio.Wave;

//namespace Tests
//{
//    class Program
//    {
//        // Other inputs are also usable. Just look through the NAudio library.
//        static  private IWaveIn waveIn;
//        static private  int fftLength = 16; // NAudio fft wants powers of two!

//        // There might be a sample aggregator in NAudio somewhere but I made a variation for my needs
//        static private SampleAggregator sampleAggregator = new SampleAggregator(fftLength);

//        static void Main()
//        {
//            sampleAggregator.FftCalculated += new EventHandler<FftEventArgs>(FftCalculated);
//            sampleAggregator.PerformFFT = true;

//            // Here you decide what you want to use as the waveIn.
//            // There are many options in NAudio and you can use other streams/files.
//            // Note that the code varies for each different source.
//            waveIn = new WasapiLoopbackCapture();

//            waveIn.DataAvailable += OnDataAvailable;

//            waveIn.StartRecording();
//        }

//       static void OnDataAvailable(object sender, WaveInEventArgs e)
//        {

//                byte[] buffer = e.Buffer;
//                int bytesRecorded = e.BytesRecorded;
//                int bufferIncrement = waveIn.WaveFormat.BlockAlign;

//                for (int index = 0; index < bytesRecorded; index += bufferIncrement)
//                {
//                    float sample32 = BitConverter.ToSingle(buffer, index);
//                    sampleAggregator.Add(sample32);
//                }
            
//        }

//        static void FftCalculated(object sender, FftEventArgs e)
//        {
//            var r = e.Result;

//            Console.WriteLine("--");
//             Console.WriteLine(String.Concat(r.Select(x=> $"{x.X:00.00}, {x.Y:00.00} | ")));
//        }








//    }
//     // The Complex and FFT are here!

//    public class SampleAggregator
//    {
//        // FFT
//        public event EventHandler<FftEventArgs> FftCalculated;
//        public bool PerformFFT { get; set; }

//        // This Complex is NAudio's own! 
//        private Complex[] fftBuffer;
//        private FftEventArgs fftArgs;
//        private int fftPos;
//        private int fftLength;
//        private int m;

//        public SampleAggregator(int fftLength)
//        {
//            if (!IsPowerOfTwo(fftLength))
//            {
//                throw new ArgumentException("FFT Length must be a power of two");
//            }
//            this.m = (int)Math.Log(fftLength, 2.0);
//            this.fftLength = fftLength;
//            this.fftBuffer = new Complex[fftLength];
//            this.fftArgs = new FftEventArgs(fftBuffer);
//        }

//        bool IsPowerOfTwo(int x)
//        {
//            return (x & (x - 1)) == 0;
//        }

//        public void Add(float value)
//        {
//            if (PerformFFT && FftCalculated != null)
//            {
//                // Remember the window function! There are many others as well.
//                fftBuffer[fftPos].X = (float)(value * FastFourierTransform.HammingWindow(fftPos, fftLength));
//                fftBuffer[fftPos].Y = 0; // This is always zero with audio.
//                fftPos++;
//                if (fftPos >= fftLength)
//                {
//                    fftPos = 0;
//                    FastFourierTransform.FFT(true, m, fftBuffer);
//                    FftCalculated(this, fftArgs);
//                }
//            }
//        }
//    }

//    public class FftEventArgs : EventArgs
//    {
 
//        public FftEventArgs(Complex[] result)
//        {
//            this.Result = result;
//        }
//        public Complex[] Result { get; private set; }
//    }

//}
