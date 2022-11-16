using System;
using System.Reactive.Linq;
using OpenCV.Net;
using NationalInstruments.DAQmx;
using System.Runtime.InteropServices;
using System.Reactive.Disposables;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Bonsai.DAQmx
{
    [Description("Writes a sequence of sample buffers to one or more DAQmx analog output channels.")]
    public class AnalogOutput : Sink<Mat>
    {
        readonly Collection<AnalogOutputChannelConfiguration> channels = new Collection<AnalogOutputChannelConfiguration>();

        public AnalogOutput()
        {
            BufferSize = 1000;
            SignalSource = string.Empty;
            ActiveEdge = SampleClockActiveEdge.Rising;
            SampleMode = SampleQuantityMode.ContinuousSamples;
        }

        [Description("The optional source terminal of the clock. If not specified, the internal clock of the device will be used.")]
        public string SignalSource { get; set; }

        [Description("The sampling rate, in samples per second.")]
        public double SampleRate { get; set; }

        [Description("The edges of sample clock pulses on which to generate samples.")]
        public SampleClockActiveEdge ActiveEdge { get; set; }

        [Description("Specifies whether signal generation is finite, or continuous.")]
        public SampleQuantityMode SampleMode { get; set; }

        [Description("The number of samples to generate, for finite samples, or the size of the buffer for continuous sampling.")]
        public int BufferSize { get; set; }

        [Description("The collection of analog output channels used to generate voltage.")]
        public Collection<AnalogOutputChannelConfiguration> Channels
        {
            get { return channels; }
        }

        public IObservable<double> Process(IObservable<double> source)
        {
            return Observable.Defer(() =>
            {
                var task = new Task();
                foreach (var channel in channels)
                {
                    task.AOChannels.CreateVoltageChannel(channel.PhysicalChannel, channel.ChannelName, channel.MinimumValue, channel.MaximumValue, channel.VoltageUnits);
                }

                task.Control(TaskAction.Verify);
                var analogOutWriter = new AnalogMultiChannelWriter(task.Stream);
                return Observable.Using(
                    () => Disposable.Create(() =>
                    {
                        task.Stop();
                        task.Dispose();
                    }),
                    resource => source.Do(input =>
                    {
                        analogOutWriter.WriteSingleSample(true, new[] { input });
                    }));
            });
        }

        public override IObservable<Mat> Process(IObservable<Mat> source)
        {
            return Observable.Defer(() =>
            {
                var task = new Task();
                foreach (var channel in channels)
                {
                    task.AOChannels.CreateVoltageChannel(channel.PhysicalChannel, channel.ChannelName, channel.MinimumValue, channel.MaximumValue, channel.VoltageUnits);
                }

                task.Control(TaskAction.Verify);
                task.Timing.ConfigureSampleClock(SignalSource, SampleRate, ActiveEdge, SampleMode, BufferSize);
                var analogOutWriter = new AnalogMultiChannelWriter(task.Stream);
                return Observable.Using(
                    () => Disposable.Create(() =>
                    {
                        if (task.Timing.SampleQuantityMode == SampleQuantityMode.FiniteSamples)
                        {
                            task.WaitUntilDone();
                        }
                        task.Stop();
                        task.Dispose();
                    }),
                    resource => source.Do(input =>
                    {
                        var data = new double[input.Rows, input.Cols];
                        var dataHandle = GCHandle.Alloc(data, GCHandleType.Pinned);
                        try
                        {
                            var dataHeader = new Mat(input.Rows, input.Cols, Depth.F64, 1, dataHandle.AddrOfPinnedObject());
                            if (input.Depth != dataHeader.Depth) CV.Convert(input, dataHeader);
                            else CV.Copy(input, dataHeader);
                            analogOutWriter.WriteMultiSample(true, data);
                        }
                        finally { dataHandle.Free(); }
                    }));
            });
        }
    }
}
