#pragma once

using namespace Windows::Foundation;

// Prototype
class MediaEngineManager;

namespace UniversalMediaEngine
{
	public enum class MediaEngineInitializationResult
	{
		Success,
		Fail
	};

	public delegate void MediaStateChangedHandler(MediaState state);

	public ref class MediaEngine sealed
    {
    public:
		
		MediaEngine();

		IAsyncOperation<MediaEngineInitializationResult>^ InitializeAsync();

		void Play(Platform::String^ url);

		void Pause();

		property double Volume
		{
			double get();
			void set(double value);
		}

		event MediaStateChangedHandler^ MediaStateChanged;

	internal:

		void TriggerMediaStateChanged(MediaState state);

	private:
		ComPtr<MediaEngineManager> spMediaEngineManager;
    };
}
