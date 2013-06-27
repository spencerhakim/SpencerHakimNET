#include "stdafx.h"
#include "AudioSessionNotificationHook.h"

using namespace SpencerHakim::CoreAudio;

AudioSessionNotificationHook::AudioSessionNotificationHook()
{
    //need to pin this and only this pointer because fuck you
    sessionManager = NULL;
    pin_ptr<IAudioSessionManager2*> pinnedSM = &sessionManager;
    THROW_HR( CreateSessionManager(pinnedSM) );

    //create a notification listener
    sessionNotification = new AudioSessionNotificationReceiver(this);
    THROW_HR( sessionManager->RegisterSessionNotification(sessionNotification) );

    //need to get list to kickstart notifications because fuck you again
    IAudioSessionEnumerator* sessionEnum = NULL;
    THROW_HR( sessionManager->GetSessionEnumerator(&sessionEnum) );
    sessionEnum->Release();
}

AudioSessionNotificationHook::~AudioSessionNotificationHook()
{
    this->!AudioSessionNotificationHook();
}

AudioSessionNotificationHook::!AudioSessionNotificationHook()
{
    if( sessionManager )
        sessionManager->UnregisterSessionNotification(sessionNotification);

    SAFE_RELEASE( sessionNotification );
    SAFE_RELEASE( sessionManager );
}

void AudioSessionNotificationHook::FireOnSessionCreated(IAudioSessionControl* asc)
{
    OnSessionCreated( IntPtr(asc) );
}

//thanks for the code, Microsoft
HRESULT AudioSessionNotificationHook::CreateSessionManager(IAudioSessionManager2** ppSessionManager)
{
    HRESULT hr = S_OK;
    
    IMMDevice* pDevice = NULL;
    IMMDeviceEnumerator* pEnumerator = NULL;
    IAudioSessionManager2* pSessionManager = NULL;

    // Create the device enumerator.
    CHECK_HR( hr = CoCreateInstance(__uuidof(MMDeviceEnumerator), NULL, CLSCTX_ALL, __uuidof(IMMDeviceEnumerator), (void**)&pEnumerator) );

    // Get the default audio device.
    CHECK_HR( hr = pEnumerator->GetDefaultAudioEndpoint(eRender, eMultimedia, &pDevice) );

    // Get the session manager.
    CHECK_HR( hr = pDevice->Activate(__uuidof(IAudioSessionManager2), CLSCTX_ALL, NULL, (void**)&pSessionManager) );

    // Return the pointer to the caller.
    *ppSessionManager = pSessionManager;
    (*ppSessionManager)->AddRef();

done: // Clean up.
    SAFE_RELEASE(pSessionManager);
    SAFE_RELEASE(pDevice);
    SAFE_RELEASE(pEnumerator);

    return hr;
}