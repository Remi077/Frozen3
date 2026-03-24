mergeInto(LibraryManager.library, {
    RequestFullscreen: function () {
        var el = document.documentElement;
        if (el.requestFullscreen) el.requestFullscreen();
        else if (el.webkitRequestFullscreen) el.webkitRequestFullscreen();
    },
    IsFullscreen: function () {
        return !!(document.fullscreenElement || document.webkitFullscreenElement);
    }
});
