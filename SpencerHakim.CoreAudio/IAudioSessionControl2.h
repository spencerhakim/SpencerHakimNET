#pragma once
#include "IAudioSessionControl.h"

using namespace System;
using namespace System::Runtime::InteropServices;

namespace SpencerHakim { namespace CoreAudio {

    [ComImport, Guid("BFB7FF88-7239-4FC9-8FA2-07C950BE9C6D"), InterfaceType(ComInterfaceType::InterfaceIsIUnknown)]
    public interface class IAudioSessionControl2 : IAudioSessionControl //need to redefine all the IAudioSessionControl methods because COM interop doesn't actually support inheritance because fuck you
    {
    public:
        ///////////////////////////////////////////////////////////////////////
        //IAudioSessionControl
        AudioSessionState GetState();

        [returnvalue: MarshalAs(UnmanagedType::LPWStr)]String^ GetDisplayName();
        void SetDisplayName([MarshalAs(UnmanagedType::LPWStr)]String^ Value, Guid EventContext);

        [returnvalue: MarshalAs(UnmanagedType::LPWStr)]String^ GetIconPath();
        void SetIconPath([MarshalAs(UnmanagedType::LPWStr)]String^ Value, Guid EventContext);

        Guid GetGroupingParam();
        void SetGroupingParam(Guid Override, Guid EventContext);

        //TODO - change IntPtr
        void RegisterAudioSessionNotification(IntPtr NewNotifications);
        void UnregisterAudioSessionNotification(IntPtr NewNotifications);

        ///////////////////////////////////////////////////////////////////////
        //IAudioSessionControl2
        [returnvalue: MarshalAs(UnmanagedType::LPWStr)]String^ GetSessionIdentifier();
        [returnvalue: MarshalAs(UnmanagedType::LPWStr)]String^ GetSessionInstanceIdentifier();

        UInt32 GetProcessId();

        bool IsSystemSoundsSession();

        void SetDuckingPreference(bool optOut);
    };

}}