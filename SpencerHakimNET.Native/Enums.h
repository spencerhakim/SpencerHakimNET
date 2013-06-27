#pragma once

namespace SpencerHakim { namespace CoreAudio {

    public enum AudioSessionState
    {
        Inactive = 0,
        Active = 1,
        Expired = 2
    };

    public enum AudioSessionDisconnectReason
    {
        DeviceRemoval = 0,
        ServerShutdown = (DeviceRemoval + 1),
        FormatChanged = (ServerShutdown + 1),
        SessionLogoff = (FormatChanged + 1),
        SessionDisconnected = (SessionLogoff + 1),
        ExclusiveModeOverride = (SessionDisconnected + 1) 
    };

}}