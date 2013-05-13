#pragma once
#include "Enums.h"

using namespace System;
using namespace System::Runtime::InteropServices;

namespace SpencerHakim { namespace CoreAudio {

    [ComImport, Guid("24918ACC-64B3-37C1-8CA9-74A66E9957A8"), InterfaceType(ComInterfaceType::InterfaceIsIUnknown)]
    public interface class IAudioSessionEvents
    {
    public:
        void OnDisplayNameChanged([MarshalAs(UnmanagedType::LPWStr)]String^ NewDisplayName, Guid EventContext);
        void OnIconPathChanged([MarshalAs(UnmanagedType::LPWStr)]String^ NewIconPath, Guid EventContext);
        void OnSimpleVolumeChanged(float NewVolume, bool NewMute, Guid EventContext);
        void OnChannelVolumeChanged(UInt32 ChannelCount, [MarshalAs(UnmanagedType::LPArray, SizeParamIndex=0)]float NewChannelVolumeArray[], UInt32 ChangedChannel, Guid EventContext);
        void OnGroupingParamChanged(Guid NewGroupingParam, Guid EventContext);
        void OnStateChanged(AudioSessionState NewState);
        void OnSessionDisconnected(AudioSessionDisconnectReason DisconnectReason);
    };

}}