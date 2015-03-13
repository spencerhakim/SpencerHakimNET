#include "stdafx.h"
#include "ProcessEx.h"

using namespace System;
using namespace System::Diagnostics;
using namespace System::Runtime::InteropServices;
using namespace System::Security::Permissions;
using namespace System::Text;
using namespace SpencerHakim;

//Ported from .NET Framework source
String^ BuildCommandLine(String^ executableFileName, String^ arguments)
{
    // Construct a StringBuilder with the appropriate command line
    // to pass to CreateProcess.  If the filename isn't already 
    // in quotes, we quote it here.  This prevents some security
    // problems (it specifies exactly which part of the string
    // is the file to execute).
    StringBuilder commandLine;
    String^ fileName = executableFileName->Trim();
    bool fileNameIsQuoted = (fileName->StartsWith("\"", StringComparison::Ordinal) && fileName->EndsWith("\"", StringComparison::Ordinal));
    if( !fileNameIsQuoted )
    {
        commandLine.Append("\"");
    }

    commandLine.Append(fileName);

    if( !fileNameIsQuoted )
    {
        commandLine.Append("\"");
    }

    if( !String::IsNullOrEmpty(arguments) )
    {
        commandLine.Append(" ");
        commandLine.Append(arguments);
    }

    return commandLine.ToString();
}

[PermissionSet(SecurityAction::LinkDemand, Name="FullTrust", Unrestricted=false)]
Process^ ProcessEx::StartAsSameUser(Process^ proc, ProcessStartInfo^ psi)
{
    HRESULT hr = S_OK;
    LPTSTR lpCmdLine = (LPTSTR)Marshal::StringToHGlobalAuto(
        BuildCommandLine(psi->FileName, psi->Arguments)
    ).ToPointer();

    // get process handle and token
    HANDLE hProcess = NULL;
    if( !(hProcess = OpenProcess(MAXIMUM_ALLOWED, false, proc->Id)) )
        goto error;

    HANDLE hToken = NULL;
    if( !OpenProcessToken(hProcess, TOKEN_DUPLICATE, &hToken) )
        goto error;

    // copy the access token of the process; the newly created token will be a primary token
    HANDLE hUserTokenDup = NULL;
    BOOL result = DuplicateTokenEx(
        hToken,
        MAXIMUM_ALLOWED,
        NULL,
        SECURITY_IMPERSONATION_LEVEL::SecurityIdentification,
        TOKEN_TYPE::TokenPrimary,
        &hUserTokenDup
    );

    if( !result )
        goto error;

    // create a new process in the current interactive session
    STARTUPINFO si;
    si.cb = sizeof(STARTUPINFO);
    si.lpDesktop = L"winsta0\\default";
    /*si.dwFlags |= STARTF_USESHOWWINDOW;
    si.wShowWindow = SW_SHOW;*/

    //TODO - figure out which flags to use
    DWORD dwCreationFlags = 
        (Environment::OSVersion->Platform == PlatformID::Win32NT ? CREATE_UNICODE_ENVIRONMENT : NULL) |
        (psi->CreateNoWindow ? CREATE_NO_WINDOW : NULL);

    PROCESS_INFORMATION pi;
    result = CreateProcessAsUser(
        hUserTokenDup,      // client's access token
        NULL,               // file to execute
        lpCmdLine,          // command line
        NULL,               // pointer to process SECURITY_ATTRIBUTES
        NULL,               // pointer to thread SECURITY_ATTRIBUTES
        false,              // handles are not inheritable
        dwCreationFlags,    // creation flags
        NULL,               // pointer to new environment block 
        NULL,               // name of current directory 
        &si,                // pointer to STARTUPINFO structure
        &pi                 // receives information about new process
    );

    if( !result )
        goto error;

    CloseHandle(pi.hProcess);
    CloseHandle(pi.hThread);

    Process^ newProc = Process::GetProcessById(pi.dwProcessId);
    goto done;

    ///////////////////////////////////////////////////////////////////////////
    // cleanup
error:
    hr = HRESULT_FROM_WIN32(GetLastError());

done:
#pragma warning(suppress: 6001)
    if( hUserTokenDup )
        CloseHandle(hUserTokenDup);

#pragma warning(suppress: 6001)
    if( hToken )
        CloseHandle(hToken);

    if( hProcess )
        CloseHandle(hProcess);

    THROW_HR(hr);
    return newProc;
}
