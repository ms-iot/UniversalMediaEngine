#pragma once

namespace UniversalMediaEngine
{
	partial ref class MediaEngine;
}

class MediaEngineNotify;

class MediaEngineManager
{
public:
	MediaEngineManager(UniversalMediaEngine::MediaEngine^ mediaEngine);

	HRESULT STDMETHODCALLTYPE QueryInterface(REFIID riid,
		LPVOID * ppvObj);

	ULONG STDMETHODCALLTYPE AddRef();

	ULONG STDMETHODCALLTYPE Release();

	HRESULT Initialize();

	HRESULT PlayUrl(BSTR url);

	HRESULT Pause();

	HRESULT Stop();

	HRESULT GetVolume(double* pVolume);
	HRESULT SetVolume(double volume);

private:
	LONG m_cRef;
	bool isInitialized;
	ComPtr<IMFMediaEngine> spMediaEngine;
	ComPtr<MediaEngineNotify> spMediaEngineNotify;
	UniversalMediaEngine::MediaEngine^ mediaEngine;

	HRESULT checkInitialized();
};


