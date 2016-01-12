using System;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System.Threading.Tasks;
using Microsoft.Maker.Media.UniversalMediaEngine;
using Windows.Storage;

namespace UniversalMediaEngine.Tests
{
    [TestClass]
    public class MediaEngineTests
    {
        [TestMethod]
        public async Task TestInitializeSuceeds()
        {
            // Arrange
            var engineUnderTest = new MediaEngine();

            // Act
            var result = await engineUnderTest.InitializeAsync();

            // Assert
            Assert.IsTrue(MediaEngineInitializationResult.Success == result);
        }

        [TestMethod]
        public async Task TestPlayWithInitializeSucceeds()
        {
            // Arrange
            var engineUnderTest = new MediaEngine();
            bool callbackOccurred = false;
            uint callbackTimeout = 10;

            // Act
            Assert.IsTrue(MediaEngineInitializationResult.Success == await engineUnderTest.InitializeAsync()
                , "Failed to initialize MediaEngine");

            engineUnderTest.MediaStateChanged += ((MediaState newState) => 
            {
                if (MediaState.Playing == newState)
                    callbackOccurred = true;
            });

            try
            {
                engineUnderTest.Play("ms-appx:///Assets/TestSilence.wav");

                // Wait for the callback to be invoked until the timeout expires
                uint counter = 0;
                while (!callbackOccurred)
                {
                    Assert.IsFalse(counter == callbackTimeout,
                        "Timed out waiting for callback from mediaengine");

                    await Task.Delay(100);

                    counter++;
                }
            }
            catch (Exception e)
            {
                Assert.Fail("An exception was thrown during execution of the test: " + e.Message);
            }

            // Assert
            Assert.IsTrue(callbackOccurred);
        }

        [TestMethod]
        public void TestPlayWithoutInitializeFails()
        {
            // Arrange
            var engineUnderTest = new MediaEngine();
            bool exceptionThrown = false;

            // Act
            try
            {
                engineUnderTest.Play("");
            }
            catch(Exception e)
            {
                exceptionThrown = true;
            }


            // Assert
            Assert.IsTrue(exceptionThrown);
                
        }

        [TestMethod]
        public async Task TestPlayWithInvalidUrlFails()
        {
            // Arrange
            var engineUnderTest = new MediaEngine();
            bool callbackOccurred = false;
            uint callbackTimeout = 10;

            // Act
            Assert.IsTrue(MediaEngineInitializationResult.Success == await engineUnderTest.InitializeAsync()
                , "Failed to initialize MediaEngine");

            engineUnderTest.MediaStateChanged += ((MediaState newState) =>
            {
                // We're looking for the error state as this should be returns for
                // an invalid URL
                if (MediaState.Error == newState)
                    callbackOccurred = true;
            });

            try
            {
                engineUnderTest.Play("");

                // Wait for the callback to be invoked until the timeout expires
                uint counter = 0;
                while (!callbackOccurred)
                {
                    Assert.IsFalse(counter == callbackTimeout, 
                        "Timed out waiting for callback from mediaengine");

                    await Task.Delay(100);

                    counter++;
                }
            }
            catch (Exception e)
            {
                Assert.Fail("An exception was thrown during execution of the test: " + e.Message);
            }

            // Assert

            // The callback should be call within the timeout period
            Assert.IsTrue(callbackOccurred);
        }

        [TestMethod]
        public async Task TestPlayStreamWithInitializeSucceeds()
        {
            // Arrange
            var engineUnderTest = new MediaEngine();
            bool callbackOccurred = false;
            uint callbackTimeout = 10;
            StorageFile testFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/TestSilence.wav"));

            // Act
            Assert.IsTrue(MediaEngineInitializationResult.Success == await engineUnderTest.InitializeAsync()
                , "Failed to initialize MediaEngine");

            engineUnderTest.MediaStateChanged += ((MediaState newState) =>
            {
                if (MediaState.Playing == newState)
                    callbackOccurred = true;
            });

            try
            {         
                // Open the file as a byte stream
                engineUnderTest.PlayStream(await testFile.OpenReadAsync());

                // Wait for the callback to be invoked until the timeout expires
                uint counter = 0;
                while (!callbackOccurred)
                {
                    Assert.IsFalse(counter == callbackTimeout,
                        "Timed out waiting for callback from mediaengine");

                    await Task.Delay(100);

                    counter++;
                }
            }
            catch (Exception e)
            {
                Assert.Fail("An exception was thrown during execution of the test: " + e.Message);
            }

            // Assert

            // The callback should be call within the timeout period
            Assert.IsTrue(callbackOccurred);
        }

        [TestMethod]
        public async Task TestPlayStreamWithInvalidStreamFails()
        {
            // Arrange
            var engineUnderTest = new MediaEngine();
            bool exceptionThrown = false;

            // Act
            Assert.IsTrue(MediaEngineInitializationResult.Success == await engineUnderTest.InitializeAsync()
                , "Failed to initialize MediaEngine");

            try
            {
                engineUnderTest.PlayStream(null);
            }
            catch (Exception e)
            {
                exceptionThrown = true;
            }

            // Assert

            // An exception should be thrown
            Assert.IsTrue(exceptionThrown);
        }

        [TestMethod]
        public void TestPlayStreamWithoutInitializeFails()
        {
            // Arrange
            var engineUnderTest = new MediaEngine();
            bool exceptionThrown = false;

            // Act
            try
            {
                engineUnderTest.PlayStream(null);
            }
            catch (Exception e)
            {
                exceptionThrown = true;
            }

            // Assert
            Assert.IsTrue(exceptionThrown);

        }

        [TestMethod]
        public async Task TestPauseAndUnPauseSucceeds()
        {
            // Arrange
            var engineUnderTest = new MediaEngine();
            bool firstPlayingCallbackOccurred = false;
            bool pausedCallbackOccurred = false;
            bool secondPlayingCallbackOccurred = false;
            uint callbackCount = 0;
            uint callbackTimeout = 10;

            // Act
            var result = await engineUnderTest.InitializeAsync();

            engineUnderTest.MediaStateChanged += ((MediaState newState) =>
            {
                switch(callbackCount)
                {
                    case 0:
                            if (MediaState.Playing == newState)
                            {
                                firstPlayingCallbackOccurred = true;
                                callbackCount++;
                            }
                            break;
                    case 1:
                            if (MediaState.Paused == newState)
                            {
                                pausedCallbackOccurred = true;
                                callbackCount++;
                            }
                            break;
                    case 2:
                            if (MediaState.Playing == newState)
                            {
                                secondPlayingCallbackOccurred = true;
                                callbackCount++;
                            }
                            break;
                }
            });

            Assert.IsTrue(MediaEngineInitializationResult.Success == result);

            try
            {
                engineUnderTest.Play("ms-appx:///Assets/TestSilence.wav");

                uint counter = 0;
                while (0 == callbackCount)
                {
                    Assert.IsFalse(counter == callbackTimeout,
                        "Timed out waiting for callback from mediaengine");

                    await Task.Delay(100);

                    counter++;
                }

                engineUnderTest.Pause();

                counter = 0;
                while (1 == callbackCount)
                {
                    Assert.IsFalse(counter == callbackTimeout,
                        "Timed out waiting for callback from mediaengine");

                    await Task.Delay(100);

                    counter++;
                }

                engineUnderTest.Pause();

                counter = 0;
                while (2 == callbackCount)
                {
                    Assert.IsFalse(counter == callbackTimeout,
                        "Timed out waiting for callback from mediaengine");

                    await Task.Delay(100);

                    counter++;
                }

            }
            catch (Exception e)
            {
                Assert.Fail("An exception was thrown during execution of the test: " + e.Message);
            }

            Assert.IsTrue(firstPlayingCallbackOccurred, "Track did not start playing");
            Assert.IsTrue(pausedCallbackOccurred, "Track did not pause");
            Assert.IsTrue(secondPlayingCallbackOccurred, "Track did not resume");
        }

        [TestMethod]
        public void TestPauseWithoutInitializeFails()
        {
            // Arrange
            var engineUnderTest = new MediaEngine();
            bool exceptionThrown = false;

            // Act
            try
            {
                engineUnderTest.Pause();
            }
            catch (Exception e)
            {
                exceptionThrown = true;
            }

            // Assert

            // An exception should be thrown
            Assert.IsTrue(exceptionThrown);

        }

        [TestMethod]
        public async Task TestStopStreamWithInitializeSucceeds()
        {
            // Arrange
            var engineUnderTest = new MediaEngine();
            bool playingCallbackOccurred = false;
            bool stoppedCallbackOccurred = false;
            uint callbackCount = 0;
            uint callbackTimeout = 10;

            // Act
            Assert.IsTrue(MediaEngineInitializationResult.Success == await engineUnderTest.InitializeAsync()
                , "Failed to initialize MediaEngine");

            engineUnderTest.MediaStateChanged += ((MediaState newState) =>
            {
                switch (callbackCount)
                {
                    case 0:
                        if (MediaState.Playing == newState)
                        {
                            playingCallbackOccurred = true;
                            callbackCount++;
                        }
                        break;
                    case 1:
                        if (MediaState.Stopped == newState)
                        {
                            stoppedCallbackOccurred = true;
                            callbackCount++;
                        }
                        break;
                }
            });

            try
            {
                // Open the file as a byte stream
                engineUnderTest.Play("ms-appx:///Assets/TestSilence.wav");

                // Wait for the callback to be invoked until the timeout expires
                uint counter = 0;
                while (0 == callbackCount)
                {
                    Assert.IsFalse(counter == callbackTimeout,
                        "Timed out waiting for callback from mediaengine");

                    await Task.Delay(100);

                    counter++;
                }

                engineUnderTest.Stop();

                counter = 0;
                while (1 == callbackCount)
                {
                    Assert.IsFalse(counter == callbackTimeout,
                        "Timed out waiting for callback from mediaengine");

                    await Task.Delay(100);

                    counter++;
                }
            }
            catch (Exception e)
            {
                Assert.Fail("An exception was thrown during execution of the test: " + e.Message);
            }

            // Assert
            Assert.IsTrue(playingCallbackOccurred, "Track did not start playing");
            Assert.IsTrue(stoppedCallbackOccurred, "Track did not stop");
        }

        [TestMethod]
        public void TestStopWithoutInitializeFails()
        {
            // Arrange
            var engineUnderTest = new MediaEngine();
            bool exceptionThrown = false;

            // Act
            try
            {
                engineUnderTest.Stop();
            }
            catch (Exception e)
            {
                exceptionThrown = true;
            }

            // Assert

            // An exception should be thrown
            Assert.IsTrue(exceptionThrown);

        }

        [TestMethod]
        public async Task TestVolumeGetandChangeSucceeds()
        {
            // Arrange
            var engineUnderTest = new MediaEngine();
            double volume = 0;

            // Act
            Assert.IsTrue(MediaEngineInitializationResult.Success == await engineUnderTest.InitializeAsync()
                , "Failed to initialize MediaEngine");

            try
            {
                // get the current volume
                volume = engineUnderTest.Volume;

                // decrease the volume by 0.1
                engineUnderTest.Volume -= 0.1;
            }
            catch (Exception e)
            {
                Assert.Fail("An exception was thrown during execution of the test: " + e.Message);
            }

            // Assert

            // Check that the current engine volume has been changed
            Assert.IsTrue((volume -= 0.1) == engineUnderTest.Volume);
        }

        [TestMethod]
        public void TestVolumeGetWithoutInitializeFails()
        {
            // Arrange
            var engineUnderTest = new MediaEngine();
            bool exceptionThrown = false;

            // Act
            try
            {
                var vol = engineUnderTest.Volume;
            }
            catch (Exception e)
            {
                exceptionThrown = true;
            }

            // Assert

            // An exception should be thrown
            Assert.IsTrue(exceptionThrown);

        }

        [TestMethod]
        public void TestVolumeSetWithoutInitializeFails()
        {
            // Arrange
            var engineUnderTest = new MediaEngine();
            bool exceptionThrown = false;

            // Act
            try
            {
                engineUnderTest.Volume = 1;
            }
            catch (Exception e)
            {
                exceptionThrown = true;
            }

            // Assert

            // An exception should be thrown
            Assert.IsTrue(exceptionThrown);

        }
    }
}
