    $(document).ready(function () {

        var jcrop_reference, jcrop_api;
        var pasteCatcher;
        var originalHeight, originalWidth;
        
        // We start by checking if the browser supports the 
        // Clipboard object. If not, we need to create a 
        // contenteditable element that catches all pasted data 

        var listener = function () {
            try {
                if (BrowserDetect.browser == "Chrome")
                {
                    pasteCatcher.style.visibility = "hidden";
                    pasteCatcher.style.opacity = 0;
                }
                
                pasteCatcher.focus();
                
                pasteCatcher.blur();
                $('.ui-dialog').focus();
            } catch (e) { }
        };

        if (!window.Clipboard) {
            pasteCatcher = document.createElement("div");
            pasteCatcher.style.visibility = "hidden";
            // Firefox allows images to be pasted into contenteditable elements
            pasteCatcher.setAttribute("contenteditable", "");

            // We can hide the element and append it to the body,
            pasteCatcher.style.opacity = 0;
            document.body.appendChild(pasteCatcher);
            pasteCatcher.style.visibility = "initial";
            // as long as we make sure it is always in focus
            //pasteCatcher.focus();
            //document.getElementById("dialog-paste-imageID").addEventListener("click", listener);
        }
        //Add the paste event listener
        window.addEventListener("paste", pasteHandler);


        /* Handle paste events */
        function pasteHandler(e) {
            // We need to check if event.clipboardData is supported (Chrome)
            if (e.clipboardData && e.clipboardData.mozItemCount == undefined) {
                // Get the items from the clipboard
                var items = e.clipboardData.items;
                if (items) {
                    // Loop through all items, looking for any kind of image
                    for (var i = 0; i < items.length; i++) {
                        if (items[i].type.indexOf("image") !== -1) {
                            // We need to represent the image as a file,
                            var blob = items[i].getAsFile();
                            // and use a URL or webkitURL (whichever is available to the browser)
                            // to create a temporary URL to the object
                            var URLObj = window.URL || window.webkitURL;
                            var source = URLObj.createObjectURL(blob);

                            var reader = new FileReader();
                            reader.onload = function (event) {
                                createImage(event.target.result); //event.target.results contains the base64 code to create the image.
                            };
                            reader.readAsDataURL(blob);//Convert the blob from clipboard to base64
                        }
                    }
                }
                // If we can't handle clipboard data directly (Firefox), 
                // we need to read what was pasted from the contenteditable element
            } else {
                // This is a cheap trick to make sure we read the data
                // AFTER it has been inserted.
                setTimeout(checkInput, 1);
            }
        }

        /* Parse the input in the paste catcher element */
        function checkInput() {
            // Store the pasted content in a variable
            var child = pasteCatcher.childNodes[0];

            // Clear the inner html to make sure we're always
            // getting the latest inserted content
            pasteCatcher.innerHTML = "";

            if (child) {
                // If the user pastes an image, the src attribute
                // will represent the image as a base64 encoded string.
                if (child.tagName === "IMG") {
                    createImage(child.src);
                }
            }
        }

        /* Creates a new image from a given source */
        function createImage(source) {
            $('.paste-dialog-instructions').hide();
            $('#testImage').attr('src', source);
            $('#img1').val(source);
            $('#testImage').css('opacity', '1');
            $('#imageOptions').show();
            document.getElementById("dialog-paste-imageID").removeEventListener("click", listener);
        }

        function linkUp(unusedIndex, container) {
                container = $(container); //We were passed a DOM reference, convert it to a jquery object

                //The code will look for a img.image, a div.preview, a.result, an input.result inside the specified container, and link them together.
                //Only 'img.image' is required, however.  

                //Find the original image
                var image = $(this).find(".image");

                //Find the original aspect ratio of the image
                var originalRatio = 1// image.width() / image.height()

                //Defaults to false if a checkbox with class="keepAspectRatio" doesn't exist.
                var keepRatio = false;// container.find('.keepAspectRatio').size() < 1 ? false : container.find('.keepAspectRatio').is(':checked');

                //jCrop will enforce this ratio:
                var forcedRatio = keepRatio ? originalRatio : null;

                //Find the URL of the original image minus the querystring.
                var path = image.attr('src');
                if (path.indexOf('?') > 0) path = path.substr(0, path.indexOf('?'));
                if (path.indexOf(';') > 0) path = path.substr(0, path.indexOf(';')); //For parsing Amazon-cloudfront compatible querystrings

                var cloudFront = image.attr('src').indexOf(';') > -1; //To use CloudFront-friendly URLs.

                //Find the preview div(s) (if they exist) and make sure the have a set height and width.
                var divPreview = container.find("div.preview");

                //What size to make the preview window (defaults to existing width/height if specified in 'style' attribute)
                var previewFallbackWidth = 300;
                var previewFallbackHeight = 300;

                //Allow the div to override the default width and height in the style attribute
                var previewMaxWidth = (divPreview.attr('style') != null && divPreview.attr('style').indexOf('width') > -1) ? divPreview.width() : previewFallbackWidth;
                var previewMaxHeight = (divPreview.attr('style') != null && divPreview.attr('style').indexOf('height') > -1) ? divPreview.height() : previewFallbackHeight;
                //Set the values explicitly.
                divPreview.css({
                    width: previewMaxWidth + 'px',
                    height: previewMaxHeight + 'px',
                    overflow: 'hidden'
                });

                //Create another child div and style it to form a 'clipping rectangle' for the preview div.
                var innerPreview = $('<div />').css({
                    overflow: 'hidden'
                }).addClass('innerPreview').appendTo(divPreview);

                //Create a copy of the image inside the inner preview div(s)
                $('<img />').attr('src', image.attr('src')).appendTo(innerPreview);

                //Find any links (if they exist)
                var links = container.find('a.result');
                //And any input fields (for postbacks, if desired)
                var inputs = container.find('input.result');


                //Create a function to update the link, hidden input, and preview pane
                var update = function (coords) {
                    if (parseInt(coords.w) <= 0 || parseInt(coords.h) <= 0) return; //Require valid width and height

                    //The aspect ratio of the cropping rectangle. If 'keepRatio', use originalRatio since it's more precise.
                    var cropRatio = keepRatio ? originalRatio : (coords.w / coords.h);


                    //When the selection aspect ratio changes, the preview clipping area has to also.
                    //Calculate the width and height.

                    var innerWidth = cropRatio >= (previewMaxWidth / previewMaxHeight) ? previewMaxWidth : previewMaxHeight * cropRatio;
                    var innerHeight = cropRatio < (previewMaxWidth / previewMaxHeight) ? previewMaxHeight : previewMaxWidth / cropRatio;


                    innerPreview.css({
                        width: Math.ceil(innerWidth) + 'px',
                        height: Math.ceil(innerHeight) + 'px',
                        marginTop: (previewMaxHeight - innerHeight) / 2 + 'px',
                        marginLeft: (previewMaxWidth - innerWidth) / 2 + 'px',
                        overflow: 'hidden'
                    });
                    //Set the outer div's padding so it stays centered
                    divPreview.css({

                    });



                    //Calculate how much we are shrinking the image inside the preview window
                    var scalex = innerWidth / coords.w;
                    var scaley = innerHeight / coords.h;

                    //Set the width and height of the image so the right areas appear at the right scale appear.
                    innerPreview.find('img').css({
                        width: Math.round(scalex * image.width()) + 'px',
                        height: Math.round(scaley * image.height()) + 'px',
                        marginLeft: '-' + Math.round(scalex * coords.x) + 'px',
                        marginTop: '-' + Math.round(scaley * coords.y) + 'px'
                    });



                    //Calculate the querystring
                    var query = '?';

                    //Add final size, if specified.
                    query += 'maxwidth=' + (coords.x2 - coords.x) + '&';
                    query += 'maxheight=' + (coords.y2 - coords.y) + '&';

                    //Add crop rectangle
                    query += 'crop=(' + coords.x + ',' + coords.y + ',' + coords.x2 + ',' + coords.y2 + ')&cropxunits=' + image.width() + '&cropyunits=' + image.height()
                    //Replace ? and & with ; if using Amazon Cloudfront
                    if (cloudFront) query = query.replace(/\?\&/g, ';');

                    //Now, update the links and input values.
                    links.attr('href', path + query);
                    inputs.attr('value', path + query);

                }

                //Start up jCrop
                jcrop_reference = $.Jcrop(image);
                jcrop_reference.setOptions({
                    onChange: update,
                    onSelect: update,
                    setSelect: [0, 0, 50, 50],
                    aspectRatio: forcedRatio,
                    bgOpacity: 0.6
                }, function () {
                    jcrop_api = this;
                });

                //Call the function to init the preview windows
                update({ x: 0, y: 0, x2: 0, y2: 0, w: 50, h: 50 });

                //Handle the 'lock ratio' checkbox change vent
                container.find('.keepAspectRatio').change(function (e) {
                    //Update keepRatio value
                    keepRatio = this.checked;

                    //Update the forcedRatio value
                    forcedRatio = keepRatio ? originalRatio : null;

                    //Update the jcrop settings
                    jcrop_reference.setOptions({ aspectRatio: forcedRatio });
                    jcrop_reference.focus();
                });

        }

        window.cleardialog = function ()
        {
            try {
                jcrop_reference.destroy();
            } catch (e) { }
            $('#img1').val('');
            $('#testImage').css('opacity', '0');
            $('#imageOptions').hide();
            $('#resizeOptions').hide();
            $('#testImage').attr('src', 'data:image/gif;base64,R0lGODlhAQABAIAAAAAAAP///ywAAAAAAQABAAACAUwAOw==');
            $('#testImage').removeAttr('width');
            $('#testImage').removeAttr('height');
            $('#cropButton').removeAttr('disabled');
            $('.paste-dialog-instructions').show();
            document.getElementById("dialog-paste-imageID").addEventListener("click", listener);
        }

        $('#clearButton').click(function (e) {
            e.preventDefault();
            e.stopPropagation();
            cleardialog();

        });

        $('#cropButton').click(function (e) {
            e.preventDefault();
            e.stopPropagation();
            $('.image-cropper').each(linkUp);
            $(this).attr('disabled', 'disabled');
        });

        $('#resizeButton').click(function (e) {
            e.preventDefault();
            e.stopPropagation();
            $('#resizeOptions').toggle();
            $('#imgWidth').val($('#testImage').width());
            $('#imgHeight').val($('#testImage').height());
            originalHeight = $('#imgHeight').val();
            originalWidth = $('#imgWidth').val();
        });

        $('#saveResizeButton').click(function (e) {
            e.preventDefault();
            e.stopPropagation();
            try{
                jcrop_reference.destroy();
            }catch (e){}

            $('#testImage').attr('width', $('#imgWidth').val());
            $('#testImage').attr('height', $('#imgHeight').val());
            $('#cropButton').removeAttr('disabled');
            $('#img1').val($('#img1').val() + "?crop=(0,0," + $('#imgWidth').val() + "," + $('#imgHeight').val() + ")&cropxunits=" + $('#imgWidth').val() + "&cropyunits=" + $('#imgHeight').val() + "&maxwidth=" + $('#imgWidth').val() + "&maxheight=" + $('#imgHeight').val())
        });

        $('#imgWidth').keyup(function () {
            if ($('#paste-dialog-aspectRatio').is(':checked'))
            {
                try{
                    $('#imgHeight').val(Math.ceil(originalHeight / originalWidth * $('#imgWidth').val()));}
                catch (e){}
            }
        });

        $('#imgHeight').keyup(function () {
            if ($('#paste-dialog-aspectRatio').is(':checked'))
            {
                try{
                    $('#imgWidth').val(Math.ceil(originalHeight / originalWidth * $('#imgHeight').val()));}
                catch (e) { }
            }
        });

    });
