String.prototype.format = function () {
    var s = this, i = arguments.length;
    while (i--) { s = s.replace(new RegExp('\\{' + i + '\\}', 'gm'), arguments[i]); }
    return s;
};

function ConvertYouTubeUrlToEmbedCode(url) {
    var width = 480;
    var height = 385;
    var id = url.replace(/^[^v]+v.(.{11}).*/, "$1");
    var template ="<div class=\"yvideo\"><object width=\"{0}\" height=\"{1}\">";
    template += "<param name=\"movie\" value=\"http://www.youtube.com/v/{2}?fs=1&amp;hl=en_GB\"></param>";
    template += "<param name=\"allowFullScreen\" value=\"true\"></param>";
    template += "<param name=\"allowscriptaccess\" value=\"always\"></param>";
    template += "<embed src=\"http://www.youtube.com/v/{2}?fs=1&amp;hl=en_GB\" type=\"application/x-shockwave-flash\" allowscriptaccess=\"always\" allowfullscreen=\"true\" width=\"{0}\" height=\"{1}\"></embed>";
    template += "</object></div>";
    return template.format(width, height, id);
}