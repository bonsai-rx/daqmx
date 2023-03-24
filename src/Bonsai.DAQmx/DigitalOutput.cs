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
    /// <summary>
    /// Represents an operator that writes logical values to one or more DAQmx digital
    /// output lines from a sequence of sample buffers.
    /// </summary>
    [DefaultProperty(nameof(Channels))]
    [Description("Writes logical values to one or more DAQmx digital output lines from a sequence of sample buffers.")]
    public class DigitalOutput : Sink<Mat>
    {
        readonly Collection<DigitalOutputChannelConfiguration> channels = new Collection<DigitalOutputChannelConfiguration>();

        /// <summary>
        /// Gets or sets the optional source terminal of the clock. If not specified,
        /// the internal clock of the device will be used.
        /// </summary>
        [Description("The optional source terminal of the clock. If not specified, the internal clock of the device will be used.")]
        public string SignalSource { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the sampling rate for writing logical values, in samples
        /// per second.
        /// </summary>
        public double SampleRate { get; set; }

        /// <summary>
        /// Gets or sets a value specifying on which edge of a clock pulse sampling takes place.
        /// </summary>
        [Description("Specifies on which edge of a clock pulse sampling takes place.")]
        public SampleClockActiveEdge ActiveEdge { get; set; } = SampleClockActiveEdge.Rising;

        /// <summary>
        /// Gets or sets a value specifying whether the writer task will generate
        /// a finite number of samples or if it continuously generates samples.
        /// </summary>
        [Description("Specifies whether the writer task will generate a finite number of samples or if it continuously generates samples.")]
        public SampleQuantityMode SampleMode { get; set; } = SampleQuantityMode.ContinuousSamples;

        /// <summary>
        /// Gets or sets the number of samples to generate, for finite samples, or the
        /// size of the buffer for continuous samples.
        /// </summary>
        [Description("The number of samples to generate, for finite samples, or the size of the buffer for continuous samples.")]
        public int BufferSize { get; set; } = 1000;

        /// <summary>
        /// Gets the collection of virtual output channels on which to write the
        /// logical values.
        /// </summary>
        [Editor("Bonsai.Design.DescriptiveCollectionEditor, Bonsai.Design", DesignTypes.UITypeEditor)]
        [Description("The collection of virtual output channels on which to write the logical values.")]
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

        IObservable<TSource> ProcessSingleSample<TSource>(IObservable<TSource> source, Action<DigitalMultiChannelWriter, TSource> onNext)
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

        /// <summary>
        /// Writes an observable sequence of logical values to one or more DAQmx
        /// digital output lines.
        /// </summary>
        /// <param name="source">
        /// A sequence of boolean values representing the logical levels to write
        /// to one or more DAQmx digital output lines.
        /// </param>
        /// <returns>
        /// An observable sequence that is identical to the <paramref name="source"/>
        /// sequence but where there is an additional side effect of writing logical
        /// values to one or more DAQmx digital output lines.
        /// </returns>
        public IObservable<bool> Process(IObservable<bool> source)
        {
            return ProcessSingleSample(source, (writer, value) =>
            {
                writer.WriteSingleSampleSingleLine(autoStart: true, new[] { value });
            });
        }

        /// <summary>
        /// Writes an observable sequence of unsigned 8-bit samples to one or more DAQmx
        /// digital output lines.
        /// </summary>
        /// <param name="source">
        /// A sequence of 8-bit unsigned integers representing the state of digital
        /// output lines in a local virtual port channel.
        /// </param>
        /// <returns>
        /// An observable sequence that is identical to the <paramref name="source"/>
        /// sequence but where there is an additional side effect of writing logical
        /// values to one or more DAQmx digital output lines.
        /// </returns>
        public IObservable<byte> Process(IObservable<byte> source)
        {
            return ProcessSingleSample(source, (writer, value) =>
            {
                writer.WriteSingleSamplePort(autoStart: true, new[] { value });
            });
        }

        /// <summary>
        /// Writes an observable sequence of boolean samples to one or more DAQmx
        /// digital output lines.
        /// </summary>
        /// <param name="source">
        /// A sequence of 1D arrays of boolean samples representing the state of a
        /// digital output channel in a local virtual port.
        /// </param>
        /// <returns>
        /// An observable sequence that is identical to the <paramref name="source"/>
        /// sequence but where there is an additional side effect of writing logical
        /// values to one or more DAQmx digital output lines.
        /// </returns>
        public IObservable<bool[]> Process(IObservable<bool[]> source)
        {
            return ProcessSingleSample(source, (writer, data) =>
            {
                writer.WriteSingleSampleSingleLine(autoStart: true, data);
            });
        }

        /// <summary>
        /// Writes an observable sequence of unsigned 8-bit samples to one or more DAQmx
        /// digital output lines.
        /// </summary>
        /// <param name="source">
        /// A sequence of 8-bit unsigned integer arrays representing the state of digital
        /// output lines in a local virtual port channel.
        /// </param>
        /// <returns>
        /// An observable sequence that is identical to the <paramref name="source"/>
        /// sequence but where there is an additional side effect of writing logical
        /// values to one or more DAQmx digital output lines.
        /// </returns>
        public IObservable<byte[]> Process(IObservable<byte[]> source)
        {
            return ProcessSingleSample(source, (writer, data) =>
            {
                writer.WriteSingleSamplePort(autoStart: true, data);
            });
        }

        /// <summary>
        /// Writes an observable sequence of unsigned 16-bit samples to one or more DAQmx
        /// digital output lines.
        /// </summary>
        /// <param name="source">
        /// A sequence of 16-bit unsigned integer arrays representing the state of digital
        /// output lines in a local virtual port channel.
        /// </param>
        /// <returns>
        /// An observable sequence that is identical to the <paramref name="source"/>
        /// sequence but where there is an additional side effect of writing logical
        /// values to one or more DAQmx digital output lines.
        /// </returns>
        public IObservable<ushort[]> Process(IObservable<ushort[]> source)
        {
            return ProcessSingleSample(source, (writer, data) =>
            {
                writer.WriteSingleSamplePort(autoStart: true, data);
            });
        }

        /// <summary>
        /// Writes an observable sequence of signed 16-bit samples to one or more DAQmx
        /// digital output lines.
        /// </summary>
        /// <param name="source">
        /// A sequence of 16-bit signed integer arrays representing the state of digital
        /// output lines in a local virtual port channel.
        /// </param>
        /// <returns>
        /// An observable sequence that is identical to the <paramref name="source"/>
        /// sequence but where there is an additional side effect of writing logical
        /// values to one or more DAQmx digital output lines.
        /// </returns>
        public IObservable<short[]> Process(IObservable<short[]> source)
        {
            return ProcessSingleSample(source, (writer, data) =>
            {
                writer.WriteSingleSamplePort(autoStart: true, data);
            });
        }

        /// <summary>
        /// Writes an observable sequence of unsigned 32-bit samples to one or more DAQmx
        /// digital output lines.
        /// </summary>
        /// <param name="source">
        /// A sequence of 32-bit unsigned integer arrays representing the state of digital
        /// output lines in a local virtual port channel.
        /// </param>
        /// <returns>
        /// An observable sequence that is identical to the <paramref name="source"/>
        /// sequence but where there is an additional side effect of writing logical
        /// values to one or more DAQmx digital output lines.
        /// </returns>
        public IObservable<uint[]> Process(IObservable<uint[]> source)
        {
            return ProcessSingleSample(source, (writer, data) =>
            {
                writer.WriteSingleSamplePort(autoStart: true, data);
            });
        }

        /// <summary>
        /// Writes an observable sequence of signed 32-bit samples to one or more DAQmx
        /// digital output lines.
        /// </summary>
        /// <param name="source">
        /// A sequence of 32-bit signed integer arrays representing the state of digital
        /// output lines in a local virtual port channel.
        /// </param>
        /// <returns>
        /// An observable sequence that is identical to the <paramref name="source"/>
        /// sequence but where there is an additional side effect of writing logical
        /// values to one or more DAQmx digital output lines.
        /// </returns>
        public IObservable<int[]> Process(IObservable<int[]> source)
        {
            return ProcessSingleSample(source, (writer, data) =>
            {
                writer.WriteSingleSamplePort(autoStart: true, data);
            });
        }

        IObservable<TSource> ProcessMultiSample<TSource>(IObservable<TSource> source, Action<DigitalMultiChannelWriter, TSource> onNext)
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
                        try { onNext(digitalOutWriter, input); }
                        catch { task.Stop(); throw; }
                    }));
            });
        }

        /// <summary>
        /// Writes logical values to one or more DAQmx digital output lines from
        /// an observable sequence of unsigned 8-bit multi-channel sample buffers.
        /// </summary>
        /// <param name="source">
        /// A sequence of multi-dimensional <see cref="byte"/> arrays storing the
        /// logical values. Each row corresponds to a channel in the signal generation
        /// task, and each column to a sample from each of the channels. The order of
        /// the channels follows the order in which you specify the channels in the
        /// <see cref="Channels"/> property. Each sample can represent either a single
        /// line or a bitmask representing the state of all digital lines in a single
        /// port, depending on the configuration of the virtual channel.
        /// </param>
        /// <returns>
        /// An observable sequence that is identical to the <paramref name="source"/>
        /// sequence but where there is an additional side effect of writing logical
        /// values to one or more DAQmx digital output lines.
        /// </returns>
        public IObservable<byte[,]> Process(IObservable<byte[,]> source)
        {
            return ProcessMultiSample(source, (writer, data) =>
            {
                writer.WriteMultiSamplePort(autoStart: true, data);
            });
        }

        /// <summary>
        /// Writes logical values to one or more DAQmx digital output lines from
        /// an observable sequence of unsigned 16-bit multi-channel sample buffers.
        /// </summary>
        /// <param name="source">
        /// A sequence of multi-dimensional <see cref="ushort"/> arrays storing the
        /// logical values. Each row corresponds to a channel in the signal generation
        /// task, and each column to a sample from each of the channels. The order of
        /// the channels follows the order in which you specify the channels in the
        /// <see cref="Channels"/> property. Each sample can represent either a single
        /// line or a bitmask representing the state of all digital lines in a single
        /// port, depending on the configuration of the virtual channel.
        /// </param>
        /// <returns>
        /// An observable sequence that is identical to the <paramref name="source"/>
        /// sequence but where there is an additional side effect of writing logical
        /// values to one or more DAQmx digital output lines.
        /// </returns>
        public IObservable<ushort[,]> Process(IObservable<ushort[,]> source)
        {
            return ProcessMultiSample(source, (writer, data) =>
            {
                writer.WriteMultiSamplePort(autoStart: true, data);
            });
        }

        /// <summary>
        /// Writes logical values to one or more DAQmx digital output lines from
        /// an observable sequence of signed 16-bit multi-channel sample buffers.
        /// </summary>
        /// <param name="source">
        /// A sequence of multi-dimensional <see cref="short"/> arrays storing the
        /// logical values. Each row corresponds to a channel in the signal generation
        /// task, and each column to a sample from each of the channels. The order of
        /// the channels follows the order in which you specify the channels in the
        /// <see cref="Channels"/> property. Each sample can represent either a single
        /// line or a bitmask representing the state of all digital lines in a single
        /// port, depending on the configuration of the virtual channel.
        /// </param>
        /// <returns>
        /// An observable sequence that is identical to the <paramref name="source"/>
        /// sequence but where there is an additional side effect of writing logical
        /// values to one or more DAQmx digital output lines.
        /// </returns>
        public IObservable<short[,]> Process(IObservable<short[,]> source)
        {
            return ProcessMultiSample(source, (writer, data) =>
            {
                writer.WriteMultiSamplePort(autoStart: true, data);
            });
        }

        /// <summary>
        /// Writes logical values to one or more DAQmx digital output lines from
        /// an observable sequence of unsigned 32-bit multi-channel sample buffers.
        /// </summary>
        /// <param name="source">
        /// A sequence of multi-dimensional <see cref="uint"/> arrays storing the
        /// logical values. Each row corresponds to a channel in the signal generation
        /// task, and each column to a sample from each of the channels. The order of
        /// the channels follows the order in which you specify the channels in the
        /// <see cref="Channels"/> property. Each sample can represent either a single
        /// line or a bitmask representing the state of all digital lines in a single
        /// port, depending on the configuration of the virtual channel.
        /// </param>
        /// <returns>
        /// An observable sequence that is identical to the <paramref name="source"/>
        /// sequence but where there is an additional side effect of writing logical
        /// values to one or more DAQmx digital output lines.
        /// </returns>
        public IObservable<uint[,]> Process(IObservable<uint[,]> source)
        {
            return ProcessMultiSample(source, (writer, data) =>
            {
                writer.WriteMultiSamplePort(autoStart: true, data);
            });
        }

        /// <summary>
        /// Writes logical values to one or more DAQmx digital output lines from
        /// an observable sequence of signed 32-bit multi-channel sample buffers.
        /// </summary>
        /// <param name="source">
        /// A sequence of multi-dimensional <see cref="int"/> arrays storing the
        /// logical values. Each row corresponds to a channel in the signal generation
        /// task, and each column to a sample from each of the channels. The order of
        /// the channels follows the order in which you specify the channels in the
        /// <see cref="Channels"/> property. Each sample can represent either a single
        /// line or a bitmask representing the state of all digital lines in a single
        /// port, depending on the configuration of the virtual channel.
        /// </param>
        /// <returns>
        /// An observable sequence that is identical to the <paramref name="source"/>
        /// sequence but where there is an additional side effect of writing logical
        /// values to one or more DAQmx digital output lines.
        /// </returns>
        public IObservable<int[,]> Process(IObservable<int[,]> source)
        {
            return ProcessMultiSample(source, (writer, data) =>
            {
                writer.WriteMultiSamplePort(autoStart: true, data);
            });
        }

        /// <summary>
        /// Writes logical values to one or more DAQmx digital output lines
        /// from an observable sequence of sample buffers.
        /// </summary>
        /// <param name="source">
        /// A sequence of 2D <see cref="Mat"/> objects storing the logical values.
        /// Each row corresponds to a channel in the signal generation task, and each
        /// column to a sample from each of the channels. The order of the channels follows
        /// the order in which you specify the channels in the <see cref="Channels"/>
        /// property. Each sample can represent either a single line or a bitmask
        /// representing the state of all digital lines in a single port, depending
        /// on the configuration of the virtual channel.
        /// </param>
        /// <returns>
        /// An observable sequence that is identical to the <paramref name="source"/>
        /// sequence but where there is an additional side effect of writing logical
        /// values to one or more DAQmx digital output lines.
        /// </returns>
        public override IObservable<Mat> Process(IObservable<Mat> source)
        {
            return ProcessMultiSample(source, (writer, input) =>
            {
                switch (input.Depth)
                {
                    case Depth.U8:
                    case Depth.S8:
                        writer.WriteMultiSamplePort(autoStart: true, GetMultiSampleArray<byte>(input));
                        break;
                    case Depth.U16:
                    case Depth.S16:
                        writer.WriteMultiSamplePort(autoStart: true, GetMultiSampleArray<ushort>(input));
                        break;
                    case Depth.S32:
                        writer.WriteMultiSamplePort(autoStart: true, GetMultiSampleArray<int>(input));
                        break;
                    default:
                        throw new InvalidOperationException("The elements in the input buffer must have an integer depth type.");
                }
            });
        }

        static TArray[,] GetMultiSampleArray<TArray>(Mat input) where TArray : unmanaged
        {
            var data = new TArray[input.Rows, input.Cols];
            var dataHandle = GCHandle.Alloc(data, GCHandleType.Pinned);
            try
            {
                var dataHeader = new Mat(input.Rows, input.Cols, input.Depth, 1, dataHandle.AddrOfPinnedObject());
                CV.Copy(input, dataHeader);
                return data;
            }
            finally { dataHandle.Free(); }
        }
    }
}
