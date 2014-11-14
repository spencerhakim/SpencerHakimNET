#pragma once

namespace SpencerHakim {

    public ref class NativeExceptions
    {
    public:
        static void ThrowSEHException();
        static void ThrowSTDException();
    };

}