using NAudio.Midi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MidiSynth
{
    class Program
    {
        static void Main(string[] args)
        {

            //for (int device = 0; device < MidiOut.NumberOfDevices; device++)
            //{
            //    Console.WriteLine(MidiOut.DeviceInfo(device).ProductName);
            //}

            MidiOut midiOut = new MidiOut(0);
            int channel = 1;

            //midiOut.Send(MidiMessage.ChangePatch(24, 1).RawData);

            for (int instrument = 0; instrument <= 127; ++instrument)
            {
                midiOut.Reset();

                midiOut.Send(MidiMessage.ChangePatch(instrument, channel).RawData);

                for (int noteNumber = 50; noteNumber <= 90; ++noteNumber)
                {
                    //int noteNumber = 50;
                    //var noteOnEvent = new NoteOnEvent(100, channel, noteNumber, 100, 50);

                    //midiOut.Send(noteOnEvent.GetAsShortMessage());
                    midiOut.Send(new NoteEvent(100, channel, MidiCommandCode.NoteOn, noteNumber, 100).GetAsShortMessage());
                    Thread.Sleep(100);
                    midiOut.Send(new NoteEvent(100, channel, MidiCommandCode.NoteOff, noteNumber, 100).GetAsShortMessage());
                }
            }
            midiOut.Dispose();
            Console.ReadKey();
        }
    }
}
