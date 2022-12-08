---
uid: daqmx-digitaloutput
title: DigitalOutput
---

[`DigitalOutput`](xref:Bonsai.DAQmx.DigitalOutput) configures and starts a task for writing logical values to one or more digital output lines. Logical values for each line are read from sample buffers in the source sequence, where each row corresponds to one of the channels in the signal generation task, and each column to a sample from each of the channels. The order of the channels follows the order in which you specify the channels in the [`Channels`](xref:Bonsai.DAQmx.DigitalOutput.Channels) property.

Each logical value sample can represent either a single line or a bitmask representing the state of all digital lines in a single port, depending on the configuration of the virtual channel. Digital lines can be grouped as a port when creating the local virtual channel, either by specifying a range of lines (e.g. `Dev1/port0/line0:3`) or by specifying an entire port at once (e.g. `Dev1/port0`).

Signals can be generated continuously, where a ring buffer is constanty updated with new data arriving from the source sequence.

:::workflow
![DigitalOutput-Continuous](~/workflows/DigitalOutput-Continuous.bonsai)
:::

Logical values can also be provided by a source of integers specifying a bitmask with the state of all digital lines in a single port.

:::workflow
![DigitalOutput-ContinuousSingleSample](~/workflows/DigitalOutput-ContinuousSingleSample.bonsai)
:::

Alternatively, signals can also be generated with a finite number of samples, in which case the input buffers will provide samples until the specified buffer size is reached. In this case, the operator will wait for the task to finish generating the specified number of samples.

:::workflow
![DigitalOutput-Finite](~/workflows/DigitalOutput-Finite.bonsai)
:::

