#include "stdafx.h"
#include <stdexcept>
#include "NativeExceptions.h"

using namespace SpencerHakim;

void NativeExceptions::ThrowSEHException()
{
#pragma warning(disable : 6011)
    //triggers SEH - http://msdn.microsoft.com/en-us/library/swezty51.aspx
    volatile int* p = NULL;
    *p = 1;
#pragma warning(default : 6011)
}

void NativeExceptions::ThrowSTDException()
{
    throw std::exception();
}