How to use
==========

`Bonsai.DAQmx` provides an interface for data acquisition and signal generation on NI hardware using four distinct operators.

In order to use any of the operators, you need to specify a collection of physical channels used to acquire or generate signals using the `Channels` property. Depending on whether you are working with analog or digital signals the properties used to configure physical channels are slightly different. Channels can be automatically enumerated by the visual interface as long as you have the device correctly plugged and configured in the host computer. 

Working examples for each of these operators can be found in the extended description for each operator, which we cover below.

## AnalogInput
[!include[AnalogInput](~/articles/daqmx-analoginput.md)]

## AnalogOutput
[!include[AnalogOutput](~/articles/daqmx-analogoutput.md)]

## DigitalInput
[!include[DigitalInput](~/articles/daqmx-digitalinput.md)]

## DigitalOutput
[!include[DigitalOutput](~/articles/daqmx-digitaloutput.md)]