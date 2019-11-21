using Bonsai;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using OpenCV.Net;
using NationalInstruments.DAQmx;
using System.Runtime.InteropServices;
using System.Reactive.Disposables;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Bonsai.DAQmx
{
    [Description("Generates a sequence of voltage measurements from one or more DAQmx analog input channels.")]
    public class AnalogInput : Source<Mat>
    {
        readonly Collection<AnalogInputChannelConfiguration> channels = new Collection<AnalogInputChannelConfiguration>();

        public AnalogInput()
        {
            BufferSize = 1000;
            SamplesPerRead = -1;
            SignalSource = string.Empty;
            ActiveEdge = SampleClockActiveEdge.Rising;
            SampleMode = SampleQuantityMode.ContinuousSamples;
        }

        [Description("The optional source terminal of the clock. If not specified, the internal clock of the device will be used.")]
        public string SignalSource { get; set; }

        [Description("The sampling rate, in samples per second.")]
        public double SampleRate { get; set; }

        [Description("The edges of sample clock pulses on which to acquire samples.")]
        public SampleClockActiveEdge ActiveEdge { get; set; }

        [Description("Specifies whether acquisition is finite, or continuous.")]
        public SampleQuantityMode SampleMode { get; set; }

        [Description("The number of samples to acquire, for finite samples, or the size of the buffer for continuous sampling.")]
        public int BufferSize { get; set; }

        [Description("The size of each read buffer, in samples.")]
        public int SamplesPerRead { get; set; }

        [Description("The collection of analog input channels from which to acquire voltage samples.")]
        public Collection<AnalogInputChannelConfiguration> Channels
        {
            get { return channels; }
        }

        public override IObservable<Mat> Generate()
        {
            return Observable.Create<Mat>(observer =>
            {
                var task = new Task();
                foreach (var channel in channels)
                {
                    task.AIChannels.CreateVoltageChannel(channel.PhysicalChannel, channel.ChannelName, channel.TerminalConfiguration, channel.MinimumValue, channel.MaximumValue, channel.VoltageUnits);
                }

                task.Timing.ConfigureSampleClock(SignalSource, SampleRate, ActiveEdge, SampleMode, BufferSize);
                task.Control(TaskAction.Verify);
                var analogInReader = new AnalogMultiChannelReader(task.Stream);
                AsyncCallback analogCallback = null;
                analogCallback = new AsyncCallback(result =>
                {
                    var data = analogInReader.EndReadMultiSample(result);
                    var dataHandle = GCHandle.Alloc(data, GCHandleType.Pinned);
                    try
                    {
                        var output = new Mat(data.GetLength(0), data.GetLength(1), Depth.F64, 1, dataHandle.AddrOfPinnedObject());
                        observer.OnNext(output.Clone());
                        analogInReader.BeginReadMultiSample(SamplesPerRead < 0 ? BufferSize : SamplesPerRead, analogCallback, null);
                    }
                    finally { dataHandle.Free(); }
                });

                analogInReader.SynchronizeCallbacks = true;
                analogInReader.BeginReadMultiSample(SamplesPerRead < 0 ? BufferSize : SamplesPerRead, analogCallback, null);
                return Disposable.Create(() =>
                {
                    task.Stop();
                    task.Dispose();
                });
            });
        }
    }
}
