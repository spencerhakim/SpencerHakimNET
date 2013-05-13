#include "stdafx.h"
#include "AudioSessionNotificationReceiver.h"

using namespace SpencerHakim::CoreAudio;

AudioSessionNotificationReceiver::AudioSessionNotificationReceiver(gcroot<AudioSessionNotificationHook^> parent)
{
    m_cRefAll = 1;
    m_parent = parent;
}

HRESULT STDMETHODCALLTYPE AudioSessionNotificationReceiver::QueryInterface(REFIID riid, void** ppv)  
{    
    if( IID_IUnknown == riid )
    {
        AddRef();
        *ppv = (IUnknown*)this;
    }
    else if( __uuidof(IAudioSessionNotification) == riid )
    {
        AddRef();
        *ppv = (IAudioSessionNotification*)this;
    }
    else
    {
        *ppv = NULL;
        return E_NOINTERFACE;
    }

    return S_OK;
}

ULONG STDMETHODCALLTYPE AudioSessionNotificationReceiver::AddRef()
{
    return InterlockedIncrement(&m_cRefAll);
}
     
ULONG STDMETHODCALLTYPE AudioSessionNotificationReceiver::Release()
{
    ULONG ulRef = InterlockedDecrement(&m_cRefAll);
    if( 0 == ulRef )
        delete this;

    return ulRef;
}

HRESULT STDMETHODCALLTYPE AudioSessionNotificationReceiver::OnSessionCreated(IAudioSessionControl* pNewSession)
{
    if( pNewSession )
        m_parent->FireOnSessionCreated(pNewSession);

    return S_OK;
}