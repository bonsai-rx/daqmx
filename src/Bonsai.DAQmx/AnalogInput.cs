using System;
using System.Reactive.Linq;
using OpenCV.Net;
using NationalInstruments.DAQmx;
using System.Reactive.Disposables;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Bonsai.DAQmx
{
    /// <summary>
    /// Represents an operator that generates a sequence of voltage measurements
    /// from one or more DAQmx analog input channels.
    /// </summary>
    [DefaultProperty(nameof(Channels))]
    [Description("Generates a sequence of voltage measurements from one or more DAQmx analog input channels.")]
    public class AnalogInput : Source<Mat>
    {
        readonly Collection<AnalogInputChannelConfiguration> channels = new Collection<AnalogInputChannelConfiguration>();


        /// <summary>
        /// Gets or sets the optional trigger terminal of the clock. If not specified,
        /// the internal trigger of the device will be used.
        /// </summary>
        [Description("The optional trigger terminal of the clock. If not specified, the internal trigger of the device will be used.")]
        public string SignalTrigger { get; set; } = string.Empty;


        /// <summary>
        /// Gets or sets the optional source terminal of the clock. If not specified,
        /// the internal clock of the device will be used.
        /// </summary>
        [Description("The optional source terminal of the clock. If not specified, the internal clock of the device will be used.")]
        public string SignalSource { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the sampling rate for acquiring voltage measurements, in
        /// samples per second.
        /// </summary>
        [Description("The sampling rate for acquiring voltage measurements, in samples per second.")]
        public double SampleRate { get; set; }

        /// <summary>
        /// Gets or sets a value specifying on which edge of a clock pulse sampling takes place.
        /// </summary>
        [Description("Specifies on which edge of a clock pulse sampling takes place.")]
        public SampleClockActiveEdge ActiveEdge { get; set; } = SampleClockActiveEdge.Rising;

        /// <summary>
        /// Gets or sets a value specifying whether the acquisition task will acquire
        /// a finite number of samples or if it continuously acquires samples.
        /// </summary>
        [Description("Specifies whether the acquisition task will acquire a finite number of samples or if it continuously acquires samples.")]
        public SampleQuantityMode SampleMode { get; set; } = SampleQuantityMode.ContinuousSamples;

        /// <summary>
        /// Gets or sets the number of samples to acquire, for finite samples, or the
        /// size of the buffer for continuous sampling.
        /// </summary>
        [Description("The number of samples to acquire, for finite samples, or the size of the buffer for continuous sampling.")]
        public int BufferSize { get; set; } = 1000;

        /// <summary>
        /// Gets or sets the number of samples per channel in each output buffer.
        /// If not specified, the number of samples will be set to the size of the buffer.
        /// </summary>
        [Description("The number of samples in each output buffer. If not specified, the number of samples will be set to the size of the buffer.")]
        public int? SamplesPerChannel { get; set; }

        /// <summary>
        /// Gets the collection of analog input channels from which to acquire voltage samples.
        /// </summary>
        [Editor("Bonsai.Design.DescriptiveCollectionEditor, Bonsai.Design", DesignTypes.UITypeEditor)]
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
                task.AIChannels.CreateVoltageChannel(
                    channel.PhysicalChannel,
                    channel.ChannelName,
                    channel.TerminalConfiguration,
                    channel.MinimumValue,
                    channel.MaximumValue,
                    channel.VoltageUnits);
            }

            return task;
        }

        /// <summary>
        /// Generates an observable sequence of voltage measurements from one or
        /// more DAQmx analog input channels.
        /// </summary>
        /// <returns>
        /// A sequence of 2D <see cref="Mat"/> objects storing the voltage samples.
        /// Each row corresponds to a channel in the acquisition task, and each column
        /// to a sample from each of the channels. The order of the channels follows
        /// the order in which you specify the channels in the <see cref="Channels"/>
        /// property.
        /// </returns>
        public override IObservable<Mat> Generate()
        {
            return Observable.Create<Mat>(observer =>
            {
                var task = CreateTask();
                var bufferSize = BufferSize;
                task.Timing.ConfigureSampleClock(SignalSource, SampleRate, ActiveEdge, SampleMode, bufferSize);

                if (!SignalTrigger.Equals(string.Empty)) { 
                    task.Triggers.StartTrigger.ConfigureDigitalEdgeTrigger(SignalTrigger, DigitalEdgeStartTriggerEdge.Rising);
                }

                task.Control(TaskAction.Verify);
                task.Start();
                
                var analogInReader = new AnalogMultiChannelReader(task.Stream);
                var samplesPerChannel = SamplesPerChannel.GetValueOrDefault(bufferSize);

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

        /// <summary>
        /// Generates an observable sequence of voltage measurements from one or
        /// more DAQmx analog input channels, where each new buffer is emitted only
        /// when an observable sequence emits a notification.
        /// </summary>
        /// <typeparam name="TSource">
        /// The type of the elements in the <paramref name="source"/> sequence.
        /// </typeparam>
        /// <param name="source">
        /// The sequence containing the notifications used for emitting sample buffers.
        /// </param>
        /// <returns>
        /// A sequence of 2D <see cref="Mat"/> objects storing the voltage samples.
        /// Each row corresponds to a channel in the acquisition task, and each column
        /// to a sample from each of the channels. The order of the channels follows
        /// the order in which you specify the channels in the <see cref="Channels"/>
        /// property.
        /// </returns>
        public IObservable<Mat> Generate<TSource>(IObservable<TSource> source)
        {
            return Observable.Defer(() =>
            {
                var task = CreateTask();
                var bufferSize = BufferSize;
                var sampleRate = SampleRate;
                if (sampleRate > 0)
                {
                    task.Timing.ConfigureSampleClock(SignalSource, sampleRate, ActiveEdge, SampleMode, bufferSize);
                }

                if (!SignalTrigger.Equals(string.Empty))
                {
                    task.Triggers.StartTrigger.ConfigureDigitalEdgeTrigger(SignalTrigger, DigitalEdgeStartTriggerEdge.Rising);
                }

                task.Control(TaskAction.Verify);
                var analogInReader = new AnalogMultiChannelReader(task.Stream);
                var samplesPerChannel = SamplesPerChannel.GetValueOrDefault(bufferSize);
                task.Start();
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
