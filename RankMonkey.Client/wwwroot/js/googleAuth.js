let googleUser = null;

function waitForGoogle(callback, maxAttempts = 10, interval = 500) {
    let attempts = 0;
    const checkGoogle = function() {
        if (typeof google !== 'undefined' && google.accounts && google.accounts.id) {
            callback();
        } else if (attempts < maxAttempts) {
            attempts++;
            setTimeout(checkGoogle, interval);
        } else {
            console.error("Google API failed to load after " + maxAttempts + " attempts");
        }
    };

    checkGoogle();
}

function initializeGoogleSignIn(googleClientId) {
    return new Promise((resolve, reject) => {
        waitForGoogle(() => {
            console.log("Initializing Google Sign-In");
            google.accounts.id.initialize({
                client_id: googleClientId,
                callback: handleCredentialResponse,
                auto_select: false, // Disable auto-select
            });
            console.log("Google Sign-In initialized");
            resolve();
        });
    });
}

function googleSignIn() {
    return new Promise((resolve, reject) => {
        if (googleUser) {
            resolve(googleUser);
        } else {
            google.accounts.id.prompt((notification) => {
                if (notification.isNotDisplayed() || notification.isSkippedMoment()) {
                    console.log("Google Sign-In not displayed or skipped:", notification);
                    reject('Google Sign-In was not displayed or was skipped');
                } else if (notification.isDismissedMoment()) {
                    reject('Google Sign-In was dismissed');
                }
                // If we get here, the sign-in is in progress, but we don't have a way to know when it's done
                // The handleCredentialResponse function will be called separately when sign-in is complete
            });
        }
    });
}

function handleCredentialResponse(response) {
    console.log("Received Google Sign-In response", response);
    googleUser = response.credential;
    if (window.handleGoogleSignIn) {
        console.log("Calling Blazor handleGoogleSignIn");
        window.handleGoogleSignIn(googleUser);
    } else {
        console.error("window.handleGoogleSignIn is not defined");
    }
}

function renderGoogleSignInButton() {
    console.log("Attempting to render Google Sign-In button");
    const buttonElement = document.getElementById("googleSignInButton");
    if (!buttonElement) {
        console.log("Google Sign-In button element not found");
        return;
    }

    google.accounts.id.renderButton(
        buttonElement,
        {
            type: "standard",
            theme: "outline",
            size: "large",
            text: "signin_with", // This will show "Sign in with Google" text
            shape: "rectangular",
            logo_alignment: "left",
            width: 250 // Adjust as needed
        }
    );
    console.log("Google Sign-In button rendered");
}

function signOut() {
    google.accounts.id.disableAutoSelect();
    googleUser = null;
    console.log("User signed out");
}

// Don't initialize here, we'll do it from Blazor
console.log("Google Auth script loaded");