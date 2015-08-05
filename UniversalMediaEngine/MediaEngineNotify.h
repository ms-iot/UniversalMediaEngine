#pragma once

namespace UniversalMediaEngine
{
	partial ref class MediaEngine;
}

class MediaEngineManager;

class MediaEngineNotify : public IMFMediaEngineNotify
{
public:
	MediaEngineNotify(UniversalMediaEngine::MediaEngine^ mediaEngine);

	HRESULT STDMETHODCALLTYPE QueryInterface(REFIID riid,
		LPVOID * ppvObj);

	ULONG STDMETHODCALLTYPE AddRef();

	ULONG STDMETHODCALLTYPE Release();

	HRESULT STDMETHODCALLTYPE EventNotify(
		DWORD     event,
		DWORD_PTR param1,
		DWORD     param2
		);

private:
	LONG m_cRef;
	UniversalMediaEngine::MediaEngine^ mediaEngine;
};


