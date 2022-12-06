using System;
using System.Reactive.Linq;
using NationalInstruments.DAQmx;
using System.Reactive.Disposables;
using System.Collections.ObjectModel;
using System.ComponentModel;
using OpenCV.Net;
using System.Runtime.InteropServices;

namespace Bonsai.DAQmx
{
    [Description("Writes a sequence of logical values to one or more DAQmx digital output channels.")]
    public class DigitalOutput : Sink<Mat>
    {
        readonly Collection<DigitalOutputChannelConfiguration> channels = new Collection<DigitalOutputChannelConfiguration>();

        public DigitalOutput()
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

        [Description("The collection of digital output channels used to generate digital signals.")]
        public Collection<DigitalOutputChannelConfiguration> Channels
        {
            get { return channels; }
        }

        Task CreateTask()
        {
            var task = new Task();
            foreach (var channel in channels)
            {
                task.DOChannels.CreateChannel(channel.Lines, channel.ChannelName, channel.Grouping);
            }

            task.Control(TaskAction.Verify);
            return task;
        }

        IObservable<TSource> Process<TSource>(IObservable<TSource> source, Action<DigitalMultiChannelWriter, TSource> onNext)
        {
            return Observable.Defer(() =>
            {
                var task = CreateTask();
                var digitalOutWriter = new DigitalMultiChannelWriter(task.Stream);
                return Observable.Using(
                    () => Disposable.Create(() =>
                    {
                        task.WaitUntilDone();
                        task.Stop();
                        task.Dispose();
                    }),
                    resource => source.Do(input =>
                    {
                        try { onNext(digitalOutWriter, input); }
                        catch { task.Stop(); throw; }
                    }));
            });
        }

        public IObservable<bool> Process(IObservable<bool> source)
        {
            return Process(source, (writer, value) =>
            {
                writer.WriteSingleSampleSingleLine(autoStart: true, new[] { value });
            });
        }

        public IObservable<byte> Process(IObservable<byte> source)
        {
            return Process(source, (writer, value) =>
            {
                writer.WriteSingleSamplePort(autoStart: true, new[] { value });
            });
        }

        public override IObservable<Mat> Process(IObservable<Mat> source)
        {
            return Observable.Defer(() =>
            {
                var task = CreateTask();
                task.Timing.ConfigureSampleClock(SignalSource, SampleRate, ActiveEdge, SampleMode, BufferSize);
                var digitalOutWriter = new DigitalMultiChannelWriter(task.Stream);
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
                        var data = new byte[input.Rows, input.Cols];
                        var dataHandle = GCHandle.Alloc(data, GCHandleType.Pinned);
                        try
                        {
                            var dataHeader = new Mat(input.Rows, input.Cols, Depth.U8, 1, dataHandle.AddrOfPinnedObject());
                            if (input.Depth != dataHeader.Depth) CV.Convert(input, dataHeader);
                            else CV.Copy(input, dataHeader);
                            digitalOutWriter.WriteMultiSamplePort(autoStart: true, data);
                        }
                        catch { task.Stop(); throw; }
                        finally { dataHandle.Free(); }
                    }));
            });
        }
    }
}
