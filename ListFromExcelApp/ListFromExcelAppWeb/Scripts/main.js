jQuery.extend({
    getUrlVars: function () {
        var vars = [], hash;
        var hashes = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
        for (var i = 0; i < hashes.length; i++) {
            hash = hashes[i].split('=');
            vars.push(hash[0]);
            vars[hash[0]] = hash[1];
        }
        return vars;
    },
    getUrlVar: function (name) {
        return jQuery.getUrlVars()[name];
    }
});

var hostweburl;
var appweburl;



$(document).ready(function () {
    
    'use strict';

    hostweburl = decodeURIComponent($.getUrlVar("SPHostUrl"));
    appweburl = decodeURIComponent($.getUrlVar("SPAppWebUrl"));

    $('#btnCreateList').click(function () {
        alert("click");
        var jqxhr = $.get("/home/createlist?fileName=" + $('#fileToUpload').val(), function () {
            alert("success");
        })
          .done(function () {
              alert("second success");
          })
          .fail(function () {
              alert("error");
          })
          .always(function () {
              alert("finished");
          });


        // Set another completion function for the request above
        jqxhr.always(function () {
            alert("second finished");
        });
    });


    $('#btnShowLists').click(function () {
        //hostweburl = decodeURIComponent($.getUrlVar("SPHostUrl"));
        //appweburl = decodeURIComponent($.getUrlVar("SPAppWebUrl"));

        // Load the SP.RequestExecutor.js file.
        $.getScript(hostweburl + "/_layouts/15/SP.RequestExecutor.js", runCrossDomainRequest);

        //$.when(
        //        //$.getScript(hostweburl + "/_layouts/15/SP.js"),
        //        $.getScript(hostweburl + "/_layouts/15/SP.RequestExecutor.js"),
        //        $.Deferred(function (deferred) {
        //            $(deferred.resolve);
        //        })
        //    ).done(function () {
        //        runCrossDomainRequest();
        //    });
    });


});




// Build and send the HTTP request.
function runCrossDomainRequest() {
    var executor = new SP.RequestExecutor(appweburl);
    executor.executeAsync({
        url: appweburl + "/_api/SP.AppContextSite(@target)/web/lists/GetByTitle('UploadedFiles')/items?$top=1000&$select=ID,FileLeafRef&@target='" + appweburl + "'",
        method: "GET",
        headers: { "Accept": "application/json; odata=verbose" },
        success: successHandler,
        error: errorHandler
    });
}


function successHandler(data) {
    var jsonObject = JSON.parse(data.body);
    var listItemsHTML = "";

    var results = jsonObject.d.results;
    for (var i = 0; i < results.length; i++) {
        listItemsHTML = listItemsHTML +
            "<p><h1>" + results[i].FileLeafRef +
            "</h1><input id='btnCreateList_" + results[i].ID + "' name='btnCreateList_" + results[i].ID + "' data-file-name='" + results[i].FileLeafRef + "' class='btn btn-primary btn-create-list' type='button' value='Create List' />"  +
            "</p><hr>";
    }

    document.getElementById("listItems").innerHTML =
        listItemsHTML;

    $('.btn-create-list').click(function () {
        alert(this.id);
        alert($(this).attr("data-file-name"));
        var myId = this.id.substr(this.id.indexOf("_") + 1);
        alert(myId);
        $.get("/home/createlist?fileName=" + $(this).attr("data-file-name"), function () {
            alert("success");
        })
        .fail(function () {
            alert("error");
        });
    });
}

// Function to handle the error event.
// Prints the error message to the page.
function errorHandler(data, errorCode, errorMessage) {
    document.getElementById("listItems").innerText =
        "Could not complete cross-domain call: " + errorMessage;
}

//$(document).ready(function ()
//{
//    $('#fileupload').fileupload({
//        dataType: 'json',
//        url: '/Home/UploadFiles',
//        autoUpload: true,
//        done: function (e, data) {
//            $('.file_name').html(data.result.name);
//            $('.file_type').html(data.result.type);
//            $('.file_size').html(data.result.size);
//        }
//    }).on('fileuploadprogressall', function (e, data) {
//        var progress = parseInt(data.loaded / data.total * 100, 10);
//        $('.progress .progress-bar').css('width', progress + '%');
//    });
//});




/*jslint unparam: true */
/*global window, $ */
$(document).ready(function () {
    'use strict';
    // Change this to the location of your server-side upload handler:
    var url = '/Home/UploadFiles';

    $('#fileupload').fileupload({
        url: url,
        dataType: 'json',
        done: function (e, data) {

            $('<p/>').text(data.result.name).appendTo('#files');
            //$.each(data.result, function (index, file) {
            //    $('<p/>').text(file.name).appendTo('#files');
            //});
        },
        progressall: function (e, data) {
            var progress = parseInt(data.loaded / data.total * 100, 10);
            $('#progress .progress-bar').css(
                'width',
                progress + '%'
            );
        }
    }).prop('disabled', !$.support.fileInput)
        .parent().addClass($.support.fileInput ? undefined : 'disabled');



});


////

//Namespace
window.AppLevelECT = window.AppLevelECT || {};

//Constructor
AppLevelECT.Grid = function (hostElement, surlWeb) {
    this.hostElement = hostElement;
    if (surlWeb.length > 0 &&
        surlWeb.substring(surlWeb.length - 1, surlWeb.length) !== "/")
        surlWeb += "/";
    this.surlWeb = surlWeb;
};

//Prototype
AppLevelECT.Grid.prototype = {

    init: function () {

        $.ajax({
            url: this.surlWeb + "_api/lists/getbytitle('Employees')/items?" +
                                "$select=BdcIdentity,FirstName,LastName",
            headers: {
                "accept": "application/json;odata=verbose",
                "X-RequestDigest": $("#__REQUESTDIGEST").val()
            },
            context: this,
            success: this.showItems
        });
    },

    showItems: function (data) {
        var items = [];

        items.push("<table>");
        items.push("<tr>" +
                   "<td>First Name</td><td>Last Name</td></tr>");

        $.each(data.d.results, function (key, val) {
            items.push('<tr id="' + val.BdcIdentity + '"><td>' +
                val.FirstName + '</td><td>' +
                val.LastName + '</td></tr>');
        });

        items.push("</table>");

        this.hostElement.html(items.join(''));
    }
};

function getProducts() {
    var grid = new AppLevelECT.Grid($("#displayDiv"),
                  appweburl);//appweburl
    grid.init();
}

// This code runs when the DOM is ready and creates a context object which is needed to use the SharePoint object model
$(document).ready(function () {
    //getProducts();
    $.getScript(hostweburl + "/_layouts/15/SP.RequestExecutor.js", getEmployees);
});




function getEmployees() {
    var executor = new SP.RequestExecutor(appweburl);
    executor.executeAsync({
        url: appweburl + "/_api/SP.AppContextSite(@target)/web/lists/GetByTitle('Employees')/items?$top=1000&$select=FirstName,LastName&@target='" + appweburl + "'",
        method: "GET",
        headers: { "Accept": "application/json; odata=verbose" },
        success: getEmployeesSuccessHandler,
        error: getEmployeesErrorHandler
    });
}

function getEmployeesSuccessHandler(data) {
    var jsonObject = JSON.parse(data.body);
    var listItemsHTML = "";

    var results = jsonObject.d.results;
    for (var i = 0; i < results.length; i++) {
        listItemsHTML = listItemsHTML +
            "<p><h2>" + results[i].FirstName + " " + results[i].LastName
            "</h2>" +
            "</p><hr>";
    }

    document.getElementById("displayDiv").innerHTML =
        listItemsHTML;
}

// Function to handle the error event.
// Prints the error message to the page.
function getEmployeesErrorHandler(data, errorCode, errorMessage) {
    document.getElementById("displayDiv").innerText =
        "Could not complete cross-domain call: " + errorMessage;
}