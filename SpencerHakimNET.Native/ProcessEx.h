#pragma once
#include <Windows.h>
#include "Utilities.h"

using namespace System::Diagnostics;

namespace SpencerHakim {

    public ref class ProcessEx
    {
    public:
        static Process^ StartAsSameUser(Process^ proc, ProcessStartInfo^ psi);
    };

}
