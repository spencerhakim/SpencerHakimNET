#pragma once

namespace SpencerHakim {

    ref class NativeExceptions
    {
    public:
        static void ThrowSEHException();
        static void ThrowSTDException();
    };

}