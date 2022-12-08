Getting Started with Bonsai - DAQmx
===================================

## What is Bonsai - DAQmx

Bonsai.DAQmx is a [Bonsai](https://bonsai-rx.org/) package that allows interfacing with data acquisition and control hardware from [National Instruments](http://www.ni.com/) within a workflow.

The following operators for acquiring and generating signals from either digital or analog channels are provided:

  - [AnalogInput](xref:Bonsai.DAQmx.AnalogInput)
  - [AnalogOutput](xref:Bonsai.DAQmx.AnalogOutput)
  - [DigitalInput](xref:Bonsai.DAQmx.DigitalInput)
  - [DigitalOutput](xref:Bonsai.DAQmx.DigitalOutput)

Each operator supports acquiring or generating both single and multi-channel data, using either finite or continuous sampling. Input sources can acquire samples either synchronously (with an input), or asynchronously (with no input).

Examples are provided for some of the most common applications.

## How to install

Bonsai.DAQmx provides a bridge between the DAQmx driver and Bonsai. The Bonsai.DAQmx package can be downloaded through the Bonsai package manager. However, the package itself is not an installer for NI acquisition runtime and SDK. You need to install a compatible version of the DAQmx runtime for your hardware from the [NI website](https://www.ni.com/en-gb/support/downloads/drivers/download.ni-daqmx.html#464560).

Make sure that the .NET Framework 4.x Language Support optional feature is selected when you install the driver. If you have installed both the package and drivers and find that the operators are not showing up in the Bonsai toolbox, please modify your NI-DAQmx installation and make sure the feature is enabled. Also verify that you have installed the 64-bit NI-DAQmx driver if you are running Bonsai in a 64-bit environment, or 32-bit NI-DAQmx driver if you are running in a 32-bit environment.
