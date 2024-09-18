---
uid: Bonsai.DAQmx.DigitalInput
---

[`DigitalInput`](xref:Bonsai.DAQmx.DigitalInput) configures and starts a data acquisition task for sampling logical values from one or more digital input lines. Logical values will be collected in a sample buffer, where each sample can represent either a single line or a bitmask representing the state of all digital lines in a single port, depending on the configuration of the virtual channel. Each row corresponds to a channel in the acquisition task, and each column to a sample from each of the channels. The order of the channels in the sample buffer follows the order in which you specify the channels in the [`Channels`](xref:Bonsai.DAQmx.DigitalInput.Channels) property.

Digital lines can be grouped as a port when creating the local virtual channel, either by specifying a range of lines (e.g. `Dev1/port0/line0:3`) or by specifying an entire port at once (e.g. `Dev1/port0`).

If no input source is specified, samples will be collected asynchronously every time a new buffer is filled.

:::workflow
![DigitalInput-Async](~/workflows/DigitalInput-Async.bonsai)
:::

Alternatively, if an input observable sequence is provided, a new sample buffer will be collected every time a new notification is emitted by the input source.

:::workflow
![DigitalInput-Sync](~/workflows/DigitalInput-Sync.bonsai)
:::