window.addEventListener('beforeunload', function(e) {
    // e.preventDefault();
    // e.returnValue = ''; // Some browsers require a return value to display the prompt
    // return ''; // For compatibility with older browsers
    logoff();
});
