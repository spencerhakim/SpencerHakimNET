#pragma once

using namespace System;

namespace SpencerHakim { namespace CoreAudio {

    [ComImport, Guid("87CE5498-68D6-44E5-9215-6DA47EF883D8"), InterfaceType(ComInterfaceType::InterfaceIsIUnknown)]
    public interface class ISimpleAudioVolume
    {
    public:
        void SetMasterVolume(float Level, Guid EventContext);
        float GetMasterVolume();

        void SetMute(bool Mute, Guid EventContext);
        bool GetMute();
    };

}}