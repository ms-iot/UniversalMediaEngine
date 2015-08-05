#pragma once

namespace UniversalMediaEngine
{
	public enum class MediaState
	{
		Error,
		Ended,
		UnableToConnect,
		Stopped,
		Loading,
		Playing,
		Paused
	};
}