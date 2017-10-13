using Infodev.Common;
using Seq = Ext.Seq;
using NAudio.CoreAudioApi;
using System.Collections.Generic;
using System.Collections;

namespace Autorawr
{
    class PeakSamples : IEnumerable<float>
    {
        CapacityQueue<float> samples;
        
        MMDevice AudioDevice = new MMDeviceEnumerator().GetDefaultAudioEndpoint(DataFlow.Render, Role.Console);
        float CurrentPeak => AudioDevice.AudioMeterInformation.MasterPeakValue;



        public PeakSamples(int capacity = 5)
        {
            samples = new CapacityQueue<float>(capacity);
            Seq.N(capacity).Each(_=>samples.Enqueue(0));

            var zz = new MMDeviceEnumerator().EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);
        }
        public void Enqueue()
        {
            //Console.WriteLine($"{CurrentPeak * 100f:00.00} {"".PadLeft((int)(CurrentPeak * 100f),'-')}");
            //Console.WriteLine($"{samples.Average() * 100f:00.00} {"".PadLeft((int)(samples.Average() * 100f), '=')}");
            samples.Enqueue(CurrentPeak);
        }
        public IEnumerator<float> GetEnumerator() => ((IEnumerable<float>)samples).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<float>)samples).GetEnumerator();
    }
}
