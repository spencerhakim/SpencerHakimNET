#pragma once
#include <mmdeviceapi.h>
#include <audiopolicy.h>
#include "Utilities.h"
#include "AudioSessionNotificationReceiver.h"

using namespace System;
using namespace System::Diagnostics;

namespace SpencerHakim { namespace CoreAudio {

    public delegate void AudioSessionNotificationDelegate(IntPtr);

    //.NET class that sets up the notification listener
    public ref class AudioSessionNotificationHook
    {
    private:
        IAudioSessionManager2* sessionManager;
        IAudioSessionNotification* sessionNotification;

        HRESULT CreateSessionManager(IAudioSessionManager2** ppSessionManager);

    public:
        AudioSessionNotificationHook();
        ~AudioSessionNotificationHook();

        event AudioSessionNotificationDelegate^ OnSessionCreated;

    protected:
        !AudioSessionNotificationHook();

    internal:
        void FireOnSessionCreated(IAudioSessionControl* asc);
    };

}}