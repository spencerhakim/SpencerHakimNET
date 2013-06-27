#include "stdafx.h"
#include <stdexcept>
#include "NativeExceptions.h"

using namespace SpencerHakim;

void NativeExceptions::ThrowSEHException()
{
    //triggers SEH - http://msdn.microsoft.com/en-us/library/swezty51.aspx
    volatile int* p = NULL;
    *p = 1;
}

void NativeExceptions::ThrowSTDException()
{
    throw std::exception();
}