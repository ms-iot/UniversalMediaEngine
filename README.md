# UniversalMediaEngine
This is a IMediaEngine (Windows Media Foundation management class) wrapper that simplifies the playing of media 
in an Windows IoT Core headless applicaiton (since the XAML MediaElement is not avaliable to developers here).

Usage:
1. Either build/add the Windows Runtime Component as a binary reference to your solution of add the TextDisplayManager project to you solution.
2. Initialize an instance of the MediaEngine object in your code like so:

            this.mediaEngine = new MediaEngine();
            var result = await this.mediaEngine.InitializeAsync();
            if (result == MediaEngineInitializationResult.Fail)
            {
                // Your error logic
                
            }

3. The MediaEngine object exposes Play (you pass a valid URL), Pause and Volume set/get as well as a callback that is fired when the state of media playback changes.
 