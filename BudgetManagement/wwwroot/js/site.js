var scrollToTopButton = document.getElementById("scrollToTopBtn");

window.onscroll = function () { scrollFunction() };

function scrollFunction() {
    if (document.body.scrollTop > 500 || document.documentElement.scrollTop > 500) {
        scrollToTopButton.style.display = "block";
    } else {
        scrollToTopButton.style.display = "none";
    }
}

function scrollToTop() {
    smoothScrollToTop(600); // You can adjust the duration (in milliseconds)
}

function smoothScrollToTop(duration) {
    var start = window.scrollY;
    var startTime = performance.now();

    function scrollStep(timestamp) {
        var currentTime = timestamp || performance.now();
        var elapsedTime = currentTime - startTime;

        window.scrollTo(0, easeInOutCubic(elapsedTime, start, -start, duration));

        if (elapsedTime < duration) {
            window.requestAnimationFrame(scrollStep);
        }
    }

    function easeInOutCubic(t, b, c, d) {
        t /= d / 2;
        if (t < 1) return c / 2 * t * t * t + b;
        t -= 2;
        return c / 2 * (t * t * t + 2) + b;
    }

    window.requestAnimationFrame(scrollStep);
}