#pragma once
#include <vcclr.h>
#include <mmdeviceapi.h>
#include <audiopolicy.h>
#include "AudioSessionNotificationHook.h"

namespace SpencerHakim { namespace CoreAudio {

    //prototype, because 
    ref class AudioSessionNotificationHook;

    //native class that receives notifications,passing them on to the hook
    class AudioSessionNotificationReceiver : public IAudioSessionNotification
    {
    private:
        LONG m_cRefAll;
        gcroot<AudioSessionNotificationHook^> m_parent;

        ~AudioSessionNotificationReceiver(){}

    public:
        AudioSessionNotificationReceiver(gcroot<AudioSessionNotificationHook^> parent);

        // IUnknown
        HRESULT STDMETHODCALLTYPE QueryInterface(REFIID riid, void** ppv) ;
        ULONG STDMETHODCALLTYPE AddRef();
        ULONG STDMETHODCALLTYPE Release();

        HRESULT STDMETHODCALLTYPE OnSessionCreated(IAudioSessionControl* pNewSession);
    };

}}