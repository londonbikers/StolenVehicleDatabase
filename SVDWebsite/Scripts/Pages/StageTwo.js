$(function () {
    // handle enter on form submit.
    $('#YouTubeVideoUrl').keypress(function (e) {
        if (e.which == 13 && $('#YouTubeVideoUrl').val() != "") {
            AddVideo();
            return false;
        }
    });
    $("#addVideoBtn").click(function (event) {
        if ($('#YouTubeVideoUrl').val() != "")
            AddVideo();
    });
    CreateUploader();
	RenderVideos();
});

// adds a youtube video to the page and video-url csv.
function AddVideo() {
    var url = $('#YouTubeVideoUrl').val();
    $.ajax({
        type: "POST",
        url: "/addvehicle/addvideo/" + vid,
        data: "url=" + url,
        success: function (result) {
            AddVideoToPage(url, result.videoid);
            $('#YouTubeVideoUrl').val('');
        },
        error: function (result) {
            alert("Whoops, couldn't save the video!");
        }
    });	
}

function AddVideoToPage(url, videoid) {
    var container = $('#videos');
    var embedCode = ConvertYouTubeUrlToEmbedCode(url);
    var videoCard = "<div class=\"photocard\" id=\"vc-" + videoid + "\">" + embedCode + "<div class=\"pclabel\"><a href=\"javascript:RemoveVideo(" + videoid + ");\"><img src=\"/content/images/icons/delete.png\" alt=\"\" /> Remove</a></div></div>";
    container.append(videoCard);
}

function RemoveVideo(videoid) {
    $.ajax({
        type: "POST",
        url: "/addvehicle/removevideo/" + vid,
        data: "videoid=" + videoid,
        success: function (result) {
        },
        error: function (result) {
            alert("Whoops, couldn't remove the video!");
        }
    });

    // remove the photo from the page.
    $('#vc-' + videoid).remove();
}

function RenderVideos() {
	if (videoUrls == null || videoUrls.length == 0){return;}
	for (var i = 0; i < videoUrls.length; i++) {
	    AddVideoToPage(videoUrls[i][0], videoUrls[i][1]);
	}
}

function AddPhotoToPage(url, pid) {
    var container = $('#photos');
    var image = "<div class=\"photocard\" id=\"pc-" + pid + "\"><img src=\"" + url + "\" alt=\"\" /><div class=\"pclabel\"><a href=\"javascript:RemovePhoto(" + pid + ");\"><img src=\"/content/images/icons/delete.png\" alt=\"\" /> Remove</a></div></div>";
    container.append(image);
}

function RemovePhoto(pid) {
    $.ajax({
        type: "POST",
        url: "/addvehicle/removephoto/" + vid,
        data: "pid=" + pid,
        success: function (result) {
        },
        error: function (result) {
            alert("Whoops, couldn't remove the photo!");
        }
    });

    // remove the photo from the page.
    $('#pc-' + pid).remove();
}

function CreateUploader() {
    var uploader = new qq.FileUploader({
        element: document.getElementById('fileUploader'),
        action: '/addvehicle/uploadphoto/' + vid,
        debug: false,
        sizeLimit: 10485760,
        onComplete: function (id, fileName, responseJSON) {
            AddPhotoToPage(responseJSON.url, responseJSON.pid);
        }
    });
}