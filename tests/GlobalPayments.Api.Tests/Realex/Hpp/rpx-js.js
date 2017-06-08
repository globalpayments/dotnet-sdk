/*jslint browser:true */
var RealexHpp = (function () {

    'use strict';

    var hppUrl = "https://pay.realexpayments.com/pay";

    var randomId = randomId || Math.random().toString(16).substr(2, 8);

    var setHppUrl = function (url) {
        hppUrl = url;
    };

    var isWindowsMobileOs = /Windows Phone|IEMobile/.test(navigator.userAgent);
    var isAndroidOrIOs = /Android|iPad|iPhone|iPod/.test(navigator.userAgent);
    var isMobileXS = ((((window.innerWidth > 0) ? window.innerWidth : screen.width) <= 360 ? true : false) || (((window.innerHeight > 0) ? window.innerHeight : screen.Height) <= 360 ? true : false));

    // Display IFrame on WIndows Phone OS mobile devices   
    var isMobileIFrame = isWindowsMobileOs;

    // For IOs/Android and small screen devices always open in new tab/window
    var isMobileNewTab = !isWindowsMobileOs && (isAndroidOrIOs || isMobileXS);
    var tabWindow;

    // Initialising some variables used throughout this file.
    var RxpLightbox = (function () {

        var instance;

        function init() {
            var overlayElement;
            var spinner;
            var iFrame;
            var closeButton;
            var token;

            function checkDevicesOrientation() {
                if (window.orientation === 90 || window.orientation === -90) {
                    return true;
                } else {
                    return false;
                }
            }

            var isLandscape = checkDevicesOrientation();

            if (isMobileIFrame) {
                if (window.addEventListener) {
                    window.addEventListener("orientationchange", function () {
                        isLandscape = checkDevicesOrientation();
                    }, false);
                }
            }

            // Initialising some variables used throughout this function.
            function createOverlay() {
                var overlay = document.createElement("div");
                overlay.setAttribute("id", "rxp-overlay-" + randomId);
                overlay.style.position = "fixed";
                overlay.style.width = "100%";
                overlay.style.height = "100%";
                overlay.style.top = "0";
                overlay.style.left = "0";
                overlay.style.transition = "all 0.3s ease-in-out";
                overlay.style.zIndex = "100";

                if (isMobileIFrame) {
                    overlay.style.position = "absolute !important";
                    overlay.style.WebkitOverflowScrolling = "touch";
                    overlay.style.overflowX = "hidden";
                    overlay.style.overflowY = "scroll";
                }

                document.body.appendChild(overlay);

                setTimeout(function () {
                    overlay.style.background = "rgba(0, 0, 0, 0.7)";
                }, 1);

                overlayElement = overlay;
            }


            function createCloseButton() {
                if (document.getElementById("rxp-frame-close-" + randomId) === null) {
                    closeButton = document.createElement("img");
                    closeButton.setAttribute("id", "rxp-frame-close-" + randomId);
                    closeButton.setAttribute("src", "data:image/gif;base64,iVBORw0KGgoAAAANSUhEUgAAABEAAAARCAYAAAA7bUf6AAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAAyJpVFh0WE1MOmNvbS5hZG9iZS54bXAAAAAAADw/eHBhY2tldCBiZWdpbj0i77u/IiBpZD0iVzVNME1wQ2VoaUh6cmVTek5UY3prYzlkIj8+IDx4OnhtcG1ldGEgeG1sbnM6eD0iYWRvYmU6bnM6bWV0YS8iIHg6eG1wdGs9IkFkb2JlIFhNUCBDb3JlIDUuMy1jMDExIDY2LjE0NTY2MSwgMjAxMi8wMi8wNi0xNDo1NjoyNyAgICAgICAgIj4gPHJkZjpSREYgeG1sbnM6cmRmPSJodHRwOi8vd3d3LnczLm9yZy8xOTk5LzAyLzIyLXJkZi1zeW50YXgtbnMjIj4gPHJkZjpEZXNjcmlwdGlvbiByZGY6YWJvdXQ9IiIgeG1sbnM6eG1wPSJodHRwOi8vbnMuYWRvYmUuY29tL3hhcC8xLjAvIiB4bWxuczp4bXBNTT0iaHR0cDovL25zLmFkb2JlLmNvbS94YXAvMS4wL21tLyIgeG1sbnM6c3RSZWY9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC9zVHlwZS9SZXNvdXJjZVJlZiMiIHhtcDpDcmVhdG9yVG9vbD0iQWRvYmUgUGhvdG9zaG9wIENTNiAoV2luZG93cykiIHhtcE1NOkluc3RhbmNlSUQ9InhtcC5paWQ6QUJFRjU1MEIzMUQ3MTFFNThGQjNERjg2NEZCRjFDOTUiIHhtcE1NOkRvY3VtZW50SUQ9InhtcC5kaWQ6QUJFRjU1MEMzMUQ3MTFFNThGQjNERjg2NEZCRjFDOTUiPiA8eG1wTU06RGVyaXZlZEZyb20gc3RSZWY6aW5zdGFuY2VJRD0ieG1wLmlpZDpBQkVGNTUwOTMxRDcxMUU1OEZCM0RGODY0RkJGMUM5NSIgc3RSZWY6ZG9jdW1lbnRJRD0ieG1wLmRpZDpBQkVGNTUwQTMxRDcxMUU1OEZCM0RGODY0RkJGMUM5NSIvPiA8L3JkZjpEZXNjcmlwdGlvbj4gPC9yZGY6UkRGPiA8L3g6eG1wbWV0YT4gPD94cGFja2V0IGVuZD0iciI/PlHco5QAAAHpSURBVHjafFRdTsJAEF42JaTKn4glGIg++qgX4AAchHAJkiZcwnAQD8AF4NFHCaC2VgWkIQQsfl/jNJUik8Duzs/XmW9mN7Xb7VRc5vP5zWKxaK5Wq8Zmu72FqobfJG0YQ9M0+/l8/qFQKDzGY1JxENd1288vLy1s786KRZXJZCLber1Wn7MZt4PLarVnWdZ9AmQ8Hncc17UvymVdBMB/MgPQm+cFFcuy6/V6lzqDf57ntWGwYdBIVx0TfkBD6I9M35iRJgfIoAVjBLDZbA4CiJ5+9AdQi/EahibqDTkQx6fRSIHcPwA8Uy9A9Gcc47Xv+w2wzhRDYzqdVihLIbsIiCvP1NNOoX/29FQx3vgOgtt4FyRdCgPRarX4+goB9vkyAMh443cOEsIAAcjncuoI4TXWMAmCIGFhCQLAdZ8jym/cRJ+Y5nC5XCYAhINKpZLgSISZgoqh5iiLQrojAFICVwGS7tCfe5DbZzkP56XS4NVxwvTI/vXVVYIDnqmnnX70ZxzjNS8THHooK5hMpxHQIREA+tEfA9djfHR3MHkdx3Hspe9r3B+VzWaj2RESyR2mlCUE4MoGQDdxiwHURq2t94+PO9bMIYyTyDNLwMoM7g8+BfKeYGniyw2MdfSehF3Qmk1IvCc/AgwAaS86Etp38bUAAAAASUVORK5CYII=");
                    closeButton.setAttribute("style", "transition: all 0.5s ease-in-out; opacity: 0; float: left; position: absolute; left: 50%; margin-left: 173px; z-index: 99999999; top: 30px;");

                    setTimeout(function () {
                        closeButton.style.opacity = "1";
                    }, 500);

                    if (isMobileIFrame) {
                        closeButton.style.position = "absolute";
                        closeButton.style.float = "right";
                        closeButton.style.top = "20px";
                        closeButton.style.left = "initial";
                        closeButton.style.marginLeft = "0px";
                        closeButton.style.right = "20px";
                    }

                    closeButton.addEventListener("click", closeModal, true);
                    overlayElement.appendChild(closeButton);
                }
            }

            function createForm(doc) {
                var form = document.createElement("form");
                form.setAttribute("method", "POST");
                form.setAttribute("action", hppUrl);

                for (var key in token) {

                    var hiddenField = document.createElement("input");
                    hiddenField.setAttribute("type", "hidden");
                    hiddenField.setAttribute("name", key);
                    hiddenField.setAttribute("value", token[key]);

                    form.appendChild(hiddenField);
                }

                var hppTemplateType = document.createElement("input");
                hppTemplateType.setAttribute("type", "hidden");
                hppTemplateType.setAttribute("name", "HPP_TEMPLATE_TYPE");
                hppTemplateType.setAttribute("value", "LIGHTBOX");

                form.appendChild(hppTemplateType);

                var parser = document.createElement('a');
                parser.href = window.location.href;
                var hppOriginParam = parser.protocol + '//' + parser.host;

                var hppOrigin = document.createElement("input");
                hppOrigin.setAttribute("type", "hidden");
                hppOrigin.setAttribute("name", "HPP_ORIGIN");
                hppOrigin.setAttribute("value", hppOriginParam);

                form.appendChild(hppOrigin);
                return form;
            }

            function createIFrame() {

                //Create the spinner
                spinner = document.createElement("img");
                spinner.setAttribute("src", "data:image/gif;base64,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");
                spinner.setAttribute("id", "rxp-loader-" + randomId);
                spinner.style.left = "50%";
                spinner.style.position = "fixed";
                spinner.style.background = "#FFFFFF";
                spinner.style.borderRadius = "50%";
                spinner.style.width = "30px";
                spinner.style.marginLeft = "-15px";
                spinner.style.zIndex = "200";
                spinner.style.marginLeft = "-15px";
                spinner.style.top = "120px";

                document.body.appendChild(spinner);

                //Create the iframe
                iFrame = document.createElement("iframe");
                iFrame.setAttribute("name", "rxp-frame-" + randomId);
                iFrame.setAttribute("id", "rxp-frame-" + randomId);
                iFrame.setAttribute("height", "85%");
                iFrame.setAttribute("frameBorder", "0");
                iFrame.setAttribute("width", "360px");
                iFrame.setAttribute("seamless", "seamless");

                iFrame.style.zIndex = "10001";
                iFrame.style.position = "absolute";
                iFrame.style.transition = "transform 0.5s ease-in-out";
                iFrame.style.transform = "scale(0.7)";
                iFrame.style.opacity = "0";

                overlayElement.appendChild(iFrame);

                if (isMobileIFrame) {
                    iFrame.style.top = "0px";
                    iFrame.style.bottom = "0px";
                    iFrame.style.left = "0px";
                    iFrame.style.marginLeft = "0px;";
                    iFrame.style.width = "100%";
                    iFrame.style.height = "100%";
                    iFrame.style.minHeight = "100%";
                    iFrame.style.WebkitTransform = "translate3d(0,0,0)";
                    iFrame.style.transform = "translate3d(0, 0, 0)";
                    var metaTag = document.createElement('meta');
                    metaTag.name = "viewport";
                    metaTag.content = "width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=0";
                    document.getElementsByTagName('head')[0].appendChild(metaTag);
                } else {
                    iFrame.style.top = "40px";
                    iFrame.style.left = "50%";
                    iFrame.style.marginLeft = "-180px";
                }

                iFrame.onload = function () {
                    iFrame.style.opacity = "1";
                    iFrame.style.transform = "scale(1)";

                    if (spinner.parentNode) {
                        spinner.parentNode.removeChild(spinner);
                    }
                    createCloseButton();
                };

                var form = createForm(document);
                if (iFrame.contentWindow.document.body) {
                    iFrame.contentWindow.document.body.appendChild(form);
                } else {
                    iFrame.contentWindow.document.appendChild(form);
                }

                form.submit();
            }

            function openWindow() {

                //open new window
                tabWindow = window.open();
                var doc = tabWindow.document;

                //add meta tag to new window (needed for iOS 8 bug)
                var meta = doc.createElement("meta");
                var name = doc.createAttribute("name");
                name.value = "viewport";
                meta.setAttributeNode(name);
                var content = doc.createAttribute("content");
                content.value = "width=device-width";
                meta.setAttributeNode(content);
                doc.head.appendChild(meta);

                //create form, append to new window and submit
                var form = createForm(doc);
                doc.body.appendChild(form);
                form.submit();
            }

            function closeModal() {

                if (closeButton.parentNode) {
                    closeButton.parentNode.removeChild(closeButton);
                }

                if (iFrame.parentNode) {
                    iFrame.parentNode.removeChild(iFrame);
                }

                if (spinner.parentNode) {
                    spinner.parentNode.removeChild(spinner);
                }

                overlayElement.className = "";
                setTimeout(function () {
                    if (overlayElement.parentNode) {
                        overlayElement.parentNode.removeChild(overlayElement);
                    }
                }, 300);
            }

            return {
                lightbox: function () {

                    if (isMobileNewTab) {
                        openWindow();
                    } else {
                        createOverlay();
                        createIFrame();
                    }
                },
                close: function () {
                    closeModal();
                },
                setToken: function (hppToken) {
                    token = hppToken;
                }
            };
        }

        return {
            // Get the Singleton instance if one exists
            // or create one if it doesn't
            getInstance: function (hppToken) {

                if (!instance) {
                    instance = init();
                }

                //Set the hpp token
                instance.setToken(hppToken);

                return instance;
            }
        };

    })();


    var init = function (idOfLightboxButton, merchantUrl, serverSdkJson) {

        //Get the lightbox instance (it's a singleton) and set the sdk json
        var lightboxInstance = RxpLightbox.getInstance(serverSdkJson);

        // Sets the event listener on the PAY button. The click will invoke the lightbox method
        if (document.getElementById(idOfLightboxButton).addEventListener) {
            document.getElementById(idOfLightboxButton).addEventListener("click", lightboxInstance.lightbox, true);
        } else {
            document.getElementById(idOfLightboxButton).attachEvent('onclick', lightboxInstance.lightbox);
        }

        function receiveMessage(event) {

            //Check the origin of the response comes from HPP
            if (getHostnameFromUrl(event.origin) === getHostnameFromUrl(hppUrl)) {

                // check for iframe resize values 
                if (event.data && JSON.parse(event.data).iframe) {
                    if (!isMobileNewTab) {
                        var iframeWidth = JSON.parse(event.data).iframe.width;
                        var iframeHeight = JSON.parse(event.data).iframe.height;

                        var iFrame = document.getElementById("rxp-frame-" + randomId);
                        iFrame.setAttribute("width", iframeWidth);
                        iFrame.setAttribute("height", iframeHeight);
                        iFrame.style.backgroundColor = "#ffffff";

                        if (isMobileIFrame) {
                            var overlay = document.getElementById("rxp-overlay-" + randomId);
                            iFrame.style.marginLeft = "0px";
                            iFrame.style.WebkitOverflowScrolling = "touch";
                            iFrame.style.overflowX = "scroll";
                            iFrame.style.overflowY = "scroll";
                            overlay.style.overflowX = "scroll";
                            overlay.style.overflowY = "scroll";

                        } else {
                            iFrame.style.marginLeft = (parseInt(iframeWidth.replace("px", ""), 10) / 2 * -1) + "px";
                        }

                        var closeButton = document.getElementById("rxp-frame-close-" + randomId);
                        closeButton.style.marginLeft = ((parseInt(iframeWidth.replace("px", ""), 10) / 2) - 7) + "px";
                    }

                } else {

                    if (isMobileNewTab) {
                        //Close the new window
                        if (tabWindow) {
                            tabWindow.close();
                        }
                    } else {
                        //Close the lightbox
                        lightboxInstance.close();
                    }

                    var response = event.data;

                    //Create a form and submit the hpp response to the merchant's response url
                    var form = document.createElement("form");
                    form.setAttribute("method", "POST");
                    form.setAttribute("action", merchantUrl);

                    var hiddenField = document.createElement("input");
                    hiddenField.setAttribute("type", "hidden");
                    hiddenField.setAttribute("name", "hppResponse");
                    hiddenField.setAttribute("value", response);

                    form.appendChild(hiddenField);

                    document.body.appendChild(form);

                    form.submit();
                }
            }
        }

        if (window.addEventListener) {
            window.addEventListener("message", receiveMessage, false);
        } else {
            window.attachEvent('message', receiveMessage);
        }

    };

    function getHostnameFromUrl(url) {
        var parser = document.createElement('a');
        parser.href = url;
        return parser.hostname;
    }

    return {
        init: init,
        setHppUrl: setHppUrl
    };

}());