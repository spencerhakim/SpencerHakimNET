#include "Stdafx.h"
#include "Utilities.h"
#include "AudioSessionNotificationHook.h"

//cheap compile time test for USING() macro
void using_test()
{
    USING( System::Management::ManagementObject, p, gcnew System::Management::ManagementObject ) //explicitly IDisposable
    USING( SpencerHakim::CoreAudio::AudioSessionNotificationHook, h, gcnew SpencerHakim::CoreAudio::AudioSessionNotificationHook ) //implicitly IDisposable, thanks to C++/CLI destructor sugar
    //USING( System::String, s, gcnew System::String("") ) //fails, since it's not disposable
        (void)0;
}