using System;
using System.Reactive.Linq;
using OpenCV.Net;
using NationalInstruments.DAQmx;
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

        Task CreateTask()
        {
            var task = new Task();
            foreach (var channel in channels)
            {
                task.AIChannels.CreateVoltageChannel(channel.PhysicalChannel, channel.ChannelName, channel.TerminalConfiguration, channel.MinimumValue, channel.MaximumValue, channel.VoltageUnits);
            }

            return task;
        }

        public override IObservable<Mat> Generate()
        {
            return Observable.Create<Mat>(observer =>
            {
                var task = CreateTask();
                task.Timing.ConfigureSampleClock(SignalSource, SampleRate, ActiveEdge, SampleMode, BufferSize);
                task.Control(TaskAction.Verify);
                var analogInReader = new AnalogMultiChannelReader(task.Stream);
                var samplesPerChannel = SamplesPerRead < 0 ? BufferSize : SamplesPerRead;
                AsyncCallback analogCallback = null;
                analogCallback = new AsyncCallback(result =>
                {
                    var data = analogInReader.EndReadMultiSample(result);
                    var output = Mat.FromArray(data);
                    observer.OnNext(output);
                    analogInReader.BeginReadMultiSample(samplesPerChannel, analogCallback, null);
                });

                analogInReader.SynchronizeCallbacks = true;
                analogInReader.BeginReadMultiSample(samplesPerChannel, analogCallback, null);
                return Disposable.Create(() =>
                {
                    task.Stop();
                    task.Dispose();
                });
            });
        }

        public IObservable<Mat> Generate<TSource>(IObservable<TSource> source)
        {
            return Observable.Defer(() =>
            {
                var task = CreateTask();
                var sampleRate = SampleRate;
                if (sampleRate > 0)
                {
                    task.Timing.ConfigureSampleClock(SignalSource, sampleRate, ActiveEdge, SampleMode, BufferSize);
                }
                task.Control(TaskAction.Verify);
                var analogInReader = new AnalogMultiChannelReader(task.Stream);
                var samplesPerChannel = SamplesPerRead < 0 ? BufferSize : SamplesPerRead;
                return Observable.Using(() => Disposable.Create(
                    () =>
                    {
                        task.Stop();
                        task.Dispose();
                    }),
                    resource => source.Select(_ =>
                    {
                        var data = analogInReader.ReadMultiSample(samplesPerChannel);
                        return Mat.FromArray(data);
                    }));
            });
        }
    }
}
