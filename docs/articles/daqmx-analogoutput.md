---
uid: daqmx-analogoutput
title: AnalogOutput
---

[`AnalogOutput`](xref:Bonsai.DAQmx.AnalogOutput) configures and starts a task for generating voltage signals in one or more physical analog output channels. Voltage samples for each channel are read from sample buffers in the source sequence, where each row corresponds to one of the channels in the signal generation task, and each column to a sample from each of the channels. The order of the channels follows the order in which you specify the channels in the [`Channels`](xref:Bonsai.DAQmx.AnalogOutput.Channels) property.

Signals can be generated continuously, where a ring buffer is constanty updated with new data arriving from the source sequence.

:::workflow
![AnalogOutput-Continuous](~/workflows/AnalogOutput-Continuous.bonsai)
:::

Alternatively, signals can also be generated with a finite number of samples, in which case the input buffers will provide samples until the specified buffer size is reached. In this case, the operator will wait for the task to finish generating the specified number of samples.

:::workflow
![AnalogOutput-Finite](~/workflows/AnalogOutput-Finite.bonsai)
:::
