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
    /// <summary>
    /// Represents an operator that generates voltage signals in one or more DAQmx analog
    /// output channels from a sequence of sample buffers.
    /// </summary>
    [DefaultProperty(nameof(Channels))]
    [Description("Generates voltage signals in one or more DAQmx analog output channels from a sequence of sample buffers.")]
    public class AnalogOutput : Sink<Mat>
    {
        readonly Collection<AnalogOutputChannelConfiguration> channels = new Collection<AnalogOutputChannelConfiguration>();

        /// <summary>
        /// Gets or sets the optional source terminal of the clock. If not specified,
        /// the internal clock of the device will be used.
        /// </summary>
        [Description("The optional source terminal of the clock. If not specified, the internal clock of the device will be used.")]
        public string SignalSource { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the sampling rate for generating voltage signals, in samples
        /// per second.
        /// </summary>
        [Description("The sampling rate for generating voltage signals, in samples per second.")]
        public double SampleRate { get; set; }

        /// <summary>
        /// Gets or sets a value specifying on which edge of a clock pulse sampling takes place.
        /// </summary>
        [Description("Specifies on which edge of a clock pulse sampling takes place.")]
        public SampleClockActiveEdge ActiveEdge { get; set; } = SampleClockActiveEdge.Rising;

        /// <summary>
        /// Gets or sets a value specifying whether the signal generation task will generate
        /// a finite number of samples or if it continuously generates samples.
        /// </summary>
        [Description("Specifies whether the signal generation task will generate a finite number of samples or if it continuously generates samples.")]
        public SampleQuantityMode SampleMode { get; set; } = SampleQuantityMode.ContinuousSamples;

        /// <summary>
        /// Gets or sets the number of samples to generate, for finite samples, or the
        /// size of the buffer for continuous signal generation.
        /// </summary>
        [Description("The number of samples to generate, for finite samples, or the size of the buffer for continuous signal generation.")]
        public int BufferSize { get; set; } = 1000;

        /// <summary>
        /// Gets the collection of analog output channels used to generate voltage signals.
        /// </summary>
        [Editor("Bonsai.Design.DescriptiveCollectionEditor, Bonsai.Design", DesignTypes.UITypeEditor)]
        [Description("The collection of analog output channels used to generate voltage signals.")]
        public Collection<AnalogOutputChannelConfiguration> Channels
        {
            get { return channels; }
        }

        /// <summary>
        /// Generates a voltage signal in one or more DAQmx analog output channels
        /// from an observable sequence of samples.
        /// </summary>
        /// <param name="source">
        /// A sequence of floating-point numbers representing the samples used to
        /// generate voltage signals.
        /// </param>
        /// <returns>
        /// An observable sequence that is identical to the <paramref name="source"/>
        /// sequence but where there is an additional side effect of generating
        /// voltage signals in one or more DAQmx analog output channels.
        /// </returns>
        public IObservable<double> Process(IObservable<double> source)
        {
            return Observable.Defer(() =>
            {
                var task = new Task();
                foreach (var channel in channels)
                {
                    task.AOChannels.CreateVoltageChannel(
                        channel.PhysicalChannel,
                        channel.ChannelName,
                        channel.MinimumValue,
                        channel.MaximumValue,
                        channel.VoltageUnits);
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
                        analogOutWriter.WriteSingleSample(autoStart: true, new[] { input });
                    }));
            });
        }

        /// <summary>
        /// Generates voltage signals in one or more DAQmx analog output channels
        /// from an observable sequence of sample buffers.
        /// </summary>
        /// <param name="source">
        /// A sequence of 2D <see cref="Mat"/> objects storing the voltage samples.
        /// Each row corresponds to one of the channels in the signal generation task,
        /// and each column to a sample from each of the channels. The order of the
        /// channels follows the order in which you specify the channels in the
        /// <see cref="Channels"/> property.
        /// </param>
        /// <returns>
        /// An observable sequence that is identical to the <paramref name="source"/>
        /// sequence but where there is an additional side effect of generating
        /// voltage signals in one or more DAQmx analog output channels.
        /// </returns>
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
                            analogOutWriter.WriteMultiSample(autoStart: true, data);
                        }
                        finally { dataHandle.Free(); }
                    }));
            });
        }
    }
}
