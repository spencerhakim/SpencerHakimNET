#pragma once
#include "IAudioSessionEvents.h"

using namespace System;
using namespace System::Runtime::InteropServices;

namespace SpencerHakim { namespace CoreAudio {

    [ComImport, Guid("F4B1A599-7266-4319-A8CA-E70ACB11E8CD"), InterfaceType(ComInterfaceType::InterfaceIsIUnknown)]
    public interface class IAudioSessionControl
    {
    public:
        AudioSessionState GetState();

        [returnvalue: MarshalAs(UnmanagedType::LPWStr)]String^ GetDisplayName();
        void SetDisplayName([MarshalAs(UnmanagedType::LPWStr)]String^ Value, Guid EventContext);

        [returnvalue: MarshalAs(UnmanagedType::LPWStr)]String^ GetIconPath();
        void SetIconPath([MarshalAs(UnmanagedType::LPWStr)]String^ Value, Guid EventContext);

        Guid GetGroupingParam();
        void SetGroupingParam(Guid Override, Guid EventContext);

        //TODO - change IntPtr
        void RegisterAudioSessionNotification([MarshalAs(UnmanagedType::IUnknown)]IAudioSessionEvents^ NewNotifications);
        void UnregisterAudioSessionNotification([MarshalAs(UnmanagedType::IUnknown)]IAudioSessionEvents^ NewNotifications);
    };

}}