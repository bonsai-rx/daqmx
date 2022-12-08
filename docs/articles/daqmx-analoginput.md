---
uid: daqmx-analoginput
title: AnalogInput
---

[`AnalogInput`](xref:Bonsai.DAQmx.AnalogInput) configures and starts a data acquisition task for sampling voltage measurements from one or more physical analog input channels. Samples from each channel will be collected in a sample buffer, where each row corresponds to a channel in the acquisition task, and each column to a sample from each of the channels. The order of the channels follows the order in which you specify the channels in the [`Channels`](xref:Bonsai.DAQmx.AnalogInput.Channels) property.

If no input source is specified, data will be collected asynchronously every time a new buffer is filled.

:::workflow
![AnalogInput-Async](~/workflows/AnalogInput-Async.bonsai)
:::

Alternatively, if an input observable sequence is provided, a new data buffer will be collected every time a new notification is emitted by the input source.

:::workflow
![AnalogInput-Sync](~/workflows/AnalogInput-Sync.bonsai)
:::
