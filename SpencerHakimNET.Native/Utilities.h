#pragma once

//http://stackoverflow.com/a/673336/489071
template <typename T>
public ref class using_auto_ptr
{
public:
    using_auto_ptr(T^ p) : m_p(p), m_used(true)
    {
        //type check, since C#'s using is only for IDisposable
        static_cast<System::IDisposable^>(p);
    }

    ~using_auto_ptr() { delete m_p; }

    T^ operator->() { return m_p; }
    static T^ operator*(using_auto_ptr<T>% p) { return p->m_p; } //C4383 says I should do it this way

    bool m_used;

private:
    T^ m_p;
};
#define USING(CLASS, VAR, ALLOC) \
    for( using_auto_ptr<CLASS> VAR(ALLOC); VAR.m_used; VAR.m_used=0)

#define SAFE_RELEASE(p) if( NULL != (p) ){ (p)->Release(); (p) = NULL; }
#define SAFE_DELETE(p) if( NULL != (p) ){ delete (p); (p) = NULL; }
#define CHECK_HR(hr) if( FAILED(hr) ){ goto done; }
#define THROW_HR(hr) System::Runtime::InteropServices::Marshal::ThrowExceptionForHR(hr)