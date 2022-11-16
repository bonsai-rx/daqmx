using System;
using System.Reactive.Linq;
using NationalInstruments.DAQmx;
using System.Reactive.Disposables;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Bonsai.DAQmx
{
    [Description("Writes a sequence of logical values to one or more DAQmx digital output channels.")]
    public class DigitalOutput : Sink<bool>
    {
        readonly Collection<DigitalOutputChannelConfiguration> channels = new Collection<DigitalOutputChannelConfiguration>();

        [Description("The collection of digital output channels used to generate digital signals.")]
        public Collection<DigitalOutputChannelConfiguration> Channels
        {
            get { return channels; }
        }

        public override IObservable<bool> Process(IObservable<bool> source)
        {
            return Observable.Defer(() =>
            {
                var task = new Task();
                foreach (var channel in channels)
                {
                    task.DOChannels.CreateChannel(channel.Lines, channel.ChannelName, channel.Grouping);
                }

                task.Control(TaskAction.Verify);
                var digitalInReader = new DigitalMultiChannelWriter(task.Stream);
                return Observable.Using(
                    () => Disposable.Create(() =>
                    {
                        task.WaitUntilDone();
                        task.Stop();
                        task.Dispose();
                    }),
                    resource => source.Do(input =>
                    {
                        digitalInReader.WriteSingleSampleSingleLine(true, new[] { input });
                    }));
            });
        }
    }
}
