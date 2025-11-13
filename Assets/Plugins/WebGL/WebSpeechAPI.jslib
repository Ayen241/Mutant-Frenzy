mergeInto(LibraryManager.library, {
    StartVoiceRecognition: function(gameObjectNamePtr) {
        var gameObjectName = UTF8ToString(gameObjectNamePtr);
        
        if (!('webkitSpeechRecognition' in window) && !('SpeechRecognition' in window)) {
            console.error("Speech recognition not supported in this browser");
            SendMessage(gameObjectName, 'OnVoiceRecognitionError', 'not-supported');
            return;
        }

        var SpeechRecognition = window.SpeechRecognition || window.webkitSpeechRecognition;
        window.recognition = new SpeechRecognition();
        window.recognition.continuous = true;
        window.recognition.interimResults = true; // Enable interim results for faster response
        window.recognition.maxAlternatives = 3; // Get multiple alternatives
        window.recognition.lang = 'en-US';

        window.recognition.onstart = function() {
            console.log("Speech recognition started");
            SendMessage(gameObjectName, 'OnVoiceRecognitionStarted', '');
        };

        window.recognition.onresult = function(event) {
            var lastResult = event.results[event.results.length - 1];
            
            // Process final results
            if (lastResult.isFinal) {
                var transcript = lastResult[0].transcript.toLowerCase().trim();
                var confidence = lastResult[0].confidence;
                
                console.log("Final: " + transcript + " (confidence: " + confidence + ")");
                
                // Check alternatives for better matches
                var alternatives = [];
                for (var i = 0; i < Math.min(lastResult.length, 3); i++) {
                    alternatives.push(lastResult[i].transcript.toLowerCase().trim());
                }
                
                try {
                    SendMessage(gameObjectName, 'OnSpeechRecognized', transcript);
                    
                    // Send alternatives if different from main transcript
                    for (var j = 1; j < alternatives.length; j++) {
                        if (alternatives[j] !== transcript) {
                            SendMessage(gameObjectName, 'OnSpeechAlternative', alternatives[j]);
                        }
                    }
                } catch (e) {
                    console.error("Error sending message to Unity: " + e);
                }
            } else {
                // Process interim results for faster feedback
                var interimTranscript = lastResult[0].transcript.toLowerCase().trim();
                console.log("Interim: " + interimTranscript);
                
                try {
                    SendMessage(gameObjectName, 'OnInterimResult', interimTranscript);
                } catch (e) {
                    // Interim results are optional, don't log errors
                }
            }
        };

        window.recognition.onerror = function(event) {
            console.error("Speech recognition error: " + event.error);
            
            try {
                SendMessage(gameObjectName, 'OnVoiceRecognitionError', event.error);
            } catch (e) {
                console.error("Error sending error message: " + e);
            }
            
            // Automatically restart on common recoverable errors
            if (event.error === 'no-speech' || event.error === 'audio-capture' || event.error === 'aborted') {
                setTimeout(function() {
                    if (window.recognition) {
                        try {
                            window.recognition.start();
                        } catch (restartError) {
                            console.log("Recognition already restarted");
                        }
                    }
                }, 1000);
            }
        };

        window.recognition.onend = function() {
            console.log("Speech recognition ended, restarting...");
            
            try {
                SendMessage(gameObjectName, 'OnVoiceRecognitionEnded', '');
            } catch (e) {
                // Optional callback
            }
            
            // Automatically restart for continuous listening
            setTimeout(function() {
                if (window.recognition) {
                    try {
                        window.recognition.start();
                    } catch (e) {
                        console.log("Recognition already running");
                    }
                }
            }, 100);
        };

        try {
            window.recognition.start();
            console.log("Speech recognition initialized for " + gameObjectName);
        } catch (e) {
            console.error("Error starting recognition: " + e);
            SendMessage(gameObjectName, 'OnVoiceRecognitionError', 'start-failed');
        }
    },

    StopVoiceRecognition: function() {
        if (window.recognition) {
            window.recognition.stop();
            window.recognition = null;
            console.log("Speech recognition stopped");
        }
    },

    IsVoiceRecognitionSupported: function() {
        return ('webkitSpeechRecognition' in window) || ('SpeechRecognition' in window);
    }
});
