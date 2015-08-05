#include "pch.h"
#include "MediaEngineManager.h"
#include "MediaEngine.h"
#include "MediaEngineNotify.h"

using namespace UniversalMediaEngine;

MediaEngineNotify::MediaEngineNotify(UniversalMediaEngine::MediaEngine^ mediaEngine) :
m_cRef(1),
mediaEngine(mediaEngine)
{
}

HRESULT MediaEngineNotify::QueryInterface(REFIID riid,
	LPVOID * ppvObj)
{
	if (!ppvObj)
		return E_INVALIDARG;

	*ppvObj = NULL;
	if (riid == IID_IUnknown || riid == __uuidof(IMFMediaEngineNotify))
	{
		*ppvObj = (LPVOID)this;
		AddRef();
		return NOERROR;
	}
	return E_NOINTERFACE;
}

ULONG MediaEngineNotify::AddRef()
{
	InterlockedIncrement(&m_cRef);
	return m_cRef;
}
ULONG MediaEngineNotify::Release()
{
	// Decrement the object's internal counter.
	ULONG ulRefCount = InterlockedDecrement(&m_cRef);
	if (0 == m_cRef)
	{
		delete this;
	}
	return ulRefCount;
}


HRESULT MediaEngineNotify::EventNotify(DWORD event, DWORD_PTR param1, DWORD param2)
{
	switch (event)
	{
	case MF_MEDIA_ENGINE_EVENT_LOADSTART:
		mediaEngine->TriggerMediaStateChanged(MediaState::Loading);
		break;
	case MF_MEDIA_ENGINE_EVENT_PLAYING:
		mediaEngine->TriggerMediaStateChanged(MediaState::Playing);
		break;
	case MF_MEDIA_ENGINE_EVENT_ENDED:
		mediaEngine->TriggerMediaStateChanged(MediaState::Ended);
		break;
	case MF_MEDIA_ENGINE_EVENT_ERROR:
		mediaEngine->TriggerMediaStateChanged(MediaState::Error);
		break;
	}

	return S_OK;
}
