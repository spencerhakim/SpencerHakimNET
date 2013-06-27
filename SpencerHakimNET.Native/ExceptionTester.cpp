#include "stdafx.h"
#include <stdexcept>
#include "ExceptionTester.h"

void ExceptionTester::ThrowSEHException()
{
    //triggers SEH - http://msdn.microsoft.com/en-us/library/swezty51.aspx
    volatile int* p = NULL;
    *p = 1;
}

void ExceptionTester::ThrowSTDException()
{
    throw std::exception();
}